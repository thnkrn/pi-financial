using Microsoft.EntityFrameworkCore.DataEncryption;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Application.Services.Measurement;
using Pi.BackofficeService.Application.Services.ReportService;
using Pi.BackofficeService.Application.Services.UserService;
using Pi.BackofficeService.Application.Services.WalletService;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.User;
using Pi.BackofficeService.Infrastructure.Repositories;
using Pi.BackofficeService.Infrastructure.Services;
using Pi.Client.UserService.Api;
using Pi.Client.WalletService.Api;
using Pi.Common.Cryptography;

namespace Pi.BackofficeService.Worker.Startup
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IResponseCodeRepository, ResponseCodeRepository>();
            services.AddScoped<IResponseCodeActionRepository, ResponseCodeActionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITicketQueries, TicketQueries>();
            services.AddScoped<IBackofficeQueries, BackofficeQueries>();
            services.AddScoped<IReportQueries, ReportQueries>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IReportService, ReportService>(sp => new ReportService(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("Lambda"),
                configuration.GetValue<string>("Lambda:Host") ?? string.Empty,
                configuration.GetValue<string>("Lambda:ApiKey") ?? string.Empty,
                sp.GetRequiredService<ILogger<ReportService>>())
            );
            services.AddScoped<IDepositWithdrawService, DepositWithdrawService>();
            services.AddScoped<ITransferCashService, TransferCashService>();
            services.AddScoped<ITicketActionService, Infrastructure.Services.WalletService>();
            services.AddScoped<IDecryption, Decryption>();
            services.AddScoped<IEncryption, Encryption>();
            services.AddScoped<IEncryptionProvider, EncryptionService>();
            services.AddScoped<ITransactionApi>(sp =>
                new TransactionApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("WalletService"),
                    configuration.GetValue<string>("WalletService:Host") ?? string.Empty));
            services.AddScoped<IActionApi>(sp =>
                new ActionApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("WalletService"),
                    configuration.GetValue<string>("WalletService:Host") ?? string.Empty));
            services.AddScoped<IDepositWithdrawApi>(sp =>
                new DepositWithdrawApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("WalletService"),
                    configuration.GetValue<string>("WalletService:Host") ?? string.Empty));
            services.AddScoped<ITransferApi>(sp =>
                new TransferApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("WalletService"),
                    configuration.GetValue<string>("WalletService:Host") ?? string.Empty));
            services.AddScoped<IUserApi>(sp =>
                new UserApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("UserService"),
                    configuration.GetValue<string>("UserService:Host") ?? string.Empty));
            services.AddSingleton<IMetric, MetricService>();

            return services;
        }
    }
}