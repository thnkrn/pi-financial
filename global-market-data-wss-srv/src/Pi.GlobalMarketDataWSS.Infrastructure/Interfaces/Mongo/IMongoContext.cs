using MongoDB.Driver;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Mongo;

public interface IMongoContext
{
    IMongoCollection<TEntity> GetCollection<TEntity>();
}