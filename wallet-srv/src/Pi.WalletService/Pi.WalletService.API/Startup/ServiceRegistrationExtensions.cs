using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.Extensions.Options;
using Pi.Client.CgsBank.Api;
using Pi.Client.ExanteTrade.Api;
using Pi.Client.ExanteUserManagement.Api;
using Pi.Client.FxService.Api;
using Pi.Client.OnboardService.Api;
using Pi.Client.PaymentService.Api;
using Pi.Client.Sirius.Api;
using Pi.Client.UserService.Api;
using Pi.Common.Features;
using Pi.Financial.Client.Freewill;
using Pi.Financial.Client.Freewill.Api;
using Pi.Financial.Client.Settrade.Api;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services;
using Pi.WalletService.Application.Services.Bank;
using Pi.WalletService.Application.Services.CustomerService;
using Pi.WalletService.Application.Services.Event;
using Pi.WalletService.Application.Services.Freewill;
using Pi.WalletService.Application.Services.FxService;
using Pi.WalletService.Application.Services.GlobalEquities;
using Pi.WalletService.Application.Services.Measurement;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.Application.Services.PaymentService;
using Pi.WalletService.Application.Services.SetTrade;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.AtsDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.AtsWithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.LogAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.RefundAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.OddWithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.RefundInfoAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
using Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;
using Pi.WalletService.Domain.AggregatesModel.WalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Infrastructure.Options;
using Pi.WalletService.Infrastructure.Repositories;
using Pi.WalletService.Infrastructure.Services;
using Pi.WalletService.Infrastructure.UnitOfWork;
using StackExchange.Redis;

