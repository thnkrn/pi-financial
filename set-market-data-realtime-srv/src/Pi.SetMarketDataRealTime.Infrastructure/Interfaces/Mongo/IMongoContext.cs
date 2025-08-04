using MongoDB.Driver;

namespace Pi.SetMarketDataRealTime.Infrastructure.Interfaces.Mongo;

public interface IMongoContext
{
    IMongoCollection<TEntity> GetCollection<TEntity>();
}