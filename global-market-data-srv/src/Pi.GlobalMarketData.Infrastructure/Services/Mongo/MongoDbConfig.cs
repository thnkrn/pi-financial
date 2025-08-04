using MongoDB.Bson.Serialization;
using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Infrastructure.Services.Mongo;

public static class MongoDbConfig
{
    public static void RegisterClassMaps()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(MorningStarStockItem)))
            BsonClassMap.RegisterClassMap<MorningStarStockItem>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

        if (!BsonClassMap.IsClassMapRegistered(typeof(MorningStarStocks)))
            BsonClassMap.RegisterClassMap<MorningStarStocks>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdProperty(c => c.Id);
            });
    }
}