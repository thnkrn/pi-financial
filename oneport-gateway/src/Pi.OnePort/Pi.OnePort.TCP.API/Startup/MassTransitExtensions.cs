using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Pi.OnePort.IntegrationEvents;

namespace Pi.OnePort.TCP.API.Startup
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection SetupMassTransit(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            var mtEndpointPrefix = configuration.GetValue<string>("MassTransit:EndpointNamePrefix");
            var endpointNameFormatter = new KebabCaseEndpointNameFormatter(mtEndpointPrefix, false);
            services.AddMassTransit(x =>
            {
                x.SetEndpointNameFormatter(endpointNameFormatter);
                x.UsingAmazonSqs((context, cfg) =>
                {
                    var region = configuration.GetValue<string>("AwsSqs:Region");
                    cfg.Host(region, (x) =>
                    {
                        if (!environment.IsDevelopment())
                        {
                            return;
                        }

                        var accessKey = configuration.GetValue<string>("AwsSqs:AccessKey");
                        var secretKey = configuration.GetValue<string>("AwsSqs:SecretKey");
                        var serviceUrl = configuration.GetValue<string>("AwsSqs:ServiceUrl");

                        x.AccessKey(accessKey);
                        x.SecretKey(secretKey);

                        x.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = serviceUrl });
                        x.Config(new AmazonSQSConfig { ServiceURL = serviceUrl });
                    });

                    cfg.UseMessageRetry(c =>
                    {
                        c.Interval(10, TimeSpan.FromMilliseconds(50));
                        c.Handle<DbUpdateConcurrencyException>();
                    });

                    ConfigOnePortOrderEvent<OnePortBrokerOrderCreated>(cfg, endpointNameFormatter);
                    ConfigOnePortOrderEvent<OnePortOrderChanged>(cfg, endpointNameFormatter);
                    ConfigOnePortOrderEvent<OnePortOrderMatched>(cfg, endpointNameFormatter);
                    ConfigOnePortOrderEvent<OnePortOrderRejected>(cfg, endpointNameFormatter);
                    ConfigOnePortOrderEvent<OnePortOrderCanceled>(cfg, endpointNameFormatter);

                    cfg.ConfigureEndpoints(context);
                });
            });


            return services;
        }

        private static void ConfigOnePortOrderEvent<T>(
            IAmazonSqsBusFactoryConfigurator cfg,
            IEndpointNameFormatter formatter
        ) where T : class
        {
            cfg.Message<T>(x => { x.SetEntityName($"{EventEntityName.EntityName}.fifo"); });
        }
    }
}

