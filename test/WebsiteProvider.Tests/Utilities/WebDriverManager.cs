using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace WebsiteProvider.Tests.Utilities
{
    public class WebDriverManager
    {
        public static IWebDriver StartDriver(Config.Browser browser, TimeSpan timeout, Uri remoteWebDriverUri)
        {
            IWebDriver driver = null;
            switch (browser)
            {
                case Config.Browser.Chrome:
                    {
                        driver = new RemoteWebDriver(remoteWebDriverUri, DesiredCapabilities.Chrome());
                        break;
                    }
                case Config.Browser.Edge:
                    {
                        driver = new RemoteWebDriver(remoteWebDriverUri, DesiredCapabilities.Edge());
                        break;
                    }
                case Config.Browser.Firefox:
                    {
                        driver = new RemoteWebDriver(remoteWebDriverUri, DesiredCapabilities.Firefox());
                        break;
                    }
            }

            IWebDriver eventDriver = new WebDriverEventListener(driver);
            driver = eventDriver;
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad = timeout;
            driver.Manage().Timeouts().AsynchronousJavaScript = timeout;
            return driver;
        }
    }
}