using Starcounter;
using Simplified.Ring6;

namespace WebsiteEditor
{
    public class DataHelper
    {
        public void ClearData()
        {
            Db.Transact(() =>
            {
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebUrlProperty");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebMap");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebUrl");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebSection");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebTemplate");
            });
        }

        public void GenerateData()
        {
            if (Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt").First != null)
            {
                return;
            }

            Db.Transact(() =>
            {
                ClearData();
                GenerateDefaultSurface();
                GenerateSidebarSurface();
                GenerateHolyGrailSurface();
            });
        }

        public void GenerateDefaultSurface()
        {
            var surface = new WebTemplate()
            {
                Name = "DefaultSurface",
                Html = "/Websiteeditor/surfaces/DefaultSurface.html"
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

            var catchAllUrl = new WebUrl()
            {
                Template = surface,
                Url = null,
                IsFinal = true
            };

            new WebMap() { Section = topbar, ForeignUrl = "/signin/user", SortNumber = 1 };
        }

        public void GenerateSidebarSurface()
        {
            var surface = new WebTemplate()
            {
                Name = "SidebarSurface",
                Html = "/Websiteeditor/surfaces/SidebarSurface.html"
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
                Url = "/websiteeditor/surfaces"
            };

            new WebMap() { Url = templatesUrl, Section = sidebarLeft, ForeignUrl = "/websiteeditor/help?topic=surfaces", SortNumber = 1 };
        }

        public void GenerateHolyGrailSurface()
        {
            var surface = new WebTemplate()
            {
                Name = "HolyGrailSurface",
                Html = "/Websiteeditor/surfaces/HolyGrailSurface.html"
            };

            var content = new WebSection()
            {
                Template = surface,
                Name = "Content",
                Default = true
            };

            var header = new WebSection()
            {
                Template = surface,
                Name = "Header",
                Default = false
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
                Url = "/content/dynamic/apps",
                IsFinal = true
            };

            var appsUrl = new WebUrl()
            {
                Template = surface,
                Url = "/content/dynamic/apps/wanted-apps",
                IsFinal = true
            };

            var profileUrl = new WebUrl()
            {
                Template = surface,
                Url = "/content/dynamic/userprofile",
                IsFinal = true
            };

            new WebMap() { Section = header, ForeignUrl = "/signin/user", SortNumber = 1 };
            new WebMap() { Section = header, ForeignUrl = "/content/dynamic/navigation", SortNumber = 2, };
            new WebMap() { Url = homeUrl, Section = header, ForeignUrl = "/content/dynamic/index/header", SortNumber = 3 };
            new WebMap() { Url = homeUrl, Section = header, ForeignUrl = "/signin/signinuser", SortNumber = 4 };
            new WebMap() { Url = homeUrl, Section = header, ForeignUrl = "/registration", SortNumber = 5 };
            new WebMap() { Url = homeUrl, Section = header, ForeignUrl = "/content/dynamic/index/registration", SortNumber = 6 };

            new WebMap() { Url = homeUrl, Section = left, ForeignUrl = "/content/dynamic/index/left", SortNumber = 1 };

            new WebMap() { Url = homeUrl, Section = right, ForeignUrl = "/content/dynamic/index/right", SortNumber = 1 };

            new WebMap() { Url = homeUrl, Section = footer, ForeignUrl = "/content/dynamic/index/footer", SortNumber = 1 };

            new WebMap() { Url = appsUrl, Section = header, ForeignUrl = "/content/dynamic/apps/header", SortNumber = 1 };
            new WebMap() { Url = appsUrl, Section = header, ForeignUrl = "/content/dynamic/apps/footer", SortNumber = 2 };

            new WebMap() { Url = profileUrl, Section = header, ForeignUrl = "/content/dynamic/userprofile/header", SortNumber = 1 };
            new WebMap() { Url = profileUrl, Section = content, ForeignUrl = "/userprofile", SortNumber = 2 };
            new WebMap() { Url = profileUrl, Section = footer, ForeignUrl = "/content/dynamic/userprofile/footer", SortNumber = 3 };
        }
    }
}
