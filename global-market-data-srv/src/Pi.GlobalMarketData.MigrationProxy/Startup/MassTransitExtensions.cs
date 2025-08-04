namespace Pi.GlobalMarketData.MigrationProxy.Startup;

public static class MassTransitExtensions
{
    public static IServiceCollection SetupMassTransit(this IServiceCollection services,
        ConfigurationManager configuration, IWebHostEnvironment environment)
    {
        // Example init MassTransit
        //
        //var endpointNameFormatter = new KebabCaseEndpointNameFormatter(configuration.GetValue<string>("MassTransit:EndpointNamePrefix"), false);
        //services.AddMassTransit(x =>
        //{
        //    x.SetEndpointNameFormatter(endpointNameFormatter);

        //    // By default, sagas are in-memory, but should be changed to a durable
        //    // saga repository.
        //    x.SetSagaRepositoryProvider(new EntityFrameworkSagaRepositoryRegistrationProvider(c =>
        //    {
        //        c.ExistingDbContext<FundAccountOpeningDbContext>();
        //        c.ConcurrencyMode = ConcurrencyMode.Optimistic;
        //        c.UseMySql();
        //    }));

        //    var applicationAssembly = typeof(OpenFundAccountConsumer).Assembly;
        //    x.AddConsumers(applicationAssembly);
        //    x.AddSagaStateMachines(applicationAssembly);
        //    x.AddSagas(applicationAssembly);
        //    x.AddActivities(applicationAssembly);

        //    x.UsingAmazonSqs((context, cfg) => {
        //        var region = configuration.GetValue<string>("AwsSqs:Region");
        //        var accessKey = configuration.GetValue<string>("AwsSqs:AccessKey");
        //        var secretKey = configuration.GetValue<string>("AwsSqs:SecretKey");
        //        var serviceUrl = configuration.GetValue<string>("AwsSqs:ServiceUrl");
        //        cfg.Host(region, (x) =>
        //        {
        //            x.AccessKey(accessKey);
        //            x.SecretKey(secretKey);
        //            if (environment.IsDevelopment())
        //            {
        //                x.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = serviceUrl });
        //                x.Config(new AmazonSQSConfig { ServiceURL = serviceUrl });
        //            }
        //        });

        //        cfg.UseMessageRetry(c => {
        //            c.Interval(10, TimeSpan.FromMilliseconds(50));
        //            c.Handle<DbUpdateConcurrencyException>();
        //        });
        //        cfg.ConfigureEndpoints(context);
        //    });
        //});

        return services;
    }
}