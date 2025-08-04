using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Core.Compression;
using MongoDB.Driver.Core.Configuration;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Models.Response;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Services.Mongo;

public class MongoContext : IMongoContext
{
    private static bool _mappingsInitialized;
    private static readonly object LockObject = new();
    private readonly IMongoDatabase _database;

    public MongoContext(IConfiguration configuration)
    {
        try
        {
            InitializeClassMappings();
        }
        catch (Exception)
        {
            // Nothing to do
        }

        var connectionString = string.Format(
            configuration.GetValue<string>(ConfigurationKeys.MongoConnection) ?? string.Empty,
            configuration.GetValue<string>(ConfigurationKeys.MongoConnectionUserName),
            configuration.GetValue<string>(ConfigurationKeys.MongoConnectionPassword)
        );

        var databaseName = configuration.GetValue<string>(ConfigurationKeys.MongoDatabase);
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.MaxConnectionPoolSize = 30;
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

        var client = new MongoClient(settings);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<TEntity> GetCollection<TEntity>()
    {
        try
        {
            ConfigureClassMapping<TEntity>();
        }
        catch (Exception)
        {
            // Nothing to do
        }

        var entityName = typeof(TEntity).Name;
        entityName = RemoveResponse(entityName);
        entityName = PascalCaseToSnakeCase(entityName);
        return _database.GetCollection<TEntity>(entityName);
    }

    private static void InitializeClassMappings()
    {
        if (_mappingsInitialized)
            return;

        lock (LockObject)
        {
            if (_mappingsInitialized) return;

            // Register base classes first
            RegisterBaseClassMaps();

            _mappingsInitialized = true;
        }
    }

    private static void RegisterBaseClassMaps()
    {
        // Register GeneralInfo if not already registered
        if (!BsonClassMap.IsClassMapRegistered(typeof(GeneralInfo)))
            BsonClassMap.RegisterClassMap<GeneralInfo>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

        // Register other base classes if needed
        if (!BsonClassMap.IsClassMapRegistered(typeof(Status)))
            BsonClassMap.RegisterClassMap<Status>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

        if (!BsonClassMap.IsClassMapRegistered(typeof(ReportInfo)))
            BsonClassMap.RegisterClassMap<ReportInfo>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
    }

    private static void ConfigureClassMapping<TEntity>()
    {
        var entityType = typeof(TEntity);

        // First check if it's already registered
        if (BsonClassMap.IsClassMapRegistered(entityType))
            return;

        try
        {
            BsonClassMap.RegisterClassMap<TEntity>(cm =>
            {
                if (typeof(GeneralInfo).IsAssignableFrom(entityType))
                {
                    // For types inheriting from GeneralInfo
                    cm.AutoMap();
                }
                else
                {
                    // For other types
                    var properties = entityType.GetProperties(BindingFlags.DeclaredOnly
                                                              | BindingFlags.Default
                                                              | BindingFlags.Instance
                                                              | BindingFlags.Public);

                    foreach (var property in properties)
                    {
                        var bsonElement = property.GetCustomAttribute<BsonElementAttribute>();
                        var elementName = bsonElement?.ElementName ?? PascalCaseToSnakeCase(property.Name);

                        try
                        {
                            cm.MapProperty(property.Name).SetElementName(elementName);
                        }
                        catch (Exception)
                        {
                            // Skip properties that can't be mapped
                        }
                    }
                }

                cm.SetIgnoreExtraElements(true);
            });
        }
        catch (Exception ex) when (ex.Message.Contains("already been added") ||
                                   ex.Message.Contains("duplicate") ||
                                   ex is ArgumentException)
        {
            // Only catch duplicate registration exceptions
            // This is more specific than catching all exceptions
        }
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