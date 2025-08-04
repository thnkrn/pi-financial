using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Pi.StructureNotes.Infrastructure.Repositories;
using Pi.StructureNotes.Migrations;
using Serilog;

namespace Pi.StructureNotes.API.Startup;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContexts(this IServiceCollection services,
        IHostEnvironment environment, IConfiguration configuration)
    {
        services.AddEntityFrameworkMySql();
        services.AddEntityFrameworkNamingConventions();

        string? migrationAssemblyName = Assembly.GetAssembly(typeof(PlaceHolderForAssemblyReference))!.GetName().Name;

        services.AddDbContext<SnDbContext>((servicesProvider, dbOptions) =>
        {
            string? connectionString = configuration.GetConnectionString("StructureNoteService");
            dbOptions.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
                {
                    o.MigrationsAssembly(migrationAssemblyName);
                    o.MigrationsHistoryTable($"__{nameof(SnDbContext)}");
                })
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging(configuration.GetValue<bool>("EnableDbSensitiveDataLogging"))
                .UseSnakeCaseNamingConvention()
                .UseInternalServiceProvider(servicesProvider)
                .LogTo(Log.Logger.Information, environment.IsDevelopment() ? LogLevel.Debug : LogLevel.Information);
            ;
        });

        return services;
    }
}
