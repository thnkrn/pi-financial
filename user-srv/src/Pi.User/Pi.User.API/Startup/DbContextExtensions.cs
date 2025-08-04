using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Pi.Common.Database;
using Pi.Common.Database.Interceptors;
using Pi.User.Infrastructure;
using Pi.User.Migrations;

namespace Pi.User.API.Startup
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            var EnableSensitiveDataLogging = configuration.GetValue<bool>("EnableSensitiveDataLogging");
            var migrationAssemblyName = Assembly.GetAssembly(typeof(PlaceHolderForAssemblyReference))!.GetName().Name;
            services.AddEntityFrameworkMySql();
            services.AddEntityFrameworkNamingConventions();
            services.AddDbContextPool<UserDbContext>((serviceProvider, optionsBuilder) =>
            {
                var connectionString = configuration.GetConnectionString("User");
                optionsBuilder
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
                    {
                        o.MigrationsAssembly(migrationAssemblyName);
                        o.MigrationsHistoryTable($"__{nameof(UserDbContext)}");
                    })
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging(EnableSensitiveDataLogging)
                    .UseSnakeCaseNamingConvention()
                    .UseInternalServiceProvider(serviceProvider)
                    .AddInterceptors(new AuditableEntitiesInterceptor());
            });

            services.AddDbContextPool<CommonDbContext>(optionsBuilder =>
            {
                var connectionString = configuration.GetConnectionString("Common");
                optionsBuilder
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging(EnableSensitiveDataLogging)
                    .UseSnakeCaseNamingConvention();
            });

            services.AddDbContextPool<LegacyITDbContext>(optionsBuilder =>
            {
                var connectionString = configuration.GetConnectionString("LegacyIT");
                optionsBuilder
                    .UseSqlServer(connectionString, options => options.EnableRetryOnFailure())
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging(EnableSensitiveDataLogging);
                ;
            });

            return services;
        }
    }
}