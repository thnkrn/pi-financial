using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Pi.BackofficeService.Infrastructure;
using Pi.BackofficeService.Migrations;

namespace Pi.BackofficeService.Worker.Startup
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddEntityFrameworkMySql();
            services.AddEntityFrameworkNamingConventions();

            services._addPiDbContext<TicketDbContext>(configuration, "BackofficeService");
            services._addPiDbContext<BackofficeDbContext>(configuration, "BackofficeService");

            if (environment.IsDevelopment())
            {
                services._addPiDbContext<DataSeedingDbContext>(configuration, "BackofficeService");
            }

            return services;
        }

        private static IServiceCollection _addPiDbContext<TContext>(this IServiceCollection services,
            IConfiguration configuration, string connection)
            where TContext : DbContext
        {
            services.AddDbContext<TContext>((servicesProvider, dbOptions) =>
            {
                var migrationAssemblyName = Assembly.GetAssembly(typeof(PlaceHolderForAssemblyReference))!.GetName().Name;
                var connectionString = configuration.GetConnectionString(connection) ?? string.Empty;
                dbOptions.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
                    {
                        o.MigrationsAssembly(migrationAssemblyName);
                        o.MigrationsHistoryTable($"__{typeof(TContext).Name}");
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