namespace Pi.WalletService.API.Startup
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            // services.AddSingleton(_ => Agent.Tracer);
            // services.AddSingleton<IMeasurement, Measurement>();
            // services.AddSingleton<IObserver, Observer>();

            if (configuration.GetValue<bool>("Redis:Enabled"))
            {
                // Add cache service
                services.AddStackExchangeRedisCache(options =>
                {
                    options.ConfigurationOptions = new ConfigurationOptions
                    {
                        AbortOnConnectFail = configuration.GetValue<bool>("Redis:AbortOnConnectFail"),
                        Ssl = configuration.GetValue<bool>("Redis:Ssl"),
                        ClientName = configuration.GetValue<string>("Redis:ClientName"),
                        ConnectRetry = configuration.GetValue<int>("Redis:ConnectRetry"),
                        ConnectTimeout = configuration.GetValue<int>("Redis:ConnectTimeout"),
                        SyncTimeout = configuration.GetValue<int>("Redis:SyncTimeout"),
                        DefaultDatabase = configuration.GetValue<int>("Redis:Database"),
                        EndPoints =
                        {
                            {
                                configuration.GetValue<string>("Redis:Host"), configuration.GetValue<int>("Redis:Port")
                            }
                        },
                        User = configuration.GetValue<string>("Redis:Username"),
                        Password = configuration.GetValue<string>("Redis:Password")
                    };
                    options.ConnectionMultiplexerFactory = async () =>
                    {
                        var connection = await ConnectionMultiplexer.ConnectAsync(options.ConfigurationOptions);
                        return connection;
                    };
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            services.AddScoped<IGlobalWalletTransactionHistoryRepository, GlobalWalletTransactionHistoryRepository>();
            services.AddScoped<ITransactionHistoryRepository, TransactionHistoryRepository>();
            services.AddScoped<IWalletQueries, WalletQueries>();
            services.AddScoped<IExchangeQueries, ExchangeQueries>();
            services.AddScoped<ITransactionQueries, TransactionQueries>();
            services.AddScoped<ITransactionQueriesV2, TransactionQueriesV2>();
            services.AddScoped<IThaiEquityQueries, ThaiEquityQueries>();
            services.AddScoped<IGlobalEquityQueries, GlobalEquityQueries>();
            services.AddScoped<IDepositRepository, DepositRepository>();
            services.AddScoped<IWithdrawRepository, WithdrawRepository>();
            services.AddScoped<ICashDepositRepository, CashDepositRepository>();
            services.AddScoped<ICashWithdrawRepository, CashWithdrawRepository>();
            services.AddScoped<IRefundRepository, RefundRepository>();
            services.AddScoped<IGlobalWalletDepositRepository, GlobalWalletDepositRepository>();
            services.AddScoped<IFreewillRequestLogRepository, FreewillRequestLogRepository>();
            services.AddScoped<IDepositEntrypointRepository, DepositEntrypointRepository>();
            services.AddScoped<IWithdrawEntrypointRepository, WithdrawEntrypointRepository>();
            services.AddScoped<IQrDepositRepository, QrDepositRepository>();
            services.AddScoped<IOddDepositRepository, OddDepositRepository>();
            services.AddScoped<IAtsDepositRepository, AtsDepositRepository>();
            services.AddScoped<IOddWithdrawRepository, OddWithdrawRepository>();
            services.AddScoped<IAtsWithdrawRepository, AtsWithdrawRepository>();
            services.AddScoped<IUpBackRepository, UpBackRepository>();
            services.AddScoped<IGlobalTransferRepository, GlobalTransferRepository>();
            services.AddScoped<ISagaUnitOfWork, SagaUnitOfWork>();
            services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IRefundInfoRepository, RefundInfoRepository>();
            services.AddScoped<IGlobalTradeService, ExanteTradeService>();
            services.AddScoped<IGlobalUserManagementService, ExanteUserManagementService>();
            services.AddScoped<IBankService, CgsBankService>();
            services.AddScoped<IBankInfoService, BankInfoService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IOnboardService, OnboardService>();
            services.AddScoped<ICustomerService, FreewillCustomerService>();
            services.AddScoped<ICallbackForwarderService, SiriusCallbackForwarderService>();
            services.AddScoped<ITransactionHistoryService, SiriusService>();
            services.AddScoped<ISetTradeService, SetTradeTransferService>();
            services.AddScoped<IEventGeneratorService, EventGeneratorService>();
            services.AddScoped<IFreewillRequestLogQueries, FreewillRequestLogQueries>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IHolidayRepository, HolidayRepository>();
            services.AddScoped<IDateQueries, DateQueries>();
            services.AddScoped<IEmailHistoryRepository, EmailHistoryRepository>();
            services.AddScoped<ICustomerModuleApi>(sp =>
                new CustomerModuleApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("Freewill"),
                    configuration.GetValue<string>("Freewill:Host") ?? string.Empty));
            services.AddScoped<ICreditModuleApi>(sp =>
                new CreditModuleApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("Freewill"),
                    configuration.GetValue<string>("Freewill:Host") ?? string.Empty));
            services.AddScoped<IFxService, FxService>();
            services.AddScoped<IEncryptionProvider, EncryptionService>();
            services.AddScoped<IExchangeApi>(sp =>
                new ExchangeApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("Fx"),
                    configuration.GetValue<string>("Fx:Host") ?? string.Empty));
            services.AddScoped<ICgsBankApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<CgsBankServiceOptions>>();
                return new CgsBankApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("CgsBank"), new Client.CgsBank.Client.Configuration
                    {
                        BasePath = options.Value.Host,
                    });
            });
            services.AddScoped<IUserApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<UserServiceOptions>>();
                return new UserApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("User"), new Client.UserService.Client.Configuration
                    {
                        BasePath = options.Value.Host,
                    });
            });
            services.AddScoped<IUserTradingAccountApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<UserServiceOptions>>();
                return new UserTradingAccountApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("User"), new Client.UserService.Client.Configuration
                    {
                        BasePath = options.Value.Host,
                    });
            });
            services.AddScoped<IUserBankAccountApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<UserServiceOptions>>();
                return new UserBankAccountApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("User"), new Client.UserService.Client.Configuration
                    {
                        BasePath = options.Value.Host,
                    });
            });
            services.AddScoped<ITradingAccountApi>(sp =>
                new TradingAccountApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Onboard"),
                    configuration.GetValue<string>("Onboard:Host") ?? string.Empty));
            services.AddScoped<IBankAccountApi>(sp =>
                new BankAccountApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Onboard"),
                    configuration.GetValue<string>("Onboard:Host") ?? string.Empty));
            services.AddScoped<ICustomerInfoApi>(sp =>
                new CustomerInfoApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Onboard"),
                    configuration.GetValue<string>("Onboard:Host") ?? string.Empty));
            services.AddScoped<IPaymentApi>(sp =>
                new PaymentApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Payment"),
                    configuration.GetValue<string>("Payment:Host") ?? string.Empty));
            services.AddScoped<IExanteUserManagementApi>(sp =>
                new ExanteUserManagementApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ExanteUserManagement"),
                    configuration.GetValue<string>("Exante:UserManagementHost") ?? string.Empty));
            services.AddScoped<IExanteTradeApi>(sp =>
                new ExanteTradeApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ExanteTrade"),
                    configuration.GetValue<string>("Exante:TradeHost") ?? string.Empty));
            services.AddScoped<ISiriusApi>(sp =>
                new SiriusApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Sirius"),
                    configuration.GetValue<string>("Sirius:Host") ?? string.Empty));
            services.AddScoped<ISetTradeApi>(sp =>
                new SetTradeApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("SetTrade"),
                    configuration.GetValue<string>("SetTrade:Host") ?? string.Empty));
            services.AddScoped<IFreewillSecurityPolicyHandler>((sp) =>
            {
                var options = sp.GetRequiredService<IOptions<FreewillOptions>>();
                return new FreewillSecurityPolicyHandler(options.Value.Requester,
                    options.Value.Application, options.Value.KeyBase, options.Value.IvCode);
            });

            services.AddScoped<IFeatureService>(sp
                =>
            {
                var context = sp.GetRequiredService<IHttpContextAccessor>();
                var headers = context.HttpContext?.Request.Headers.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value.ToArray(),
                    StringComparer.OrdinalIgnoreCase
                );

                return new FeatureService(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("GrowthBook"),
                    sp.GetRequiredService<ILogger<FeatureService>>(),
                    configuration.GetValue<string>("GrowthBook:ApiKey") ?? string.Empty,
                    configuration.GetValue<string>("GrowthBook:ProjectId") ?? string.Empty,
                    attributes: headers != null ? FeatureService.GetAttributes(headers!) : new Dictionary<string, string>()
                );
            });

            services.AddOptions<DatabaseOptions>()
                .Bind(configuration.GetSection(DatabaseOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<OnlineDirectDebitOptions>()
                .Bind(configuration.GetSection(OnlineDirectDebitOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<CgsBankServiceOptions>()
                .Bind(configuration.GetSection(CgsBankServiceOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<UserServiceOptions>()
                .Bind(configuration.GetSection(UserServiceOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<OnboardServiceOptions>()
                .Bind(configuration.GetSection(OnboardServiceOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<PaymentServiceOptions>()
                .Bind(configuration.GetSection(PaymentServiceOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<FreewillOptions>()
                .Bind(configuration.GetSection(FreewillOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<SetTradeOptions>()
                .Bind(configuration.GetSection(SetTradeOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<IMetric, MetricService>();

            services.AddOptions<FeeOptions>()
                .Bind(configuration.GetSection(FeeOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<FeaturesOptions>()
                .Bind(configuration.GetSection(FeaturesOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<MockOptions>()
                .Bind(configuration.GetSection(MockOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<QrCodeOptions>()
                .Bind(configuration.GetSection(QrCodeOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<TransactionNoCutOffTimeOptions>()
                .Bind(configuration.GetSection(TransactionNoCutOffTimeOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<FxOptions>()
                .Bind(configuration.GetSection(FxOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}
