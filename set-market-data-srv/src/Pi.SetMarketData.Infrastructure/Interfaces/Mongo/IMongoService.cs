using System.Linq.Expressions;
using MongoDB.Driver;
using Pi.SetMarketData.Domain.Models.Request.BrokerInfo;

namespace Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

public interface IMongoService<TEntity>
    where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(string id);
    Task<TEntity?> GetByFilterAsync(Expression<Func<TEntity, bool>> filterExpression);
    Task<IEnumerable<TEntity>> GetAllByFilterAsync(Expression<Func<TEntity, bool>> filterExpression);
    Task<IEnumerable<TEntity>> GetAllByFiltersAsync(IEnumerable<Expression<Func<TEntity, bool>>> expressions);
    Task<TEntity?> GetByOrderBookId(string orderBookId);
    Task CreateAsync(TEntity entity);
    Task UpdateAsync(string id, TEntity entity);
    Task UpdateManyAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> idSelector);
    Task UpsertAsync(string id, TEntity entity);
    Task<BulkWriteResult?> UpsertManyAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> idSelector);
    Task UpsertAsyncByFilter(Expression<Func<TEntity, bool>> filterExpression, TEntity entity);
    Task UpsertAsyncByFilterBsonDocument(Expression<Func<TEntity, bool>> filterExpression, TEntity entity);
    Task UpsertBrokerInfoAsync(BrokerInfoRequest request);
    Task ReplaceAsync(string id, TEntity entity);
    Task DeleteAsync(string id);
}
