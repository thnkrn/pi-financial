using Microsoft.Extensions.Configuration;

namespace Pi.SetMarketDataWSS.DataStreamer;

public static class ConfigurationHelper
{
    public static IConfiguration GetConfiguration()
    {
        try
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json", true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            return config;
        }
        catch (Exception ex)
        {
            // Log and handle the error appropriately
            throw new Exception("Failed to load configuration", ex);
        }
    }
}