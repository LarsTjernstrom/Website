using System;
using System.Linq;
using Simplified.Ring6;
using Starcounter;
using WebsiteEditor;
using WebsiteEditor.ViewModels;

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

            Handle.GET("/websiteeditor", () => Self.GET("/WebsiteEditor/surfaceGroups"));

            Handle.GET("/WebsiteEditor/surfaceGroups", () =>
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMasterPageFromSession();
                    master.ShowNavigation = false;
                    master.RefreshCurrentPage("/WebsiteEditor/partials/surfaceGroups");

                    return master;
                });
            });

            Handle.GET("/WebsiteEditor/surface/{?}/general", (string key) =>
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMasterPageFromSession();
                    master.ShowNavigation = true;
                    master.Surface.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ?", key).First();

                    master.RefreshCurrentPage("/WebsiteEditor/partials/general");

                    return master;
                });
            });

            //Handle.GET("/WebsiteEditor/surfaces", () =>
            //{
            //    return Db.Scope<MasterPage>(() =>
            //    {
            //        MasterPage master = this.GetMasterPageFromSession();

            //        master.RefreshCurrentPage("/WebsiteEditor/partials/surfaces");

            //        return master;
            //    });
            //});

            Handle.GET("/WebsiteEditor/surface/{?}/blendingpoints", (string key) =>
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMasterPageFromSession();
                    master.ShowNavigation = true;
                    master.Surface.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ?", key).First();

                    master.RefreshCurrentPage("/WebsiteEditor/partials/blendingpoints");

                    return master;
                });
            });

            Handle.GET("/WebsiteEditor/surface/{?}/catchingrules", (string key) =>
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMasterPageFromSession();
                    master.ShowNavigation = true;
                    master.Surface.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ?", key).First();

                    master.RefreshCurrentPage("/WebsiteEditor/partials/catchingrules");

                    return master;
                });
            });

            Handle.GET("/WebsiteEditor/surface/{?}/pinningrules", (string key) =>
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMasterPageFromSession();
                    master.ShowNavigation = true;
                    master.Surface.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ?", key).First();

                    master.RefreshCurrentPage("/WebsiteEditor/partials/pinningrules");

                    return master;
                });
            });
        }

        protected void RegisterPartials()
        {
            Handle.GET("/WebsiteEditor/partials/surfaceGroups", () =>
            {
                SurfaceGroupsPage page = new SurfaceGroupsPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/WebsiteEditor/partials/general", () =>
            {
                GeneralPage page = new GeneralPage();

                return page;
            });

            //Handle.GET("/WebsiteEditor/partials/surfaces", () =>
            //{
            //    CmsSurfacesPage page = new CmsSurfacesPage();

            //    page.RefreshData();

            //    return page;
            //});

            Handle.GET("/WebsiteEditor/partials/blendingpoints", () =>
            {
                BlendingPointsPage page = new BlendingPointsPage();

                return page;
            });

            Handle.GET("/WebsiteEditor/partials/catchingrules", () =>
            {
                CatchingRulesPage page = new CatchingRulesPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/WebsiteEditor/partials/pinningrules", () =>
            {
                PinningRulesPage page = new PinningRulesPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/WebsiteEditor/partials/deny", () => new DenyPage());
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
