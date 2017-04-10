using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Ui
{
    public class AcceptanceHelperOneMasterPage : BasePage
    {
        [FindsBy(How = How.XPath, Using = "//a[@href = '/WebsiteProvider_AcceptanceHelperTwo']")]
        public IWebElement AcceptanceHelperTwoLink { get; set; }

        [FindsBy(How = How.XPath, Using = "//a[@href = '/WebsiteProvider_AcceptanceHelperOne/SimplePage']")]
        public IWebElement AcceptanceHelperOneSimplePageLink { get; set; }

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

        public AcceptanceHelperOneSimplePage GoToSimplePage()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SimplePage");
            return new AcceptanceHelperOneSimplePage(Driver);
        }

        public AcceptanceHelperTwoMasterPage GoToAcceptanceHelperTwoPage()
        {
            ClickOn(AcceptanceHelperTwoLink);
            return new AcceptanceHelperTwoMasterPage(Driver);
        }
    }
}