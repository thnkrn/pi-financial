using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Pi.Client.OnboardService.Api;
using Pi.Client.PiSsoV2.Api;
using Pi.Client.Sirius.Api;
using Pi.Common.Cryptography;
using Pi.Common.Database.Repositories;
using Pi.Common.Domain.AggregatesModel.ProductAggregate;
using Pi.Common.Features;
using Pi.Common.HealthCheck;
using Pi.Financial.Client.Freewill.Api;
using Pi.User.Application.Queries;
using Pi.User.Application.Services.DeviceService;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
using Pi.User.Domain.AggregatesModel.TransactionIdAggregate;
using Pi.User.Infrastructure.Repositories;
using Pi.User.Infrastructure.Services;
using Pi.Financial.Client.Freewill;
using Pi.Common.Utilities;
using Pi.Financial.Client.PdfService.Api;
using Pi.User.Application.Options;
using Pi.User.Application.Services.Cryptography;
using Pi.User.Application.Services.LegacyUserInfo;
using Pi.User.Application.Services.Storage;
using Pi.User.Application.Services.Onboard;
using Pi.User.Domain.AggregatesModel.BankAccountAggregate;
using Pi.User.Domain.AggregatesModel.DocumentAggregate;
using Pi.User.Application.Queries.Storage;
using Pi.User.Application.Queries.Document;
using Pi.User.Application.Queries.BankAccount;
using Pi.User.Domain.Metrics;
using Pi.User.Application.Queries.Examination;
using Pi.User.Application.Services.Customer;
using Pi.User.Application.Services.Pdf;
using Pi.User.Application.Services.SSO;
using Pi.User.Domain.AggregatesModel.ExamAggregate;
using Pi.User.Domain.AggregatesModel.TradingAccountAggregate;
using Pi.User.Domain.AggregatesModel.UserAccountAggregate;
using Pi.User.Infrastructure.Options;

