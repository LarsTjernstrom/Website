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
        [OneTimeTearDown]
        public void SetUp()
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
        public void GoToOwnPage_NoFinalCatchAllRules_WrappedAndLoaded()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetupNoFinalCatchAllRulesTest");
            GoToAndCheckAcceptanceHelperOneSimplePage();
        }

        [Test]
        public void GoToOtherAppPage_NoCatchAllRules_LoadedAsIs()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetupNoCatchAllRuleTest");
            GoToAndCheckAcceptanceHelperTwoPage();
        }

        [Test]
        public void GoToOwnPage_NoCatchAllRules_LoadedAsIs()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetupNoCatchAllRuleTest");
            GoToAndCheckAcceptanceHelperOneSimplePage();
        }

        protected void GoToAndCheckAcceptanceHelperTwoPage()
        {
            var acceptanceHelperOneMasterPage = new AcceptanceHelperOneMasterPage(Driver).LoadMasterPage();
            var acceptanceHelperTwoMasterPage = acceptanceHelperOneMasterPage.GoToAcceptanceHelperTwoPage();
            WaitForText(acceptanceHelperTwoMasterPage.HeaderElement, "Acceptance Helper 2");
        }

        protected void GoToAndCheckAcceptanceHelperOneSimplePage()
        {
            var masterPage = new AcceptanceHelperOneMasterPage(Driver).LoadMasterPage();
            var simplePage = masterPage.GoToSimplePage();
            WaitForText(simplePage.HeaderElement, "Simple Page");
        }
    }
}