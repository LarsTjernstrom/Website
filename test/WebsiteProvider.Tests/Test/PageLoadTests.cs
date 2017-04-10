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
            var page = new AcceptanceHelperOneMasterPage(Driver).LoadSimplePage();
            WaitForText(page.HeaderElement, "Simple Page");
        }

        [Test]
        public void LoadPage_ExcludedHtmlField_ExceptionThrownAndRendered()
        {
            var page = new AcceptanceHelperOneMasterPage(Driver).LoadEmptyJson();
            WaitUntil(x => x.PageSource.Contains("System.InvalidOperationException: ScErrInvalidOperation (SCERR1025)"));
        }

        [Test]
        public void GoToOwnPage_WithCorrespondingCatchingRule_Loaded()
        {
            var masterPage = new AcceptanceHelperOneMasterPage(Driver).LoadMasterPage();
            var simplePage = masterPage.GoToSimplePage();
            WaitForText(simplePage.HeaderElement, "Simple Page");
        }

        [Test]
        public void GoToOtherAppPage_WithCorrespondingCatchingRule_Loaded()
        {
            var acceptanceHelperOneMasterPage = new AcceptanceHelperOneMasterPage(Driver).LoadMasterPage();
            var acceptanceHelperTwoMasterPage = acceptanceHelperOneMasterPage.GoToAcceptanceHelperTwoPage();
            WaitForText(acceptanceHelperTwoMasterPage.HeaderElement, "Acceptance Helper 2");
        }
    }
}