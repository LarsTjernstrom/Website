using System;
using Starcounter;
using Simplified.Ring6;

namespace Website
{
    public class DataHelper
    {
        public void ClearData()
        {
            Db.Transact(() => {
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebTemplate");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebSection");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebMap");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebUrl");
            });
        }

        public void GenerateData()
        {
            if (Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt").First != null)
            {
                return;
            }

            Db.Transact(() => {
                ClearData();
                GenerateDefaultSurface();
                GenerateSidebarSurface();
                GenerateAppHubSurface();
                GenerateLauncherSurface();
            });
        }

        public void GenerateDefaultSurface()
        {
            var surface = new WebTemplate()
            {
                Default = true,
                Name = "DefaultTemplate",
                Html = "/Website/templates/DefaultTemplate.html"
            };

            var topbar = new WebSection()
            {
                Template = surface,
                Name = "TopBar",
                Default = false
            };

            var main = new WebSection()
            {
                Template = surface,
                Name = "Main",
                Default = true
            };

            new WebMap() { Section = topbar, ForeignUrl = "/signin/user", SortNumber = 1 };
        }

        public void GenerateSidebarSurface()
        {
            var surface = new WebTemplate()
            {
                Default = false,
                Name = "SidebarTemplate",
                Html = "/Website/templates/SidebarTemplate.html"
            };

            var sidebarLeft = new WebSection()
            {
                Template = surface,
                Name = "Left",
                Default = false
            };

            new WebSection()
            {
                Template = surface,
                Name = "Right",
                Default = true
            };

            var templatesUrl = new WebUrl()
            {
                Template = surface,
                Url = "/website/cms/templates"
            };

            new WebMap() { Url = templatesUrl, Section = sidebarLeft, ForeignUrl = "/website/help?topic=template", SortNumber = 1 };
        }

        public void GenerateAppHubSurface()
        {
            var surface = new WebTemplate()
            {
                Default = false,
                Name = "AppHubTemplate",
                Html = "/Website/templates/AppHubTemplate.html"
            };

            var navigation = new WebSection()
            {
                Template = surface,
                Name = "Navigation",
                Default = false
            };

            var header = new WebSection()
            {
                Template = surface,
                Name = "Header",
                Default = true
            };

            var left = new WebSection()
            {
                Template = surface,
                Name = "Left",
                Default = false
            };

            var right = new WebSection()
            {
                Template = surface,
                Name = "Right",
                Default = false
            };

            var footer = new WebSection()
            {
                Template = surface,
                Name = "Footer",
                Default = false
            };

            var homeUrl = new WebUrl()
            {
                Template = surface,
                Url = "/content/dynamic/apps"
            };

            var appsUrl = new WebUrl()
            {
                Template = surface,
                Url = "/content/dynamic/apps/wanted-apps"
            };

            var profileUrl = new WebUrl()
            {
                Template = surface,
                Url = "/content/dynamic/userprofile"
            };

            new WebMap() { Section = navigation, ForeignUrl = "/signin/user", SortNumber = 1 };
            new WebMap() { Section = navigation, ForeignUrl = "/content/dynamic/navigation", SortNumber = 2, };
            new WebMap() { Url = homeUrl, Section = navigation, ForeignUrl = "/content/dynamic/index/header", SortNumber = 3 };

            new WebMap() { Url = homeUrl, Section = header, ForeignUrl = "/signin/signinuser", SortNumber = 1 };
            new WebMap() { Url = homeUrl, Section = header, ForeignUrl = "/registration", SortNumber = 2 };
            new WebMap() { Url = homeUrl, Section = header, ForeignUrl = "/content/dynamic/index/registration", SortNumber = 3 };

            new WebMap() { Url = homeUrl, Section = left, ForeignUrl = "/content/dynamic/index/left", SortNumber = 1 };

            new WebMap() { Url = homeUrl, Section = right, ForeignUrl = "/content/dynamic/index/right", SortNumber = 1 };

            new WebMap() { Url = homeUrl, Section = footer, ForeignUrl = "/content/dynamic/index/footer", SortNumber = 1 };

            new WebMap() { Url = appsUrl, Section = header, ForeignUrl = "/content/dynamic/apps/header", SortNumber = 1 };
            new WebMap() { Url = appsUrl, Section = header, ForeignUrl = "/content/dynamic/apps/footer", SortNumber = 2 };

            new WebMap() { Url = profileUrl, Section = header, ForeignUrl = "/content/dynamic/userprofile/header", SortNumber = 1 };
            new WebMap() { Url = profileUrl, Section = header, ForeignUrl = "/userprofile", SortNumber = 2 };
            new WebMap() { Url = profileUrl, Section = footer, ForeignUrl = "/content/dynamic/userprofile/footer", SortNumber = 3 };
        }

        public void GenerateLauncherSurface()
        {
            var surface = new WebTemplate()
            {
                Default = false,
                Name = "LauncherTemplate",
                Html = "/Website/templates/LauncherTemplate.html"
            };

            var topbar = new WebSection()
            {
                Template = surface,
                Name = "TopBar",
                Default = false
            };

            var leftbar = new WebSection()
            {
                Template = surface,
                Name = "LeftBar",
                Default = false
            };

            var main = new WebSection()
            {
                Template = surface,
                Name = "Main",
                Default = true
            };

            new WebMap() { Section = topbar, ForeignUrl = "/signin/user", SortNumber = 1 };
        }
    }
}
