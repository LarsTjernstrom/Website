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

            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/pin1", () => new PinPage { MarkerText = "Pin 1" });
            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/pin2", () => new PinPage { MarkerText = "Pin 2" });
            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/pin3", () => new PinPage { MarkerText = "Pin 3" });
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