using MongoDB.Driver;

namespace Pi.MarketData.Infrastructure.Interfaces.Mongo;

public interface IMongoContext
{
    IMongoCollection<TEntity> GetCollection<TEntity>();
}