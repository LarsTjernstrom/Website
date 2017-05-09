using NUnit.Framework;
using WebsiteProvider.Tests.Ui;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Test
{
    /// <remarks>
    /// These tests rely on the Website data which is different for different fixtures and tests.
    /// One test case can be incompatible with another one so some tests can't be passed.
    /// So we cannot run them in parallel.
    /// </remarks>
    [Parallelizable(ParallelScope.None)]
    [TestFixture(Config.Browser.Chrome)]
    [TestFixture(Config.Browser.Edge)]
    [TestFixture(Config.Browser.Firefox)]
    public class PageLoadTest : BaseTest
    {
        const string MiddlewareTestCookieName = "AcceptanceHelperTwoMiddlewareTestCookie";

        private AcceptanceHelperOneMasterPage _acceptanceHelperOneMasterPage;
        private AcceptanceHelperTwoMasterPage _acceptanceHelperTwoMasterPage;

        public PageLoadTest(Config.Browser browser) : base(browser)
        {
        }

        // TODO : Remove these 2 methods when merging with the PR Website#65
        [OneTimeSetUp]
        public void SetUpFixture()
        {
            Driver.Navigate().GoToUrl("http://localhost:8080/website/resetdata");
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetupPageLoadTests");
        }
        [OneTimeTearDown]
        public void ResetData()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/ResetData");
        }

        [Test]
        public void EmptyPageLoadsTest()
        {
            _acceptanceHelperOneMasterPage = new AcceptanceHelperOneMasterPage(Driver).GoToEmptyPage();
            WaitForText(_acceptanceHelperOneMasterPage.H1Element, "Acceptance Helper 1", 10);
        }

        [Test]
        public void EmptyJsonLoadsTest()
        {
            _acceptanceHelperOneMasterPage = new AcceptanceHelperOneMasterPage(Driver).GoToEmptyJson();
            WaitUntil(x => x.PageSource.Contains("System.InvalidOperationException: ScErrInvalidOperation (SCERR1025)"));
        }

        [Test]
        public void RedirectToOtherAppPageTest()
        {
            _acceptanceHelperOneMasterPage = new AcceptanceHelperOneMasterPage(Driver).GoToMasterPage();
            _acceptanceHelperTwoMasterPage = _acceptanceHelperOneMasterPage.GoToAcceptanceHelperTwoPage();
            WaitForText(_acceptanceHelperTwoMasterPage.H1Element, "Acceptance Helper 2", 10);
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
            new AcceptanceHelperOneMasterPage(Driver).GoToMasterPage();
            var cookie = Driver.Manage().Cookies.GetCookieNamed(MiddlewareTestCookieName);
            Assert.IsNotNull(cookie);
            Driver.Manage().Cookies.DeleteCookieNamed(MiddlewareTestCookieName);
        }
    }
}