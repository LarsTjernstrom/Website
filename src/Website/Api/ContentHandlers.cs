using System;
using System.Linq;
using System.Text.RegularExpressions;
using Starcounter;
using Simplified.Ring6;
using Starcounter.Advanced.XSON;

namespace Website {
    public class ContentHandlers {
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

            Handle.GET("/website/partial/layout", () => {
                if (Session.Current != null && Session.Current.Data is WrapperPage)
                {
                    return Session.Current.Data;
                }

                if (Session.Current == null)
                {
                    Session.Current = new Session(SessionOptions.PatchVersioning);
                }

                var page = new WrapperPage();

                if (Session.Current.PublicViewModel is StandalonePage)
                {
                    page.UnwrappedPublicViewModel = Session.Current.PublicViewModel;
                }

                page.Session = Session.Current;

                if (page.Session.PublicViewModel != page)
                {
                    page.Session.PublicViewModel = page;
                }

                return page;
            });

            RegisterFilter();
        }

        protected void RegisterFilter()
        {
            var runResponseMiddleware = "X-Run-Response-Middleware";

            Application.Current.Use((Request request) => {
                //Mark this request to be wrapped by Website response middleware.
                //Without this we would wrap ALL requests, including the ones that shouldn't be wrapped
                //(e.g. "Sign out" button in SignIn app, which uses HandlerOptions.SkipRequestFilters = true)
                //Remove this when we have a flag to disable all middleware
                request.Headers[runResponseMiddleware] = "Yes";
                return null;
            });

            Application.Current.Use((Request request, Response response) => {
                if (!(response.Resource is Json))
                {
                    return response;
                }

                if (request.Headers[runResponseMiddleware] == null)
                {
                    return response;
                }

                string[] parts = request.Uri.Split(new char[] { '/' });
                WebUrl webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?", request.Uri).First;

                if (webUrl == null) {
                    string wildCard = GetWildCardUrl(request.Uri);

                    webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?", wildCard).First;
                }

                WebTemplate template;

                if (webUrl != null) {
                    template = webUrl.Template;
                } else {
                    template = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.Default = ?", true).First;
                }

                if (template == null) {
                    return response;
                }
                
                WrapperPage master = GetLayoutPage();

                if (!template.Equals(master.WebTemplatePage.Data))
                {
                    master.WebTemplatePage.Data = template;
                    InitializeTemplate(master.WebTemplatePage);
                }
                UpdateTemplateSections(request, response, master.WebTemplatePage, webUrl);

                return master;
            });
        }

        protected void InitializeTemplate(WebTemplatePage content)
        {
            dynamic namedSections = new Json();
            content.Sections = namedSections;

            foreach (WebSection section in content.Data.Sections) {
                var sectionJson = new SectionPage();
                namedSections[section.Name] = sectionJson;
                sectionJson.Name = section.Name;
            }
        }

        protected void UpdateTemplateSections(Request req, Response res, WebTemplatePage content, WebUrl url)
        {
            string[] parts = req.Uri.Split(new char[] { '/' });

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
                        sectionJson.Rows[index] = GetConainterPage(uri);
                    }
                    index++;
                }

                if (section.Default && url == null)
                {
                    if (!IsSectionRowAtUri(sectionJson.Rows, index, req.Uri))
                    {
                        sectionJson.Rows[index].Key = req.Uri;
                        sectionJson.Rows[index].MergeJson(res.Resource as Json);
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

        private ContainerPage GetConainterPage(string uri)
        {
            var json = Self.GET<ContainerPage>(uri, () => {
                return new ContainerPage()
                {
                    Key = uri
                };
            });
            return json;
        }

        private bool IsSectionRowAtUri(Arr<ContainerPage> rows, int index, string uri)
        {
            if (rows.Count > index)
            {
                return (rows[index].Key == uri);
            }

            rows.Add();
            return false;
        }

        protected WrapperPage GetLayoutPage() {
            return Self.GET<WrapperPage>("/website/partial/layout");
        }
    }
}
