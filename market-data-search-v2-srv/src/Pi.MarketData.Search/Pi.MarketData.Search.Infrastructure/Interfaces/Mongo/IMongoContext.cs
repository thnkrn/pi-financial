using MongoDB.Driver;

namespace Pi.MarketData.Search.Infrastructure.Interfaces.Mongo;

public interface IMongoContext
{
    IMongoCollection<TEntity> GetCollection<TEntity>();
}