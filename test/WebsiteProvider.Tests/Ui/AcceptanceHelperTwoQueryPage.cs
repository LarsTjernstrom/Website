using System.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Ui
{
    public class AcceptanceHelperTwoQueryPage : BasePage
    {
        [FindsBy(How = How.XPath, Using = "//span")]
        public IWebElement ContentElement { get; set; }

        public AcceptanceHelperTwoQueryPage(IWebDriver driver) : base(driver)
        {
        }

        public AcceptanceHelperTwoQueryPage Query(string query)
        {
            Driver.Navigate().GoToUrl($"{Config.AcceptanceHelperTwoUrl}/query?{HttpUtility.UrlEncode(query)}");
            return this;
        }
    }
}