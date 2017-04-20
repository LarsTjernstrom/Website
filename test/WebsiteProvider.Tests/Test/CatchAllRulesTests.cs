using NUnit.Framework;
using WebsiteProvider.Tests.Ui;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Test
{
    /// <summary>
    /// Tests for catching rules
    /// </summary>
    [Parallelizable(ParallelScope.None)]
    [TestFixture(Config.Browser.Chrome)]
    [TestFixture(Config.Browser.Edge)]
    [TestFixture(Config.Browser.Firefox)]
    public class CatchAllRulesTests : BaseTest
    {
        public CatchAllRulesTests(Config.Browser browser) : base(browser)
        {
        }

        /// <summary>
        /// Clear all Website's data
        /// </summary>
        [SetUp]
        [OneTimeTearDown]
        public void ResetData()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/ResetData");
        }

        /// <summary>
        /// Firstly set up catching rules so there are no "final" catch-all rules.
        /// Load a simple stateless page from another app by clicking on a link
        /// and check the content and the loaded template.
        /// </summary>
        [Test]
        public void GoToOtherAppPage_NoFinalCatchAllRules_WrappedAndLoaded()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetupNoFinalCatchAllRulesTest");
            var acceptanceHelperOneMasterPage = new AcceptanceHelperOneMasterPage(Driver).LoadMasterPage();
            var acceptanceHelperTwoMasterPage = acceptanceHelperOneMasterPage.GoToAcceptanceHelperTwoPage();
            WaitForText(acceptanceHelperTwoMasterPage.HeaderElement, "Acceptance Helper 2");
            WaitUntil(x => acceptanceHelperTwoMasterPage.CheckForDefaultSurface());
        }

        /// <summary>
        /// Firstly set up catching rules so there are no "final" catch-all rules.
        /// Load a simple stateless page from a current app by clicking on a link
        /// and check the content and the loaded template.
        /// </summary>
        [Test]
        public void GoToOwnPage_NoFinalCatchAllRules_WrappedAndLoaded()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetupNoFinalCatchAllRulesTest");
            var masterPage = new AcceptanceHelperOneMasterPage(Driver).LoadMasterPage();
            var simplePage = masterPage.GoToSimplePage();
            WaitForText(simplePage.HeaderElement, "Simple Page");
            WaitUntil(x => simplePage.CheckForDefaultSurface());
        }

        /// <summary>
        /// Firstly set up catching rules so there are no catch-all rules at all.
        /// Load a simple stateless page from another app by clicking on a link
        /// and check the content and the loaded template.
        /// </summary>
        [Test]
        public void GoToOtherAppPage_NoCatchAllRules_LoadedAsIs()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetupNoCatchAllRuleTest");
            var acceptanceHelperOneMasterPage = new AcceptanceHelperOneMasterPage(Driver).LoadMasterPage();
            var acceptanceHelperTwoMasterPage = acceptanceHelperOneMasterPage.GoToAcceptanceHelperTwoPage();
            WaitForText(acceptanceHelperTwoMasterPage.HeaderElement, "Acceptance Helper 2");
            WaitUntil(x => acceptanceHelperTwoMasterPage.CheckForNoSurface());
        }

        /// <summary>
        /// Firstly set up catching rules so there are no catch-all rules at all.
        /// Load a simple stateless page from a current app by clicking on a link
        /// and check the content and the loaded template.
        /// </summary>
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