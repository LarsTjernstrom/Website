using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Starcounter;
using Website.ViewModels;
using Website.Models;

namespace Website {
    class Program {
        static LayoutPage GetLayoutPage() {
            return Self.GET<LayoutPage>("/website/partial/layout");
        }

        static void Main() {
            GenerateData();

            Handle.GET("/website", () => {
                LayoutPage master = GetLayoutPage();

                master.TemplateContent = null;

                return master;
            });

            /*Handle.GET("/website/partial/content/{?}", (string value) => {
                string[] parts = value.Split(new char[] { '-' });
                WebMap map = null;

                if (parts.Length == 3) {
                    map = Db.SQL<WebMap>("SELECT wm FROM Website.Models.WebMap wm WHERE wm.Section.Template.Name = ? AND wm.Section.Name = ? AND (wm.Content.Name = ? OR wm.Content.Name IS NULL)", parts[0], parts[1], parts[2]).First;
                } else if (parts.Length == 2) {
                    map = Db.SQL<WebMap>("SELECT wm FROM Website.Models.WebMap wm WHERE wm.Section.Template.Name = ? AND wm.Section.Name = ?", parts[0], parts[1]).First;
                }

                if (map == null) {
                    return new ContainerPage();
                }

                ContainerPage json = new ContainerPage() {
                    Key = string.Format("/{0}/{1}/{2}", map.Section.Template.Name, map.Section.Name, map.Content.Name),
                    Html = map.Content.Html,
                    Value = new Json(map.Content.Value)
                };

                return json;
            });*/

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
                    AddHandleWithParameter(url, page);
                } else {
                    AddHandle(url, page);
                }
            }

