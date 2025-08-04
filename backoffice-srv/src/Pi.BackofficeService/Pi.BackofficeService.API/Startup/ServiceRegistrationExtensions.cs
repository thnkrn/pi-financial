using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Application.Services.CuratedManagerService;
using Pi.BackofficeService.Application.Services.Measurement;
using Pi.BackofficeService.Application.Services.OcrService;
using Pi.BackofficeService.Application.Services.OnboardService;
using Pi.BackofficeService.Application.Services.ReportService;
using Pi.BackofficeService.Application.Services.SblService;
using Pi.BackofficeService.Application.Services.UserService;
using Pi.BackofficeService.Application.Services.WalletService;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.User;
using Pi.BackofficeService.Infrastructure.Options;
using Pi.BackofficeService.Infrastructure.Repositories;
using Pi.BackofficeService.Infrastructure.Services;
using Pi.Client.ActivityService.Api;
using Pi.Client.OnboardService.Api;
using Pi.Client.SetService.Api;
using Pi.Client.UserService.Api;
using Pi.Client.WalletService.Api;
using Pi.Common.Cryptography;

namespace Pi.BackofficeService.API.Startup
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services,
            ConfigurationManager configuration,
            IHostEnvironment environment)
        {
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IResponseCodeRepository, ResponseCodeRepository>();
            services.AddScoped<IResponseCodeActionRepository, ResponseCodeActionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITicketQueries, TicketQueries>();
            services.AddScoped<IBackofficeQueries, BackofficeQueries>();
            services.AddScoped<IOnboardingQueries, OnboardingQueries>();
            services.AddScoped<IReportQueries, ReportQueries>();
            services.AddScoped<ISblQueries, SblQueries>();
            services.AddScoped<ISblService, SblService>();
            services.AddScoped<IDepositWithdrawService, DepositWithdrawService>();
            services.AddScoped<ITransferCashService, TransferCashService>();
            services.AddScoped<IReportService, ReportService>(sp => new ReportService(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("Lambda"),
                configuration.GetValue<string>("Lambda:Host") ?? string.Empty,
                configuration.GetValue<string>("Lambda:ApiKey") ?? string.Empty,
                sp.GetRequiredService<ILogger<ReportService>>())
            );
            services.AddScoped<IOnboardService, OnboardService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDecryption, Decryption>();
            services.AddScoped<IEncryption, Encryption>();
            services.AddScoped<IEncryptionProvider, EncryptionService>();
            services.AddScoped<ITransactionApi>(sp =>
                new TransactionApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("WalletService"),
                    configuration.GetValue<string>("WalletService:Host") ?? string.Empty));
            services.AddScoped<IActionApi>(sp =>
                new ActionApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("WalletService"),
                    configuration.GetValue<string>("WalletService:Host") ?? string.Empty));
            services.AddScoped<IDepositWithdrawApi>(sp =>
                new DepositWithdrawApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("WalletService"),
                    configuration.GetValue<string>("WalletService:Host") ?? string.Empty));
            services.AddScoped<ITransferApi>(sp =>
                new TransferApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("WalletService"),
                    configuration.GetValue<string>("WalletService:Host") ?? string.Empty));
            services.AddScoped<IOpenAccountApi>(sp =>
                new OpenAccountApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("OnboardService"),
                    configuration.GetValue<string>("OnboardService:Host") ?? string.Empty));
            services.AddScoped<IAtsApi>(sp =>
                new AtsApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("OnboardService"),
                    configuration.GetValue<string>("OnboardService:Host") ?? string.Empty));
            services.AddScoped<IUserApi>(sp =>
                new UserApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("UserService"),
                    configuration.GetValue<string>("UserService:Host") ?? string.Empty));
            services.AddScoped<IActivityApi>(sp =>
                new ActivityApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ActivityService"),
                    configuration.GetValue<string>("ActivityService:Host") ?? string.Empty));
            services.AddScoped<ISblApi>(sp =>
                new SblApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("SblService"),
                    configuration.GetValue<string>("SblService:Host") ?? string.Empty));
            services.AddScoped<ISyncApi>(sp =>
                new SyncApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("SblService"),
                    configuration.GetValue<string>("SblService:Host") ?? string.Empty));
            services.AddScoped<IOcrService, OcrService>(sp =>
                new OcrService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("OcrHttpClient"),
                    sp.GetRequiredService<ILogger<OcrService>>()));
            services.AddScoped<ICuratedManagerService, CuratedManagerService>(sp =>
                new CuratedManagerService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("CuratedManagerHttpClient"),
                    configuration.GetValue<string>("CuratedManagerService:Host") ?? string.Empty,
                    sp.GetRequiredService<ILogger<CuratedManagerService>>()));
            services.AddSingleton<IMetric, MetricService>();

            if (environment.IsDevelopment())
            {
                var region = configuration.GetValue<string>("AWS:Region");
                var accessKey = configuration.GetValue<string>("AWS:AccessKey");
                var secretKey = configuration.GetValue<string>("AWS:SecretKey");
                var serviceUrl = configuration.GetValue<string>("AWS:ServiceUrl");
                services.AddScoped<IAmazonS3, AmazonS3Client>(_ =>
                {
                    var config = new AmazonS3Config
                    {
                        RegionEndpoint = RegionEndpoint.GetBySystemName(region),
                        ServiceURL = serviceUrl,
                        ForcePathStyle = true,
                    };
                    var credentials = new BasicAWSCredentials(accessKey, secretKey);

                    {
                        config.ServiceURL = serviceUrl;
                        config.ForcePathStyle = true;
                    }

                    return new AmazonS3Client(credentials, config);
                });
            }
            else
            {
                services.AddDefaultAWSOptions(configuration.GetAWSOptions());
                services.AddAWSService<IAmazonS3>();
            }

            services.AddOptions<SblServiceOptions>()
                .Bind(configuration.GetSection(SblServiceOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}

