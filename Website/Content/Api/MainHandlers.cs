using System;
using Starcounter;

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

                return standalone;
            });

            Handle.GET("/content/cms", () => {
                StandalonePage master = this.GetMaster();

                master.CurrentPage = Self.GET("/content/partials/cms");

                return master;
            });

            Handle.GET("/content/cms/item/{?}", (string key) => {
                return Db.Scope<StandalonePage>(() => {
                    StandalonePage master = this.GetMaster();

                    master.CurrentPage = Self.GET("/content/partials/cms/item/" + key);

                    return master;
                });
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
        }

        protected StandalonePage GetMaster() {
            return Self.GET<StandalonePage>("/content/standalone");
        }
    }
}
