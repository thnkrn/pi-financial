using MongoDB.Driver;

namespace Pi.SetMarketDataWSS.Infrastructure.Interfaces.Mongo;

public interface IMongoContext
{
    IMongoCollection<TEntity> GetCollection<TEntity>();
}