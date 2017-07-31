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
            DataHelper = dataHelper ?? throw new ArgumentNullException(nameof(dataHelper));
        }

        public void Register()
        {
            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo", () => this.GetMasterPageFromSession(new AcceptanceHelperTwoPage()));

            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/SetDefaultCatchingRules", () =>
            {
                DataHelper.SetDefaultCatchingRules();
                Handle.SetOutgoingStatusCode(200);
                return "Default catching rules are successfully set";
            });

            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/content/{?}",
                (string resourceName) => this.GetMasterPageFromSession(new ContentPage { ResourceName = resourceName }));
            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/query?{?}",
                (string query) => this.GetMasterPageFromSession(new QueryPage { QueryString = HttpUtility.UrlDecode(query) }));

            for (int i = 1; i <= 10; i++)
            {
                var markerText = "Pin " + i;
                Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/pin" + i, () => new PinPage { MarkerText = markerText });
            }

            // Should be called "/WebsiteProvider_AcceptanceHelperOne/SetupPinningRulesMappingTests" first
            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/pinning",
                () => Db.Scope(() => this.GetMasterPageFromSession(PinningPage.Create())));
        }

        protected MasterPage GetMasterPageFromSession(Json content = null)
        {
            MasterPage master = Session.Ensure().Store[nameof(MasterPage)] as MasterPage;

            if (master == null)
            {
                master = new MasterPage();
                Session.Current.Store[nameof(MasterPage)] = master;
            }

            if (content != null)
            {
                master.Content = content;
            }

            return master;
        }
    }
}