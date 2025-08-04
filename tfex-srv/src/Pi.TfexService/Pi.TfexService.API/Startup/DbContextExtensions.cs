using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Pi.TfexService.Infrastructure;
using Pi.TfexService.Migrations;
using Serilog;

namespace Pi.TfexService.API.Startup
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            services.AddEntityFrameworkMySql();
            services.AddEntityFrameworkNamingConventions();
            var migrationAssemblyName = Assembly.GetAssembly(typeof(PlaceHolderForAssemblyReference))!.GetName().Name;

            services.AddDbContext<TfexDbContext>((servicesProvider, dbOptions) =>
            {
                var connectionString = configuration.GetConnectionString("TfexService");
                dbOptions.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
                    {
                        o.MigrationsAssembly(migrationAssemblyName);
                        o.MigrationsHistoryTable($"__{nameof(TfexDbContext)}");
                    })
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging(configuration.GetValue<bool>("EnableDbSensitiveDataLogging"))
                    .UseSnakeCaseNamingConvention()
                    .UseInternalServiceProvider(servicesProvider)
                    .LogTo(Log.Logger.Information, environment.IsDevelopment() ? LogLevel.Debug : LogLevel.Information, null);
            });

            return services;
        }
    }
}

