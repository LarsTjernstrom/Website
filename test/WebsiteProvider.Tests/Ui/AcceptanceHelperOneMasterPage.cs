using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Ui
{
    public class AcceptanceHelperOneMasterPage : BasePage
    {
        [FindsBy(How = How.XPath, Using = "//a")]
        public IWebElement AcceptanceHelperOnePageLink { get; set; }

        [FindsBy(How = How.XPath, Using = "//h1")]
        public IWebElement H1Element { get; set; }

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

        public AcceptanceHelperTwoMasterPage GoToAcceptanceHelperTwoPage()
        {
            ClickOn(AcceptanceHelperOnePageLink);
            return new AcceptanceHelperTwoMasterPage(Driver);
        }
    }
}