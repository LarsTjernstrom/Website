using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using WebsiteProvider.Tests.Ui;
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
    [Parallelizable(ParallelScope.None)]
    [TestFixture(Config.Browser.Chrome)]
    [TestFixture(Config.Browser.Edge)]
    [TestFixture(Config.Browser.Firefox)]
    public class PinningRulesMappingTests : BaseTest
    {
        public PinningRulesMappingTests(Config.Browser browser) : base(browser)
        {
        }

        [SetUp]
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
        /// Load page with 3 defined pinning rules content and check it.
        /// </summary>
        [Test]
        public void RequestPage_PinningRulesDefined_ThePinningContentLoaded()
        {
            var page = new AcceptanceHelperOneMasterPage(Driver).LoadSimplePage();
            WaitForText(page.HeaderElement, "Simple Page");
            var pins = Driver.FindElements(By.ClassName("websiteprovider-acceptancehelperone-pin-marker"));
            WaitUntil(x => pins.Any(p => p.Text == "Pin 1"));
            WaitUntil(x => pins.Any(p => p.Text == "Pin 2"));
            WaitUntil(x => pins.Any(p => p.Text == "Pin 3"));
        }

        /// <summary>
        /// Delete one pinning rule, load page with 2 ones, check it and check that page odes not contain remove rule content.
        /// </summary>
        [Test]
        public void RequestPage_PinningRuleDeleted_TheDeletedRuleContenNotLoaded()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/pin/2/delete");
            var page = new AcceptanceHelperOneMasterPage(Driver).LoadSimplePage();
            WaitForText(page.HeaderElement, "Simple Page");

            var pins = Driver.FindElements(By.ClassName("websiteprovider-acceptancehelperone-pin-marker"));
            WaitUntil(x => pins.Any(p => p.Text == "Pin 1"));
            WaitUntil(x => pins.All(p => p.Text != "Pin 2"));
            WaitUntil(x => pins.Any(p => p.Text == "Pin 3"));
        }

        /// <summary>
        /// Delete blending point, load page without any pinning rules content and check it.
        /// </summary>
        [Test]
        public void RequestPage_BlendingPointsDeleted_ThePinningContenNotLoaded()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/sections/renew");
            var page = new AcceptanceHelperOneMasterPage(Driver).LoadSimplePage();
            WaitForText(page.HeaderElement, "Simple Page");

            WaitUntil(x => !Driver.FindElements(By.ClassName("websiteprovider-acceptancehelperone-pin-marker")).Any());
        }

        /// <summary>
        /// Delete surface, load page without any pinning rules content and check it.
        /// </summary>
        [Test]
        public void RequestPage_SurfacesDeleted_ThePinningContenNotLoaded()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/surfaces/renew");
            var page = new AcceptanceHelperOneMasterPage(Driver).LoadSimplePage();
            WaitForText(page.HeaderElement, "Simple Page");

            WaitUntil(x => !Driver.FindElements(By.ClassName("websiteprovider-acceptancehelperone-pin-marker")).Any());
        }
    }
}