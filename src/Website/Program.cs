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

                if (webUrl == null) {
                    string wildCard = GetWildCardUrl(request.Uri);

                    webUrl = Db.SQL<WebUrl>("SELECT wu FROM Website.Models.WebUrl wu WHERE wu.Url = ?", wildCard).First;
                }

                WebTemplate template;

                if (webUrl != null) {
                    template = webUrl.Template;
                } else {
                    template = Db.SQL<WebTemplate>("SELECT wt FROM Website.Models.WebTemplate wt WHERE wt.Default = ?", true).First;
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
                    master.TemplateContent = template.Content;
                    master.TemplateModel = content;
                } else {
                    UpdateTemplate(request, template, content, parts, webUrl);
                }

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

            Handle.GET("/website/cleardata", () => {
                ClearData();
                return 200;
            });

            Handle.GET("/website/resetdata", () => {
                ClearData();
                GenerateData();
                return 200;
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

        static void ClearData() {
            Db.Transact(() => {
                Db.SlowSQL("DELETE FROM Website.Models.WebTemplate");
                Db.SlowSQL("DELETE FROM Website.Models.WebSection");
                Db.SlowSQL("DELETE FROM Website.Models.WebMap");
                Db.SlowSQL("DELETE FROM Website.Models.WebUrl");
            });
        }

        static void GenerateData() {
            if (Db.SQL<WebTemplate>("SELECT wt FROM Website.Models.WebTemplate wt").First != null) {
                return;
            }

            Db.Transact(() => {
                ClearData();

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

                WebUrl homeUrl = new WebUrl() {
                    Template = template,
                    Url = "/content/dynamic/apps"
                };

                WebUrl appsUrl = new WebUrl() {
                    Template = template,
                    Url = "/content/dynamic/apps/wanted-apps"
                };

                WebUrl profileUrl = new WebUrl() {
                    Template = template,
                    Url = "/content/dynamic/userprofile"
                };

                new WebMap() { Section = navigation, ForeignUrl = "/signin/user", SortNumber = 1 };
                new WebMap() { Section = navigation, ForeignUrl = "/content/dynamic/navigation", SortNumber = 2, };
                new WebMap() { Url = homeUrl, Section = navigation, ForeignUrl = "/content/dynamic/index/header", SortNumber = 3 };
                
                new WebMap() { Url = homeUrl, Section = header, ForeignUrl = "/signin/signinuser", SortNumber = 1 };
                new WebMap() { Url = homeUrl, Section = header, ForeignUrl = "/registration", SortNumber = 2 };
                new WebMap() { Url = homeUrl, Section = header, ForeignUrl = "/content/dynamic/index/registration", SortNumber = 3 };

                new WebMap() { Url = homeUrl, Section = left, ForeignUrl = "/content/dynamic/index/left", SortNumber = 1 };

                new WebMap() { Url = homeUrl, Section = right, ForeignUrl = "/content/dynamic/index/right", SortNumber = 1 };

                new WebMap() { Url = homeUrl, Section = footer, ForeignUrl = "/content/dynamic/index/footer", SortNumber = 1 };

                new WebMap() { Url = appsUrl, Section = header, ForeignUrl = "/content/dynamic/apps/header", SortNumber = 1 };
                new WebMap() { Url = appsUrl, Section = header, ForeignUrl = "/content/dynamic/apps/footer", SortNumber = 2 };

                new WebMap() { Url = profileUrl, Section = header, ForeignUrl = "/content/dynamic/userprofile/header", SortNumber = 1 };
                new WebMap() { Url = profileUrl, Section = header, ForeignUrl = "/userprofile", SortNumber = 2 };
                new WebMap() { Url = profileUrl, Section = footer, ForeignUrl = "/content/dynamic/userprofile/footer", SortNumber = 3 };
            });
        }
    }
}