using Starcounter;

namespace WebsiteProvider_AcceptanceHelperOne
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/WebsiteProvider_AcceptanceHelperOne", () => Db.Scope(() => new AcceptanceHelperOnePage().Init()));
        }
    }
}