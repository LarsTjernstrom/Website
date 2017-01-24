using System;
using Starcounter;
using Simplified.Ring6;

namespace Website {
    public class DataHelper {
        public void ClearData() {
            Db.Transact(() => {
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebTemplate");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebSection");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebMap");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebUrl");
            });
        }

        public void GenerateData() {
            if (Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt").First != null) {
                return;
            }

            Db.Transact(() => {
                ClearData();

                // DefaultTemplate

                WebTemplate template = new WebTemplate()
                {
                    Default = true,
                    Name = "DefaultTemplate",
                    Html = "/Website/templates/DefaultTemplate.html"
                };

                WebSection topbar = new WebSection()
                {
                    Template = template,
                    Name = "TopBar",
                    Default = false
                };

                WebSection main = new WebSection()
                {
                    Template = template,
                    Name = "Main",
                    Default = true
                };

                new WebMap() { Section = topbar, ForeignUrl = "/signin/user", SortNumber = 1 };

                // AppHubTemplate

                template = new WebTemplate()
                {
                    Default = false,
                    Name = "AppHubTemplate",
                    Html = "/Website/templates/AppHubTemplate.html"
                };

                WebSection navigation = new WebSection() {
                    Template = template,
                    Name = "Navigation",
                    Default = false
                };

                WebSection header = new WebSection() {
                    Template = template,
                    Name = "Header",
                    Default = true
                };

                WebSection left = new WebSection() {
                    Template = template,
                    Name = "Left",
                    Default = false
                };

                WebSection right = new WebSection() {
                    Template = template,
                    Name = "Right",
                    Default = false
                };

                WebSection footer = new WebSection() {
                    Template = template,
                    Name = "Footer",
                    Default = false
                };

                WebUrl homeUrl = new WebUrl() {
                    Template = template,
                    Url = "/content/dynamic/apps"
                };

                WebUrl appsUrl = new WebUrl() {
                    Template = template,
                    Url = "/content/dynamic/apps/wanted-apps"
                };

                WebUrl profileUrl = new WebUrl() {
                    Template = template,
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
            });
        }
    }
}
