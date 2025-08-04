using System.Collections;
using System.Text;
using Serilog;

namespace Pi.GlobalMarketData.Infrastructure.Services.Logging;

public static class ConfigurationLogger
{
    public static void LogConfigurations(bool maskSensitiveData = true)
    {
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        if (string.IsNullOrEmpty(env))
            env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        if (!env.Equals("Production", StringComparison.OrdinalIgnoreCase))
        {
            // Get all environment variables
            var environmentVariables = Environment.GetEnvironmentVariables();

            foreach (DictionaryEntry entry in environmentVariables)
            {
                var key = entry.Key.ToString();
                var value = entry.Value?.ToString() ?? string.Empty;

                // Mask sensitive data
                if (maskSensitiveData && key != null && IsSensitiveKey(key)) value = string.Empty;

                var builder = new StringBuilder();
                builder.Append($"Configuration Key: {key}, Value: {value}");

                Log.Information(builder.ToString());
            }
        }
    }

    private static bool IsSensitiveKey(string key)
    {
        var sensitivePatterns = new[]
        {
            "PASSWORD",
            "SECRET",
            "KEY",
            "TOKEN",
            "CREDENTIAL",
            "AUTH",
            "APIKEY",
            "CONNECTION",
            "PASS_WORD",
            "API_KEY"
        };

        return Array.Exists(sensitivePatterns, pattern =>
            key.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }
}