using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;

namespace AzureFunctionUsingSelenium.Utilities
{
    public static class WebDriverUtility
    {
        public static IWebElement WaitForElement(RemoteWebDriver driver, By by, TimeSpan timeout)
        {
            WebDriverWait wait = new(driver, timeout);
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            return wait.Until(d => d.FindElement(by));
        }

        public static IWebElement WaitForElementToBeEnabled(RemoteWebDriver driver, By by, TimeSpan timeout)
        {
            WebDriverWait wait = new(driver, timeout);

            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));

            return wait.Until(driver =>
            {
                IWebElement element = driver.FindElement(by);
                return element.Enabled ? element : null;
            });
        }

        public static void HandleCaptcha(RemoteWebDriver driver)
        {
            IWebElement iframeElement = WaitForElement(driver, By.CssSelector("iframe[title='reCAPTCHA']"), TimeSpan.FromSeconds(22));
            driver.SwitchTo().Frame(iframeElement);

            IWebElement captchaCheckbox = WaitForElement(driver, By.CssSelector(".recaptcha-checkbox-border"), TimeSpan.FromSeconds(23));
            captchaCheckbox.Click();

            driver.SwitchTo().DefaultContent();
        }

        public static void WaitForUrlChange(RemoteWebDriver driver, string originalUrl, TimeSpan timeout)
        {
            WebDriverWait wait = new(driver, timeout);
            wait.Until(d => d.Url != originalUrl);
        }        
    }

}
