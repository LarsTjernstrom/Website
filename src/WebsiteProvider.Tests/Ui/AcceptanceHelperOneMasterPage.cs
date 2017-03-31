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

        public AcceptanceHelperOneMasterPage GoToEmptyJson()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/EmptyJson");
            return this;
        }

        public AcceptanceHelperOneMasterPage GoToEmptyPage()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/EmptyPage");
            return this;
        }
    }
}