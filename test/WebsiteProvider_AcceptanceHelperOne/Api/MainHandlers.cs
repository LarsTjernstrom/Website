using Starcounter;

namespace WebsiteProvider_AcceptanceHelperOne
{
    public class MainHandlers
    {
        public void Register()
        {
            Handle.GET("/WebsiteProvider_AcceptanceHelperOne", () => Db.Scope(() => new AcceptanceHelperOnePage()));

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/EmptyJson", () => new Json());
            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/EmptyPage",
                () => new Page { Html = "/WebsiteProvider_AcceptanceHelperOne/Master.html" });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/ResetData", () =>
            {
                var dataHelper = new DataHelper();
                dataHelper.ResetData();
                Handle.SetOutgoingStatusCode(200);
                return "Catching rules are reseted";
            });

            // TODO : Remove this handler when merging with the PR Website#65
            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/SetupPageLoadTests", () =>
            {
                var dataHelper = new DataHelper();
                dataHelper.GenerateData();
                Handle.SetOutgoingStatusCode(200);
                return "Data generated";
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/SetupCatchingRuleWildcardTests", () =>
            {
                var dataHelper = new DataHelper();
                dataHelper.SetCatchingRulesWithWildcards();
                Handle.SetOutgoingStatusCode(200);
                return "Data for testing catching rule wildcards is set up";
            });
        }
    }
}