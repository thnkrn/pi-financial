using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Pi.BackofficeService.Infrastructure;
using Pi.BackofficeService.Migrations;

namespace Pi.BackofficeService.API.Startup;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContexts(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddEntityFrameworkMySql();
        services.AddEntityFrameworkNamingConventions();

        services._addPiDbContext<TicketDbContext>(configuration, "BackofficeService");
        services._addPiDbContext<BackofficeDbContext>(configuration, "BackofficeService");

        return services;
    }

    private static IServiceCollection _addPiDbContext<TContext>(this IServiceCollection services,
        ConfigurationManager configuration, string connection)
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
