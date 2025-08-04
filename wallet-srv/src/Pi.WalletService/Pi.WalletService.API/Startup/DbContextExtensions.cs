using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Pi.Common.Database;
using Pi.WalletService.Infrastructure;
using Pi.WalletService.Migrations;
using Serilog;

namespace Pi.WalletService.API.Startup
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            services.AddEntityFrameworkMySql();
            services.AddEntityFrameworkNamingConventions();
            var migrationAssemblyName = Assembly.GetAssembly(typeof(PlaceHolderForAssemblyReference))!.GetName().Name;

            services.AddDbContext<WalletDbContext>((servicesProvider, dbOptions) =>
            {
                var connectionString = configuration.GetConnectionString("WalletService");
                dbOptions.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
                    {
                        o.MigrationsAssembly(migrationAssemblyName);
                        o.MigrationsHistoryTable($"__{nameof(WalletDbContext)}");
                    })
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging(configuration.GetValue<bool>("EnableDbSensitiveDataLogging"))
                    .UseSnakeCaseNamingConvention()
                    .UseInternalServiceProvider(servicesProvider)
                    .LogTo(Log.Logger.Information, environment.IsDevelopment() ? LogLevel.Debug : LogLevel.Information, null); ;
            });

            services.AddDbContext<CommonDbContext>((servicesProvider, dbOptions) =>
            {
                var connectionString = configuration.GetConnectionString("Common") ?? string.Empty;
                dbOptions.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
                    {
                        o.MigrationsAssembly(migrationAssemblyName);
                        o.MigrationsHistoryTable($"__{nameof(CommonDbContext)}");
                    })
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging(configuration.GetValue<bool>("EnableDbSensitiveDataLogging"))
                    .UseSnakeCaseNamingConvention();
            });

            return services;
        }
    }
}
