using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Simplified.Ring6;
using Starcounter;
using Starcounter.Advanced.XSON;
using WebsiteProvider.Helpers;
using WebsiteProvider.ViewModels;

namespace WebsiteProvider.Api
{
    public class ContentHandlers
    {
        private static string runResponseMiddleware = "X-Run-Response-Middleware";

        protected Storage<Request> RequestStorage { get; }
        protected Storage<Response> ResponseStorage { get; }

        public ContentHandlers()
        {
            this.RequestStorage = new Storage<Request>();
            this.ResponseStorage = new Storage<Response>();
        }

        public string GetWildCardUrl(string url)
        {
            var reg = new Regex(@"[/]\w*$", RegexOptions.IgnoreCase);

            if (reg.IsMatch(url))
            {
                return reg.Replace(url, "/{?}");
            }

            reg = new Regex(@"[?](\w|\d|%)*$", RegexOptions.IgnoreCase);
            return reg.Replace(url, "?{?}");
        }

        public void Register()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/WebsiteProvider", () => "Welcome to WebsiteProvider.");

            Handle.GET("/WebsiteProvider/partial/wrapper?uri={?}&response={?}&request={?}", (string requestUri, string responseKey, string requestKey) =>
            {
                requestUri = Uri.UnescapeDataString(requestUri);
                Response currentResponse = this.ResponseStorage.Get(responseKey);
                Request originalRequest = this.RequestStorage.Get(requestKey);

                WebUrl webUrl = originalRequest.Uri == requestUri
                    ? this.GetWebUrl(originalRequest)
                    : this.GetWebUrl(requestUri);
                WebTemplate template = webUrl?.Template;

                if (template == null)
                {
                    throw new Exception("Default surface is missing");
                }

                SurfacePage surfacePage = GetSurfacePage(template);
                surfacePage.IsFinal = webUrl.IsFinal || string.IsNullOrEmpty(webUrl.Url);

                if (!template.Equals(surfacePage.Data))
                {
                    InitializeTemplate(surfacePage, template);
                }

                UpdateTemplateSections(requestUri, currentResponse, surfacePage, webUrl);

                return surfacePage;
            });

            RegisterFilter();
        }

        protected void RegisterFilter()
        {

            Application.Current.Use((Request request) =>
            {
                //Mark this request to be wrapped by Websiteeditor response middleware.
                //Without this we would wrap ALL requests, including the ones that shouldn't be wrapped
                //(e.g. "Sign out" button in SignIn app, which uses HandlerOptions.SkipRequestFilters = true)
                //Remove this when we have a flag to disable all middleware
                request.Headers[runResponseMiddleware] = "Yes";
                return null;
            });

            Application.Current.Use((Request request, Response response) =>
            {
                var json = response.Resource as Json;
                if (json != null && request.Headers[runResponseMiddleware] != null)
                {
                    var htmlField = json["Html"] as string;
                    if (htmlField != null)
                    {
                        var wrapper = response.Resource as SurfacePage;
                        var requestUri = request.Uri;
                        var requestKey = this.RequestStorage.Put(request);

                        try
                        {
                            while ((wrapper == null || wrapper.IsFinal == false) && this.HasCatchingRule(requestUri))
                            {
                                var responseKey = this.ResponseStorage.Put(response);

                                try
                                {
                                    var escapedRequestUri = Uri.EscapeDataString(requestUri ?? string.Empty);
                                    response = Self.GET($"/WebsiteProvider/partial/wrapper?uri={escapedRequestUri}&response={responseKey}&request={requestKey}");
                                }
                                finally
                                {
                                    this.ResponseStorage.Remove(responseKey);
                                }

                                wrapper = response.Resource as SurfacePage;
                                requestUri = wrapper?.Data.Html;
                            }
                        }
                        finally
                        {
                            this.RequestStorage.Remove(requestKey);
                        }

                        return response;
                    }
                }
                return null;
            });
        }

