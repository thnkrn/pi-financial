using IBM.EntityFrameworkCore;
using IBM.Extensions.DependencyInjection;
using Pi.OnePort.Db2;

namespace Pi.OnePort.API.Startup
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddEntityFrameworkDb2();
            services.AddDbContext<IFisDbContext>((servicesProvider, x) =>
            {
                var connectionString = configuration.GetConnectionString("IFis");
                x.UseDb2(connectionString, p => p.SetServerInfo(IBMDBServerType.LUW, IBMDBServerVersion.LUW_11_01_2020))
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging(configuration.GetValue<bool>("EnableDbSensitiveDataLogging"));
            });

            return services;
        }
    }
}

