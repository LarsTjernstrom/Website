using NUnit.Framework;
using WebsiteProvider.Tests.Ui;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Test
{
    [Parallelizable(ParallelScope.None)]
    [TestFixture(Config.Browser.Chrome)]
    [TestFixture(Config.Browser.Edge)]
    [TestFixture(Config.Browser.Firefox)]
    public class PageLoadTests : BaseTest
    {
        public PageLoadTests(Config.Browser browser) : base(browser)
        {
        }

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            Driver.Navigate().GoToUrl(Config.WebsiteUrl + "/resetdata");
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetDefaultCatchingRules");
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperTwoUrl + "/SetDefaultCatchingRules");
        }

        [Test]
        public void LoadPage_IncludedHtmlField_WrappedAndLoaded()
        {
            var page = new AcceptanceHelperOneMasterPage(Driver).GoToEmptyPage();
            WaitForText(page.H1Element, "Acceptance Helper 1");
        }

        [Test]
        public void LoadPage_ExcludedHtmlField_ExceptionThrownAndRendered()
        {
            var page = new AcceptanceHelperOneMasterPage(Driver).GoToEmptyJson();
            WaitUntil(x => x.PageSource.Contains("System.InvalidOperationException: ScErrInvalidOperation (SCERR1025)"));
        }

        [Test]
        public void GoToOtherAppPage_WithCorrespondingCatchingRule_Loaded()
        {
            var acceptanceHelperOneMasterPage = new AcceptanceHelperOneMasterPage(Driver).GoToMasterPage();
            var acceptanceHelperTwoMasterPage = acceptanceHelperOneMasterPage.GoToAcceptanceHelperTwoPage();
            WaitForText(acceptanceHelperTwoMasterPage.H1Element, "Acceptance Helper 2");
        }
    }
}