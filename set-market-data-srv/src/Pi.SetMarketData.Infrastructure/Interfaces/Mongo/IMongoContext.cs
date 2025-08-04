using MongoDB.Driver;

namespace Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

public interface IMongoContext
{
    IMongoCollection<TEntity> GetCollection<TEntity>();
}