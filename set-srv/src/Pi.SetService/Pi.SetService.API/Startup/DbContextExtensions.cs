using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Pi.SetService.Infrastructure;
using Pi.SetService.Migrations;

namespace Pi.SetService.API.Startup
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddEntityFrameworkMySql();
            services.AddEntityFrameworkNamingConventions();
            var migrationAssemblyName = Assembly.GetAssembly(typeof(PlaceHolderForAssemblyReference))!.GetName().Name;
            services.AddDbContext<SetDbContext>((servicesProvider, x) =>
            {
                var connectionString = configuration.GetConnectionString("SetService");
                x.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
                    {
                        o.MigrationsAssembly(migrationAssemblyName);
                        o.MigrationsHistoryTable($"__{nameof(SetDbContext)}");
                    })
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging(configuration.GetValue<bool>("EnableDbSensitiveDataLogging"))
                    .UseSnakeCaseNamingConvention()
                    .UseInternalServiceProvider(servicesProvider);
            });

            return services;
        }
    }
}

