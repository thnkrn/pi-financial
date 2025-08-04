using Pi.Financial.Client.SetTradeOms.Api;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Infrastructure.Options;
using Pi.TfexService.Infrastructure.Services;
using Pi.TfexService.Listener.Models;
using StackExchange.Redis;

namespace Pi.TfexService.Listener.Startup
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHostedService<Sockets.TfexListener>();

            if (configuration.GetValue<bool>("Redis:Enabled"))
            {
                // Add cache service
                services.AddStackExchangeRedisCache(options =>
                {
                    options.ConfigurationOptions = new ConfigurationOptions
                    {
                        AbortOnConnectFail = configuration.GetValue<bool>("Redis:AbortOnConnectFail"),
                        Ssl = configuration.GetValue<bool>("Redis:Ssl"),
                        ClientName = configuration.GetValue<string>("Redis:ClientName"),
                        ConnectRetry = configuration.GetValue<int>("Redis:ConnectRetry"),
                        ConnectTimeout = configuration.GetValue<int>("Redis:ConnectTimeout"),
                        SyncTimeout = configuration.GetValue<int>("Redis:SyncTimeout"),
                        DefaultDatabase = configuration.GetValue<int>("Redis:Database"),
                        EndPoints =
                        {
                            {
                                configuration.GetValue<string>("Redis:Host")!, configuration.GetValue<int>("Redis:Port")
                            }
                        },
                        User = configuration.GetValue<string>("Redis:Username"),
                        Password = configuration.GetValue<string>("Redis:Password")
                    };
                    options.ConnectionMultiplexerFactory = async () =>
                    {
                        var connection = await ConnectionMultiplexer.ConnectAsync(options.ConfigurationOptions);
                        return connection;
                    };
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            services.AddScoped<ISetTradeOmsApi>(sp =>
                new SetTradeOmsApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("SetTrade"),
                    configuration.GetValue<string>("SetTrade:Host") ?? string.Empty));

            services.AddScoped<ISetTradeService, SetTradeService>();
            services.AddScoped<IMqttClientFactory, MqttClientFactory>();

            services.AddOptions<SetTradeOptions>()
                .Bind(configuration.GetSection(SetTradeOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<SetTradeStreamOptions>()
                .Bind(configuration.GetSection(SetTradeStreamOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<OperationHoursOptions>()
                .Bind(configuration.GetSection(OperationHoursOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<FeaturesOptions>()
                .Bind(configuration.GetSection(FeaturesOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}
