using System.Linq;
using Simplified.Ring6;
using Starcounter;

namespace WebsiteProvider_AcceptanceHelperOne
{
    public class DataHelper
    {
        const string DefaultSurfaceName = "TestDefaultSurface";
        const string HolyGrailSurfaceName = "TestHolyGrailSurface";

        public void ResetData()
        {
            Db.Transact(() =>
            {
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebUrlProperty");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebMap");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebUrl");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebSection");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebTemplate");
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebTemplateGroup");
            });
        }

        public void SetDefaultCatchingRules()
        {
            Db.Transact(() =>
            {
                var defaultSurface = GenerateDefaultSurface();
                var holyGrailSurface = GenerateHolyGrailSurface();

                var webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ? OR wu.Url IS NULL", string.Empty).FirstOrDefault()
                    ?? new WebUrl
                    {
                        Template = defaultSurface,
                        Url = string.Empty,
                        IsFinal = true
                    };
                webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?",
                             "/WebsiteProvider_AcceptanceHelperOne/SimplePage").FirstOrDefault() ??
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
                var webUrl2 = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ? OR wu.Url IS NULL", string.Empty).FirstOrDefault()
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
                             "/WebsiteProvider_AcceptanceHelperOne").FirstOrDefault() ??
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
                                 "/WebsiteProvider_AcceptanceHelperTwo/content/{?}").FirstOrDefault() ??
                             new WebUrl
                             {
                                 Url = "/WebsiteProvider_AcceptanceHelperTwo/content/{?}",
                             };
                webUrl.Template = holyGrailSurface;
                webUrl.IsFinal = true;

                var defaultSurface = GenerateDefaultSurface();
                webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ?",
                                 "/WebsiteProvider_AcceptanceHelperTwo/query?{?}").FirstOrDefault() ??
                             new WebUrl
                             {
                                 Url = "/WebsiteProvider_AcceptanceHelperTwo/query?{?}",
                             };
                webUrl.Template = defaultSurface;
                webUrl.IsFinal = true;
            });
        }

        public void SetCatchingRulesHeaders()
        {
            Db.Transact(() =>
            {
                var defaultSurface = this.GenerateDefaultSurface();
                var holyGrailSurface = this.GenerateHolyGrailSurface();
                var webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Template = ? AND (wu.Url = ? OR wu.Url IS NULL)", defaultSurface, string.Empty).FirstOrDefault()
                             ?? new WebUrl
                             {
                                 Template = defaultSurface,
                                 Url = string.Empty
                             };
                webUrl.IsFinal = true;
                webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Template = ? AND (wu.Url = ? OR wu.Url IS NULL)", holyGrailSurface, string.Empty).FirstOrDefault()
                         ?? new WebUrl
                         {
                             Template = holyGrailSurface,
                             Url = string.Empty
                         };
                webUrl.IsFinal = true;
                var header = new WebHttpHeader
                {
                    Url = webUrl,
                    Name = "test-header",
                    Value = "test-header-value"
                };
            });
        }

        public void SetupPinningRules()
        {
            Db.Transact(() =>
            {
                var defaultSurface = GenerateDefaultSurface();
                var webUrl = new WebUrl
                             {
                                 Template = defaultSurface,
                                 Url = null,
                                 IsFinal = true
                             };
                var topBar = defaultSurface.Sections.First(x => x.Name == "TopBar");
                var main = defaultSurface.Sections.First(x => x.Name == "Main");
                for (int i = 1; i <= 3; i++)
                {
                    var webMap = new WebMap
                    {
                        Section = topBar,
                        ForeignUrl = "/WebsiteProvider_AcceptanceHelperTwo/pin" + i
                    };
                }
                for (int i = 6; i <= 7; i++)
                {
                    var webMap = new WebMap
                    {
                        Section = main,
                        ForeignUrl = "/WebsiteProvider_AcceptanceHelperTwo/pin" + i
                    };
                }
                webUrl = new WebUrl
                {
                    Template = defaultSurface,
                    Url = "/WebsiteProvider_AcceptanceHelperTwo",
                    IsFinal = true
                };
                for (int i = 8; i <= 9; i++)
                {
                    var webMap = new WebMap
                    {
                        Section = topBar,
                        Url = webUrl,
                        ForeignUrl = "/WebsiteProvider_AcceptanceHelperTwo/pin" + i
                    };
                }
            });
        }

        public bool DeleteWebMap(string pinningRuleId)
        {
            bool isDeleted = false;
            Db.Transact(() =>
            {
                var webMap = Db.SQL<WebMap>("SELECT m FROM Simplified.Ring6.WebMap m WHERE m.ForeignUrl = ?", "/WebsiteProvider_AcceptanceHelperTwo/pin" + pinningRuleId).FirstOrDefault();
                isDeleted = webMap != null;
                webMap?.Delete();
            });
            return isDeleted;
        }

        public bool EditWebMap(string pinningRuleId, string newId)
        {
            bool isEdited = false;
            Db.Transact(() =>
            {
                var webMap = Db.SQL<WebMap>("SELECT m FROM Simplified.Ring6.WebMap m WHERE m.ForeignUrl = ?", "/WebsiteProvider_AcceptanceHelperTwo/pin" + pinningRuleId).FirstOrDefault();
                if (webMap != null)
                {
                    webMap.ForeignUrl = "/WebsiteProvider_AcceptanceHelperTwo/pin" + newId;
                    isEdited = true;
                }
            });
            return isEdited;
        }

        public bool RenewWebSectionForPinningRules(string sectionId)
        {
            bool isUpdated = false;
            Db.Transact(() =>
            {
                var section = Db.SQL<WebSection>("SELECT s FROM Simplified.Ring6.WebSection s WHERE s.Name = ?", sectionId).FirstOrDefault();
                if (section != null)
                {
                    section.Delete();
                    new WebSection
                    {
                        Template = Db.SQL<WebTemplate>("SELECT t FROM Simplified.Ring6.WebTemplate t WHERE t.Name = ?", DefaultSurfaceName).First(),
                        Name = sectionId,
                        Default = false
                    };
                    isUpdated = true;
                }
            });
            return isUpdated;
        }

        public void RenewWebTemplatesForPinningRules()
        {
            Db.Transact(() =>
            {
                Db.SlowSQL("DELETE FROM Simplified.Ring6.WebTemplate");
                var defaultSurface = this.GenerateDefaultSurface();
                var webUrl = Db.SQL<WebUrl>("SELECT wu FROM Simplified.Ring6.WebUrl wu WHERE wu.Url = ? OR wu.Url IS NULL", string.Empty).FirstOrDefault()
                             ?? new WebUrl
                             {
                                 Url = string.Empty,
                                 IsFinal = true
                             };
                webUrl.Template = defaultSurface;
            });
        }

        public bool ChangeWebMapUrl(string pinningRuleId)
        {
            bool isEdited = false;
            Db.Transact(() =>
            {
                var webMap = Db.SQL<WebMap>("SELECT m FROM Simplified.Ring6.WebMap m WHERE m.ForeignUrl = ?", "/WebsiteProvider_AcceptanceHelperTwo/pin" + pinningRuleId).FirstOrDefault();
                if (webMap != null)
                {
                    webMap.Url = string.IsNullOrEmpty(webMap.Url?.Url)
                        ? Db.SQL<WebUrl>("SELECT u FROM Simplified.Ring6.WebUrl u WHERE u.Url = ?", "/WebsiteProvider_AcceptanceHelperTwo").First()
                        : Db.SQL<WebUrl>("SELECT u FROM Simplified.Ring6.WebUrl u WHERE u.Url IS NULL OR u.Url = ?", string.Empty).First();
                    isEdited = true;
                }
            });
            return isEdited;
        }

        protected WebTemplate GenerateDefaultSurface()
        {
            var surface = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.Name = ?", DefaultSurfaceName).FirstOrDefault();

            if (surface != null) return surface;

            surface = new WebTemplate
            {
                Name = DefaultSurfaceName,
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
            var surface = Db.SQL<WebTemplate>("SELECT wt FROM Simplified.Ring6.WebTemplate wt WHERE wt.Name = ?", HolyGrailSurfaceName).FirstOrDefault();

            if (surface != null) return surface;

            surface = new WebTemplate
            {
                Name = HolyGrailSurfaceName,
                Html = "/Websiteeditor/surfaces/HolyGrailSurface.html"
            };

            new WebSection
            {
                Template = surface,
                Name = "Content",
                Default = true
            };

            new WebSection
            {
                Template = surface,
                Name = "Header",
                Default = false
            };

            new WebSection
            {
                Template = surface,
                Name = "Left",
                Default = false
            };

            new WebSection
            {
                Template = surface,
                Name = "Right",
                Default = false
            };

            new WebSection
            {
                Template = surface,
                Name = "Footer",
                Default = false
            };

            return surface;
        }
    }
}
