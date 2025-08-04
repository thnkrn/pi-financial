using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Pi.Common.Observability;
using Pi.User.Application.Commands;

namespace Pi.User.API.Startup
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection SetupMassTransit(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            var endpointNameFormatter = new KebabCaseEndpointNameFormatter(configuration.GetValue<string>("MassTransit:EndpointNamePrefix"), false);
            services.AddMassTransit(x =>
            {
                x.SetEndpointNameFormatter(endpointNameFormatter);
                x.UsingAmazonSqs((context, cfg) =>
                {
                    var region = configuration.GetValue<string>("AwsSqs:Region");
                    cfg.Host(region, x =>
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
                    cfg.AutoStart = true;
                });
            });

            var applicationAssembly = typeof(CreateUserInfoConsumer).Assembly;
            services.AddMediator(cfg =>
            {
                cfg.AddConsumers(applicationAssembly);
            });

            return services;
        }
    }
}