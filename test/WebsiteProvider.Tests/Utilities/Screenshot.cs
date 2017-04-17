using System;
using NUnit.Framework;
using OpenQA.Selenium;

namespace WebsiteProvider.Tests.Utilities
{
    class Screenshot
    {
        public static void MakeScreenshot(IWebDriver driver)
        {
            var driverScreenshot = (ITakesScreenshot)driver;
            var screenShot = driverScreenshot.GetScreenshot();
            if (screenShot != null)
            {
                var now = DateTime.Now.ToString("yyyyMMddHHmmss");
                var path = $@"\webdriver_screenshot_{now}.png";
                screenShot.SaveAsFile(TestContext.CurrentContext.TestDirectory + path, ScreenshotImageFormat.Png);
            }
            else
            {
                throw new Exception("GetScreenshot() returned null, can't save file!");
            }
        }
    }
}