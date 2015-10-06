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

            Handle.AddResponseFilter((request, response) => {
                if (response.FileExists) {
                    response.HeadersDictionary["Access-Control-Allow-Origin"] = "*";
                }

                return response;
            });

            Handle.AddFilterToMiddleware((request) => {
                string[] parts = request.Uri.Split(new char[] { '/' });
                WebUrl webUrl = Db.SQL<WebUrl>("SELECT wu FROM Website.Models.WebUrl wu WHERE wu.Url = ?", request.Uri).First;

                if (webUrl == null) {
                    string wildCard = GetWildCardUrl(request.Uri);

                    webUrl = Db.SQL<WebUrl>("SELECT wu FROM Website.Models.WebUrl wu WHERE wu.Url = ?", wildCard).First;
                }

                WebTemplate template;

                if (webUrl != null) {
                    template = webUrl.Page.Template;
                } else {
                    template = Db.SQL<WebTemplate>("SELECT wt FROM Website.Models.WebTemplate wt WHERE wt.Default = ?", true).First;
                }

                if (template == null) {
                    return null;
                }

                LayoutPage master = GetLayoutPage();
                ResultPage content = master.TemplateModel as ResultPage;

                if (content == null || master.TemplateName != template.Name) {
                    content = new ResultPage();

                    InitializeTemplate(request, template, content, parts, webUrl);

                    master.TemplateName = template.Name;
                    master.TemplateHtml = template.Html;
                    master.TemplateContent = template.Content;
                    master.TemplateModel = content;
                } else {
                    UpdateTemplate(request, template, content, parts, webUrl);
                }

                Handle.AddOutgoingHeader("Access-Control-Expose-Headers", "Location, X-Location");
                Handle.AddOutgoingHeader("Access-Control-Allow-Headers", "Accept, Content-Type, X-Location, Location");
                Handle.AddOutgoingHeader("Access-Control-Allow-Methods", "GET, POST, HEAD, OPTIONS, PUT, DELETE, PATCH");
                Handle.AddOutgoingHeader("Access-Control-Allow-Origin", "*");

                return master;
            });

            Handle.GET("/website", () => {
                LayoutPage master = GetLayoutPage();

                master.TemplateHtml = null;
                master.TemplateModel = null;
                master.TemplateName = null;
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
        }

        static void InitializeTemplate(Request Request, WebTemplate Template, ResultPage Content, string[] Parts, WebUrl Url) {
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

        static void UpdateTemplate(Request Request, WebTemplate Template, ResultPage Content, string[] UrlParts, WebUrl Url) {
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

        static LayoutPage GetLayoutPage() {
            return Self.GET<LayoutPage>("/website/partial/layout");
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

                WebTemplate template = new WebTemplate() {
                    Default = false,
                    Name = "DefaultTemplate",
                    Html = null,
                    Content = "/Website/templates/DefaultTemplate.html"
                };

                WebSection navigation = new WebSection() {
                    Template = template,
                    Name = "Navigation",
                    Default = false
                };

                WebSection header = new WebSection() {
                    Template = template,
                    Name = "Header",
                    Default = true
                };

                WebSection left = new WebSection() {
                    Template = template,
                    Name = "Left",
                    Default = false
                };

                WebSection right = new WebSection() {
                    Template = template,
                    Name = "Right",
                    Default = false
                };

                WebSection footer = new WebSection() {
                    Template = template,
                    Name = "Footer",
                    Default = false
                };

                WebPage index = new WebPage() {
                    Template = template,
                    Name = "Index page"
                };

                WebUrl homeUrl = new WebUrl() {
                    Page = index,
                    Url = "/content"
                };

                WebUrl appsUrl = new WebUrl() {
                    Page = index,
                    Url = "/content/apps"
                };

                new WebMap() { Section = navigation, ForeignUrl = "/signin/user", SortNumber = 1 };
                new WebMap() { Section = navigation, ForeignUrl = "/content/navigation", SortNumber = 2, };

                new WebMap() { Url = homeUrl, Section = header, ForeignUrl = "/content/index/header", SortNumber = 1 };

                new WebMap() { Url = homeUrl, Section = left, ForeignUrl = "/content/index/left", SortNumber = 1 };
                new WebMap() { Url = homeUrl, Section = left, ForeignUrl = "/signin/signinuser", SortNumber = 2 };
                
                new WebMap() { Url = homeUrl, Section = right, ForeignUrl = "/content/index/right", SortNumber = 1 };

                new WebMap() { Url = homeUrl, Section = footer, ForeignUrl = "/content/index/footer", SortNumber = 1 };

                new WebMap() { Url = appsUrl, Section = header, ForeignUrl = "/content/apps/header", SortNumber = 1 };
            });
        }

        static void GenerateDataOld() {
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