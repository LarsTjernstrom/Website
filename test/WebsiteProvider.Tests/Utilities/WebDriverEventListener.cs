using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;

namespace WebsiteProvider.Tests.Utilities
{
    public class WebDriverEventListener : EventFiringWebDriver
    {
        public WebDriverEventListener(IWebDriver driver) : base(driver)
        {
            ExceptionThrown += (sender, e) =>
            {
                if (e != null && sender != null)
                {
                    //Screenshot.MakeScreenshot(e.Driver);
                }
            };
        }
    }
}