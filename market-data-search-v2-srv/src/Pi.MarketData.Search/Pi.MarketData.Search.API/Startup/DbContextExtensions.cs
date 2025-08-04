using OpenSearch.Client;
using Pi.MarketData.Search.Infrastructure.Interfaces.Mongo;
using Pi.MarketData.Search.Infrastructure.Services.Mongo;
using StackExchange.Redis;

namespace Pi.MarketData.Search.API.Startup
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddSingleton<IMongoContext, MongoContext>();
            services.AddSingleton(typeof(IMongoService<>), typeof(MongoService<>));
            services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));

            services.AddSingleton<IOpenSearchClient>(sp =>
            {
                var openSearchConfig = configuration.GetSection("OpenSearch");
                var url = openSearchConfig["Host"] ?? throw new InvalidOperationException("OpenSearch Host not configured");
                var username = openSearchConfig["Username"] ?? throw new InvalidOperationException("OpenSearch username not configured");
                var password = openSearchConfig["Password"] ?? throw new InvalidOperationException("OpenSearch password not configured");
                var defaultIndex = openSearchConfig["DefaultIndex"] ?? throw new InvalidOperationException("OpenSearch default index not configured");

                var settings = new ConnectionSettings(new Uri(url))
                .BasicAuthentication(username, password)
                .DefaultIndex(defaultIndex)
                .ConnectionLimit(100)
                .RequestTimeout(TimeSpan.FromSeconds(30))
                .EnableHttpCompression();

                // Only disable certificate validation in development
                if (sp.GetService<IWebHostEnvironment>()?.IsDevelopment() ?? false)
                {
                    settings.ServerCertificateValidationCallback((o, certificate, chain, errors) => true);
                }

                return new OpenSearchClient(settings);
            });

            // Add Redis configuration
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConfig = configuration.GetSection("Redis");
                var host = redisConfig["Host"] ?? throw new InvalidOperationException("Redis host not configured");
                var port = redisConfig["Port"] ?? throw new InvalidOperationException("Redis port not configured");
                var username = redisConfig["Username"];
                var password = redisConfig["Password"];
                var useTls = redisConfig.GetValue<bool>("UseTls");
                var options = new ConfigurationOptions
                {
                    EndPoints = { $"{host}:{port}" },
                    AbortOnConnectFail = false,
                    Ssl = useTls
                };

                if (!string.IsNullOrEmpty(username))
                    options.User = username;

                if (!string.IsNullOrEmpty(password))
                    options.Password = password;

                return ConnectionMultiplexer.Connect(options);
            });

            return services;
        }
    }
}