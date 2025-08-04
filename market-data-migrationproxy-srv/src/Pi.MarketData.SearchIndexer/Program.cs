using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using OpenSearch.Client;
using Pi.MarketData.SearchIndexer.Models;
using Pi.MarketData.SearchIndexer.Services;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Configure MongoDB
        services.Configure<MongoDbSettings>(
            context.Configuration.GetSection("MongoDB"));


        var mongoSettings = context.Configuration.GetSection("MongoDB:GE").Get<MongoDbConfig>()
                ?? throw new InvalidOperationException("MongoDB:GE configuration is missing");

        services.AddKeyedSingleton<IMongoClient, MongoClient>("GE", (sp, key) => new MongoClient(mongoSettings.ConnectionString));
        services.AddKeyedSingleton<IMongoClient, MongoClient>("SET_TFEX", (sp, key) => new MongoClient(mongoSettings.ConnectionString));

        // Configure OpenSearch
        services.Configure<OpenSearchSettings>(
            context.Configuration.GetSection("OpenSearch"));

        services.AddSingleton<IOpenSearchClient>(sp =>
        {
            var openSearchSettings = context.Configuration.GetSection("OpenSearch").Get<OpenSearchSettings>()
                ?? throw new InvalidOperationException("OpenSearch configuration is missing");
            var settings = new ConnectionSettings(new Uri(openSearchSettings.Host))
                .BasicAuthentication(openSearchSettings.Username, openSearchSettings.Password)
                .ServerCertificateValidationCallback((o, cert, chain, errors) => true); // For development only

            return new OpenSearchClient(settings);
        });

        // Register the DataSyncService
        services.AddSingleton<DataSyncService>();
    })
    .Build();

// Run the synchronization
var syncService = host.Services.GetRequiredService<DataSyncService>();
await syncService.SyncAllSourcesAsync();
