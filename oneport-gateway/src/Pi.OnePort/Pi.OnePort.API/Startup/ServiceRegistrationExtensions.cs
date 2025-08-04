using Pi.OnePort.Db2.Repositories;

namespace Pi.OnePort.API.Startup
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddScoped<ISetRepo, SetRepo>();

            return services;
        }
    }
}

