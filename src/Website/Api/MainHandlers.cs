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
                standalone.User = Self.GET("/Website/user");

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
                    StandalonePage master = this.GetMaster();

                    master.RefreshCurrentPage("/website/partials/cms");

                    return master;
                });
            });

            Handle.GET("/website/cms/templates", () => {
                return Db.Scope<StandalonePage>(() => {
                    StandalonePage master = this.GetMaster();

                    master.RefreshCurrentPage("/website/partials/cms/templates");

                    return master;
                });
            });

            Handle.GET("/website/cms/sections", () => {
                return Db.Scope<StandalonePage>(() => {
                    StandalonePage master = this.GetMaster();

                    master.RefreshCurrentPage("/website/partials/cms/sections");

                    return master;
                });
            });

            Handle.GET("/website/cms/urls", () => {
                return Db.Scope<StandalonePage>(() => {
                    StandalonePage master = this.GetMaster();

                    master.RefreshCurrentPage("/website/partials/cms/urls");

                    return master;
                });
            });

            Handle.GET("/website/cms/maps", () => {
                return Db.Scope<StandalonePage>(() => {
                    StandalonePage master = this.GetMaster();

                    master.RefreshCurrentPage("/website/partials/cms/maps");

                    return master;
                });
            });

            Handle.GET("/website/user", () => new Json(), new HandlerOptions() { SelfOnly = true });
        }

        protected void RegisterPartials() {
            Handle.GET("/website/partials/cms", () => {
                CmsPage page = new CmsPage();

                return page;
            });

            Handle.GET("/website/partials/cms/templates", () => {
                CmsTemplatesPage page = new CmsTemplatesPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/website/partials/cms/sections", () => {
                CmsSectionsPage page = new CmsSectionsPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/website/partials/cms/urls", () => {
                CmsUrlsPage page = new CmsUrlsPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/website/partials/cms/maps", () => {
                CmsMapsPage page = new CmsMapsPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/website/partials/deny", () => {
                return new DenyPage();
            });
        }

        protected StandalonePage GetMaster() {
            return Self.GET<StandalonePage>("/website/standalone");
        }
    }
}
