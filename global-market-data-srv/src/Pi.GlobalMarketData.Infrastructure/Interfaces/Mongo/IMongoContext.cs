using MongoDB.Driver;

namespace Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

public interface IMongoContext
{
    IMongoCollection<TEntity> GetCollection<TEntity>();
}