using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace AzureFunctionUsingSelenium.Services
{
    public class ErrorHandlingService
    {
        private readonly ILogger _logger;

        public ErrorHandlingService(ILogger logger)
        {
            _logger = logger;
        }

        public IActionResult HandleError(Exception ex)
        {
            _logger.LogError($"Error during execution: {ex.Message}");

            var errorMessage = new
            {
                Message = "An error occurred.",
                ExceptionMessage = ex.Message,
                ExceptionType = ex.GetType().ToString(),
                StackTrace = ex.StackTrace
            };

            return new ObjectResult(errorMessage)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
