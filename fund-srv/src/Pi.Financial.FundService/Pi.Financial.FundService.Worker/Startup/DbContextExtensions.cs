using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Pi.Common.Database;
using Pi.Financial.FundService.Infrastructure;
using Pi.Financial.FundService.Migrations;

namespace Pi.Financial.FundService.Worker.Startup
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEntityFrameworkMySql();
            services.AddEntityFrameworkNamingConventions();
            var migrationAssemblyName = Assembly.GetAssembly(typeof(PlaceHolderForAssemblyReference))!.GetName().Name;
            services.AddDbContext<FundDbContext>((servicesProvider, x) =>
            {
                var connectionString = configuration.GetConnectionString("Fund");
                x.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
                {
                    o.MigrationsAssembly(migrationAssemblyName);
                    o.MigrationsHistoryTable($"__{nameof(FundDbContext)}");
                })
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging(configuration.GetValue<bool>("EnableDbSensitiveDataLogging"))
                .UseSnakeCaseNamingConvention()
                .UseInternalServiceProvider(servicesProvider);
            });
            services.AddDbContext<CommonDbContext>(x =>
            {
                var connectionString = configuration.GetConnectionString("Common");
                x.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging(configuration.GetValue<bool>("EnableDbSensitiveDataLogging"))
                    .UseSnakeCaseNamingConvention();
            });

            services.AddDbContext<CustomerDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Customer")));

            return services;
        }
    }
}
