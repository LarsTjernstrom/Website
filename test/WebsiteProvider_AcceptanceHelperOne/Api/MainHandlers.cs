using System;
using Starcounter;

namespace WebsiteProvider_AcceptanceHelperOne
{
    public class MainHandlers
    {
        public DataHelper DataHelper { get; }

        public MainHandlers(DataHelper dataHelper)
        {
            if (dataHelper == null) throw new ArgumentNullException(nameof(dataHelper));

            DataHelper = dataHelper;
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

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/ResetData", () =>
            {
                DataHelper.ResetData();
                Handle.SetOutgoingStatusCode(200);
                return "Catching rules are reseted";
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/SetupCatchingRuleWildcardTests", () =>
            {
                DataHelper.SetCatchingRulesWithWildcards();
                Handle.SetOutgoingStatusCode(200);
                return "Data for testing catching rule wildcards is set up";
            });
        }
    }
}