            //PolyjuiceNamespace.Polyjuice.Map("/website/partial/content/@w", "/polyjuice/website/partial/content/@w");
        }

        static void AddHandle(string Url, WebPage Page) {
            Handle.GET(Url, () => {
                LayoutPage master = GetLayoutPage();
                ResultPage content = master.TemplateContent as ResultPage;

                if (content == null || master.TemplateName != Page.Template.Name) {
                    content = new ResultPage();

                    foreach (WebSection section in Page.Template.Sections) {
                        var sectionJson = content.Sections.Add();

                        foreach (WebMap map in section.Maps.OrderBy(x => x.SortNumber)) {
                            Json json = Self.GET<Json>(map.ForeignUrl, () => {
                                return new ContainerPage() {
                                    Html = string.Format("Website application: {0}.{1}", section.Template.Name, section.Name),
                                    Key = map.GetObjectID()
                                };
                            });

                            sectionJson.Rows.Add(json);
                        }

                        //ContainerPage json = (ContainerPage)Self.GET(GetContentUrl(section));
                        //content[section.Name] = json;
                    }

                    master.TemplateName = Page.Template.Name;
                    master.TemplateHtml = Page.Template.Html;
                    master.TemplateContent = content;
                } else {
                    foreach (WebSection section in Page.Template.Sections) {
                        int index = 0;

                        foreach (WebMap map in section.Maps.OrderBy(x => x.SortNumber)) {
                            var sectionJson = content.Sections.FirstOrDefault(x => x.Name == section.Name);

                            if ((sectionJson.Rows[index] as ContainerPage).Key != map.GetObjectID()) {
                                sectionJson.Rows[index] = Self.GET<ContainerPage>(map.ForeignUrl, () => {
                                    return new ContainerPage() {
                                        Html = string.Format("Website application: {0}.{1}", section.Template.Name, section.Name),
                                        Key = map.GetObjectID()
                                    };
                                });
                            }

                            index++;
                        }

                        /*ContainerPage json = (ContainerPage)Self.GET(GetContentUrl(section));

                        if (content[section.Name].Key != json.Key) {
                            content[section.Name] = json;
                        }*/
                    }
                }

                return master;
            });
        }

        static void AddHandleWithParameter(string Url, WebPage Page) {
            Handle.GET(Url, (string name) => {
                LayoutPage master = GetLayoutPage();
                dynamic content = master.TemplateContent;

                if (content == null || master.TemplateName != Page.Template.Name) {
                    content = new Json();

                    foreach (WebSection section in Page.Template.Sections) {
                        Arr<Json> result = new Arr<Json>();

                        foreach (WebMap map in section.Maps.OrderBy(x => x.SortNumber)) {
                            string url = map.ForeignUrl.Replace("{?}", name);

                            ContainerPage json = Self.GET<ContainerPage>(url, () => {
                                return new ContainerPage() {
                                    Html = string.Format("Website application: {0}.{1}.{2}", section.Template.Name, section.Name, name),
                                    Key = map.GetObjectID()
                                };
                            });

                            result.Add(json);
                        }

                        //ContainerPage json = (ContainerPage)Self.GET(GetContentUrl(section));
                        //content[section.Name] = json;

                        content[section.Name] = result.ToArray();
                    }

                    master.TemplateName = Page.Template.Name;
                    master.TemplateHtml = Page.Template.Html;
                    master.TemplateContent = content;
                } else {
                    foreach (WebSection section in Page.Template.Sections) {
                        Arr<Json> result = content[section.Name] as Arr<Json>;
                        int index = 0;

                        foreach (WebMap map in section.Maps.OrderBy(x => x.SortNumber)) {
                            if ((result[index] as ContainerPage).Key != map.GetObjectID()) {
                                string url = map.ForeignUrl.Replace("{?}", name);

                                result[index] = Self.GET<ContainerPage>(url, () => {
                                    return new ContainerPage() {
                                        Html = string.Format("Website application: {0}.{1}.{2}", section.Template.Name, section.Name, name),
                                        Key = map.GetObjectID()
                                    };
                                });
                            }

                            index++;
                        }

                        /*ContainerPage json = (ContainerPage)Self.GET(GetContentUrl(section));

                        if (content[section.Name].Key != json.Key) {
                            content[section.Name] = json;
                        }*/
                    }
                }

                return master;
            });
        }

        static string GetContentUrl(WebSection Section, string Name = null) {
            if (string.IsNullOrEmpty(Name)) {
                return string.Format("/website/partial/content/{0}-{1}", Section.Template.Name, Section.Name);
            } else {
                return string.Format("/website/partial/content/{0}-{1}-{2}", Section.Template.Name, Section.Name, Name);
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
                //Db.SlowSQL("DELETE FROM Website.Models.WebContent");
                Db.SlowSQL("DELETE FROM Website.Models.WebSection");
                Db.SlowSQL("DELETE FROM Website.Models.WebMap");

                WebTemplate defaultTemplate = new WebTemplate() {
                    Name = "DefaultTemplate",
                    Html = @"<div class=""single""><website-section model=""{{model.Center}}"" path=""model.Content.Center""></website-section></div>"
                };

                WebTemplate sideTemplate = new WebTemplate() {
                    Name = "SideTemplate",
                    Html = @"<div class=""side""><website-section model=""{{model.Side}}"" path=""model.Content.Side""></website-section></div>" +
                           @"<div class=""center""><website-section model=""{{model.Center}}"" path=""model.Content.Center""></website-section></div>"
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

                /*WebContent content = new WebContent() {
                    Html = "<ul><template is='dom-repeat' items='{{model.People}}'><li><a href='{{item.Url}}'><span>{{item.FirstName}}</span> <span>{{item.LastName}}</span></a></li></template></ul>" +
                            @"<iframe width=""200"" height=""150"" src=""https://www.youtube.com/embed/Xp7HVA5CPjQ"" frameborder=""0"" allowfullscreen></iframe>",
                    Value = "{\"People\":[{\"Key\":\"konstantin\",\"FirstName\":\"Konstantin\",\"LastName\":\"Mi\",\"Url\":\"/website/team/konstantin\"},{\"Key\":\"tomek\",\"FirstName\":\"Tomek\",\"LastName\":\"Wytrębowicz\",\"Url\":\"/website/team/tomek\"},{\"Key\":\"marcin\",\"FirstName\":\"Marcin\",\"LastName\":\"Warpechowski\",\"Url\":\"/website/team/marcin\"}]}"
                };*/

                WebMap map = new WebMap() {
                    //Content = content,
                    Section = defaultTemplate.Sections.FirstOrDefault(),
                    ForeignUrl = "/content/team"
                };

                map = new WebMap() {
                    //Content = content,
                    Section = sideTemplate.Sections.FirstOrDefault(x => x.Name == "Side"),
                    ForeignUrl = "/content/team"
                };

                /*content = new WebContent() {
                    Name = "konstantin",
                    Html = "<div>{{model.FirstName}}</div><div>{{model.LastName}}</div>",
                    Value = "{\"Key\":\"konstantin\",\"FirstName\":\"Konstantin\",\"LastName\":\"Mi\",\"Url\":\"/website/team/konstantin\"}",
                };*/

                map = new WebMap() {
                    //Content = content,
                    Section = sideTemplate.Sections.FirstOrDefault(x => x.Name == "Center"),
                    ForeignUrl = "/content/team/{?}"
                };

                /*content = new WebContent() {
                    Name = "tomek",
                    Html = "<div>{{model.FirstName}}</div><div>{{model.LastName}}</div>",
                    Value = "{\"Key\":\"tomek\",\"FirstName\":\"Tomek\",\"LastName\":\"Wytrębowicz\",\"Url\":\"/website/team/tomek\"}",
                };

                map = new WebMap() {
                    Content = content,
                    Section = sideTemplate.Sections.FirstOrDefault(x => x.Name == "Center")
                };

                content = new WebContent() {
                    Name = "marcin",
                    Html = "<div>{{model.FirstName}}</div><div>{{model.LastName}}</div>",
                    Value = "{\"Key\":\"marcin\",\"FirstName\":\"Marcin\",\"LastName\":\"Warpechowski\",\"Url\":\"/website/team/marcin\"}",
                };

                map = new WebMap() {
                    Content = content,
                    Section = sideTemplate.Sections.FirstOrDefault(x => x.Name == "Center")
                };*/
            });
        }
    }
}