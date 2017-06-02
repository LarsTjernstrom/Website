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

            Handle.GET("/website/master", () =>
            {
                Session session = Session.Current;
                MasterPage master = GetMasterFromSession(session);

                if (master != null)
                {
                    return master;
                }

                master = new MasterPage();

                if (session == null)
                {
                    session = new Session(SessionOptions.PatchVersioning);
                }

                master.Session = session;

                return master;
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
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMaster();

                    master.RefreshCurrentPage("/website/partials/cms");

                    return master;
                });
            });

            Handle.GET("/website/cms/surfaces", () => 
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMaster();

                    master.RefreshCurrentPage("/website/partials/cms/surfaces");

                    return master;
                });
            });

            Handle.GET("/website/cms/blendingpoints", () => 
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMaster();

                    master.RefreshCurrentPage("/website/partials/cms/blendingpoints");

                    return master;
                });
            });

            Handle.GET("/website/cms/catchingrules", () => 
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMaster();

                    master.RefreshCurrentPage("/website/partials/cms/catchingrules");

                    return master;
                });
            });

            Handle.GET("/website/cms/pinningrules", () => 
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMaster();

                    master.RefreshCurrentPage("/website/partials/cms/pinningrules");

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

            Handle.GET("/website/partials/cms/blendingpoints", () => {
                CmsBlendingPointsPage page = new CmsBlendingPointsPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/website/partials/cms/catchingrules", () => {
                CmsCatchingRulesPage page = new CmsCatchingRulesPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/website/partials/cms/pinningrules", () => {
                CmsPinningRulesPage page = new CmsPinningRulesPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/website/partials/deny", () =>
            {
                return new DenyPage();
            });
        }

        protected MasterPage GetMaster()
        {
            return Self.GET<MasterPage>("/website/master");
        }

        protected MasterPage GetMasterFromSession(Session session)
        {
            if (session != null)
            {
                if (session.Data is MasterPage)
                {
                    return (MasterPage)session.Data;
                }
            }
            return null;
        }
    }
}
