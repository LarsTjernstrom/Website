using System;
using System.Linq;
using System.Text.RegularExpressions;
using Simplified.Ring6;
using Starcounter;
using Starcounter.Advanced.XSON;

namespace WebsiteProvider
{
    public class ContentHandlers
    {
        static string runResponseMiddleware = "X-Run-Response-Middleware";

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

            Handle.GET("/WebsiteProvider/partial/template/{?}", (string templateId) =>
            {
                var page = new WebTemplatePage
                {
                    Data = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.Key = ?", templateId).First
                };
                InitializeTemplate(page);
                return page;
            });

            Handle.GET("/WebsiteProvider/partial/layout", () =>
            {
                WrapperPage page;

                if (Session.Current != null)
                {
                    page = Session.Current.Data as WrapperPage;
                    var currentTemplate = page?.WebTemplatePage.Data;

                    if (currentTemplate != null && currentTemplate.Equals(this.CurrentTemplate))
                    {
                        return page;
                    }
                }
                else
                {
                    Session.Current = new Session(SessionOptions.PatchVersioning);
                }

                page = new WrapperPage
                {
                    Session = Session.Current
                };

                if (page.Session.PublicViewModel != page)
                {
                    page.Session.PublicViewModel = page;
                }

                return page;
            });

            Handle.GET("/WebsiteProvider/partial/wrapper?uri={?}", (string requestUri) =>
            {
                WebUrl webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?", requestUri).First;

                if (webUrl == null)
                {
                    string wildCard = GetWildCardUrl(requestUri);

                    webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?", wildCard).First
                             ?? Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE (wu.Url IS NULL OR wu.Url = ?) AND wu.IsFinal = ?", string.Empty, true).First
                             ?? Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url IS NULL OR wu.Url = ?", string.Empty).First;
                }

                WebTemplate template = webUrl?.Template;

                if (template == null)
                {
                    throw new Exception("Default template is missing");
                }

                this.CurrentTemplate = template;
                WrapperPage master = GetLayoutPage();
                master.IsFinal = webUrl.IsFinal;
                if (!template.Equals(master.WebTemplatePage.Data))
                {
                    master.WebTemplatePage = GetTemplatePage(template.GetObjectID());
                }
                UpdateTemplateSections(requestUri, this.CurrentResponse, master.WebTemplatePage, webUrl);

                return master;
            });

            RegisterFilter();
        }

        private Response CurrentResponse;
        private WebTemplate CurrentTemplate;

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
                        var wrapper = response.Resource as WrapperPage;
                        var requestUri = request.Uri;
                        while (wrapper == null || wrapper.IsFinal == false)
                        {
                            this.CurrentResponse = response;

                            response = Self.GET("/WebsiteProvider/partial/wrapper?uri=" + requestUri);
                            wrapper = response.Resource as WrapperPage;
                            requestUri = wrapper.WebTemplatePage.Data.Html;
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
            const int pinsIndex = 0;
            const int mainIndex = 1;

            foreach (WebSection section in content.Data.Sections)
            {
                var sectionJson = (SectionPage) content.Sections[section.Name];

                var pinsBlendingUri = section.GetMappingUrl(url);
                if (!IsSectionRowAtUri(sectionJson.Rows, pinsIndex, pinsBlendingUri))
                {
                    var page = GetContainerPage(pinsBlendingUri);
                    page.RequestUri = pinsBlendingUri;
                    page.Reset();
                    sectionJson.Rows[pinsIndex] = page;
                }

                var json = response.Resource as Json;
                if (section.Default && json != null && !IsSectionRowAtUri(sectionJson.Rows, mainIndex, requestUri))
                {
                    if (json is WrapperPage)
                    {
                        //we are inserting WebsiteProvider to WebsiteProvider
                        sectionJson.Rows[mainIndex] = json as WrapperPage;
                    }
                    else
                    {
                        //we are inserting different app to WebsiteProvider
                        var page = sectionJson.Rows[mainIndex];
                        if (page == null || page.WebTemplatePage.Data != null)
                        {
                            page = GetContainerPage(requestUri);
                            sectionJson.Rows[mainIndex] = page;
                        }

                        page.RequestUri = requestUri;
                        page.Reset();
                        page.MergeJson(json);
                    }
                }
            }
        }

        private bool IsSectionRowAtUri(Arr<WrapperPage> rows, int index, string uri)
        {
            if (rows.Count > index)
            {
                return rows[index].RequestUri == uri;
            }

            rows.Add();
            return false;
        }

        private WrapperPage GetContainerPage(string uri)
        {
            return Self.GET<WrapperPage>(uri, () => new WrapperPage());
        }

        protected WrapperPage GetLayoutPage()
        {
            return Self.GET<WrapperPage>("/WebsiteProvider/partial/layout");
        }

        protected WebTemplatePage GetTemplatePage(string templateId)
        {
            return Self.GET<WebTemplatePage>("/WebsiteProvider/partial/template/" + templateId);
        }
    }
}
