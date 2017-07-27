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

            Handle.GET("/websiteeditor/help?topic={?}", (string topic) =>
            {
                var json = new CmsHelp();
                return json;
            });

            Handle.GET("/websiteeditor/cleardata", () =>
            {
                DataHelper helper = new DataHelper();

                helper.ClearData();

                return 200;
            });

            Handle.GET("/websiteeditor/resetdata", () =>
            {
                DataHelper helper = new DataHelper();

                helper.ClearData();
                helper.GenerateData();

                return 200;
            });

            Handle.GET("/websiteeditor/test", () =>
            {
                var a = new AuthorizedHandlers();
                a.Register();
                return 200;
            });

            Handle.GET("/websiteeditor", () => Self.GET("/websiteeditor/surfacegroups"));

            Handle.GET("/websiteeditor/surfacegroups", () =>
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMasterPageFromSession();
                    master.ShowNavigation = false;

                    master.RefreshCurrentPage("/websiteeditor/partials/surfacegroups");

                    return master;
                });
            });

            Handle.GET("/websiteeditor/surface/{?}/general", (string key) =>
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMasterPageFromSession();
                    master.ShowNavigation = true;
                    master.Surface.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ?", key).First();
                    SetMasterCurrentPage(master, "/websiteeditor/partials/general");

                    return master;
                });
            });

            Handle.GET("/websiteeditor/surface/{?}/blendingpoints", (string key) =>
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMasterPageFromSession();
                    master.ShowNavigation = true;
                    master.Surface.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ?", key).First();
                    SetMasterCurrentPage(master, "/websiteeditor/partials/blendingpoints");

                    return master;
                });
            });

            Handle.GET("/websiteeditor/surface/{?}/catchingrules", (string key) =>
            {
                return Db.Scope<MasterPage>(() =>
                {
                    MasterPage master = this.GetMasterPageFromSession();
                    master.ShowNavigation = true;
                    master.Surface.Data = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Key = ?", key).First();
                    SetMasterCurrentPage(master, "/websiteeditor/partials/catchingrules");

                    return master;
                });
            });
        }

        protected void RegisterPartials()
        {
            Handle.GET("/websiteeditor/partials/surfacegroups", () =>
            {
                SurfaceGroupsPage page = new SurfaceGroupsPage();

                page.RefreshData();

                return page;
            });

            Handle.GET("/websiteeditor/partials/general", () => new GeneralPage());

            Handle.GET("/websiteeditor/partials/blendingpoints", () => new BlendingPointsPage());

            Handle.GET("/websiteeditor/partials/catchingrules", () => new CatchingRulesPage());

            Handle.GET("/websiteeditor/partials/deny", () => new DenyPage());
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
