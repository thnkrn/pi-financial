using MongoDB.Driver;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Mongo;

public interface IMongoContext
{
    IMongoCollection<TEntity> GetCollection<TEntity>();
}