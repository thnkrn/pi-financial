using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.EntityFrameworkCore;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.ScopedFilters;
using Pi.Financial.FundService.Infrastructure;

namespace Pi.Financial.FundService.Worker.Startup
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection SetupMassTransit(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            var endpointNameFormatter = new KebabCaseEndpointNameFormatter(configuration.GetValue<string>("MassTransit:EndpointNamePrefix"), false);
            EndpointConvention.Map<SendOpenSuccessCallback>(new Uri($"queue:{endpointNameFormatter.Consumer<SendOpenSuccessCallbackConsumer>()}"));
            EndpointConvention.Map<SyncCustomerData>(new Uri($"queue:{endpointNameFormatter.Consumer<SyncCustomerDataConsumer>()}"));
            EndpointConvention.Map<SubscriptFund>(new Uri($"queue:{endpointNameFormatter.Consumer<SubscriptionFundConsumer>()}"));
            EndpointConvention.Map<RedeemFund>(new Uri($"queue:{endpointNameFormatter.Consumer<RedemptionFundConsumer>()}"));

            services.AddMassTransit(x =>
            {
                x.SetEndpointNameFormatter(endpointNameFormatter);

                // By default, sagas are in-memory, but should be changed to a durable
                // saga repository.
                x.SetSagaRepositoryProvider(new EntityFrameworkSagaRepositoryRegistrationProvider(c =>
                {
                    c.ExistingDbContext<FundDbContext>();
                    c.ConcurrencyMode = ConcurrencyMode.Optimistic;
                    c.UseMySql();
                }));

                var applicationAssembly = typeof(OpenFundAccountConsumer).Assembly;
                x.AddConsumers(applicationAssembly);
                x.AddSagaStateMachines(applicationAssembly);
                x.AddSagas(applicationAssembly);
                x.AddActivities(applicationAssembly);

                x.UsingAmazonSqs((context, cfg) =>
                {
                    var region = configuration.GetValue<string>("AwsSqs:Region");
                    var accessKey = configuration.GetValue<string>("AwsSqs:AccessKey");
                    var secretKey = configuration.GetValue<string>("AwsSqs:SecretKey");
                    var serviceUrl = configuration.GetValue<string>("AwsSqs:ServiceUrl");
                    cfg.Host(region, (s) =>
                    {
                        if (environment.IsDevelopment())
                        {
                            s.AccessKey(accessKey);
                            s.SecretKey(secretKey);
                            s.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = serviceUrl });
                            s.Config(new AmazonSQSConfig { ServiceURL = serviceUrl });
                        }
                    });

                    cfg.UseConsumeFilter(typeof(TraceIdFilter<>), context);
                    cfg.UseExecuteActivityFilter(typeof(TraceIdFilter<>), context);
                    cfg.UseMessageRetry(c =>
                    {
                        c.Interval(10, TimeSpan.FromMilliseconds(50));
                        c.Handle<DbUpdateConcurrencyException>();
                    });
                    cfg.ConfigureEndpoints(context);
                    if (configuration.GetValue<bool>("IsPrEnv"))
                    {
                        cfg.MessageTopology.SetEntityNameFormatter(new PrefixEntityNameFormatter(
                            AmazonSqsBusFactory.CreateMessageTopology().EntityNameFormatter,
                            configuration.GetValue<string>("MassTransit:EndpointNamePrefix")! + "_"));
                        cfg.AutoDelete = true;
                    }
                });
            });

            services.AddOptions<MassTransitHostOptions>()
                .Configure(options =>
                {
                    options.WaitUntilStarted = true;
                });

            services.AddMediator(cfg =>
            {
                cfg.AddConsumer<SendOpenSuccessCallbackConsumer>();
                cfg.AddConsumer<SyncCustomerDataConsumer>();
                cfg.AddConsumer<SubscriptionFundConsumer>();
                cfg.AddConsumer<RedemptionFundConsumer>();
            });

            return services;
        }
    }
}
