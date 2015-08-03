using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Starcounter;
using Website.ViewModels;
using Website.Models;

namespace Website {
    class Program {
        static string GetWildCardUrl(string Url) {
            Regex reg = new Regex(@"[/]\w*$", RegexOptions.IgnoreCase);

            Url = reg.Replace(Url, "/{?}");

            return Url;
        }

        static void Main() {
            GenerateData();

            Handle.AddFilterToMiddleware((request) => {
                string[] parts = request.Uri.Split(new char[] { '/' });
                WebUrl webUrl = Db.SQL<WebUrl>("SELECT wu FROM Website.Models.WebUrl wu WHERE wu.Url = ?", request.Uri).First;
                bool pageFound = true;

                if (webUrl == null) {
                    string wildCard = GetWildCardUrl(request.Uri);

                    webUrl = Db.SQL<WebUrl>("SELECT wu FROM Website.Models.WebUrl wu WHERE wu.Url = ?", wildCard).First;
                }
                
                WebTemplate template;

                if (webUrl != null) {
                    template = webUrl.Page.Template;
                } else {
                    template = Db.SQL<WebTemplate>("SELECT wt FROM Website.Models.WebTemplate wt WHERE wt.Default = ?", true).First;
                    pageFound = false;
                }

                if (template == null) {
                    return null;
                }

                LayoutPage master = GetLayoutPage();
                ResultPage content = master.TemplateModel as ResultPage;

                if (content == null || master.TemplateName != template.Name) {
                    content = new ResultPage();

                    foreach (WebSection section in template.Sections) {
                        var sectionJson = content.Sections.Add();

                        sectionJson.Name = section.Name;

                        foreach (WebMap map in section.Maps.OrderBy(x => x.SortNumber)) {
                            string url = FormatUrl(map.ForeignUrl, parts.Last());

                            ContainerPage json = Self.GET<ContainerPage>(url, () => {
                                return new ContainerPage() {
                                    Key = url
                                };
                            });

                            sectionJson.Rows.Add(json);
                        }

                        if (section.Default && !pageFound) {
                            ContainerPage json = Self.GET<ContainerPage>(request.Uri, () => {
                                return new ContainerPage() {
                                    Key = request.Uri
                                };
                            });

                            sectionJson.Rows.Add(json);
                        }
                    }

                    master.TemplateName = template.Name;
                    master.TemplateHtml = template.Html;
                    master.TemplateContent = template.Content;
                    master.TemplateModel = content;
                } else {
                    foreach (WebSection section in template.Sections) {
                        var sectionJson = content.Sections.FirstOrDefault(x => x.Name == section.Name);
                        var maps = section.Maps.OrderBy(x => x.SortNumber).ToList();
                        int index = 0;

                        foreach (WebMap map in maps) {
                            string url = FormatUrl(map.ForeignUrl, parts.Last());

                            if ((sectionJson.Rows[index] as ContainerPage).Key != url) {
                                sectionJson.Rows[index] = Self.GET<ContainerPage>(url, () => {
                                    return new ContainerPage() {
                                        Key = url
                                    };
                                });
                            }

                            index++;
                        }

                        if (section.Default && !pageFound && (sectionJson.Rows[index] as ContainerPage).Key != request.Uri) {
                            sectionJson.Rows[index] = Self.GET<ContainerPage>(request.Uri, () => {
                                return new ContainerPage() {
                                    Key = request.Uri
                                };
                            });
                        }
                    }
                }

                return master;
            });

            Handle.GET("/website", () => {
                LayoutPage master = GetLayoutPage();

                master.TemplateContent = null;

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

            /*foreach (WebPage page in Db.SQL<WebPage>("SELECT wp FROM Website.Models.WebPage wp")) {
                string url = "/website" + page.Url;

                if (page.Url.Contains("{?}")) {
                    Handle.GET(url, (string name) => {
                        return HandleRequest(page, name);
                    });
                } else {
                    Handle.GET(url, () => {
                        return HandleRequest(page, null);
                    });
                }
            }*/
        }

        static LayoutPage GetLayoutPage() {
            return Self.GET<LayoutPage>("/website/partial/layout");
        }

        static LayoutPage HandleRequest(WebPage Page, string Name) {
            LayoutPage master = GetLayoutPage();
            ResultPage content = master.TemplateModel as ResultPage;

            if (content == null || master.TemplateName != Page.Template.Name) {
                content = new ResultPage();

                foreach (WebSection section in Page.Template.Sections) {
                    var sectionJson = content.Sections.Add();

                    sectionJson.Name = section.Name;

                    foreach (WebMap map in section.Maps.OrderBy(x => x.SortNumber)) {
                        string url = FormatUrl(map.ForeignUrl, Name);

                        ContainerPage json = Self.GET<ContainerPage>(url, () => {
                            return new ContainerPage() {
                                Key = url
                            };
                        });

                        sectionJson.Rows.Add(json);
                    }
                }

                master.TemplateName = Page.Template.Name;
                master.TemplateHtml = Page.Template.Html;
                master.TemplateModel = content;
            } else {
                foreach (WebSection section in Page.Template.Sections) {
                    var sectionJson = content.Sections.FirstOrDefault(x => x.Name == section.Name);
                    var maps = section.Maps.OrderBy(x => x.SortNumber).ToList();
                    int index = 0;

                    foreach (WebMap map in maps) {
                        string url = FormatUrl(map.ForeignUrl, Name);

                        if ((sectionJson.Rows[index] as ContainerPage).Key != url) {
                            sectionJson.Rows[index] = Self.GET<ContainerPage>(url, () => {
                                return new ContainerPage() {
                                    Key = url
                                };
                            });
                        }

                        index++;
                    }
                }
            }

            return master;
        }

        static string FormatUrl(string Url, string Name) {
            if (string.IsNullOrEmpty(Name)) {
                return Url;
            } else {
                return Url.Replace("{?}", Name);
            }
        }

        static void GenerateData() {
            Db.Transact(() => {
                WebPage item = Db.SQL<WebPage>("SELECT wp FROM Website.Models.WebPage wp").First;

                if (item != null) {
                    //return;
                }

                Db.SlowSQL("DELETE FROM Website.Models.WebPage");
                Db.SlowSQL("DELETE FROM Website.Models.WebTemplate");
                Db.SlowSQL("DELETE FROM Website.Models.WebSection");
                Db.SlowSQL("DELETE FROM Website.Models.WebMap");
                Db.SlowSQL("DELETE FROM Website.Models.WebUrl");

                WebTemplate defaultTemplate = new WebTemplate() {
                    Default = false,
                    Name = "DefaultTemplate",
                    Html = null,
                    Content = "/Website/templates/DefaultTemplate.html"
                };

                WebTemplate sideTemplate = new WebTemplate() {
                    Default = false,
                    Name = "SideTemplate",
                    Html = null,
                    Content = "/Website/templates/SideTemplate.html"
                };

                WebTemplate emptySideTemplate = new WebTemplate() {
                    Default = true,
                    Name = "EmptySideTemplate",
                    Html = null,
                    Content = "/Website/templates/SideTemplate.html"
                };

                WebSection section = new WebSection() {
                    Template = defaultTemplate,
                    Name = "Navigation",
                    Default = false
                };

                section = new WebSection() {
                    Template = defaultTemplate,
                    Name = "Center",
                    Default = true
                };

                section = new WebSection() {
                    Template = sideTemplate,
                    Name = "Navigation",
                    Default = false
                };

                section = new WebSection() {
                    Template = sideTemplate,
                    Name = "Side",
                    Default = false
                };

                section = new WebSection() { 
                    Template = sideTemplate,
                    Name = "Center",
                    Default = true
                };

                section = new WebSection() {
                    Template = emptySideTemplate,
                    Name = "Navigation",
                    Default = false
                };

                section = new WebSection() {
                    Template = emptySideTemplate,
                    Name = "Side",
                    Default = false
                };

                section = new WebSection() {
                    Template = emptySideTemplate,
                    Name = "Center",
                    Default = true
                };

                WebPage teamPage = new WebPage() {
                    Template = defaultTemplate,
                    Name = "Team page"
                };

                WebUrl webUrl = new WebUrl() {
                    Page = teamPage,
                    Url = "/content/team"
                };

                WebPage memberPage = new WebPage() {
                    Template = sideTemplate,
                    Name = "Team member page"
                };

                webUrl = new WebUrl() {
                    Page = memberPage,
                    Url = "/content/team/{?}"
                };

                WebMap map = new WebMap() {
                    Section = defaultTemplate.Sections.FirstOrDefault(x => x.Name == "Navigation"),
                    ForeignUrl = "/content/navigation",
                    SortNumber = 1,
                };

                map = new WebMap() {
                    Section = defaultTemplate.Sections.FirstOrDefault(x => x.Name == "Center"),
                    ForeignUrl = "/content/team",
                    SortNumber = 1
                };

                map = new WebMap() {
                    Section = sideTemplate.Sections.FirstOrDefault(x => x.Name == "Navigation"),
                    ForeignUrl = "/content/navigation",
                    SortNumber = 1
                };

                map = new WebMap() {
                    Section = sideTemplate.Sections.FirstOrDefault(x => x.Name == "Side"),
                    ForeignUrl = "/content/team",
                    SortNumber = 1
                };

                map = new WebMap() {
                    Section = sideTemplate.Sections.FirstOrDefault(x => x.Name == "Side"),
                    ForeignUrl = "/content/team-ad/{?}",
                    SortNumber = 2
                };

                map = new WebMap() {
                    Section = sideTemplate.Sections.FirstOrDefault(x => x.Name == "Center"),
                    ForeignUrl = "/content/team/{?}",
                    SortNumber = 1
                };

                map = new WebMap() {
                    Section = sideTemplate.Sections.FirstOrDefault(x => x.Name == "Center"),
                    ForeignUrl = "/content/description/{?}",
                    SortNumber = 2
                };

                map = new WebMap() {
                    Section = emptySideTemplate.Sections.FirstOrDefault(x => x.Name == "Navigation"),
                    ForeignUrl = "/content/navigation",
                    SortNumber = 1
                };

                map = new WebMap() {
                    Section = emptySideTemplate.Sections.FirstOrDefault(x => x.Name == "Side"),
                    ForeignUrl = "/content/team",
                    SortNumber = 1
                };
            });
        }
    }
}