using System;
using System.Text.RegularExpressions;
using Simplified.Ring6;
using Starcounter;
using Starcounter.Advanced.XSON;

namespace WebsiteProvider
{
    public class ContentHandlers
    {
        private static string runResponseMiddleware = "X-Run-Response-Middleware";

        protected Storage<Response> ResponseStorage { get; private set; }

        public ContentHandlers()
        {
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
            else {
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

            Handle.GET("/WebsiteProvider/partial/wrapper?uri={?}&response={?}", (string requestUri, string responseKey) =>
            {
                Response currentResponse = ResponseStorage.Get(responseKey);
                WebUrl webUrl = this.GetWebUrl(requestUri);
                WebTemplate template = webUrl?.Template;

                if (template == null)
                {
                    throw new Exception("Default template is missing");
                }

                SurfacePage surfacePage = GetSurfacePage(template);
                surfacePage.IsFinal = webUrl.IsFinal || string.IsNullOrEmpty(webUrl.Url);

                if (!template.Equals(surfacePage.WebTemplatePage.Data))
                {
                    surfacePage.WebTemplatePage = GetTemplatePage(template);
                }

                UpdateTemplateSections(requestUri, currentResponse, surfacePage.WebTemplatePage, webUrl);

                return surfacePage;
            });

            RegisterFilter();
        }

        protected void RegisterFilter()
        {

            Application.Current.Use((Request request) =>
            {
                //Mark this request to be wrapped by Website response middleware.
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

                        while ((wrapper == null || wrapper.IsFinal == false) && this.HasCatchingRule(requestUri))
                        {
                            var responseKey = ResponseStorage.Put(response);
                            isWrapped = true;

                            response = Self.GET($"/WebsiteProvider/partial/wrapper?uri={requestUri}&response={responseKey}");

                            ResponseStorage.Remove(responseKey);
                            wrapper = response.Resource as SurfacePage;
                            requestUri = wrapper?.WebTemplatePage.Data.Html;
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

        protected void InitializeTemplate(WebTemplatePage content)
        {
            dynamic namedSections = new Json();
            content.Sections = namedSections;

            foreach (WebSection section in content.Data.Sections)
            {
                SectionPage sectionJson = new SectionPage();
                namedSections[section.Name] = sectionJson;
                sectionJson.Name = section.Name;
            }
        }

        protected void UpdateTemplateSections(string requestUri, Response response, WebTemplatePage content, WebUrl url)
        {
            foreach (WebSection section in content.Data.Sections)
            {
                var sectionJson = (SectionPage) content.Sections[section.Name];

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
                        var page = sectionJson.MainContent;
                        if (page == null || page.WebTemplatePage.Data != null)
                        {
                            page = WrapExternalRequest(requestUri);
                            sectionJson.MainContent = page;
                        }

                        page.RequestUri = requestUri;
                        page.Reset();
                        page.MergeJson(json);
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
                var sessionWebTemplate = page?.WebTemplatePage.Data;

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

        protected WebTemplatePage GetTemplatePage(WebTemplate template)
        {
            var page = new WebTemplatePage
            {
                Data = template
            };
            InitializeTemplate(page);
            return page;
        }

        public bool HasCatchingRule(string requestUri)
        {
            return this.GetWebUrl(requestUri) != null;
        }

        protected WebUrl GetWebUrl(string requestUri)
        {
            WebUrl webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?", requestUri).First;

            if (webUrl == null)
            {
                string wildCard = GetWildCardUrl(requestUri);

                webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?", wildCard).First
                         ?? Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE (wu.Url IS NULL OR wu.Url = ?) AND wu.IsFinal = ?", string.Empty, true).First
                         ?? Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url IS NULL OR wu.Url = ?", string.Empty).First;
            }
            return webUrl;
        }
    }
}
