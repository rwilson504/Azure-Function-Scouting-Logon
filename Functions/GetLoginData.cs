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
    public class GetLoginData
    {
        private readonly ILogger<GetLoginData> _logger;
        private readonly SeleniumService _seleniumService;
        private readonly ScoutingService _scoutingService;
        private readonly ConfigurationService _configurationService;
        private readonly ErrorHandlingService _errorHandlingService;

        public GetLoginData(ILogger<GetLoginData> log)
        {
            _logger = log;
            _configurationService = new ConfigurationService();            
            _seleniumService = new SeleniumService(_configurationService.GetEnvironmentVariable("SELENIUM_GRID_URL"), _logger);
            _scoutingService = new ScoutingService(_seleniumService, _logger, _configurationService.GetEnvironmentVariable("BSA_USERNAME"), _configurationService.GetEnvironmentVariable("BSA_PASSWORD"));
            _errorHandlingService = new ErrorHandlingService(_logger);
        }

        [FunctionName("GetLoginData")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("GetLoginData Function Called");
            RemoteWebDriver driver = null;

            try
            {                
                _logger.LogInformation("Initializing WebDriver");
                driver = _seleniumService.InitializeWebDriver();

                _logger.LogInformation("Perform the login operation and get the tokens");
                dynamic loginData = await _scoutingService.PerformLogin(driver);

                return new JsonResult(loginData)
                {
                    StatusCode = StatusCodes.Status200OK 
                };
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
