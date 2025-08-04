using Microsoft.Extensions.Configuration;

namespace Pi.SetMarketDataWSS.Infrastructure.Helpers;

public static class ConfigurationHelper
{
    public static IConfiguration GetConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                          ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                          ?? "Production";

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{environment}.json", true, true)
            .AddEnvironmentVariables();

        var config = builder.Build();

        if (config.GetValue<bool>("RemoteConfig:Enable"))
            try
            {
                var prefix = config.GetValue<string>("RemoteConfig:Prefix");
                var lifetime = config.GetValue<double>("RemoteConfig:LifetimeMS");

                builder.AddSystemsManager($"/{environment.ToLowerInvariant()}/{prefix}",
                    TimeSpan.FromMilliseconds(lifetime));
                return builder.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The Amazon SystemsManager builder error: {ex.Message}");
            }

        return config;
    }
}