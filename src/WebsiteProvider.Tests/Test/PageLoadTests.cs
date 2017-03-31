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
        public void EmptyPageLoadsTest()
        {
            _acceptanceHelperOneMasterPage = new AcceptanceHelperOneMasterPage(Driver).GoToEmptyPage();

            for (int second = 0; ; second++)
            {
                if (second >= 60) Assert.Fail("timeout");
                try
                {
                    if (Driver.Title == "WebsiteProvider_AcceptanceHelperOne") break;
                }
                catch (Exception)
                { }
                Thread.Sleep(1000);
            }
        }

        [Test]
        public void EmptyJsonLoadsTest()
        {
            _acceptanceHelperOneMasterPage = new AcceptanceHelperOneMasterPage(Driver).GoToEmptyJson();

            for (int second = 0; ; second++)
            {
                if (second >= 60) Assert.Fail("timeout");
                try
                {
                    if (Driver.PageSource.Contains("System.InvalidOperationException: ScErrInvalidOperation (SCERR1025)"))
                    {
                        break;
                    }
                }
                catch (Exception)
                { }
                Thread.Sleep(1000);
            }
        }
    }
}