using System;
using System.Web;
using Starcounter;

namespace WebsiteProvider_AcceptanceHelperTwo
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
            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo", () => new AcceptanceHelperTwoPage());

            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/SetDefaultCatchingRules", () =>
            {
                DataHelper.SetDefaultCatchingRules();
                Handle.SetOutgoingStatusCode(200);
                return "Default catching rules are successfully set";
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/content/{?}",
                (string resourceName) => new ContentPage {ResourceName = resourceName});
            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/query?{?}",
                (string query) => new QueryPage {QueryString = HttpUtility.UrlDecode(query)});

            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/pin1", () => null);
            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/pin2", () => null);
            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/pin3", () => null);
        }
    }
}