using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Pi.FundMarketData.API.Startup
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, ConfigurationManager configuration)
        {
            // Example init DbContext
            //
            // services.AddDbContext<FundAccountOpeningDbContext>(x =>
            // {
            //     var connectionString = configuration.GetConnectionString("FundMarketData");
            //     x.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
            //     {
            //         o.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            //         o.MigrationsHistoryTable($"__{nameof(FundAccountOpeningDbContext)}");
            //     })
            //     .EnableDetailedErrors()
            //     .EnableSensitiveDataLogging(configuration.GetValue<bool>("EnableDbSensitiveDataLogging"))
            //     .UseSnakeCaseNamingConvention();
            // });

            return services;
        }
    }
}

