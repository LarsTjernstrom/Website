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
        public void ResetData()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/ResetData");
        }

        [Test]
        public void GoToOtherAppPage_NoFinalCatchAllRules_WrappedAndLoaded()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetupNoFinalCatchAllRulesTest");
            var acceptanceHelperOneMasterPage = new AcceptanceHelperOneMasterPage(Driver).LoadMasterPage();
            var acceptanceHelperTwoMasterPage = acceptanceHelperOneMasterPage.GoToAcceptanceHelperTwoPage();
            WaitForText(acceptanceHelperTwoMasterPage.HeaderElement, "Acceptance Helper 2");
            WaitUntil(x => acceptanceHelperTwoMasterPage.CheckForDefaultSurface());
        }

        [Test]
        public void GoToOwnPage_NoFinalCatchAllRules_WrappedAndLoaded()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetupNoFinalCatchAllRulesTest");
            var masterPage = new AcceptanceHelperOneMasterPage(Driver).LoadMasterPage();
            var simplePage = masterPage.GoToSimplePage();
            WaitForText(simplePage.HeaderElement, "Simple Page");
            WaitUntil(x => simplePage.CheckForDefaultSurface());
        }

        [Test]
        public void GoToOtherAppPage_NoCatchAllRules_LoadedAsIs()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetupNoCatchAllRuleTest");
            var acceptanceHelperOneMasterPage = new AcceptanceHelperOneMasterPage(Driver).LoadMasterPage();
            var acceptanceHelperTwoMasterPage = acceptanceHelperOneMasterPage.GoToAcceptanceHelperTwoPage();
            WaitForText(acceptanceHelperTwoMasterPage.HeaderElement, "Acceptance Helper 2");
            WaitUntil(x => acceptanceHelperTwoMasterPage.CheckForNoSurface());
        }

        [Test]
        public void GoToOwnPage_NoCatchAllRules_LoadedAsIs()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetupNoCatchAllRuleTest");
            var masterPage = new AcceptanceHelperOneMasterPage(Driver).LoadMasterPage();
            var simplePage = masterPage.GoToSimplePage();
            WaitForText(simplePage.HeaderElement, "Simple Page");
            WaitUntil(x => simplePage.CheckForNoSurface());
        }
    }
}