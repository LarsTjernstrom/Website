using System;
using System.Linq;
using Starcounter;
using Simplified.Ring3;
using Simplified.Ring5;

namespace Content {
    class Program {
        static ContentCachePage GetCachePage() {
            if (Session.Current.Data is ContentCachePage) {
                return Session.Current.Data as ContentCachePage;
            }

            ContentCachePage page = new ContentCachePage();
            Session.Current.Data = page;

            return page;
        }

        static void Main() {
            MainHandlers handlers = new MainHandlers();
            CommitHooks hooks = new CommitHooks();

            GenerateData();
            handlers.Register();
            hooks.Register();

            foreach (var entry in Db.SQL<ContentEntry>("SELECT e FROM Content.ContentEntry e")) {
                Handle.GET(entry.Url, () => new Page());
            }

            foreach (var item in Db.SQL<ContentItem>("SELECT i FROM Content.ContentItem i")) {
                RegisterContentHandler(item.Url, item.HtmlPath);
            }

            Handle.GET("/content/cleardata", () => {
                ClearData();
                return 200;
            });

            RegisterHooks();
        }

        static void RegisterContentHandler(string Url, string Html) {
            Handle.GET(Url, () => {
                ContentCachePage master = GetCachePage();
                ContentPage page = new ContentPage() {
                    Html = Html,
                    Data = null
                };

                master.Pages.Add(page);

                return page;
            });
        }

        static void RefreshSignInState() {
            if (!(Session.Current.Data is ContentCachePage)) {
                return;
            }

            ContentCachePage master = GetCachePage();

            foreach (ContentPage page in master.Pages) {
                page.Data = null;
            }
        }

        static void RegisterHooks() {
            Hook<SystemUserSession>.CommitInsert += (s, a) => {
                RefreshSignInState();
            };

            Hook<SystemUserSession>.CommitDelete += (s, a) => {
                RefreshSignInState();
            };

            Hook<SystemUserSession>.CommitUpdate += (s, a) => {
                RefreshSignInState();
            };
        }

        static void ClearData() {
            Db.Transact(() => {
                Db.SlowSQL("DELETE FROM Content.ContentEntry");
                Db.SlowSQL("DELETE FROM Content.ContentItem");
            });
        }

        static void GenerateData() {
            if (Db.SQL("SELECT p FROM Content.ContentEntry p").First != null) {
                return;
            }

            Db.Transact(() => {
                ClearData();

                new ContentEntry() {
                    Url = "/content/apps"
                };

                new ContentEntry() {
                    Url = "/content/apps/wanted-apps"
                };

                new ContentEntry() {
                    Url = "/content/userprofile"
                };

                new ContentItem() {
                    Url = "/content/navigation",
                    HtmlPath = "/Content/cms/NavigationPage.html"
                };

                new ContentItem() {
                    Url = "/content/index/header",
                    HtmlPath = "/Content/cms/index/HeaderPage.html"
                };

                new ContentItem() {
                    Url = "/content/index/left",
                    HtmlPath = "/Content/cms/index/LeftPage.html"
                };

                new ContentItem() {
                    Url = "/content/index/right",
                    HtmlPath = "/Content/cms/index/RightPage.html"
                };

                new ContentItem() {
                    Url = "/content/index/registration",
                    HtmlPath = "/Content/cms/index/RegistrationPage.html"
                };

                new ContentItem() {
                    Url = "/content/index/footer",
                    HtmlPath = "/Content/cms/index/FooterPage.html"
                };

                new ContentItem() {
                    Url = "/content/apps/header",
                    HtmlPath = "/Content/cms/apps/HeaderPage.html"
                };

                new ContentItem() {
                    Url = "/content/userprofile/header",
                    HtmlPath = "/Content/cms/userprofile/HeaderPage.html"
                };

                new ContentItem() {
                    Url = "/content/userprofile/footer",
                    HtmlPath = "/Content/cms/userprofile/FooterPage.html"
                };
            });
        }
    }
}