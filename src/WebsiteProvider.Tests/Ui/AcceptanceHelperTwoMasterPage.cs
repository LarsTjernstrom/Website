using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Ui
{
    public class AcceptanceHelperTwoMasterPage : BasePage
    {
        [FindsBy(How = How.XPath, Using = "//h1")]
        public IWebElement H1Element { get; set; }

        public AcceptanceHelperTwoMasterPage(IWebDriver driver) : base(driver)
        {
        }

        public AcceptanceHelperTwoMasterPage GoToMasterPage()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperTwoUrl);
            return this;
        }
    }
}