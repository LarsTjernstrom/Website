using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace WebsiteProvider.Tests.Ui
{
    public class BasePage
    {
        public IWebDriver Driver;

        public BasePage(IWebDriver driver)
        {
            Driver = driver;
            PageFactory.InitElements(Driver, this);
        }

        public IWebElement WaitForElementToBeClickable(IWebElement elementName, int seconds)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(seconds));
            return wait.Until(ExpectedConditions.ElementToBeClickable(elementName));
        }

        public void ClickOn(IWebElement elementName, int seconds = 10)
        {
            IWebElement element = WaitForElementToBeClickable(elementName, seconds);
            element.Click();
        }

        public IWebElement ExpandShadowRoot(IWebElement shadowRootElement)
        {
            IWebElement shadowTreeParent = (IWebElement)((IJavaScriptExecutor)Driver)
           .ExecuteScript("return arguments[0].shadowRoot", shadowRootElement);
            if (shadowTreeParent == null)
            {
                //Shadow DOM not supported, fall back to DOM
                return shadowRootElement;
            }
            return shadowTreeParent;
        }

        public void ScrollToTheTop()
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("window.scrollTo(0, 0)");
        }

        public bool CheckForLauncherSurface()
        {
            return CheckForSurface("launcher-layout");
        }

        public bool CheckForDefaultSurface()
        {
            return CheckForSurface("website-defaulttemplate-main");
        }

        protected bool CheckForSurface(string searchingClassName)
        {
            var shadowRoot = ExpandShadowRoot(Driver.FindElement(By.XPath("//juicy-composition")));
            return shadowRoot.FindElements(By.ClassName(searchingClassName)).Any();
        }
    }
}