using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace WebsiteProvider.Tests.Ui
{
    public class AcceptanceHelperOneSimplePage : BasePage
    {
        [FindsBy(How = How.XPath, Using = "//h1")]
        public IWebElement H1Element { get; set; }

        public AcceptanceHelperOneSimplePage(IWebDriver driver) : base(driver)
        {
        }
    }
}