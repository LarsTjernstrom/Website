using Starcounter;

namespace WebsiteProvider_AcceptanceHelperTwo
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/WebsiteProvider_AcceptanceHelperTwo", () => Db.Scope(() => new AcceptanceHelperTwoPage().Init()));
        }
    }
}