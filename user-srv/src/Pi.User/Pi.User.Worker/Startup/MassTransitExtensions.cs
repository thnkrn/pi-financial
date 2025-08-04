using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Pi.Common.Observability;
using Pi.User.Application.Commands;
using Pi.User.Application.IntegrationEventHandlers.OnlineDirectDebitRegistrationSuccess;
using Pi.User.Application.IntegrationEventHandlers.OpenAccountSuccess;
using Pi.User.Worker.Consumers;

namespace Pi.User.Worker.Startup
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection SetupMassTransit(this IServiceCollection services,
            IConfiguration configuration, IHostEnvironment environment)
        {
            var endpointNameFormatter =
                new KebabCaseEndpointNameFormatter(configuration.GetValue<string>("MassTransit:EndpointNamePrefix"),
                    false);
            services.AddMassTransit(x =>
            {
                x.SetEndpointNameFormatter(endpointNameFormatter);
                x.UsingAmazonSqs((context, cfg) =>
                {
                    var region = configuration.GetValue<string>("AwsSqs:Region");
                    cfg.Host(region, (x) =>
                    {
                        if (environment.IsDevelopment())
                        {
                            var accessKey = configuration.GetValue<string>("AwsSqs:AccessKey");
                            var secretKey = configuration.GetValue<string>("AwsSqs:SecretKey");
                            var serviceUrl = configuration.GetValue<string>("AwsSqs:ServiceUrl");

                            x.AccessKey(accessKey);
                            x.SecretKey(secretKey);

                            x.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = serviceUrl });
                            x.Config(new AmazonSQSConfig { ServiceURL = serviceUrl });
                        }
                    });

                    cfg.UseMessageRetry(c =>
                    {
                        c.Interval(10, TimeSpan.FromMilliseconds(50));
                        c.Handle<DbUpdateConcurrencyException>();
                    });

                    if (configuration.GetValue<bool>("IsPrEnv"))
                    {
                        cfg.MessageTopology.SetEntityNameFormatter(new PrefixEntityNameFormatter(
                            AmazonSqsBusFactory.CreateMessageTopology().EntityNameFormatter,
                            configuration.GetValue<string>("MassTransit:EndpointNamePrefix")! + "_"));
                        cfg.AutoDelete = true;
                    }

                    cfg.Message<CreateOrUpdateDevice>(x => { x.SetEntityName($"{endpointNameFormatter.Consumer<CreateOrUpdateDeviceConsumer>()}.fifo"); });

                    cfg.ConfigureEndpoints(context);
                });

                // Add Consumer
                var applicationAssembly = typeof(UpdateNotificationPreferenceConsumer).Assembly;
                x.AddConsumers(applicationAssembly);
                x.AddConsumer<CreateOrUpdateDeviceConsumer, CreateOrUpdateDeviceConsumerDefinition>();
            });

            return services;
        }
    }
}