using System;
using System.Threading;
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

        public PageLoadTest(Config.Browser browser) : base(browser)
        {
        }

        [Test]
        public void ThePageLoadsTest()
        {
            _acceptanceHelperOneMasterPage = new AcceptanceHelperOneMasterPage(Driver).GoToMasterPage();

            for (int second = 0; ; second++)
            {
                if (second >= 60) Assert.Fail("timeout");
                try
                {
                    if ("WebsiteProvider_AcceptanceHelperOne" == Driver.Title) break;
                }
                catch (Exception)
                { }
                Thread.Sleep(1000);
            }
        }
    }
}