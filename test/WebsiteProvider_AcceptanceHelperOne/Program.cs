using Starcounter;

namespace WebsiteProvider_AcceptanceHelperOne
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            var mainHandlers = new MainHandlers(dataHelper);

            mainHandlers.Register();
        }
    }
}