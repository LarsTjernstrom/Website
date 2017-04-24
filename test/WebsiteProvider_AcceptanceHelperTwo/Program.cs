using System;
using System.Globalization;
using System.Linq;
using Starcounter;

namespace WebsiteProvider_AcceptanceHelperTwo
{
    class Program
    {
        const string MiddlewareTestCookieName = "AcceptanceHelperTwoMiddlewareTestCookie";

        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Application.Current.Use((request, response) =>
            {
                // Set cookie to test handling of a request by a middleware
                if (request.Cookies.Select(Cookie.Parse).All(x => x.Name != MiddlewareTestCookieName))
                {
                    var cookie = new Cookie
                    {
                        Name = MiddlewareTestCookieName,
                        Value = $"{MiddlewareTestCookieName}-{DateTime.Now.ToString(CultureInfo.InvariantCulture)}",
                        Expires = DateTime.Now.AddDays(1)
                    };
                    response.Cookies.Add(cookie.ToString());
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