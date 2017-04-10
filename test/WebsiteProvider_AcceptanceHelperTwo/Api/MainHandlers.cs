using System;
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
                return Response.FromStatusCode(200);
            });
        }
    }
}