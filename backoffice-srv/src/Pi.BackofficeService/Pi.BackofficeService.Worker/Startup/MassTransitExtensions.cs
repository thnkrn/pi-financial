using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Pi.BackofficeService.Application.Commands.Ticket;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Infrastructure;
using StateMachine = Pi.BackofficeService.Application.StateMachines.Ticket.StateMachine;

namespace Pi.BackofficeService.Worker.Startup
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection SetupMassTransit(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            var endpointNameFormatter = new KebabCaseEndpointNameFormatter(configuration.GetValue<string>("MassTransit:EndpointNamePrefix"), false);
            services.AddMassTransit(x =>
            {
                x.SetEndpointNameFormatter(endpointNameFormatter);

                var assembly = typeof(CreateTicketConsumer).Assembly;
                x.AddConsumers(assembly);
                x.AddSagaRepository<TicketState>().EntityFrameworkRepository(r =>
                {
                    r.UseMySql();
                    r.ExistingDbContext<TicketDbContext>();
                });
                x.AddSagas(assembly);
                x.AddActivities(assembly);
                x.AddSagaStateMachine<StateMachine, TicketState>();

                x.UsingAmazonSqs((context, cfg) =>
                {
                    var region = configuration.GetValue<string>("AwsSqs:Region");
                    var accessKey = configuration.GetValue<string>("AwsSqs:AccessKey");
                    var secretKey = configuration.GetValue<string>("AwsSqs:SecretKey");
                    var serviceUrl = configuration.GetValue<string>("AwsSqs:ServiceUrl");
                    cfg.UseDelayedMessageScheduler();
                    cfg.Host(region, (c) =>
                    {
                        c.AccessKey(accessKey);
                        c.SecretKey(secretKey);

                        if (!environment.IsDevelopment())
                        {
                            return;
                        }

                        c.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = serviceUrl });
                        c.Config(new AmazonSQSConfig { ServiceURL = serviceUrl });
                    });
                    cfg.UseMessageRetry(c =>
                    {
                        c.Interval(10, TimeSpan.FromMilliseconds(50));
                        c.Handle<DbUpdateConcurrencyException>();
                    });

                    cfg.MessageTopology.SetEntityNameFormatter(new PrefixEntityNameFormatter(
                        AmazonSqsBusFactory.MessageTopology.EntityNameFormatter,
                        configuration.GetValue<string>("MassTransit:EndpointNamePrefix")! + "_"));
                    cfg.AutoDelete = true;

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}

