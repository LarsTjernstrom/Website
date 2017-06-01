using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Ui
{
    public class AcceptanceHelperTwoContentPage : BasePage
    {
        [FindsBy(How = How.XPath, Using = "//span")]
        public IWebElement ContentElement { get; set; }

        public AcceptanceHelperTwoContentPage(IWebDriver driver) : base(driver)
        {
        }

        public AcceptanceHelperTwoContentPage GoTo(string resourceName)
        {
            Driver.Navigate().GoToUrl($"{Config.AcceptanceHelperTwoUrl}/content/{resourceName}");
            return this;
        }
    }
}