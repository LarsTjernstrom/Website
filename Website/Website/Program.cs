using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Starcounter;
using Website.ViewModels;
using Website.Models;

namespace Website {
    class Program {
        static void Main() {
            GenerateData();

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

            foreach (WebPage page in Db.SQL<WebPage>("SELECT wp FROM Website.Models.WebPage wp")) {
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
            }
        }

        static LayoutPage GetLayoutPage() {
            return Self.GET<LayoutPage>("/website/partial/layout");
        }

        static LayoutPage HandleRequest(WebPage Page, string Name) {
            LayoutPage master = GetLayoutPage();
            ResultPage content = master.TemplateContent as ResultPage;

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
                master.TemplateContent = content;
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
                    return;
                }

                Db.SlowSQL("DELETE FROM Website.Models.WebPage");
                Db.SlowSQL("DELETE FROM Website.Models.WebTemplate");
                Db.SlowSQL("DELETE FROM Website.Models.WebSection");
                Db.SlowSQL("DELETE FROM Website.Models.WebMap");

                WebTemplate defaultTemplate = new WebTemplate() {
                    Name = "DefaultTemplate",
                    Html = @"<div class=""single""><template is=""dom-repeat"" items=""{{model.Sections.0.Rows}}""><website-section model=""{{item}}"" pathName=""model.Sections.0.Rows"" pathIndex=""{{index}}""></website-section></template></div>"
                };

                WebTemplate sideTemplate = new WebTemplate() {
                    Name = "SideTemplate",
                    Html = @"<div class=""side""><template is=""dom-repeat"" items=""{{model.Sections.0.Rows}}""><website-section model=""{{item}}"" pathName=""model.Sections.0.Rows"" pathIndex=""{{index}}""></website-section></template></div>" +
                           @"<div class=""center""><template is=""dom-repeat"" items=""{{model.Sections.1.Rows}}""><website-section model=""{{item}}"" pathName=""model.Sections.1.Rows"" pathIndex=""{{index}}""></website-section></template></div>"
                };

                WebSection section = new WebSection() {
                    Template = defaultTemplate,
                    Name = "Center"
                };

                section = new WebSection() {
                    Template = sideTemplate,
                    Name = "Side"
                };

                section = new WebSection() { 
                    Template = sideTemplate,
                    Name = "Center"
                };

                WebPage teamPage = new WebPage() {
                    Template = defaultTemplate,
                    Url = "/team"
                };

                WebPage memberPage = new WebPage() {
                    Template = sideTemplate,
                    Url = "/team/{?}"
                };

                WebMap map = new WebMap() {
                    Section = defaultTemplate.Sections.FirstOrDefault(),
                    ForeignUrl = "/content/team"
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
            });
        }
    }
}