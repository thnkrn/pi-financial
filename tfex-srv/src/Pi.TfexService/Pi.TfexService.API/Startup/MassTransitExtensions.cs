using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;

namespace Pi.TfexService.API.Startup;
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
                cfg.Host(region, (amazonSqsHostConfigurator) =>
                {
                    if (!environment.IsDevelopment())
                    {
                        return;
                    }

                    var accessKey = configuration.GetValue<string>("AwsSqs:AccessKey");
                    var secretKey = configuration.GetValue<string>("AwsSqs:SecretKey");
                    var serviceUrl = configuration.GetValue<string>("AwsSqs:ServiceUrl");

                    amazonSqsHostConfigurator.AccessKey(accessKey);
                    amazonSqsHostConfigurator.SecretKey(secretKey);

                    amazonSqsHostConfigurator.Config(new AmazonSimpleNotificationServiceConfig
                    { ServiceURL = serviceUrl });
                    amazonSqsHostConfigurator.Config(new AmazonSQSConfig { ServiceURL = serviceUrl });
                });

                if (configuration.GetValue<bool>("IsPrEnv"))
                {
                    cfg.MessageTopology.SetEntityNameFormatter(new PrefixEntityNameFormatter(
                        AmazonSqsBusFactory.CreateMessageTopology().EntityNameFormatter,
                        configuration.GetValue<string>("MassTransit:EndpointNamePrefix")! + "_"));
                    cfg.AutoDelete = true;
                }

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}


