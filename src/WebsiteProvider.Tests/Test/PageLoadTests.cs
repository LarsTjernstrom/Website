using NUnit.Framework;
using WebsiteProvider.Tests.Ui;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Test
{
    [Parallelizable(ParallelScope.Fixtures)]
    [TestFixture(Config.Browser.Chrome)]
    [TestFixture(Config.Browser.Edge)]
    [TestFixture(Config.Browser.Firefox)]
    public class PageLoadTest : BaseTest
    {
        private AcceptanceHelperOneMasterPage _acceptanceHelperOneMasterPage;
        private AcceptanceHelperTwoMasterPage _acceptanceHelperTwoMasterPage;

        public PageLoadTest(Config.Browser browser) : base(browser)
        {
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
    }
}