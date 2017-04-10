using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace WebsiteProvider.Tests.Ui
{
    public class AcceptanceHelperOneSimplePage : BasePage
    {
        [FindsBy(How = How.XPath, Using = "//h2")]
        public IWebElement HeaderElement { get; set; }

        public AcceptanceHelperOneSimplePage(IWebDriver driver) : base(driver)
        {
        }
    }
}