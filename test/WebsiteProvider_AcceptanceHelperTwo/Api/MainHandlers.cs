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

        protected AcceptanceHelperTwoPage GetMasterFromSession()
        {
            if (Session.Current == null)
            {
                Session.Current = new Session(SessionOptions.PatchVersioning);
            }

            var master = Session.Current.Data as AcceptanceHelperTwoPage;

            if (master == null)
            {
                master = new AcceptanceHelperTwoPage();
                Session.Current.Data = master;
            }

            return master;
        }

        public void Register()
        {
            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo", () => Db.Scope(() => GetMasterFromSession()));

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
        }
    }
}