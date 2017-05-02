using System;
using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider_AcceptanceHelperOne
{
    public class DataHelper
    {
        public void ResetData()
        {
            Db.Transact(() =>
            {
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebTemplate");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebSection");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebMap");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebUrl");
            });
        }

        public void GenerateData()
        {
            var defaultTemplate = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.Name = ?", "DefaultTemplate").First;
            var sidebarTemplate = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.Name = ?", "SidebarTemplate").First;
            if (defaultTemplate == null || sidebarTemplate == null)
            {
                throw new Exception("Website surfaces is not found.");
            }

            Db.Transact(() =>
            {
                var webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?",
                             "/WebsiteProvider_AcceptanceHelperOne").First ??
                         new WebUrl
                         {
                             Template = defaultTemplate,
                             Url = "/WebsiteProvider_AcceptanceHelperOne",
                             IsFinal = true
                         };
                webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?",
                             "/WebsiteProvider_AcceptanceHelperOne/EmptyPage").First ??
                         new WebUrl
                         {
                             Template = sidebarTemplate,
                             Url = "/WebsiteProvider_AcceptanceHelperOne/EmptyPage",
                             IsFinal = true
                         };
            });
        }

        public void SetCatchingRulesWithWildcards()
        {
            Db.Transact(() =>
            {
                var launcherSurface = GenerateLauncherSurface();
                var webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?",
                                 "/WebsiteProvider_AcceptanceHelperTwo/content/{?}").First ??
                             new WebUrl
                             {
                                 Url = "/WebsiteProvider_AcceptanceHelperTwo/content/{?}",
                             };
                webUrl.Template = launcherSurface;
                webUrl.IsFinal = true;

                var defaultSurface = GenerateDefaultSurface();
                webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?",
                                 "/WebsiteProvider_AcceptanceHelperTwo/query?{?}").First ??
                             new WebUrl
                             {
                                 Url = "/WebsiteProvider_AcceptanceHelperTwo/query?{?}",
                             };
                webUrl.Template = defaultSurface;
                webUrl.IsFinal = true;
            });
        }

        protected WebTemplate GenerateDefaultSurface()
        {
            const string surfaceName = "TestDefaultTemplate";
            var surface = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.Name = ?", surfaceName).First;

            if (surface != null) return surface;

            surface = new WebTemplate
            {
                Name = surfaceName,
                Html = "/Website/templates/DefaultTemplate.html"
            };

            new WebSection
            {
                Template = surface,
                Name = "TopBar",
                Default = false
            };
            new WebSection
            {
                Template = surface,
                Name = "Main",
                Default = true
            };

            return surface;
        }

        protected WebTemplate GenerateLauncherSurface()
        {
            const string surfaceName = "TestLauncherTemplate";
            var surface = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.Name = ?", surfaceName).First;

            if (surface != null) return surface;

            surface = new WebTemplate
            {
                Name = surfaceName,
                Html = "/Website/templates/LauncherTemplate.html"
            };

            new WebSection
            {
                Template = surface,
                Name = "TopBar",
                Default = false
            };
            new WebSection
            {
                Template = surface,
                Name = "LeftBar",
                Default = false
            };
            new WebSection
            {
                Template = surface,
                Name = "Main",
                Default = true
            };

            return surface;
        }
    }
}
