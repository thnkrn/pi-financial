using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

public interface IMongoRepository<TEntity>
    where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(string id);
    Task<TEntity?> GetByFilterAsync(Expression<Func<TEntity, bool>> filterExpression);
    Task<IEnumerable<TEntity>> GetAllByFilterAsync(Expression<Func<TEntity, bool>> filterExpression);
    Task CreateAsync(TEntity entity);
    Task CreateManyAsync(IEnumerable<TEntity> entities);
    Task UpdateAsync(string id, TEntity entity);
    Task UpdateManyAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> idSelector);
    Task<BulkWriteResult?> UpsertManyAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> idSelector);
    Task UpsertAsync(string id, TEntity entity);
    Task ReplaceAsync(string id, TEntity entity);
    Task ReplaceWithObjectIdAsync(ObjectId id, TEntity entity);
    Task DeleteAsync(string id);
    Task DeleteByFilter(Expression<Func<TEntity,bool>> filterExpression);
    Task UpsertAsyncByFilter(Expression<Func<TEntity, bool>> filterExpression, TEntity entity);
    Task UpsertAsyncByFilterBsonDocument(Expression<Func<TEntity, bool>> filterExpression, TEntity entity);
    Task ReplaceByFiterAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, bool>> filter);
}
