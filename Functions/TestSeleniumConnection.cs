using AzureFunctionUsingSelenium.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenQA.Selenium.Remote;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AzureFunctionUsingSelenium.Functions
{
    public class TestSeleniumConnection
    {
        private readonly ILogger<GetLoginData> _logger;
        private readonly SeleniumService _seleniumService;
        private readonly ConfigurationService _configurationService;
        private readonly ErrorHandlingService _errorHandlingService;

        public TestSeleniumConnection(ILogger<GetLoginData> log)
        {
            _logger = log;
            _configurationService = new ConfigurationService();
            _seleniumService = new SeleniumService(_configurationService.GetEnvironmentVariable("SELENIUM_GRID_URL"), _logger);
            _errorHandlingService = new ErrorHandlingService(_logger);
        }

        [FunctionName("TestSeleniumConnection")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("TestSeleniumConnection Function Called");
            RemoteWebDriver driver = null;

            try
            {                

                // Initialize the Selenium WebDriver
                driver = _seleniumService.InitializeWebDriver();

                // Navigate to microsoft.com
                driver.Navigate().GoToUrl("https://www.microsoft.com");

                // Retrieve the page title
                var pageTitle = driver.Title;
                _logger.LogInformation($"Page title: {pageTitle}");

                // Return the page title as a response
                return new OkObjectResult($"Page Title: {pageTitle}");
            }
            catch (Exception ex)
            {
                return _errorHandlingService.HandleError(ex);                
            }
            finally
            {
               driver?.Dispose();
            }
        }
    }

}
