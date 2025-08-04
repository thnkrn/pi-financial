using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Pi.FundMarketData.Repositories;
using Pi.FundMarketData.Repositories.SqlDatabase;

namespace Pi.FundMarketData.Worker.Startup
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CommonDbContext>(x =>
            {
                var connectionString = configuration.GetConnectionString("CommonDatabase");
                x.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
                {
                    o.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                })
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging(configuration.GetValue<bool>("EnableDbSensitiveDataLogging"))
                .UseSnakeCaseNamingConvention();
            });

            return services;
        }
    }
}

