using Starcounter;

namespace WebsiteProvider_AcceptanceHelperTwo
{
    public class MainHandlers
    {
        public void Register()
        {
            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo", () => Db.Scope(() => new AcceptanceHelperTwoPage()));
        }
    }
}