using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Commands.ODD;
using Pi.WalletService.Application.Commands.Withdraw;
using Pi.WalletService.Infrastructure;
using Pi.WalletService.Infrastructure.SagaConfigs;
using Pi.WalletService.Worker.ConsumerDefinitions;
using AtsDepositState = Pi.WalletService.Domain.AggregatesModel.AtsDepositAggregate.AtsDepositState;
using CashDepositState = Pi.WalletService.Domain.AggregatesModel.CashAggregate.CashDepositState;
using CashWithdrawState = Pi.WalletService.Domain.AggregatesModel.CashAggregate.CashWithdrawState;
using DepositEntrypointState = Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate.DepositEntrypointState;
using WithdrawEntrypointState = Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate.WithdrawEntrypointState;
using DepositState = Pi.WalletService.Domain.AggregatesModel.DepositAggregate.DepositState;
using GlobalManualAllocationState = Pi.WalletService.Domain.AggregatesModel.GlobalWalletManualAllocationAggregate.GlobalManualAllocationState;
using GlobalTransferState = Pi.WalletService.Domain.AggregatesModel.GlobalTransfer.GlobalTransferState;
using GlobalWalletTransferState = Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState;
using OddDepositState = Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate.OddDepositState;
using OddWithdrawState = Pi.WalletService.Domain.AggregatesModel.OddWithdrawAggregate.OddWithdrawState;
using AtsWithdrawState = Pi.WalletService.Domain.AggregatesModel.AtsWithdrawAggregate.AtsWithdrawState;
using QrDepositState = Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate.QrDepositState;
using RecoveryState = Pi.WalletService.Domain.AggregatesModel.RecoveryAggregate.RecoveryState;
using RefundState = Pi.WalletService.Domain.AggregatesModel.RefundAggregate.RefundState;
using UpBackState = Pi.WalletService.Domain.AggregatesModel.UpBackAggregate.UpBackState;
using StateMachine = Pi.WalletService.Application.StateMachines.GlobalTransfer.StateMachine;
using WithdrawState = Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate.WithdrawState;

