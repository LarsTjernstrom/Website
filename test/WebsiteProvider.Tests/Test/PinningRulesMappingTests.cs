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
        /// Load page with additionaly for catching rule defined pinning rules content and check it.
        /// </summary>
        [Test]
        public void RequestPage_PinningRulesDefinedForCatchingRule_ThePinningContentLoaded()
        {
            var page = new AcceptanceHelperTwoMasterPage(Driver).LoadMasterPage();
            WaitForText(page.HeaderElement, "Acceptance Helper 2");

            var topPins = this.GetTopBarPins();
            var mainPins = this.GetMainPins();
            var expectedTopPinsContent = new[] { "Pin 1", "Pin 2", "Pin 3", "Pin 8", "Pin 9" };
            var expectedMainPinsContent = new[] { "Pin 6", "Pin 7" };

            Assert.AreEqual(5, topPins.Count);
            Assert.AreEqual(2, mainPins.Count);
            WaitUntil(x => topPins.All(p => expectedTopPinsContent.Contains(p.Text)));
            WaitUntil(x => mainPins.All(p => expectedMainPinsContent.Contains(p.Text)));
        }

        /// <summary>
        /// Load page with additionaly for catching rule defined pinning rules content and check it.
        /// </summary>
        [Test]
        public void RequestPage_PinningRulesNotDefinedForCatchingRule_ThePinningContentLoaded()
        {
            var page = new AcceptanceHelperTwoContentPage(Driver).GoTo("test55");
            WaitForText(page.ContentElement, "test55");

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
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/pin/7/change-url");
            Driver.Navigate().GoToUrl(Config.AcceptanceHelperOneUrl + "/pin/9/change-url");

            // check with catch-all rule
            var pageOne = new AcceptanceHelperOneMasterPage(Driver).LoadSimplePage();
            WaitForText(pageOne.HeaderElement, "Simple Page");

            var topPins = this.GetTopBarPins();
            var mainPins = this.GetMainPins();
            var expectedTopPinsContent = new[] { "Pin 1", "Pin 5", "Pin 3", "Pin 9" };
            var expectedMainPinsContent = new[] { "Pin 6" };

            Assert.AreEqual(4, topPins.Count);
            Assert.AreEqual(1, mainPins.Count);
            WaitUntil(x => topPins.All(p => expectedTopPinsContent.Contains(p.Text)));
            WaitUntil(x => mainPins.All(p => expectedMainPinsContent.Contains(p.Text)));

            // check with custom catching rule
            var page = new AcceptanceHelperTwoMasterPage(Driver).LoadMasterPage();
            WaitForText(page.HeaderElement, "Acceptance Helper 2");

            var topPins2 = this.GetTopBarPins();
            var mainPins2 = this.GetMainPins();
            var expectedTopPinsContent2 = new[] { "Pin 1", "Pin 5", "Pin 3", "Pin 8", "Pin 9" };
            var expectedMainPinsContent2 = new[] { "Pin 6", "Pin 7" };

            Assert.AreEqual(5, topPins2.Count);
            Assert.AreEqual(2, mainPins2.Count);
            WaitUntil(x => topPins2.All(p => expectedTopPinsContent2.Contains(p.Text)));
            WaitUntil(x => mainPins2.All(p => expectedMainPinsContent2.Contains(p.Text)));
        }

        /// <summary>
        /// Load page in scope of long-running transaction, change pinning rules, refresh and check that nothing was changed in blending
        /// </summary>
        [Test]
        public void ChangeRulesInTransaction_ChangeAndRollback_RulesIsNotChanged()
        {
            var page = new AcceptanceHelperTwoPinningPage(Driver).Load();
            WaitForText(page.HeaderElement, "Pinning Rules");

            page.DeleteRuleButton.Click();
            page.EditRuleButton.Click();
            WaitUntil(x => !page.DeleteRuleButton.Enabled);
            WaitUntil(x => !page.EditRuleButton.Enabled);
            page.CancelChangesButton.Click();
            WaitUntil(x => !page.CancelChangesButton.Enabled);

            Driver.Navigate().Refresh();
            WaitForText(page.HeaderElement, "Pinning Rules");

            var topPins = this.GetTopBarPins();
            var mainPins = this.GetMainPins();
            var expectedTopPinsContent = new[] { "Pin 1", "Pin 2", "Pin 3", "Pin 10" };
            var expectedMainPinsContent = new[] { "Pin 6", "Pin 7" };

            Assert.AreEqual(4, topPins.Count);
            Assert.AreEqual(2, mainPins.Count);
            WaitUntil(x => topPins.All(p => expectedTopPinsContent.Contains(p.Text)));
            WaitUntil(x => mainPins.All(p => expectedMainPinsContent.Contains(p.Text)));
        }

        /// <summary>
        /// Load page in scope of long-running transaction, change pinning rules, commit, refresh and check changes was applied in blending
        /// </summary>
        [Test]
        public void ChangeRulesInTransaction_ChangeAndCommit_RulesIsChanged()
        {
            var page = new AcceptanceHelperTwoPinningPage(Driver).Load();
            WaitForText(page.HeaderElement, "Pinning Rules");

            page.DeleteRuleButton.Click();
            page.EditRuleButton.Click();
            WaitUntil(x => !page.DeleteRuleButton.Enabled);
            WaitUntil(x => !page.EditRuleButton.Enabled);
            page.SaveChangesButton.Click();
            WaitUntil(x => !page.SaveChangesButton.Enabled);

            Driver.Navigate().Refresh();
            WaitForText(page.HeaderElement, "Pinning Rules");

            var topPins = this.GetTopBarPins();
            var mainPins = this.GetMainPins();
            var expectedTopPinsContent = new[] { "Pin 3", "Pin 4", "Pin 10" };
            var expectedMainPinsContent = new[] { "Pin 6", "Pin 7" };

            Assert.AreEqual(3, topPins.Count);
            Assert.AreEqual(2, mainPins.Count);
            WaitUntil(x => topPins.All(p => expectedTopPinsContent.Contains(p.Text)));
            WaitUntil(x => mainPins.All(p => expectedMainPinsContent.Contains(p.Text)));
        }

        /// <summary>
        /// Load page in scope of long-running transaction, replace blending point, refresh and check that nothing was changed in blending
        /// </summary>
        [Test]
        public void ChangePointInTransaction_ChangeAndRollback_RulesIsNotChanged()
        {
            var page = new AcceptanceHelperTwoPinningPage(Driver).Load();
            WaitForText(page.HeaderElement, "Pinning Rules");

            page.DeletePointButton.Click();
            WaitUntil(x => !page.DeletePointButton.Enabled);
            page.CancelChangesButton.Click();
            WaitUntil(x => !page.CancelChangesButton.Enabled);

            Driver.Navigate().Refresh();
            WaitForText(page.HeaderElement, "Pinning Rules");

            var topPins = this.GetTopBarPins();
            var mainPins = this.GetMainPins();
            var expectedTopPinsContent = new[] { "Pin 1", "Pin 2", "Pin 3", "Pin 10" };
            var expectedMainPinsContent = new[] { "Pin 6", "Pin 7" };

            Assert.AreEqual(4, topPins.Count);
            Assert.AreEqual(2, mainPins.Count);
            WaitUntil(x => topPins.All(p => expectedTopPinsContent.Contains(p.Text)));
            WaitUntil(x => mainPins.All(p => expectedMainPinsContent.Contains(p.Text)));
        }

        /// <summary>
        /// Load page in scope of long-running transaction, change blending point, commit, refresh and check that changes was applied in blending
        /// </summary>
        [Test]
        public void ChangePointInTransaction_ChangeAndCommit_RulesIsChanged()
        {
            var page = new AcceptanceHelperTwoPinningPage(Driver).Load();
            WaitForText(page.HeaderElement, "Pinning Rules");

            page.DeletePointButton.Click();
            WaitUntil(x => !page.DeletePointButton.Enabled);
            page.SaveChangesButton.Click();
            WaitUntil(x => !page.SaveChangesButton.Enabled);

            Driver.Navigate().Refresh();
            WaitForText(page.HeaderElement, "Pinning Rules");

            var topPins = this.GetTopBarPins();
            var mainPins = this.GetMainPins();
            var expectedMainPinsContent = new[] { "Pin 6", "Pin 7" };

            Assert.AreEqual(0, topPins.Count);
            Assert.AreEqual(2, mainPins.Count);
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