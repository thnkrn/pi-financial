namespace Pi.SetMarketData.DataMigrationConsumer.Startup;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Example init DbContext
        //
        // services.AddDbContext<FundAccountOpeningDbContext>(x =>
        // {
        //     var connectionString = configuration.GetConnectionString("SetMarketData");
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