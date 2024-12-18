using Microsoft.Extensions.Configuration;
using System;

namespace Utils
{
    public static class AppSettingsHelper
    {
        public static string GetStringValueFromConfig(this IConfiguration configuration, string configName)
        {
            string configValue = configuration[configName];

            if (string.IsNullOrEmpty(configValue))
            {
                throw new Exception("Error");
            }

            return configValue;
        }
    }
}