using System;
using Starcounter;

namespace Website
{
    public class MainHandlers
    {
        public void Register()
        {
            RegisterPartials();

            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/website/standalone", () =>
            {
                Session session = Session.Current;
                StandalonePage standalone = GetMasterFromSession(session);

                if (standalone != null)
                {
                    return standalone;
                }

                standalone = new StandalonePage();

                if (session == null)
                {
                    session = new Session(SessionOptions.PatchVersioning);
                }

                standalone.Session = session;

                return standalone;
            });

            Handle.GET("/website/help?topic={?}", (string topic) =>
            {
                var json = new CmsHelp();
                return json;
            });

            Handle.GET("/website/cleardata", () =>
            {
                DataHelper helper = new DataHelper();

                helper.ClearData();

                return 200;
            });

            Handle.GET("/website/resetdata", () =>
            {
                DataHelper helper = new DataHelper();

                helper.ClearData();
                helper.GenerateData();

                return 200;
            });

            Handle.GET("/website", () =>
            {
                return Self.GET("/website/cms");
            });

            Handle.GET("/website/cms", () =>
            {
                return Db.Scope<StandalonePage>(() =>
                {
                    StandalonePage master = this.GetMaster();

                    master.RefreshCurrentPage("/website/partials/cms");

                    return master;
                });
            });

            Handle.GET("/website/cms/surfaces", () => 
            {
                return Db.Scope<StandalonePage>(() =>
                {
                    StandalonePage master = this.GetMaster();

                    master.RefreshCurrentPage("/website/partials/cms/surfaces");

                    return master;
                });
            });

            Handle.GET("/website/cms/blendingPoints", () => 
            {
                return Db.Scope<StandalonePage>(() =>
                {
                    StandalonePage master = this.GetMaster();

                    master.RefreshCurrentPage("/website/partials/cms/blendingPoints");

                    return master;
                });
            });

            Handle.GET("/website/cms/catchingRules", () => 
            {
                return Db.Scope<StandalonePage>(() =>
                {
                    StandalonePage master = this.GetMaster();

                    master.RefreshCurrentPage("/website/partials/cms/catchingRules");

                    return master;
                });
            });

            Handle.GET("/website/cms/pinningRules", () => {
            {
                return Db.Scope<StandalonePage>(() =>
                {
                    StandalonePage master = this.GetMaster();

                    master.RefreshCurrentPage("/website/partials/cms/pinningRules");

                    return master;
                });
            });
        }

        protected void RegisterPartials()
        {
            Handle.GET("/website/partials/cms", () =>
            {
                CmsPage page = new CmsPage();

                return page;
            });

            Handle.GET("/website/partials/cms/surfaces", () => {
                CmsSurfacesPage page = new CmsSurfacesPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/website/partials/cms/blendingPoints", () => {
                CmsBlendingPointsPage page = new CmsBlendingPointsPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/website/partials/cms/catchingRules", () => {
                CmsCatchingRulesPage page = new CmsCatchingRulesPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/website/partials/cms/pinningRules", () => {
                CmsPinningRulesPage page = new CmsPinningRulesPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/website/partials/deny", () =>
            {
                return new DenyPage();
            });
        }

        protected StandalonePage GetMaster()
        {
            return Self.GET<StandalonePage>("/website/standalone");
        }

        protected StandalonePage GetMasterFromSession(Session session)
        {
            if (session != null)
            {
                if (session.Data is StandalonePage)
                {
                    return (StandalonePage)session.Data;
                }
            }
            return null;
        }
    }
}
