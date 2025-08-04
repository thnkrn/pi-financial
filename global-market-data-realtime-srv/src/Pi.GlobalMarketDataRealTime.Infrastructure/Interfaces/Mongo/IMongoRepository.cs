using System.Linq.Expressions;
using MongoDB.Bson;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Mongo;

public interface IMongoRepository<TEntity>
    where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(string id);
    Task CreateAsync(TEntity entity);
    Task CreateManyAsync(TEntity[] entities);
    Task UpdateAsync(string id, TEntity entity);
    Task UpsertAsync(string id, TEntity entity);
    Task ReplaceAsync(ObjectId id, TEntity entity);
    Task DeleteAsync(string id);
    Task DeleteByColumnAsync(string columnName, string columnValue);
    Task DeleteByMultipleColumnAsync(Dictionary<string, object> columns);
    Task<IEnumerable<TEntity>> GetListByFilterAsync(Expression<Func<TEntity, bool>> filterExpression);
    Task<long> CountAllAsync();
}