using Pi.OnePort.TCP.API.BackgroundServices;
using Pi.OnePort.TCP.API.Options;
using Pi.OnePort.TCP.Api;
using Pi.OnePort.TCP.Client;
using Pi.OnePort.TCP.Generators;

namespace Pi.OnePort.TCP.API.Startup
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddSingleton<IOnePortResponseMapClient, OnePortResponseMapClient>();
            services.AddSingleton<IOnePortApi, OnePortApi>();
            services.AddSingleton<IResponseKeyGenerator, ResponseKeyGenerator>();
            services.AddSingleton<IResponseMap, ResponseMap>();
            services.AddOptions<OnePortOptions>()
                .Bind(configuration.GetSection(OnePortOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddOptions<OperationHoursOptions>()
                .Bind(configuration.GetSection(OperationHoursOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddHostedService<OnePortListener>();

            return services;
        }
    }
}

