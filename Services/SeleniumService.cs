using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Remote;
using System;

namespace AzureFunctionUsingSelenium.Services
{
    public class SeleniumService
    {
        private readonly string _seleniumGridUrl;
        private readonly ILogger _logger;

        public SeleniumService(string seleniumGridUrl, ILogger logger)
        {
            _seleniumGridUrl = seleniumGridUrl;
            _logger = logger;
        }

        public RemoteWebDriver InitializeWebDriver()
        {
            var edgeOptions = new EdgeOptions
            {
                PlatformName = "Linux"
            };

            return new RemoteWebDriver(new Uri(_seleniumGridUrl), edgeOptions.ToCapabilities(), TimeSpan.FromSeconds(180));
        }

        public void DisposeDriver(RemoteWebDriver driver)
        {
            driver?.Dispose();
        }        
    }
}
