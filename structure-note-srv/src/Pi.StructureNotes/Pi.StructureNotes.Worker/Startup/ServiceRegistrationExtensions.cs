using System.Text;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.Client.OnboardService.Api;
using Pi.Client.UserService.Api;
using Pi.Common.HealthCheck;
using Pi.StructureNotes.Infrastructure;
using Pi.StructureNotes.Infrastructure.Repositories;
using Pi.StructureNotes.Infrastructure.Services;
using Pi.StructureNotes.Worker.Services;
using Renci.SshNet;

namespace Pi.StructureNotes.Worker.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();

        // AddSftpClient(services);
        // services.AddSingleton<INoteFileReader, SftpFileReader>();

        services.Configure<S3NoteReaderConfig>(configuration.GetSection("S3NoteReaderConfig"));
        services.AddSingleton<INoteFileReader, S3NoteReader>();

        services.AddSingleton<INotesSource, CsvNoteSource>();
        services.AddSingleton<INoteRepository, NoteRepository>();

        services.AddSingleton<IUserApi>(sp =>
            new UserApi(new HttpClient { Timeout = TimeSpan.FromSeconds(30) },
                configuration.GetValue<string>("UserApiConfig:Url") ?? string.Empty)
        );
        services.AddSingleton<ITradingAccountApi>(sp =>
            new TradingAccountApi(new HttpClient { Timeout = TimeSpan.FromSeconds(30) },
                configuration.GetValue<string>("OnboardingApiConfig:Url") ?? string.Empty)
        );
        services.AddSingleton<ICustomerInfoApi>(sp =>
            new CustomerInfoApi(new HttpClient { Timeout = TimeSpan.FromSeconds(30) },
                configuration.GetValue<string>("OnboardingApiConfig:Url") ?? string.Empty)
        );

        services.Configure<DataCacheConfig>(configuration.GetSection("DataCacheConfig"));
        services.AddSingleton<IUserDataCache, UserDataCache>();
        services.AddSingleton<IAccountService, AccountService>();

        services.AddHostedService<NotesImportService>();

        return services;
    }

    private static void AddSftpClient(IServiceCollection services)
    {
        string host = "transfer.pi.financial";
        string user = "pi-mohamad-nonprod";
        Stream pkStream = CreatePrivateKey();
        PrivateKeyFile pk = new(pkStream);
        PrivateKeyFile[] keyFiles = { pk };

        AuthenticationMethod[] methods = { new PrivateKeyAuthenticationMethod(user, keyFiles) };

        ConnectionInfo config = new(host, user, methods);

        SftpClient client = new(config);

        services.AddSingleton(client);

        Stream CreatePrivateKey()
        {
            string str =
                "-----BEGIN OPENSSH PRIVATE KEY-----\r\n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\r\n-----END OPENSSH PRIVATE KEY-----\r\n";

            byte[] bytes = Encoding.UTF8.GetBytes(str);

            return new MemoryStream(bytes);
        }
    }
}
