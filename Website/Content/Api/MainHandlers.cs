using System;
using Starcounter;
using Simplified.Ring3;

namespace Content {
    public class MainHandlers {
        public void Register() {
            RegisterPartials();

            Handle.GET("/content/standalone", () => {
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

            Handle.GET("/content/cms", () => {
                string url = "/content/partials/cms";
                StandalonePage master = this.GetMaster();

                master.PartialUrl = url;
                master.RefreshCurrentPage();

                return master;
            });

            Handle.GET("/content/cms/item/{?}", (string key) => {
                return Db.Scope<StandalonePage>(() => {
                    string url = "/content/partials/cms/item/" + key;
                    StandalonePage master = this.GetMaster();

                    master.PartialUrl = url;
                    master.RefreshCurrentPage();

                    return master;
                });
            });

            Handle.GET("/content/cleardata", () => {
                DataHelper data = new DataHelper();

                data.ClearData();

                return 200;
            });

            Handle.GET("/content/resetdata", () => {
                DataHelper data = new DataHelper();

                data.ClearData();
                data.GenerateData();

                return 200;
            });
        }

        protected void RegisterPartials() {
            Handle.GET("/content/partials/cms", () => {
                CmsPage page = new CmsPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/content/partials/cms/item/{?}", (string key) => {
                CmsItemPage page = new CmsItemPage();
                
                page.Data = DbHelper.FromID(DbHelper.Base64DecodeObjectID(key)) as ContentItem;

                return page;
            });

            Handle.GET("/content/partials/deny", () => {
                return new Page() {
                    Html = "/Content/viewmodels/DenyPage.html"
                };
            });
        }

        protected StandalonePage GetMaster() {
            return Self.GET<StandalonePage>("/content/standalone");
        }
    }
}
