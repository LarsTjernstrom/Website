using System;
using Starcounter;

namespace Website {
    public class MainHandlers {
        public void Register() {
            RegisterPartials();

            Handle.GET("/website/standalone", () => {
                Session session = Session.Current;

                if (session != null && session.Data != null && session.Data is StandalonePage) {
                    return session.Data as StandalonePage;
                }

                StandalonePage standalone = new StandalonePage();

                if (session == null) {
                    session = new Session(SessionOptions.PatchVersioning);
                }

                standalone.Session = session;
                standalone.User = Self.GET("/sc/mapping/user", () => new Page());

                return standalone;
            });

            Handle.GET("/website/cleardata", () => {
                DataHelper helper = new DataHelper();

                helper.ClearData();

                return 200;
            });

            Handle.GET("/website/resetdata", () => {
                DataHelper helper = new DataHelper();

                helper.ClearData();
                helper.GenerateData();

                return 200;
            });

            Handle.GET("/website/cms", () => {
                return Db.Scope<StandalonePage>(() => {
                    string url = "/website/partials/cms";
                    StandalonePage master = this.GetMaster();

                    master.PartialUrl = url;
                    master.RefreshCurrentPage();

                    return master;
                });
            });
        }

        protected void RegisterPartials() {
            Handle.GET("/website/partials/cms", () => {
                CmsPage page = new CmsPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/website/partials/deny", () => {
                return new Page() {
                    Html = "/Website/viewmodels/DenyPage.html"
                };
            });
        }

        protected StandalonePage GetMaster() {
            return Self.GET<StandalonePage>("/website/standalone");
        }
    }
}
