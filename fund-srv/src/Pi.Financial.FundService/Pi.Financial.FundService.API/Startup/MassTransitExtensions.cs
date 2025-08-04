using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.ScopedFilters;

namespace Pi.Financial.FundService.API.Startup
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection SetupMassTransit(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            var endpointNameFormatter = new KebabCaseEndpointNameFormatter(configuration.GetValue<string>("MassTransit:EndpointNamePrefix"), false);
            EndpointConvention.Map<SendOpenSuccessCallback>(new Uri($"queue:{endpointNameFormatter.Consumer<SendOpenSuccessCallbackConsumer>()}"));
            EndpointConvention.Map<SyncCustomerData>(new Uri($"queue:{endpointNameFormatter.Consumer<SyncCustomerDataConsumer>()}"));

            services.AddMassTransit(x =>
            {
                x.SetEndpointNameFormatter(endpointNameFormatter);

                x.UsingAmazonSqs((context, cfg) =>
                {
                    var region = configuration.GetValue<string>("AwsSqs:Region");
                    cfg.Host(region, (s) =>
                    {
                        if (environment.IsDevelopment())
                        {
                            var accessKey = configuration.GetValue<string>("AwsSqs:AccessKey");
                            var secretKey = configuration.GetValue<string>("AwsSqs:SecretKey");
                            var serviceUrl = configuration.GetValue<string>("AwsSqs:ServiceUrl");

                            s.AccessKey(accessKey);
                            s.SecretKey(secretKey);

                            s.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = serviceUrl });
                            s.Config(new AmazonSQSConfig { ServiceURL = serviceUrl });
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
                    }

                    cfg.AutoDelete = true;
                });
            });

            services.AddOptions<MassTransitHostOptions>()
                .Configure(options =>
                {
                    options.WaitUntilStarted = true;
                });

            return services;
        }
    }
}
