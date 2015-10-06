using System;
using System.Linq;
using Starcounter;
using Simplified.Ring3;
using Simplified.Ring5;

namespace Content {
    class Program {
        static MasterPage GetMaster() {
            if (Session.Current.Data is MasterPage) {
                return Session.Current.Data as MasterPage;
            }

            MasterPage page = new MasterPage();
            Session.Current.Data = page;

            return page;
        }

        static void Main() {
            Handle.GET("/content", () => new Page());
            Handle.GET("/content/apps", () => new Page());

            RegisterContentHandler("/content/navigation", "/Content/viewmodels/NavigationPage.html");

            RegisterContentHandler("/content/index/header", "/Content/viewmodels/index/HeaderPage.html");
            RegisterContentHandler("/content/index/left", "/Content/viewmodels/index/LeftPage.html");
            RegisterContentHandler("/content/index/right", "/Content/viewmodels/index/RightPage.html");
            RegisterContentHandler("/content/index/footer", "/Content/viewmodels/index/FooterPage.html");

            RegisterContentHandler("/content/apps/header", "/Content/viewmodels/apps/HeaderPage.html");

            RegisterHooks();
        }

        static void RegisterContentHandler(string Url, string Html) {
            Handle.GET(Url, () => {
                MasterPage master = GetMaster();
                ContentPage page = new ContentPage() {
                    Html = Html,
                    Data = null
                };

                master.Pages.Add(page);

                return page;
            });
        }

        static void RefreshSignInState() {
            MasterPage master = GetMaster();

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
    }
}