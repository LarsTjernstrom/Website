using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Simplified.Ring6;
using Starcounter;
using Starcounter.Advanced.XSON;

namespace WebsiteProvider
{
    public class ContentHandlers
    {
        private static string runResponseMiddleware = "X-Run-Response-Middleware";

        protected Storage<Request> RequestStorage { get; private set; }
        protected Storage<Response> ResponseStorage { get; private set; }

        public ContentHandlers()
        {
            RequestStorage = new Storage<Request>();
            ResponseStorage = new Storage<Response>();
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

        public string FormatUrl(string Url, string Name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return Url;
            }
            else
            {
                return Url.Replace("{?}", Name);
            }
        }

        public void Register()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/WebsiteProvider", () =>
            {
                return "Welcome to WebsiteProvider.";
            });

            Handle.GET("/WebsiteProvider/partial/wrapper?uri={?}&response={?}&request={?}", (string requestUri, string responseKey, string requestKey) =>
            {
                requestUri = Uri.UnescapeDataString(requestUri);
                Response currentResponse = ResponseStorage.Get(responseKey);
                Request originalRequest = RequestStorage.Get(requestKey);

                WebUrl webUrl = this.GetWebUrl(originalRequest);
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
                        var isWrapped = false;
                        var requestKey = RequestStorage.Put(request);

                        try
                        {
                            while ((wrapper == null || wrapper.IsFinal == false) && this.HasCatchingRule(request))
                            {
                                isWrapped = true;
                                var responseKey = ResponseStorage.Put(response);

                                try
                                {
                                    var escapedRequestUri = Uri.EscapeDataString(requestUri);
                                    response = Self.GET($"/WebsiteProvider/partial/wrapper?uri={escapedRequestUri}&response={responseKey}&request={requestKey}");
                                }
                                finally
                                {
                                    ResponseStorage.Remove(responseKey);
                                }

                                wrapper = response.Resource as SurfacePage;
                                requestUri = wrapper?.Data.Html;
                            }
                        }
                        finally
                        {
                            RequestStorage.Remove(requestKey);
                        }

                        if (!isWrapped)
                        {
                            //Morph to a view that is stateless and not catched by any surface
                            //This is tested by RedirectToOtherAppPageTest
                            //Should be improved by https://github.com/Starcounter/level1/issues/4159
                            Session.Current.Data = response.Resource as Json;
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
                SectionPage sectionJson = new SectionPage();
                namedSections[section.Name] = sectionJson;
                sectionJson.Name = section.Name;
            }
        }

        protected void UpdateTemplateSections(string requestUri, Response response, SurfacePage content, WebUrl url)
        {
            foreach (WebSection section in content.Data.Sections)
            {
                var sectionJson = (SectionPage)content.Sections[section.Name];

                var uri = section.GetMappingUrl(url);
                sectionJson.PinContent = Self.GET(uri);

                var json = response.Resource as Json;
                if (section.Default && json != null && sectionJson.MainContent?.RequestUri != requestUri)
                {
                    if (json is SurfacePage)
                    {
                        //we are inserting WebsiteProvider to WebsiteProvider
                        sectionJson.MainContent = json as SurfacePage;
                    }
                    else
                    {
                        //we are inserting different app to WebsiteProvider
                        if (sectionJson.MainContent == null || sectionJson.MainContent.LastJson != json)
                        {
                            sectionJson.MainContent = new SurfacePage();
                        }

                        //these two lines should be in the above if, but do not work then
                        sectionJson.MainContent.LastJson = json;
                        sectionJson.MainContent.MergeJson(json);

                        sectionJson.MainContent.RequestUri = requestUri;
                    }
                }
            }
        }

        private SurfacePage WrapExternalRequest(string uri)
        {
            return Self.GET<SurfacePage>(uri, () => new SurfacePage());
        }

        protected SurfacePage GetSurfacePage(WebTemplate template)
        {
            SurfacePage page;

            if (Session.Current != null)
            {
                page = Session.Current.Data as SurfacePage;
                var sessionWebTemplate = page?.Data;

                if (sessionWebTemplate != null)
                {
                    if (sessionWebTemplate.Equals(template))
                    {
                        return page;
                    }
                }
            }
            else
            {
                Session.Current = new Session(SessionOptions.PatchVersioning);
            }

            page = new SurfacePage
            {
                Session = Session.Current
            };

            if (page.Session.PublicViewModel != page)
            {
                page.Session.PublicViewModel = page;
            }

            return page;
        }

        public bool HasCatchingRule(Request request)
        {
            return this.GetWebUrl(request) != null;
        }

        protected WebUrl GetWebUrl(Request request)
        {
            var headers = request.HeadersDictionary;

            WebUrl webUrl = this.FindUrlByHeaders(Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?", request.Uri), headers);

            if (webUrl == null)
            {
                string wildCard = GetWildCardUrl(request.Uri);

                webUrl = this.FindUrlByHeaders(Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?", wildCard), headers)
                         ?? this.FindUrlByHeaders(Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE (wu.Url IS NULL OR wu.Url = ?) AND wu.IsFinal = ?", string.Empty, true), headers)
                         ?? this.FindUrlByHeaders(Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url IS NULL OR wu.Url = ?", string.Empty), headers);
            }
            return webUrl;
        }

        private WebUrl FindUrlByHeaders(QueryResultRows<WebUrl> webUrls, Dictionary<string, string> requestHeaders)
        {
            return webUrls.Count() == 1 && webUrls.All(x => !x.Headers.Any())
                ? webUrls.FirstOrDefault()
                : (webUrls.FirstOrDefault(x => x.Headers.All(
                       wh => requestHeaders.Any(
                           rh => rh.Key.Equals(wh.Name, StringComparison.InvariantCultureIgnoreCase) &&
                                 rh.Value == wh.Value)))
                   ?? webUrls.FirstOrDefault(x => !x.Headers.Any()));
        }
    }
}
