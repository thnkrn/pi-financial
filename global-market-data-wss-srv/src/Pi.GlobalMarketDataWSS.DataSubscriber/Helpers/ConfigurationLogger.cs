using System.Collections;
using System.Text;
using Serilog;

namespace Pi.GlobalMarketDataWSS.DataSubscriber.Helpers;

public static class ConfigurationLogger
{
    public static void LogConfigurations()
    {
        const string production = "Production";
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? production;

        if (!env.Equals(production, StringComparison.OrdinalIgnoreCase))
        {
            Log.Information("---------- Environment Variables ----------");

            // Get all environment variables
            var environmentVariables = Environment.GetEnvironmentVariables();

            foreach (DictionaryEntry entry in environmentVariables)
            {
                var key = entry.Key.ToString();
                var value = entry.Value?.ToString() ?? string.Empty;

                // Mask sensitive data
                if (key != null && IsSensitiveKey(key)) value = "********";

                var builder = new StringBuilder();
                builder.Append($"Configuration Key: {key}{Environment.NewLine}");
                builder.Append($"Configuration Value: {value}{Environment.NewLine}");
                builder.Append($"{new string('-', 100)}{Environment.NewLine}");

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
            "KEY"
        };

        return Array.Exists(sensitivePatterns, pattern =>
            key.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }
}