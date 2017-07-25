using NUnit.Framework;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Test
{
    /// <summary>
    /// Tests for correct mapping/unmapping pinning rules
    /// </summary>
    /// <remarks>
    /// These tests rely on the Website data which is different for different fixtures and tests.
    /// One test case can be incompatible with another one so some tests can't be passed.
    /// So we cannot run them in parallel.
    /// </remarks>
    public class PinningRulesMappingTests : BaseTest
    {
        public PinningRulesMappingTests(Config.Browser browser) : base(browser)
        {
        }

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/ResetData");
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/SetupPinningRulesMappingTests");
        }

        [OneTimeTearDown]
        public void ResetData()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/ResetData");
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void RequestPage_ResourceAfterSlash_RuleFoundAndApplied()
        {
        }
    }
}