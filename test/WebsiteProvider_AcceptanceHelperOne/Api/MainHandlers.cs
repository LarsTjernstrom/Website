using System;
using Starcounter;

namespace WebsiteProvider_AcceptanceHelperOne
{
    public class MainHandlers
    {
        public DataHelper DataHelper { get; }

        public MainHandlers(DataHelper dataHelper)
        {
            DataHelper = dataHelper ?? throw new ArgumentNullException(nameof(dataHelper));
        }

        public void Register()
        {
            Handle.GET("/WebsiteProvider_AcceptanceHelperOne", () => Db.Scope(() => new AcceptanceHelperOnePage()));

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/EmptyJson", () => new Json());
            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/SimplePage",
                () => new Page { Html = "/WebsiteProvider_AcceptanceHelperOne/SimplePage.html" });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/ResetData", () =>
            {
                DataHelper.ResetData();
                Handle.SetOutgoingStatusCode(200);
                return "Catching rules are reseted";
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/SetDefaultCatchingRules", () =>
            {
                DataHelper.SetDefaultCatchingRules();
                Handle.SetOutgoingStatusCode(200);
                return "Default catching rules are successfully set";
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/SetupNoFinalCatchAllRulesTest", () =>
            {
                DataHelper.SetNoFinalCatchAllRules();
                Handle.SetOutgoingStatusCode(200);
                return "Data for testing catching rules (without existing final catch-all rules) is set";
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/SetupNoCatchAllRuleTest", () =>
            {
                DataHelper.SetNoCatchAllRules();
                Handle.SetOutgoingStatusCode(200);
                return "Data for testing catching rules (without existing catch-all rules) is set";
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/SetupCatchingRuleWildcardTests", () =>
            {
                DataHelper.SetCatchingRulesWithWildcards();
                Handle.SetOutgoingStatusCode(200);
                return "Data for testing catching rule wildcards is set up";
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/SetupCatchingRuleHeadersTests", () =>
            {
                DataHelper.SetCatchingRulesHeaders();
                Handle.SetOutgoingStatusCode(200);
                return "Data for testing catching rule headers is set up";
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/SetupPinningRulesMappingTests", () =>
            {
                DataHelper.SetupPinningRules();
                Handle.SetOutgoingStatusCode(200);
                return "Data for testing pinning rules mapping/unmapping is set up";
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/pin/{?}/delete", (string pinId) =>
            {
                if (this.DataHelper.DeleteWebMap(pinId))
                {
                    Handle.SetOutgoingStatusCode(200);
                    return "Pinning rule deleted";
                }
                else
                {
                    Handle.SetOutgoingStatusCode(404);
                    return "Pinning rule is not found";
                }
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/pin/{?}/edit/{?}", (string pinId, string newPinId) =>
            {
                if (this.DataHelper.EditWebMap(pinId, newPinId))
                {
                    Handle.SetOutgoingStatusCode(200);
                    return "Pinning rule edited";
                }
                else
                {
                    Handle.SetOutgoingStatusCode(404);
                    return "Pinning rule is not found";
                }
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/pin/{?}/change-url", (string pinId) =>
            {
                if (this.DataHelper.ChangeWebMapUrl(pinId))
                {
                    Handle.SetOutgoingStatusCode(200);
                    return "Pinning rule edited";
                }
                else
                {
                    Handle.SetOutgoingStatusCode(404);
                    return "Pinning rule is not found";
                }
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/section/renew/{?}", (string sectionId) =>
            {
                if (this.DataHelper.RenewWebSectionForPinningRules(sectionId))
                {
                    Handle.SetOutgoingStatusCode(200);
                    return "Blending points are deleted and recreated";
                }
                else
                {
                    Handle.SetOutgoingStatusCode(404);
                    return "Blending point is not found";
                }
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/surfaces/renew", () =>
            {
                DataHelper.RenewWebTemplatesForPinningRules();
                Handle.SetOutgoingStatusCode(200);
                return "Surfaces are deleted and recreated";
            });
        }
    }
}