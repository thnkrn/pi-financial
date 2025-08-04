using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Compression;
using MongoDB.Driver.Core.Configuration;
using Pi.GlobalMarketDataWSS.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Services.Mongo;

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

        var settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.MaxConnectionPoolSize = 50;
        settings.MinConnectionPoolSize = 5;
        settings.MaxConnectionIdleTime = TimeSpan.FromMinutes(5);
        settings.WaitQueueTimeout = TimeSpan.FromSeconds(10);
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
        settings.ConnectTimeout = TimeSpan.FromSeconds(10);
        settings.SocketTimeout = TimeSpan.FromSeconds(10);
        settings.Compressors = [new CompressorConfiguration(CompressorType.Zlib)];
        settings.WriteConcern = WriteConcern.WMajority.With(wTimeout: TimeSpan.FromSeconds(5));
        settings.ReadConcern = ReadConcern.Majority;
        settings.ReadPreference = ReadPreference.SecondaryPreferred;

        IMongoClient client = new MongoClient(settings);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<TEntity> GetCollection<TEntity>()
    {
        var entityName = typeof(TEntity).Name;
        entityName = RemoveResponse(entityName);
        entityName = PascalCaseToSnakeCase(entityName);

        var collection = _database.GetCollection<TEntity>(entityName);
        return collection;
    }

    public async Task<bool> PingAsync()
    {
        try
        {
            await _database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public IMongoCollection<TEntity> GetCollectionWithWriteConcern<TEntity>(WriteConcern writeConcern)
    {
        var collection = GetCollection<TEntity>();
        return collection.WithWriteConcern(writeConcern);
    }

    public IMongoCollection<TEntity> GetCollectionWithReadPreference<TEntity>(ReadPreference readPreference)
    {
        var collection = GetCollection<TEntity>();
        return collection.WithReadPreference(readPreference);
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