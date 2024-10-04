using System;

namespace AzureFunctionUsingSelenium.Services
{
    public class ConfigurationService
    {
        public string GetEnvironmentVariable(string key)
        {
            string value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception($"Environment variable {key} is not set.");
            }
            return value;
        }
    }

}
