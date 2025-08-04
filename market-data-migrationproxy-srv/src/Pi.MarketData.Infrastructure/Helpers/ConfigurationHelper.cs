using Microsoft.Extensions.Configuration;

namespace Pi.MarketData.Infrastructure.Helpers;

public static class ConfigurationHelper
{
    public static IConfiguration GetConfiguration()
    {
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        if (string.IsNullOrEmpty(env))
            env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{env}.json", true)
            .AddEnvironmentVariables();

        var config = builder.Build();

        if (!config.GetValue<bool>("RemoteConfig:Enable")) return config;

        try
        {
            var prefix = config.GetValue<string>("RemoteConfig:Prefix");
            var lifetime = config.GetValue<double>("RemoteConfig:LifetimeMS");

            builder.AddSystemsManager($"/{env.ToLowerInvariant()}/{prefix}", TimeSpan.FromMilliseconds(lifetime));
            return builder.Build();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"The Amazon SystemsManager builder error: {ex.Message}");
            return config;
        }
    }
}