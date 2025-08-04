using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Core.Compression;
using MongoDB.Driver.Core.Configuration;
using Pi.MarketData.Search.Domain.ConstantConfigurations;
using Pi.MarketData.Search.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Search.Infrastructure.Services.Mongo;

public class MongoContext : IMongoContext
{
    private readonly IMongoDatabase _database;
    public MongoContext(IConfiguration configuration)
    {
        var connectionString = string.Format(
            configuration.GetValue<string>(ConfigurationKeys.MongoConnection) ?? string.Empty,
            configuration.GetValue<string>(ConfigurationKeys.MongoConnectionUserName),
            configuration.GetValue<string>(ConfigurationKeys.MongoConnectionPassword)
        );

        var databaseName = configuration.GetValue<string>(ConfigurationKeys.MongoDatabase);

        if (string.IsNullOrEmpty(connectionString))
        {
            var mongoSettings = configuration.GetSection("MongoDB:SET_TFEX").Get<MongoDbConfig>();
            connectionString = mongoSettings?.ConnectionString;
            databaseName = mongoSettings?.Database;
        }

        var settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.MaxConnectionPoolSize = 30;
        settings.MinConnectionPoolSize = 5;
        settings.MaxConnectionIdleTime = TimeSpan.FromMinutes(5);
        settings.WaitQueueTimeout = TimeSpan.FromSeconds(10);
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
        settings.ConnectTimeout = TimeSpan.FromSeconds(10);
        settings.SocketTimeout = TimeSpan.FromSeconds(10);
        settings.Compressors = [new CompressorConfiguration(CompressorType.Zlib)];
        settings.WriteConcern = WriteConcern.WMajority.With(wTimeout: TimeSpan.FromSeconds(5));
        settings.ReadConcern = ReadConcern.Majority;
        settings.ReadPreference = ReadPreference.SecondaryPreferred;

        var client = new MongoClient(settings);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<TEntity> GetCollection<TEntity>()
    {
        var entityName = typeof(TEntity).Name;
        entityName = RemoveResponse(entityName);
        entityName = PascalCaseToSnakeCase(entityName);
        return _database.GetCollection<TEntity>(entityName);
    }

    private static string PascalCaseToSnakeCase(string input)
    {
        return Regex.Replace(input, "(?<!^)([A-Z])", "_$1",
            RegexOptions.CultureInvariant, TimeSpan.FromSeconds(3)).ToLower();
    }

    private static string RemoveResponse(string input)
    {
        return Regex.Replace(input, "Response", string.Empty,
            RegexOptions.IgnoreCase, TimeSpan.FromSeconds(3));
    }
}