using System.Web;
using Starcounter;

namespace WebsiteProvider_AcceptanceHelperTwo
{
    public class MainHandlers
    {
        public void Register()
        {
            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo", () => Db.Scope(() => new AcceptanceHelperTwoPage()));

            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/content/{?}",
                (string resourceName) => new ContentPage {ResourceName = resourceName});
            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo/query?{?}",
                (string query) => new QueryPage {QueryString = HttpUtility.UrlDecode(query)});
        }
    }
}