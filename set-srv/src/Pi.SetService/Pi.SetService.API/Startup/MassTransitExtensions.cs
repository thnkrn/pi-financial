using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.EntityFrameworkCore;
using Pi.SetService.Application.ScopedFilters;
using Pi.SetService.Infrastructure;

namespace Pi.SetService.API.Startup
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection SetupMassTransit(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            var endpointNameFormatter = new KebabCaseEndpointNameFormatter(configuration.GetValue<string>("MassTransit:EndpointNamePrefix"), false);
            services.AddMassTransit(x =>
            {
                x.SetEndpointNameFormatter(endpointNameFormatter);

                // By default, sagas are in-memory, but should be changed to a durable
                // saga repository.
                x.SetSagaRepositoryProvider(new EntityFrameworkSagaRepositoryRegistrationProvider(c =>
                {
                    c.ExistingDbContext<SetDbContext>();
                    c.ConcurrencyMode = ConcurrencyMode.Optimistic;
                    c.UseMySql();
                }));

                x.UsingAmazonSqs((context, cfg) =>
                {
                    var region = configuration.GetValue<string>("AwsSqs:Region");
                    var accessKey = configuration.GetValue<string>("AwsSqs:AccessKey");
                    var secretKey = configuration.GetValue<string>("AwsSqs:SecretKey");
                    var serviceUrl = configuration.GetValue<string>("AwsSqs:ServiceUrl");
                    cfg.Host(region, (x) =>
                    {
                        x.AccessKey(accessKey);
                        x.SecretKey(secretKey);
                        if (environment.IsDevelopment())
                        {
                            x.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = serviceUrl });
                            x.Config(new AmazonSQSConfig { ServiceURL = serviceUrl });
                        }
                    });

                    cfg.UseSendFilter(typeof(TraceIdFilter<>), context);
                    cfg.UsePublishFilter(typeof(TraceIdFilter<>), context);
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

            return services;
        }
    }
}