namespace Pi.User.API.Startup
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services,
            ConfigurationManager configuration, IHostEnvironment environment)
        {
            services.AddSingleton<OtelMetrics>();

            services.AddOptions<DbConfig>()
                .Bind(configuration.GetSection(DbConfig.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddOptions<AwsS3Option>()
                .Bind(configuration.GetSection(AwsS3Option.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddOptions<FreewillOptions>()
                .Bind(configuration.GetSection(FreewillOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddOptions<CreditLineOptions>()
                .Bind(configuration.GetSection(CreditLineOptions.Options));
            services.AddOptions<OnboardServiceOptions>()
                .Bind(configuration.GetSection(OnboardServiceOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddScoped<IUserInfoRepository, UserInfoRepository>();
            services.AddScoped<ITransactionIdRepository, TransactionIdRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IProductRepository, ProductDbRepository>();
            services.Decorate<IProductRepository, ProductCacheRepository>();
            services.AddScoped<IExaminationRepository, ExaminationRepository>();
            services.AddScoped<ITradingAccountRepository, TradingAccountDbRepository>();
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();

            services.AddScoped<IUserQueries, UserQueries>();
            services.AddScoped<IUserTradingAccountQueries, UserTradingAccountQueries>();
            services.AddScoped<ICustomerQueries, CustomerQueries>();
            services.AddScoped<ITransactionIdQueries, TransactionIdQueries>();
            services.AddScoped<INotificationPreferenceQueries, NotificationPreferenceQueries>();
            services.AddScoped<IStorageQueries, StorageQueries>();
            services.AddScoped<IDocumentQueries, DocumentQueries>();
            services.AddScoped<IBankAccountQueries, BankAccountQueries>();
            services.AddScoped<IExaminationQueries, ExaminationQueries>();
            services.AddScoped<IUserTradingAccountQueries, UserTradingAccountQueries>();

            services.AddScoped<IDeviceService, AwsService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IPdfService, PdfService>();

            services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
            services.AddSingleton<IEncryption, Encryption>();
            services.AddSingleton<IDecryption, Decryption>();
            services.AddSingleton<IEncryptionProvider, EncryptionService>();
            services.AddSingleton<ICryptographyService, EncryptionService>();

            AWSOptions? awsOption = null;
            if (environment.IsDevelopment())
            {
                awsOption = new AWSOptions
                {
                    DefaultClientConfig =
                    {
                        AuthenticationRegion = configuration.GetValue<string>("AwsSns:Region"),
                        ServiceURL = configuration.GetValue<string>("AwsSns:ServiceUrl"),
                    },
                    Credentials = new BasicAWSCredentials(
                        configuration.GetValue<string>("AwsSns:AssetKey") ?? string.Empty,
                        configuration.GetValue<string>("AwsSns:SecretKey") ?? string.Empty
                    ),
                };
            }

            services.AddAWSService<IAmazonSimpleNotificationService>(awsOption);
            services.AddSingleton<DateTimeProvider>();
            services.AddScoped<IFreewillSecurityPolicyHandler>((sp) =>
            {
                var options = sp.GetRequiredService<IOptions<FreewillOptions>>();
                return new FreewillSecurityPolicyHandler(options.Value.Requester, options.Value.Application,
                    options.Value.KeyBase, options.Value.IvCode);
            });
            services.AddScoped<IUserBankAccountService, FreewillCustomerService>();
            services.AddScoped<IUserInfoService, SiriusService>();
            services.AddScoped<IUserTradingAccountService, FreewillCustomerService>();
            services.AddScoped<ISiriusApi>(sp =>
                new SiriusApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("Sirius"),
                    configuration.GetValue<string>("Sirius:Host")!));
            services.AddScoped<ICustomerModuleApi>((sp) =>
            {
                var options = sp.GetRequiredService<IOptions<FreewillOptions>>();
                return new CustomerModuleApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Freewill"),
                    new Pi.Financial.Client.Freewill.Client.Configuration
                    {
                        BasePath = options.Value.Host,
                    });
            });
            services.AddScoped<ITradingAccountApi>((sp) =>
            {
                var options = sp.GetRequiredService<IOptions<OnboardServiceOptions>>();
                return new TradingAccountApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Onboard"),
                    new Pi.Client.OnboardService.Client.Configuration()
                    {
                        BasePath = options.Value.Host,
                    });
            });
            services.AddScoped<ICustomerInfoApi>((sp) =>
            {
                var options = sp.GetRequiredService<IOptions<OnboardServiceOptions>>();
                return new CustomerInfoApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Onboard"),
                    new Pi.Client.OnboardService.Client.Configuration()
                    {
                        BasePath = options.Value.Host,
                    });
            });
            services.AddScoped<IPdfServiceApi>(sp =>
                new PdfServiceApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("Pdf"), configuration.GetValue<string>("Pdf:Host")!));

            services.AddScoped<IOnboardTradingAccountService, OnboardTradingAccountService>();
            services.AddScoped<ITradingAccountApi>((sp) =>
            {
                var options = sp.GetRequiredService<IOptions<OnboardServiceOptions>>();
                return new TradingAccountApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Onboard"),
                    new Pi.Client.OnboardService.Client.Configuration()
                    {
                        BasePath = options.Value.Host,
                    });
            });

            services.AddScoped<IFeatureService>(sp =>
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
                    configuration.GetValue<string>("GrowthBook:ApiKey"),
                    configuration.GetValue<string>("GrowthBook:ProjectId"),
                    configuration.GetValue<string>("GrowthBook:Host"),
                    attributes: headers != null ? FeatureService.GetAttributes(headers!) : new Dictionary<string, string>()
                );
            });

            services.AddScoped<ISsoService, SsoService>();
            services.AddScoped<IAuthApi>(sp =>
                new AuthApi(
                    sp.GetRequiredService<IHttpClientFactory>()
                        .CreateClient("SsoService"),
                    configuration.GetValue<string>("SsoService:Host") ?? string.Empty)
            );
            services.AddScoped<IAccountApi>(sp =>
                new AccountApi(
                    sp.GetRequiredService<IHttpClientFactory>()
                        .CreateClient("SsoService"),
                    configuration.GetValue<string>("SsoService:Host") ?? string.Empty)
            );

            return services;
        }
    }
}