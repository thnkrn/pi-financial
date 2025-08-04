using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.Extensions.Options;
using Pi.Client.OnboardService.Api;
using Pi.Client.Sirius.Api;
using Pi.Common.Cryptography;
using Pi.Common.Database.Repositories;
using Pi.Common.Domain.AggregatesModel.ProductAggregate;
using Pi.Common.Features;
using Pi.Common.Utilities;
using Pi.Financial.Client.Freewill.Api;
using Pi.User.Application.Options;
using Pi.User.Application.Services.DeviceService;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
using Pi.User.Domain.AggregatesModel.TransactionIdAggregate;
using Pi.User.Infrastructure.Repositories;
using Pi.User.Infrastructure.Services;
using Pi.User.Domain.AggregatesModel.BankAccountAggregate;
using Pi.User.Domain.AggregatesModel.DocumentAggregate;
using Pi.User.Application.Services.Storage;
using Pi.User.Application.Services.Cryptography;
using Pi.User.Application.Services.LegacyUserInfo;
using Pi.User.Domain.Metrics;
using Pi.User.Domain.AggregatesModel.ExamAggregate;
using Pi.User.Application.Services.Pdf;
using Pi.Financial.Client.PdfService.Api;
using Pi.User.Application.Services.Customer;
using Pi.User.Domain.AggregatesModel.UserAccountAggregate;
using Pi.User.Infrastructure.Options;

namespace Pi.User.Worker.Startup
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
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

            services.AddScoped<IUserInfoRepository, UserInfoRepository>();
            services.AddScoped<ITransactionIdRepository, TransactionIdRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IProductRepository, ProductDbRepository>();
            services.AddScoped<IExaminationRepository, ExaminationRepository>();
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();

            services.AddScoped<IDeviceService, AwsService>();
            services.AddSingleton<IEncryption, Encryption>();
            services.AddSingleton<IDecryption, Decryption>();
            services.AddSingleton<IEncryptionProvider, EncryptionService>();
            services.AddSingleton<ICryptographyService, EncryptionService>();
            services.AddScoped<IUserInfoService, SiriusService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<ICustomerService, CustomerService>();

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
            services.AddScoped<ISiriusApi>(sp =>
                new SiriusApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Sirius"),
                    configuration.GetValue<string>("Sirius:Host") ?? string.Empty));

            services.AddSingleton<DateTimeProvider>();
            services.Configure<BankAccountOptions>(configuration.GetSection(BankAccountOptions.Options));

            services.AddScoped<ICustomerModuleApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<FreewillOptions>>();
                return new CustomerModuleApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Freewill"), new Pi.Financial.Client.Freewill.Client.Configuration
                    {
                        BasePath = options.Value.Host,
                    });
            });
            services.AddScoped<IPdfServiceApi>(sp =>
                new PdfServiceApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("Pdf"), configuration.GetValue<string>("Pdf:Host")!));
            services.AddScoped<IUserBankAccountService, FreewillCustomerService>();
            services.AddScoped<IUserTradingAccountService, FreewillCustomerService>();
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

            return services;
        }
    }
}