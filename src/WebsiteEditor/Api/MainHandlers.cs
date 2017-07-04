using System;
using Starcounter;
using WebsiteEditor;

namespace WebsiteEditor
{
    public class MainHandlers
    {
        public void Register()
        {
            RegisterPartials();

            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/WebsiteEditor/help?topic={?}", (string topic) =>
            {
                var json = new CmsHelp();
                return json;
            });

            Handle.GET("/WebsiteEditor/cleardata", () =>
            {
                DataHelper helper = new DataHelper();

                helper.ClearData();

                return 200;
            });

            Handle.GET("/WebsiteEditor/resetdata", () =>
            {
                DataHelper helper = new DataHelper();

                helper.ClearData();
                helper.GenerateData();

                return 200;
            });

            Handle.GET("/websiteeditor", () =>
            {
                return Self.GET("/WebsiteEditor/cms");
            });

            Handle.GET("/WebsiteEditor/cms", () =>
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMasterPageFromSession();

                    master.RefreshCurrentPage("/WebsiteEditor/partials/cms");

                    return master;
                });
            });

            Handle.GET("/WebsiteEditor/cms/surfaces", () => 
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMasterPageFromSession();

                    master.RefreshCurrentPage("/WebsiteEditor/partials/cms/surfaces");

                    return master;
                });
            });

            Handle.GET("/WebsiteEditor/cms/blendingpoints", () => 
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMasterPageFromSession();

                    master.RefreshCurrentPage("/WebsiteEditor/partials/cms/blendingpoints");

                    return master;
                });
            });

            Handle.GET("/WebsiteEditor/cms/catchingrules", () => 
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMasterPageFromSession();

                    master.RefreshCurrentPage("/WebsiteEditor/partials/cms/catchingrules");

                    return master;
                });
            });

            Handle.GET("/WebsiteEditor/cms/pinningrules", () => 
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMasterPageFromSession();

                    master.RefreshCurrentPage("/WebsiteEditor/partials/cms/pinningrules");

                    return master;
                });
            });
        }

        protected void RegisterPartials()
        {
            Handle.GET("/WebsiteEditor/partials/cms", () =>
            {
                CmsPage page = new CmsPage();

                return page;
            });

            Handle.GET("/WebsiteEditor/partials/cms/surfaces", () => {
                CmsSurfacesPage page = new CmsSurfacesPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/WebsiteEditor/partials/cms/blendingpoints", () => {
                CmsBlendingPointsPage page = new CmsBlendingPointsPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/WebsiteEditor/partials/cms/catchingrules", () => {
                CmsCatchingRulesPage page = new CmsCatchingRulesPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/WebsiteEditor/partials/cms/pinningrules", () => {
                CmsPinningRulesPage page = new CmsPinningRulesPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/WebsiteEditor/partials/deny", () =>
            {
                return new DenyPage();
            });
        }

        protected MasterPage GetMasterPageFromSession()
        {
            if (Session.Current == null)
            {
                Session.Current = new Session(SessionOptions.PatchVersioning);
            }

            MasterPage master = Session.Current.Data as MasterPage;

            if (master == null)
            {
                master = new MasterPage();
                Session.Current.Data = master;
            }

            return master;
        }
    }
}
