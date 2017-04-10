using NUnit.Framework;
using WebsiteProvider.Tests.Ui;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Test
{
    [Parallelizable(ParallelScope.None)]
    [TestFixture(Config.Browser.Chrome)]
    [TestFixture(Config.Browser.Edge)]
    [TestFixture(Config.Browser.Firefox)]
    public class CatchAllRulesTests : BaseTest
    {
        public CatchAllRulesTests(Config.Browser browser) : base(browser)
        {
        }

        [SetUp]
        public void SetUp()
        {
            Driver.Navigate().GoToUrl(Config.WebsiteUrl + "/resetdata");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Driver.Navigate().GoToUrl(Config.WebsiteUrl + "/resetdata");
        }

        [Test]
        public void GoToOtherAppPage_NoFinalCatchAllRules_WrappedAndLoaded()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetupNoFinalCatchAllRulesTest");
            GoToAndCheckAcceptanceHelperTwoPage();
        }

        [Test]
        public void GoToOtherAppPage_NoCatchAllRules_LoadedAsIs()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetupNoCatchAllRuleTest");
            GoToAndCheckAcceptanceHelperTwoPage();
        }

        protected void GoToAndCheckAcceptanceHelperTwoPage()
        {
            var acceptanceHelperOneMasterPage = new AcceptanceHelperOneMasterPage(Driver).GoToMasterPage();
            var acceptanceHelperTwoMasterPage = acceptanceHelperOneMasterPage.GoToAcceptanceHelperTwoPage();
            WaitForText(acceptanceHelperTwoMasterPage.H1Element, "Acceptance Helper 2");
        }
    }
}