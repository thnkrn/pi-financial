using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Pi.GlobalMarketData.Infrastructure.Helpers;

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

    public static List<string> GetTopicList(IConfiguration configuration, string configurationKeys)
    {
        try
        {
            // Try to get topics from environment variables (Kubernetes injection)
            var topicsFromEnv = configuration.GetValue(configurationKeys, string.Empty);
            if (!string.IsNullOrWhiteSpace(topicsFromEnv))
                try
                {
                    var topicList = TopicList(topicsFromEnv);
                    if (topicList != null)
                        return topicList;
                }
                catch (JsonException)
                {
                    // Try with a simple comma-separated list approach as last resort
                    if (topicsFromEnv.Contains("[") && topicsFromEnv.Contains("]"))
                    {
                        var stripped = topicsFromEnv.Trim('[', ']');
                        var manualTopics = stripped
                            .Split(',')
                            .Select(t => t.Trim(' ', '"', '\\'))
                            .Where(t => !string.IsNullOrWhiteSpace(t))
                            .ToList();

                        if (manualTopics.Count > 0)
                        {
                            Console.WriteLine($"Manually extracted topics: {string.Join(", ", manualTopics)}");
                            return manualTopics;
                        }
                    }
                }

            // Fallback to appsettings.json if environment variables are not set
            var topicsFromConfig = configuration.GetSection(configurationKeys)
                .GetChildren()
                .Select(x => x.Value)
                .Where(topic => !string.IsNullOrWhiteSpace(topic))
                .ToList();

            if (topicsFromConfig.Count > 0)
            {
                Console.WriteLine($"Using topics from configuration: {string.Join(", ", topicsFromConfig)}");
                return topicsFromConfig.OfType<string>().ToList();
            }

            throw new InvalidOperationException("No Kafka topics configured. Please check your configuration.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"No Kafka topics configured. {ex.Message}.", ex);
        }
    }

    private static List<string>? TopicList(string topicsFromEnv)
    {
        if (topicsFromEnv.Contains("\\\""))
        {
            // Replace escaped quotes
            var normalizedJson = topicsFromEnv.Replace("\\\"", "\"");
            var topics = JsonConvert.DeserializeObject<List<string>>(normalizedJson);
            if (topics is { Count: > 0 })
                return topics;
        }
        else
        {
            // Regular JSON parsing
            var topics = JsonConvert.DeserializeObject<List<string>>(topicsFromEnv);
            if (topics is { Count: > 0 })
                return topics;
        }

        return null;
    }
}