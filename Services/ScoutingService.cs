using AzureFunctionUsingSelenium.Utilities;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Threading.Tasks;

namespace AzureFunctionUsingSelenium.Services
{
    public class ScoutingService
    {
        private readonly SeleniumService _seleniumService;
        private readonly ILogger _logger;
        private readonly string _username;
        private readonly string _password;

        public ScoutingService(SeleniumService seleniumService, ILogger logger, string username, string password)
        {
            _seleniumService = seleniumService;
            _logger = logger;
            _username = username;
            _password = password;
        }

        public async Task<dynamic> PerformLogin(RemoteWebDriver driver)
        {
            return await Task.Run(() =>
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                string loginUrl = "https://advancements.scouting.org/login";
                driver.Navigate().GoToUrl(loginUrl);                

                // Handle CAPTCHA
                WebDriverUtility.HandleCaptcha(driver);

                // Input credentials
                IWebElement userNameInput = WebDriverUtility.WaitForElement(driver, By.Id("qa_username"), TimeSpan.FromSeconds(20));
                userNameInput.SendKeys(_username);
                driver.FindElement(By.Id("qa_password")).SendKeys(_password);

                // Click login button
                IWebElement loginButton = WebDriverUtility.WaitForElementToBeEnabled(driver, By.Id("qa_login"), TimeSpan.FromSeconds(21));
                if (loginButton != null) loginButton.Click();

                // Wait for login to complete
                WebDriverUtility.WaitForUrlChange(driver, loginUrl, TimeSpan.FromSeconds(24));

                // Retrieve tokens from localStorage
                string loginDataJson = (string)js.ExecuteScript("return window.localStorage.getItem('LOGIN_DATA');");
                dynamic loginData = Newtonsoft.Json.JsonConvert.DeserializeObject(loginDataJson);

                return loginData;
            });
        }
    }
}
