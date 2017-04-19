using Starcounter;

namespace WebsiteProvider_AcceptanceHelperTwo
{
    class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Application.Current.Use((request, response) =>
            {
                if (request.Uri == "/sys/starcounter.html")
                {
                    var inserter = @"<script>
(function () {
  var inserter = document.createElement(""input"");
  inserter.setAttribute(""type"", ""hidden"")
  inserter.setAttribute(""test"", ""acceptance-helper-two-middleware"")
  document.body.insertBefore(inserter, document.body.firstChild);
})();
</script>";
                    return new Response { Body = $"{response.Body}\n{inserter}" };
                }

                return null;
            });

            var dataHelper = new DataHelper();
            var mainHandlers = new MainHandlers();

            dataHelper.GenerateData();
            mainHandlers.Register();
        }
    }
}