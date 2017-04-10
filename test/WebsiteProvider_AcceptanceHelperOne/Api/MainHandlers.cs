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
            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/EmptyPage",
                () => new Page { Html = "/WebsiteProvider_AcceptanceHelperOne/Master.html" });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/SetDefaultCatchingRules", () =>
            {
                DataHelper.SetDefaultCatchingRules();
                return Response.FromStatusCode(200);
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/SetupNoFinalCatchAllRulesTest", () =>
            {
                DataHelper.SetNoFinalCatchAllRules();
                return Response.FromStatusCode(200);
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne/SetupNoCatchAllRuleTest", () =>
            {
                DataHelper.DeleteCatchAllRules();
                return Response.FromStatusCode(200);
            });
        }
    }
}