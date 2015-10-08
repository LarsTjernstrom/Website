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

                return standalone;
            });

            Handle.GET("/content/cms", () => {
                StandalonePage master = this.GetMaster();

                master.CurrentPage = GetPartial("/content/partials/cms");

                return master;
            });

            Handle.GET("/content/cms/item/{?}", (string key) => {
                return Db.Scope<StandalonePage>(() => {
                    StandalonePage master = this.GetMaster();

                    master.CurrentPage = GetPartial("/content/partials/cms/item/" + key);

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

            Handle.GET("/content/partial/deny", () => {
                return new Page() {
                    Html = "/Content/viewmodels/DenyPage.html"
                };
            });
        }

        protected Json GetPartial(string Url) {
            SystemUser user = SystemUser.GetCurrentSystemUser();

            if (user == null || !SystemUser.IsMemberOfGroup(user, "cms-admin")) {
                return Self.GET("/content/partial/deny");
            }

            return Self.GET(Url);
        }

        protected StandalonePage GetMaster() {
            return Self.GET<StandalonePage>("/content/standalone");
        }
    }
}
