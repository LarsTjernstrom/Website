using Starcounter;

namespace WebsiteProvider_AcceptanceHelperTwo
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            var dataHelper = new DataHelper();
            var mainHandlers = new MainHandlers();

            dataHelper.GenerateData();
            mainHandlers.Register();
        }
    }
}