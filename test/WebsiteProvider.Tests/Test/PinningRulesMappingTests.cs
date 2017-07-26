using System.Collections.Generic;
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
        /// Load page with defined pinning rules content and check it.
        /// </summary>
        [Test]
        public void RequestPage_PinningRulesDefined_ThePinningContentLoaded()
        {
            var page = new AcceptanceHelperOneMasterPage(Driver).LoadSimplePage();
            WaitForText(page.HeaderElement, "Simple Page");

            var topPins = this.GetTopBarPins();
            var mainPins = this.GetMainPins();
            var expectedTopPinsContent = new[] { "Pin 1", "Pin 2", "Pin 3" };
            var expectedMainPinsContent = new[] { "Pin 6", "Pin 7" };

            Assert.AreEqual(3, topPins.Count);
            Assert.AreEqual(2, mainPins.Count);
            WaitUntil(x => topPins.All(p => expectedTopPinsContent.Contains(p.Text)));
            WaitUntil(x => mainPins.All(p => expectedMainPinsContent.Contains(p.Text)));
        }

        /// <summary>
        /// Delete one pinning rule, load page with another ones, check it and check that page does not contain the removed rule content.
        /// </summary>
        [Test]
        public void RequestPage_PinningRuleDeleted_TheDeletedRuleContenNotLoaded()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/pin/2/delete");
            var page = new AcceptanceHelperOneMasterPage(Driver).LoadSimplePage();
            WaitForText(page.HeaderElement, "Simple Page");

            var topPins = this.GetTopBarPins();
            var mainPins = this.GetMainPins();
            var expectedTopPinsContent = new[] { "Pin 1", "Pin 3" };
            var expectedMainPinsContent = new[] { "Pin 6", "Pin 7" };

            Assert.AreEqual(2, topPins.Count);
            Assert.AreEqual(2, mainPins.Count);
            WaitUntil(x => topPins.All(p => expectedTopPinsContent.Contains(p.Text)));
            WaitUntil(x => mainPins.All(p => expectedMainPinsContent.Contains(p.Text)));
        }

        /// <summary>
        /// Delete and recreate blending point, load page without pinning rules content on the changed blending point and check it.
        /// </summary>
        [Test]
        public void RequestPage_BlendingPointDeleted_ThePinningContenLoadedOnlyForAnotherPoint()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/section/renew/TopBar");
            var page = new AcceptanceHelperOneMasterPage(Driver).LoadSimplePage();
            WaitForText(page.HeaderElement, "Simple Page");

            var topPins = this.GetTopBarPins();
            var mainPins = this.GetMainPins();
            var expectedMainPinsContent = new[] { "Pin 6", "Pin 7" };

            Assert.AreEqual(0, topPins.Count);
            Assert.AreEqual(2, mainPins.Count);
            WaitUntil(x => mainPins.All(p => expectedMainPinsContent.Contains(p.Text)));
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

            var topPins = this.GetTopBarPins();
            var mainPins = this.GetMainPins();

            Assert.AreEqual(0, topPins.Count);
            Assert.AreEqual(0, mainPins.Count);
        }

        /// <summary>
        /// Edit pinning rules, load page with pinning rules content, check that page contains changed rule content.
        /// </summary>
        [Test]
        public void RequestPage_PinningRuleUrlEdited_TheEditedRuleContenLoadedChanged()
        {
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/pin/2/edit/5");
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/pin/7/change-section");
            var page = new AcceptanceHelperOneMasterPage(Driver).LoadSimplePage();
            WaitForText(page.HeaderElement, "Simple Page");

            var topPins = this.GetTopBarPins();
            var mainPins = this.GetMainPins();
            var expectedTopPinsContent = new[] { "Pin 1", "Pin 5", "Pin 3", "Pin 7" };
            var expectedMainPinsContent = new[] { "Pin 6" };

            Assert.AreEqual(4, topPins.Count);
            Assert.AreEqual(1, mainPins.Count);
            WaitUntil(x => topPins.All(p => expectedTopPinsContent.Contains(p.Text)));
            WaitUntil(x => mainPins.All(p => expectedMainPinsContent.Contains(p.Text)));
        }

        private ICollection<IWebElement> GetTopBarPins()
        {
            var topBar = Driver.FindElement(By.ClassName("website-defaultsurface-topbar"));
            return topBar.FindElements(By.ClassName("websiteprovider-acceptancehelperone-pin-marker"));
        }

        private ICollection<IWebElement> GetMainPins()
        {
            var main = Driver.FindElement(By.XPath("//starcounter-include[@slot = 'websiteeditor/main']"));
            return main.FindElements(By.ClassName("websiteprovider-acceptancehelperone-pin-marker"));
        }
    }
}