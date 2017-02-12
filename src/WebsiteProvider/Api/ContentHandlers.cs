using System;
using System.Linq;
using System.Text.RegularExpressions;
using Starcounter;
using Simplified.Ring6;
using Starcounter.Advanced.XSON;
using Starcounter.Internal;

namespace WebsiteProvider {
    public class ContentHandlers {
        static string runResponseMiddleware = "X-Run-Response-Middleware";

        public string GetWildCardUrl(string Url) {
            Regex reg = new Regex(@"[/]\w*$", RegexOptions.IgnoreCase);

            Url = reg.Replace(Url, "/{?}");

            return Url;
        }

        public string FormatUrl(string Url, string Name) {
            if (string.IsNullOrEmpty(Name)) {
                return Url;
            } else {
                return Url.Replace("{?}", Name);
            }
        }

        public void Register()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/website", () => {
                WrapperPage master = GetLayoutPage();

                master.WebTemplatePage.Data = null;

                return master;
            });

            Handle.GET("/website/partial/layout", () =>
            {
                WrapperPage page = null;

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

                page = new WrapperPage();

                if (Session.Current.PublicViewModel is Json)
                {
                    page.UnwrappedPublicViewModel = Session.Current.PublicViewModel;
                }

                var final = Session.Current.Data as WrapperPage;
                if (final == null || final.IsFinal == false)
                {
                    page.Session = Session.Current;

                    if (page.Session.PublicViewModel != page)
                    {
                        page.Session.PublicViewModel = page;
                    }
                }

                return page;
            });

            Handle.GET("/website/partial/wrapper?uri={?}", (string requestUri) => {
                string[] parts = requestUri.Split(new char[] { '/' });
                WebUrl webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?", requestUri).First;

                if (webUrl == null)
                {
                    string wildCard = GetWildCardUrl(requestUri);

                    webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?", wildCard).First;
                }

                WebTemplate template;

                if (webUrl != null)
                {
                    template = webUrl.Template;
                }
                else
                {
                    template = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.Default = ?", true).First;
                }

                if (template == null)
                {
                    throw new Exception("Default template is missing");
                }

                this.CurrentTemplate = template;
                WrapperPage master = GetLayoutPage();

                if (template.Name == "DefaultTemplate")
                {
                    master.IsFinal = true;
                }

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

            Application.Current.Use((Request request, Response response) => {
                if (response.Resource is Json)
                {
                    if (request.Headers[runResponseMiddleware] != null)
                    {
                        var wrapper = response.Resource as WrapperPage;
                        var requestUri = request.Uri;
                        while (wrapper == null || wrapper.IsFinal == false)
                        {
                            this.CurrentResponse = response;

                                response = Self.GET("/website/partial/wrapper?uri=" + requestUri);
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

            foreach (WebSection section in content.Data.Sections) {
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
                            //uncommenting the below lines results in MergeJson producing invalid patches
                            //it is good to fix it, so we reuse existing wrapped pages
                            //if (page == null) 
                            //{
                                page = GetConainterPage(requestUri);
                                sectionJson.Rows[index] = page;
                            //}

                            page.RequestUri = requestUri;
                            page.Reset();
                            page.MergeJson(json);
                            sectionJson.Rows[index] = page;
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

        protected WrapperPage GetLayoutPage() {
            return Self.GET<WrapperPage>("/website/partial/layout");
        }

        protected WrapperPage FindWrapperPageForTemplate(WrapperPage page, WebTemplate template)
        {
            if (page != null)
            {
                if (page.WebTemplatePage.Data != null && page.WebTemplatePage.Data.Equals(template))
                {
                    return page;
                }
                return FindWrapperPageForTemplate(page.UnwrappedPublicViewModel as WrapperPage, template);               
            }
            return null;
        }
    }
}
