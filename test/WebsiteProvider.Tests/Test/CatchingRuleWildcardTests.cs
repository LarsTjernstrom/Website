using NUnit.Framework;
using WebsiteProvider.Tests.Ui;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Test
{
    /// <summary>
    /// Tests for correct defining of catching rules with wildcards
    /// </summary>
    [Parallelizable(ParallelScope.None)]
    [TestFixture(Config.Browser.Chrome)]
    [TestFixture(Config.Browser.Edge)]
    [TestFixture(Config.Browser.Firefox)]
    public class CatchingRuleWildcardTests : BaseTest
    {
        public CatchingRuleWildcardTests(Config.Browser browser) : base(browser)
        {
        }

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/ResetData");
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetupCatchingRuleWildcardTests");
        }

        [OneTimeTearDown]
        public void ResetData()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/ResetData");
        }

        /// <summary>
        /// Request a page with a URI '/application/content/{?}'
        /// </summary>
        [Test]
        public void RequestPage_ResourceAfterSlash_RuleFoundAndApplied()
        {
            var resourceName = "resource";
            var page = new AcceptanceHelperTwoContentPage(Driver).GoTo(resourceName);
            WaitForText(page.ContentElement, resourceName, 10);
            WaitUntil(x => page.CheckForLauncherSurface());
        }

        /// <summary>
        /// Request a page with a URI '/application/query?{?}'
        /// </summary>
        [Test]
        public void RequestPage_ParametersWithEncodedNonHttpSymbols_RuleFoundAndApplied()
        {
            var query = "return=/website/cms";
            var page = new AcceptanceHelperTwoQueryPage(Driver).Query(query);
            WaitForText(page.ContentElement, query, 10);
            WaitUntil(x => page.CheckForDefaultSurface());
        }
    }
}