        protected void InitializeTemplate(SurfacePage content, WebTemplate template)
        {
            content.Data = template;

            dynamic namedSections = new Json();
            content.Sections = namedSections;

            foreach (WebSection section in template.Sections)
            {
                namedSections[section.Name] = new SectionPage();
            }
        }

        protected void UpdateTemplateSections(string requestUri, Response response, SurfacePage content, WebUrl url)
        {
            foreach (WebSection section in content.Data.Sections)
            {
                var sectionJson = (SectionPage)content.Sections[section.Name];
                var json = response.Resource as Json;

                var uri = section.GetMappingUrl(url);
                var newJson = Self.GET(uri, () =>
                {
                    if (section.Default)
                    {
                        return json;
                    }
                    else
                    {
                        return new Json();
                    }
                });

                sectionJson.MergeJson(newJson);

                //if (section.Default && json != null && sectionJson.MainContent?.RequestUri != requestUri)
                //{
                //    if (json is SurfacePage)
                //    {
                //        //we are inserting WebsiteProvider to WebsiteProvider
                //        sectionJson.MainContent = json as SurfacePage;
                //    }
                //    else
                //    {
                //        //we are inserting different app to WebsiteProvider
                //        if (sectionJson.MainContent == null || sectionJson.MainContent.LastJson != json)
                //        {
                //            sectionJson.MainContent = new SurfacePage();
                //        }

                //        //these two lines should be in the above if, but do not work then
                //        sectionJson.MainContent.LastJson = json;
                //        sectionJson.MainContent.MergeJson(json);

                //        sectionJson.MainContent.RequestUri = requestUri;
                //    }
                //}
            }
        }

        protected SurfacePage GetSurfacePage(WebTemplate template)
        {
            var page = Session.Ensure().Store[nameof(SurfacePage)] as SurfacePage;
            var sessionWebTemplate = page?.Data;

            if (sessionWebTemplate != null && sessionWebTemplate.Equals(template))
            {
                return page;
            }

            page = new SurfacePage();
            Session.Current.Store[nameof(SurfacePage)] = page;

            return page;
        }

        public bool HasCatchingRule(string requestUri)
        {
            return this.GetWebUrl(requestUri) != null;
        }

        protected WebUrl GetWebUrl(string requestUri)
        {
            return this.GetWebUrl(requestUri, urls => urls.FirstOrDefault());
        }

        protected WebUrl GetWebUrl(Request request)
        {
            return this.GetWebUrl(request.Uri, urls => this.FindUrlByHeaders(urls, request.HeadersDictionary));
        }

        private WebUrl GetWebUrl(string requestUri, Func<QueryResultRows<WebUrl>, WebUrl> findWebUrlFunc)
        {
            WebUrl webUrl = findWebUrlFunc(Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?", requestUri));

            if (webUrl == null)
            {
                string wildCard = GetWildCardUrl(requestUri);

                webUrl = findWebUrlFunc(Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?", wildCard))
                         ?? findWebUrlFunc(Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE (wu.Url IS NULL OR wu.Url = ?) AND wu.IsFinal = ?", string.Empty, true))
                         ?? findWebUrlFunc(Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url IS NULL OR wu.Url = ?", string.Empty));
            }

            return webUrl;
        }

        private WebUrl FindUrlByHeaders(QueryResultRows<WebUrl> webUrls, Dictionary<string, string> requestHeaders)
        {
            if (!webUrls.Any())
            {
                return null;
            }
            if (webUrls.All(x => !x.Headers.Any()))
            {
                return webUrls.First();
            }

            return webUrls.Where(x => x.Headers.Any())
                       .FirstOrDefault(x => x.Headers.All(
                           wh => requestHeaders.Any(
                               rh => rh.Key.Equals(wh.Name, StringComparison.InvariantCultureIgnoreCase) &&
                                     rh.Value == wh.Value)))
                   ?? webUrls.FirstOrDefault(x => !x.Headers.Any());
        }
    }
}
