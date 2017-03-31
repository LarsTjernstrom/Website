using OpenQA.Selenium;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Ui
{
    public class AcceptanceHelperOneMasterPage : BasePage
    {
        public AcceptanceHelperOneMasterPage(IWebDriver driver) : base(driver)
        {
        }

        public AcceptanceHelperOneMasterPage GoToMasterPage()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl);
            return this;
        }
    }
}