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
                LayoutPage master = GetLayoutPage();

                master.TemplateHtml = null;
                master.TemplateModel = null;
                master.TemplateName = null;

                return master;
            });

            Handle.GET("/website/partial/layout", () => {
                if (Session.Current != null && Session.Current.Data is LayoutPage)
                {
                    return Session.Current.Data;
                }

                if (Session.Current == null)
                {
                    Session.Current = new Session(SessionOptions.PatchVersioning);
                }

                var page = new LayoutPage()
                {
                    Session = Session.Current
                };

                if (page.Session.PublicViewModel != page)
                {
                    page.Session.PublicViewModel = page;
                }

                return page;
            });

            RegisterFilter();
        }

        private Boolean IsFullHtml(String html) {
            if (html.IndexOf("FullPage") > -1) {
                return true; //it's a full HTML page
            }
            return false; //it's a partial
        }

        protected void RegisterFilter() {
            Application.Current.Use((Request request, Response response) => {
                if (!(response.Resource is Json))
                {
                    return response;
                }

                if (request.Uri.StartsWith("/website/cms")) {
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

                LayoutPage master = GetLayoutPage();

                ResultPage content = master.TemplateModel as ResultPage;

                if (content == null || master.TemplateName != template.Name) {
                    content = new ResultPage();

                    InitializeTemplate(template, content);
                    UpdateTemplateSections(request, response, template, content, parts, webUrl);

                    master.TemplateName = template.Name;
                    master.TemplateHtml = template.Html;
                    if(IsFullHtml(template.Html) == true) {
                        master.Html = template.Html;
                    }
                    master.TemplateModel = content;
                } else {
                    UpdateTemplateSections(request, response, template, content, parts, webUrl);
                }

                return master;
            });
        }

        protected void InitializeTemplate(WebTemplate template, ResultPage content)
        {
            dynamic namedSections = new Json();
            content.Sections = namedSections;

            foreach (WebSection section in template.Sections) {
                var sectionJson = new SectionPage();
                namedSections[section.Name] = sectionJson;
                sectionJson.Name = section.Name;
            }
        }

        protected void UpdateTemplateSections(Request req, Response res, WebTemplate template, ResultPage content, string[] parts, WebUrl url)
        {
            foreach (WebSection section in template.Sections) {
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

        protected LayoutPage GetLayoutPage() {
            return Self.GET<LayoutPage>("/website/partial/layout");
        }
    }
}
