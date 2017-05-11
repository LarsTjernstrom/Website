using NUnit.Framework;
using WebsiteProvider.Tests.Ui;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Test
{
    /// <summary>
    /// Tests for correct loading of the pages with basic configuration
    /// </summary>
    /// <remarks>
    /// These tests rely on the Website data which is different for different fixtures and tests.
    /// One test case can be incompatible with another one so some tests can't be passed.
    /// So we cannot run them in parallel.
    /// </remarks>
    [Parallelizable(ParallelScope.None)]
    [TestFixture(Config.Browser.Chrome)]
    [TestFixture(Config.Browser.Edge)]
    [TestFixture(Config.Browser.Firefox)]
    public class PageLoadTests : BaseTest
    {
        const string MiddlewareTestCookieName = "AcceptanceHelperTwoMiddlewareTestCookie";

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

        /// <summary>
        /// The test method expects that a middleware was registered in an application which was started last after others.
        /// Such middleware is defined in the WebsiteProvider_AcceptanceHelperTwo application.
        /// The middleware sets a cookie with name <see cref="MiddlewareTestCookieName"/> which existance must be checking by the test.
        /// In the beginning and in the end of a test the method executes deleting of the cookie.
        /// </summary>
        [Test]
        public void LoadPage_MiddlewareHandling_CookieAdded()
        {
            Driver.Manage().Cookies.DeleteCookieNamed(MiddlewareTestCookieName);
            new AcceptanceHelperOneMasterPage(Driver).LoadMasterPage();
            var cookie = Driver.Manage().Cookies.GetCookieNamed(MiddlewareTestCookieName);
            Assert.IsNotNull(cookie);
            Driver.Manage().Cookies.DeleteCookieNamed(MiddlewareTestCookieName);
        }
    }
}
