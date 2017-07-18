using System;
using System.Linq;
using Simplified.Ring6;
using Starcounter;

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
                    SetMasterCurrentPage(master, "/WebsiteEditor/partials/general");

                    return master;
                });
            });

            Handle.GET("/WebsiteEditor/surface/{?}/blendingpoints", (string key) =>
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMasterPageFromSession();
                    master.ShowNavigation = true;
                    master.Surface.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ?", key).First();
                    SetMasterCurrentPage(master, "/WebsiteEditor/partials/blendingpoints");

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
                    SetMasterCurrentPage(master, "/WebsiteEditor/partials/catchingrules");

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
                    SetMasterCurrentPage(master, "/WebsiteEditor/partials/pinningrules");

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

            Handle.GET("/WebsiteEditor/partials/general", () => new GeneralPage());

            Handle.GET("/WebsiteEditor/partials/blendingpoints", () => new BlendingPointsPage());

            Handle.GET("/WebsiteEditor/partials/catchingrules", () => new CatchingRulesPage());

            Handle.GET("/WebsiteEditor/partials/pinningrules", () => new PinningRulesPage());

            Handle.GET("/WebsiteEditor/partials/deny", () => new DenyPage());
        }

        protected MasterPage GetMasterPageFromSession()
        {
            MasterPage master = Session.Ensure().Store[nameof(MasterPage)] as MasterPage;

            if (master == null)
            {
                master = new MasterPage();
                Session.Current.Store[nameof(MasterPage)] = master;
            }
 
            return master;
        }

        private static void SetMasterCurrentPage(MasterPage master, string uri)
        {
            try
            {
                master.RefreshCurrentPage(uri);
            }
            catch (InvalidOperationException ex)
            {
                master.CurrentPage = new ErrorPage { Message = ex.Message };
            }
        }
    }
}
