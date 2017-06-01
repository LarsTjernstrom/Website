using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WebsiteProvider.Tests.Utilities;

namespace WebsiteProvider.Tests.Test
{
    public class BaseTest
    {
        public IWebDriver Driver;
        private readonly Config.Browser _browser;
        private readonly string _browsersTc = TestContext.Parameters["Browsers"];
        private List<string> _browsersToRun = new List<string>();

        public BaseTest(Config.Browser browser)
        {
            _browser = browser;
        }

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            if (_browsersTc != null)
            {
                _browsersToRun = _browsersTc.Split(',').ToList();
            }
            else
            {
                _browsersToRun.Add("Chrome");
                _browsersToRun.Add("Firefox");
                //_browsersToRun.Add("Edge");
            }

            if (_browsersToRun.Contains(Config.BrowserDictionary[_browser]))
                Driver = WebDriverManager.StartDriver(_browser, Config.Timeout, Config.RemoteWebDriverUri);
            else
            {
                Assert.Ignore(Config.BrowserDictionary[_browser] + " is on browsers ignore list");
            }
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            Driver?.Quit();
        }

        protected TResult WaitUntil<TResult>(Func<IWebDriver, TResult> condition)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            return wait.Until(condition);
        }

        public bool WaitForText(IWebElement elementName, string text, int seconds = 10)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(seconds));
            return wait.Until(ExpectedConditions.TextToBePresentInElement(elementName, text));
        }
    }
}