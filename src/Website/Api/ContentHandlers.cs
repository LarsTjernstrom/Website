using System;
using System.Linq;
using System.Text.RegularExpressions;
using Starcounter;
using Simplified.Ring6;

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

        public void Register() {
            Handle.GET("/website", () => {
                LayoutPage master = GetLayoutPage();

                master.TemplateHtml = null;
                master.TemplateModel = null;
                master.TemplateName = null;

                return master;
            });

            Handle.GET("/website/partial/layout", () => {
                LayoutPage page = null;

                if (Session.Current != null) {
                    page = Session.Current.Data as LayoutPage;

                    return Session.Current.Data;
                }

                Session.Current = new Session(SessionOptions.PatchVersioning);

                page = new LayoutPage();
                page.Session = Session.Current;

                return page;
            });

            RegisterFilter();
        }

        protected void RegisterFilter() {
            Handle.AddFilterToMiddleware((request) => {
                if (request.Uri.StartsWith("/website/cms")) {
                    return null;
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
                    return null;
                }

                LayoutPage master = GetLayoutPage();

                if (master == null) {
                    return null;
                }

                ResultPage content = master.TemplateModel as ResultPage;

                if (content == null || master.TemplateName != template.Name) {
                    content = new ResultPage();

                    InitializeTemplate(request, template, content, parts, webUrl);

                    master.TemplateName = template.Name;
                    master.TemplateHtml = template.Html;
                    master.TemplateModel = content;
                } else {
                    UpdateTemplate(request, template, content, parts, webUrl);
                }

                return master;
            });
        }

        protected void InitializeTemplate(Request Request, WebTemplate Template, ResultPage Content, string[] Parts, WebUrl Url) {
            foreach (WebSection section in Template.Sections) {
                var sectionJson = Content.Sections.Add();

                sectionJson.Name = section.Name;

                foreach (WebMap map in section.Maps.OrderBy(x => x.SortNumber)) {
                    if (map.Url != null && map.Url.GetObjectID() != Url.GetObjectID()) {
                        continue;
                    }

                    string url = FormatUrl(map.ForeignUrl, Parts.Last());

                    ContainerPage json = Self.GET<ContainerPage>(url, () => {
                        return new ContainerPage() {
                            Key = url
                        };
                    });

                    sectionJson.Rows.Add(json);
                }

                if (section.Default && Url == null) {
                    ContainerPage json = Self.GET<ContainerPage>(Request.Uri, () => {
                        return new ContainerPage() {
                            Key = Request.Uri
                        };
                    });

                    sectionJson.Rows.Add(json);
                }
            }
        }

        protected void UpdateTemplate(Request Request, WebTemplate Template, ResultPage Content, string[] UrlParts, WebUrl Url) {
            foreach (WebSection section in Template.Sections) {
                var sectionJson = Content.Sections.FirstOrDefault(x => x.Name == section.Name);
                var maps = section.Maps.Where(x => x.Url == null || x.Url.GetObjectID() == Url.GetObjectID()).OrderBy(x => x.SortNumber).ToList();
                int index = 0;

                if (sectionJson.Rows.Count == maps.Count) {
                    foreach (WebMap map in maps) {
                        string url = FormatUrl(map.ForeignUrl, UrlParts.Last());

                        if ((sectionJson.Rows[index] as ContainerPage).Key != url) {
                            sectionJson.Rows[index] = Self.GET<ContainerPage>(url, () => {
                                return new ContainerPage() {
                                    Key = url
                                };
                            });
                        }

                        index++;
                    }
                } else {
                    sectionJson.Rows.Clear();

                    foreach (WebMap map in maps) {
                        string url = FormatUrl(map.ForeignUrl, UrlParts.Last());

                        sectionJson.Rows.Add(Self.GET<ContainerPage>(url, () => {
                            return new ContainerPage() {
                                Key = url
                            };
                        }));

                        index++;
                    }
                }

                if (section.Default && Url == null && (sectionJson.Rows[index] as ContainerPage).Key != Request.Uri) {
                    sectionJson.Rows[index] = Self.GET<ContainerPage>(Request.Uri, () => {
                        return new ContainerPage() {
                            Key = Request.Uri
                        };
                    });
                }
            }
        }

        protected LayoutPage GetLayoutPage() {
            return Self.GET<LayoutPage>("/website/partial/layout");
        }
    }
}
