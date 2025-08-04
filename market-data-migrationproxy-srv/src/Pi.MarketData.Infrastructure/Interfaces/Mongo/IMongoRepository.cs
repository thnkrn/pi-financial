using System.Linq.Expressions;

namespace Pi.MarketData.Infrastructure.Interfaces.Mongo;

public interface IMongoRepository<TEntity>
    where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(string id);
    Task<TEntity> GetBySymbolAsync(string symbol);
    Task<TEntity> GetByFilterAsync(Expression<Func<TEntity, bool>> filterExpression);
    Task<IEnumerable<TEntity>> GetAllByFilterAsync(Expression<Func<TEntity, bool>> filterExpression);
    Task<TEntity> GetMongoBySymbolAsync(string symbol);
    Task<TEntity?> GetByOrderBookId(string orderBookId);
    Task CreateAsync(TEntity entity);
    Task UpdateAsync(string id, TEntity entity);
    Task UpsertAsync(string id, TEntity entity);
    Task ReplaceAsync(string id, TEntity entity);
    Task DeleteAsync(string id);
}