namespace Pi.WalletService.Worker.Startup
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection SetupMassTransit(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            var endpointNameFormatter = new KebabCaseEndpointNameFormatter(configuration.GetValue<string>("MassTransit:EndpointNamePrefix"), false);

            services.AddMassTransit(x =>
            {
                x.SetEndpointNameFormatter(endpointNameFormatter);

                var applicationAssembly = typeof(RequestOnlineDirectDebitRegistrationConsumer).Assembly;
                x.AddConsumers(applicationAssembly);
                x.AddConsumer<ProcessKkpWithdrawCallbackConsumer, KkpWithdrawConsumerDefinition>();
                x.AddConsumer<ProcessKkpDepositCallbackConsumer, KkpDepositConsumerDefinition>();
                x.AddSagaStateMachine<Application.StateMachines.Deposit.StateMachine, DepositState, DepositStateSagaDefinitions>();
                x.AddSagaStateMachine<Application.StateMachines.Withdraw.StateMachine, WithdrawState, WithdrawStateSagaDefinitions>();
                x.AddSagaStateMachine<Application.StateMachines.GlobalWalletTransfer.StateMachine, GlobalWalletTransferState, GlobalWalletTransferStateSagaDefinitions>();
                x.AddSagaStateMachine<Application.StateMachines.GlobalWalletManualAllocation.StateMachine, GlobalManualAllocationState, GlobalManualAllocationStateSagaDefinitions>();
                x.AddSagaStateMachine<Application.StateMachines.Refund.StateMachine, RefundState, RefundStateSagaDefinitions>();
                x.AddSagaStateMachine<Application.StateMachines.CashDeposit.StateMachine, CashDepositState, CashDepositStateSagaDefinitions>();
                x.AddSagaStateMachine<Application.StateMachines.CashWithdraw.StateMachine, CashWithdrawState, CashWithdrawStateSagaDefinitions>();
                // wallet v2
                x.AddSagaStateMachine<Application.StateMachines.DepositEntrypoint.StateMachine, DepositEntrypointState, DepositEntrypointStateSagaDefinitions>();
                x.AddSagaStateMachine<Application.StateMachines.WithdrawEntrypoint.StateMachine, WithdrawEntrypointState, WithdrawEntrypointStateSagaDefinitions>();
                x.AddSagaStateMachine<Application.StateMachines.QrDeposit.StateMachine, QrDepositState, QrDepositStateSagaDefinitions>();
                x.AddSagaStateMachine<Application.StateMachines.OddDeposit.StateMachine, OddDepositState, OddDepositStateSagaDefinitions>();
                x.AddSagaStateMachine<Application.StateMachines.OddWithdraw.StateMachine, OddWithdrawState, OddWithdrawStateSagaDefinitions>();
                x.AddSagaStateMachine<Application.StateMachines.AtsDeposit.StateMachine, AtsDepositState, AtsDepositStateSagaDefinitions>();
                x.AddSagaStateMachine<Application.StateMachines.AtsWithdraw.StateMachine, AtsWithdrawState, AtsWithdrawStateSagaDefinitions>();
                x.AddSagaStateMachine<Application.StateMachines.UpBack.StateMachine, UpBackState, UpBackStateSagaDefinitions>();
                x.AddSagaStateMachine<StateMachine, GlobalTransferState, GlobalTransferStateSagaDefinitions>();
                x.AddSagaStateMachine<Application.StateMachines.Recovery.StateMachine, RecoveryState, RecoveryStateSagaDefinitions>();
                x.AddSagaRepository<DepositState>().EntityFrameworkRepository(UseWalletDbContext);
                x.AddSagaRepository<WithdrawState>().EntityFrameworkRepository(UseWalletDbContext);
                x.AddSagaRepository<GlobalWalletTransferState>().EntityFrameworkRepository(UseWalletDbContext);
                x.AddSagaRepository<GlobalManualAllocationState>().EntityFrameworkRepository(UseWalletDbContext);
                x.AddSagaRepository<RefundState>().EntityFrameworkRepository(UseWalletDbContext);
                x.AddSagaRepository<CashDepositState>().EntityFrameworkRepository(UseWalletDbContext);
                x.AddSagaRepository<CashWithdrawState>().EntityFrameworkRepository(UseWalletDbContext);
                // Wallet V2
                x.AddSagaRepository<DepositEntrypointState>().EntityFrameworkRepository(UseWalletDbContext);
                x.AddSagaRepository<WithdrawEntrypointState>().EntityFrameworkRepository(UseWalletDbContext);
                x.AddSagaRepository<QrDepositState>().EntityFrameworkRepository(UseWalletDbContext);
                x.AddSagaRepository<OddDepositState>().EntityFrameworkRepository(UseWalletDbContext);
                x.AddSagaRepository<OddWithdrawState>().EntityFrameworkRepository(UseWalletDbContext);
                x.AddSagaRepository<AtsDepositState>().EntityFrameworkRepository(UseWalletDbContext);
                x.AddSagaRepository<AtsWithdrawState>().EntityFrameworkRepository(UseWalletDbContext);
                x.AddSagaRepository<UpBackState>().EntityFrameworkRepository(UseWalletDbContext);
                x.AddSagaRepository<GlobalTransferState>().EntityFrameworkRepository(UseWalletDbContext);
                x.AddSagaRepository<RecoveryState>().EntityFrameworkRepository(UseWalletDbContext);

                x.AddSagas(applicationAssembly);
                x.AddActivities(applicationAssembly);
                x.AddDelayedMessageScheduler();
                x.UsingAmazonSqs((context, cfg) =>
                {
                    cfg.UseDelayedMessageScheduler();
                    var region = configuration.GetValue<string>("AwsSqs:Region");
                    cfg.Host(region, (c) =>
                    {
                        if (!environment.IsDevelopment())
                        {
                            return;
                        }

                        var accessKey = configuration.GetValue<string>("AwsSqs:AccessKey");
                        var secretKey = configuration.GetValue<string>("AwsSqs:SecretKey");
                        var serviceUrl = configuration.GetValue<string>("AwsSqs:ServiceUrl");
                        c.AccessKey(accessKey);
                        c.SecretKey(secretKey);

                        c.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = serviceUrl });
                        c.Config(new AmazonSQSConfig { ServiceURL = serviceUrl });
                    });
                    cfg.Publish<KkpWithdraw>(c =>
                    {
                        c.TopicAttributes["FifoTopic"] = "true";
                        c.TopicAttributes["ContentBasedDeduplication"] = "true";
                    });
                    cfg.Publish<KkpDeposit>(c =>
                    {
                        c.TopicAttributes["FifoTopic"] = "true";
                        c.TopicAttributes["ContentBasedDeduplication"] = "true";
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

            return services;
        }

        private static void UseWalletDbContext(IEntityFrameworkSagaRepositoryConfigurator r)
        {
            r.ConcurrencyMode = ConcurrencyMode.Optimistic;
            r.ExistingDbContext<WalletDbContext>();
            r.LockStatementProvider = new MySqlLockStatementProvider();
        }
    }
}
