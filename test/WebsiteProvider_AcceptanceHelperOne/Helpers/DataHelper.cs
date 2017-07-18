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
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebMap");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebUrl");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebSection");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebTemplate");
            });
        }

        public void SetDefaultCatchingRules()
        {
            Db.Transact(() =>
            {
                var defaultSurface = GenerateDefaultSurface();
                var holyGrailSurface = GenerateHolyGrailSurface();

                var webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ? OR wu.Url IS NULL", string.Empty).First
                    ?? new WebUrl
                    {
                        Template = defaultSurface,
                        Url = string.Empty,
                        IsFinal = true
                    };
                webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?",
                             "/WebsiteProvider_AcceptanceHelperOne/SimplePage").First ??
                         new WebUrl
                         {
                             Template = holyGrailSurface,
                             Url = "/WebsiteProvider_AcceptanceHelperOne/SimplePage",
                             IsFinal = true
                         };
            });
        }

        public void SetNoFinalCatchAllRules()
        {
            Db.Transact(() =>
            {
                SetCatchingRulesForCatchAll(true);
            });
        }

        public void SetNoCatchAllRules()
        {
            Db.Transact(() =>
            {
                SetCatchingRulesForCatchAll(false);
            });
        }

        private void SetCatchingRulesForCatchAll(bool createCatchAllRule)
        {
            if (createCatchAllRule)
            {
                var defaultSurface = GenerateDefaultSurface();
                var webUrl2 = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ? OR wu.Url IS NULL", string.Empty).First
                        ?? new WebUrl
                        {
                            Url = string.Empty,
                        };
                webUrl2.Template = defaultSurface;
                webUrl2.IsFinal = false;

                var catchAllRules = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE (wu.Url IS NULL OR wu.Url = ?) AND wu.IsFinal = ?", string.Empty, true);
                foreach (WebUrl rule in catchAllRules)
                {
                    rule.IsFinal = false;
                }
            }
            else
            {
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebUrl WHERE Url IS NULL OR Url = ''");
            }

            var holyGrailSurface = GenerateHolyGrailSurface();
            var webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?",
                             "/WebsiteProvider_AcceptanceHelperOne").First ??
                         new WebUrl
                         {
                             Url = "/WebsiteProvider_AcceptanceHelperOne",
                         };
            webUrl.Template = holyGrailSurface;
            webUrl.IsFinal = true;
        }

        public void SetCatchingRulesWithWildcards()
        {
            Db.Transact(() =>
            {
                var holyGrailSurface = GenerateHolyGrailSurface();
                var webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?",
                                 "/WebsiteProvider_AcceptanceHelperTwo/content/{?}").First ??
                             new WebUrl
                             {
                                 Url = "/WebsiteProvider_AcceptanceHelperTwo/content/{?}",
                             };
                webUrl.Template = holyGrailSurface;
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
            const string surfaceName = "TestDefaultSurface";
            var surface = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.Name = ?", surfaceName).First;

            if (surface != null) return surface;

            surface = new WebTemplate
            {
                Name = surfaceName,
                Html = "/websiteeditor/surfaces/DefaultSurface.html"
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

        protected WebTemplate GenerateHolyGrailSurface()
        {
            const string surfaceName = "TestHolyGrailSurface";
            var surface = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.Name = ?", surfaceName).First;

            if (surface != null) return surface;

            surface = new WebTemplate
            {
                Name = surfaceName,
                Html = "/Websiteeditor/surfaces/HolyGrailSurface.html"
            };

            new WebSection()
            {
                Template = surface,
                Name = "Content",
                Default = true
            };

            new WebSection()
            {
                Template = surface,
                Name = "Header",
                Default = false
            };

            new WebSection()
            {
                Template = surface,
                Name = "Left",
                Default = false
            };

            new WebSection()
            {
                Template = surface,
                Name = "Right",
                Default = false
            };

            new WebSection()
            {
                Template = surface,
                Name = "Footer",
                Default = false
            };

            return surface;
        }
    }
}
