using System.Linq.Expressions;
using MongoDB.Bson;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Mongo;

public interface IMongoRepository<TEntity>
    where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(string id);
    Task<TEntity?> GetByFilterAsync(Expression<Func<TEntity, bool>> filterExpression);
    Task<IEnumerable<TEntity>> GetListByFilterAsync(Expression<Func<TEntity, bool>> filterExpression);

    Task CreateAsync(TEntity entity);
    Task UpdateAsync(string id, TEntity entity);
    Task UpsertAsync(string id, TEntity entity);
    Task ReplaceAsync(ObjectId id, TEntity entity);
    Task DeleteAsync(string id);
}
