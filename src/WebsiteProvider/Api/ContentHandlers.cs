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

        public string GetWildCardUrl(string Url)
        {
            Regex reg = new Regex(@"[/]\w*$", RegexOptions.IgnoreCase);

            Url = reg.Replace(Url, "/{?}");

            return Url;
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

            Handle.GET("/WebsiteProvider/partial/layout", () =>
            {
                WrapperPage page;

                if (Session.Current != null)
                {
                    page = FindWrapperPageForTemplate(Session.Current.Data as WrapperPage, this.CurrentTemplate);
                    if (page != null)
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
                    master.WebTemplatePage.Data = template;
                    InitializeTemplate(master.WebTemplatePage);
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
                if (response.Resource is Json)
                {
                    if (request.Headers[runResponseMiddleware] != null)
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
                    }
                }
                return response;
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
            string[] parts = requestUri.Split(new char[] { '/' });

            foreach (WebSection section in content.Data.Sections)
            {
                var sectionJson = content.Sections[section.Name] as SectionPage;
                int index = 0;

                foreach (WebMap map in section.Maps.OrderBy(x => x.SortNumber))
                {
                    if (map.Url != null)
                    {
                        //it is not a catch-all map
                        if (!map.Url.Equals(url))
                        {
                            //it is a map for a different entry URI, skip
                            continue;
                        }
                    }

                    string uri = FormatUrl(map.ForeignUrl, parts.Last());
                    if (!IsSectionRowAtUri(sectionJson.Rows, index, uri))
                    {
                        var page = GetConainterPage(uri);
                        page.RequestUri = uri;
                        page.Reset();
                        sectionJson.Rows[index] = page;
                    }
                    index++;
                }

                var json = response.Resource as Json;
                if (section.Default && json != null)
                {
                    if (!IsSectionRowAtUri(sectionJson.Rows, index, requestUri))
                    {
                        if (json is WrapperPage)
                        {
                            //we are inserting WebsitePrivider to WebsitePrivider
                            sectionJson.Rows[index] = json as WrapperPage;
                        }
                        else
                        {
                            //we are inserting different app to WebsitePrivider
                            var page = sectionJson.Rows[index] as WrapperPage;
                            if (page == null || page.WebTemplatePage.Data != null)
                            {
                                page = GetConainterPage(requestUri);
                                sectionJson.Rows[index] = page;
                            }

                            page.RequestUri = requestUri;
                            page.Reset();
                            page.MergeJson(json);
                        }
                    }
                    index++;
                }

                //remove unused elements at the end of Rows
                while (sectionJson.Rows.Count > index)
                {
                    sectionJson.Rows.RemoveAt(sectionJson.Rows.Count - 1);
                }
            }
        }

        private WrapperPage GetConainterPage(string uri)
        {
            var json = Self.GET<WrapperPage>(uri, () =>
            {
                return new WrapperPage();
            });
            return json;
        }

        private bool IsSectionRowAtUri(Arr<WrapperPage> rows, int index, string uri)
        {
            if (rows.Count > index)
            {
                return (rows[index].RequestUri == uri);
            }

            rows.Add();
            return false;
        }

        protected WrapperPage GetLayoutPage()
        {
            return Self.GET<WrapperPage>("/WebsiteProvider/partial/layout");
        }

        protected WrapperPage FindWrapperPageForTemplate(WrapperPage page, WebTemplate template)
        {
            if (page?.WebTemplatePage.Data != null && page.WebTemplatePage.Data.Equals(template))
            {
                return page;
            }
            return null;
        }
    }
}
