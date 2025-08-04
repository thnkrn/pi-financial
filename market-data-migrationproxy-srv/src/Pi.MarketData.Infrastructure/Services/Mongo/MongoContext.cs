using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Pi.MarketData.Domain.ConstantConfigurations;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Services.Mongo;

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
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
        settings.ConnectTimeout = TimeSpan.FromSeconds(5);
        settings.SocketTimeout = TimeSpan.FromSeconds(5);

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