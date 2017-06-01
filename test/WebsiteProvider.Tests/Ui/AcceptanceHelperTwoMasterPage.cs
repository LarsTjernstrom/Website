using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Ui
{
    public class AcceptanceHelperTwoMasterPage : BasePage
    {
        [FindsBy(How = How.XPath, Using = "//h2")]
        public IWebElement HeaderElement { get; set; }

        public AcceptanceHelperTwoMasterPage(IWebDriver driver) : base(driver)
        {
        }

        public AcceptanceHelperTwoMasterPage LoadMasterPage()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperTwoUrl);
            return this;
        }
    }
}