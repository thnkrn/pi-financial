using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Pi.WalletService.Application.Commands.Ats;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Commands.Withdraw;
using Pi.WalletService.Application.Commands.ODD;
using Pi.WalletService.Infrastructure;

namespace Pi.WalletService.API.Startup
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection SetupMassTransit(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            var endpointNameFormatter = new KebabCaseEndpointNameFormatter(configuration.GetValue<string>("MassTransit:EndpointNamePrefix"), false);
            EndpointConvention.Map<UpdateOnlineDirectDebitRegistration>(new Uri($"queue:{endpointNameFormatter.Consumer<UpdateOnlineDirectDebitRegistrationConsumer>()}"));
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

                    if (configuration.GetValue<bool>("IsPrEnv"))
                    {
                        cfg.MessageTopology.SetEntityNameFormatter(new PrefixEntityNameFormatter(
                            AmazonSqsBusFactory.MessageTopology.EntityNameFormatter,
                            configuration.GetValue<string>("MassTransit:EndpointNamePrefix")! + "_"));
                        cfg.AutoDelete = true;
                    }

                    cfg.ConfigureEndpoints(context);
                });

                x.AddEntityFrameworkOutbox<WalletDbContext>(o =>
                {
                    o.UseMySql();
                    o.UseBusOutbox();
                });
            });

            services.AddOptions<MassTransitHostOptions>().Configure(options =>
            {
                options.WaitUntilStarted = true;
            });

            services.AddMediator(cfg =>
            {
                cfg.AddConsumer<DepositCallbackConsumer>();
                cfg.AddConsumer<WithdrawCallbackConsumer>();
                cfg.AddConsumer<DepositAtsCallbackConsumer>();
                cfg.ConfigureMediator((context, mcfg) =>
                {
                    mcfg.UseInMemoryOutbox();
                });
            });

            return services;
        }
    }
}
