using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Ui
{
    public class AcceptanceHelperTwoPinningPage : BasePage
    {
        [FindsBy(How = How.XPath, Using = "//h3")]
        public IWebElement HeaderElement { get; set; }

        [FindsBy(How = How.ClassName, Using = "websiteprovider-acceptancehelpertwo-pinning-save")]
        public IWebElement SaveChangesButton { get; set; }

        [FindsBy(How = How.ClassName, Using = "websiteprovider-acceptancehelpertwo-pinning-cancel")]
        public IWebElement CancelChangesButton { get; set; }

        [FindsBy(How = How.ClassName, Using = "websiteprovider-acceptancehelpertwo-pinning-edit-rule")]
        public IWebElement EditRuleButton { get; set; }

        [FindsBy(How = How.ClassName, Using = "websiteprovider-acceptancehelpertwo-pinning-delete-rule")]
        public IWebElement DeleteRuleButton { get; set; }

        [FindsBy(How = How.ClassName, Using = "websiteprovider-acceptancehelpertwo-pinning-delete-point")]
        public IWebElement DeletePointButton { get; set; }

        public AcceptanceHelperTwoPinningPage(IWebDriver driver) : base(driver)
        {
        }

        public AcceptanceHelperTwoPinningPage Load()
        {
            Driver.Navigate().GoToUrl($"{Config.AcceptanceHelperTwoUrl}/pinning");
            return this;
        }
    }
}