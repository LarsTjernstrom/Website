using NUnit.Framework;
using WebsiteProvider.Tests.Ui;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Test
{
    /// <summary>
    /// Tests for correct loading of the pages with basic configuration
    /// </summary>
    [Parallelizable(ParallelScope.None)]
    [TestFixture(Config.Browser.Chrome)]
    [TestFixture(Config.Browser.Edge)]
    [TestFixture(Config.Browser.Firefox)]
    public class PageLoadTests : BaseTest
    {
        public PageLoadTests(Config.Browser browser) : base(browser)
        {
        }

        /// <summary>
        /// Clean all Website's data and create default for the test fixture.
        /// </summary>
        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/ResetData");
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetDefaultCatchingRules");
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperTwoUrl + "/SetDefaultCatchingRules");
        }

        /// <summary>
        /// Clear all Website's data.
        /// </summary>
        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/ResetData");
        }

        /// <summary>
        /// Load a simple stateless page and check the content and the loaded template.
        /// </summary>
        [Test]
        public void LoadPage_IncludedHtmlField_WrappedAndLoaded()
        {
            var page = new AcceptanceHelperOneMasterPage(Driver).LoadSimplePage();
            WaitForText(page.HeaderElement, "Simple Page");
            WaitUntil(x => page.CheckForLauncherSurface());
        }

        /// <summary>
        /// Test if an exception data was rendered when a responded Json doesn't have an Html field.
        /// </summary>
        [Test]
        public void LoadPage_ExcludedHtmlField_ExceptionThrownAndRendered()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/EmptyJson");
            WaitUntil(x => x.PageSource.Contains("System.InvalidOperationException: ScErrInvalidOperation (SCERR1025)"));
        }

        /// <summary>
        /// Load a simple stateless page from a current app by clicking on a link
        /// and check the content and the loaded template.
        /// </summary>
        [Test]
        public void GoToOwnPage_WithCorrespondingCatchingRule_Loaded()
        {
            var masterPage = new AcceptanceHelperOneMasterPage(Driver).LoadMasterPage();
            var simplePage = masterPage.GoToSimplePage();
            WaitForText(simplePage.HeaderElement, "Simple Page");
            WaitUntil(x => simplePage.CheckForLauncherSurface());
        }

        /// <summary>
        /// Load a simple stateless page from another app by clicking on a link
        /// and check the content and the loaded template.
        /// </summary>
        [Test]
        public void GoToOtherAppPage_WithCorrespondingCatchingRule_Loaded()
        {
            var acceptanceHelperOneMasterPage = new AcceptanceHelperOneMasterPage(Driver).LoadMasterPage();
            var acceptanceHelperTwoMasterPage = acceptanceHelperOneMasterPage.GoToAcceptanceHelperTwoPage();
            WaitForText(acceptanceHelperTwoMasterPage.HeaderElement, "Acceptance Helper 2");
            WaitUntil(x => acceptanceHelperTwoMasterPage.CheckForSidebarSurface());
        }
    }
}