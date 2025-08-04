using System.Linq.Expressions;

namespace Pi.MarketData.Search.Infrastructure.Interfaces.Mongo;

public interface IMongoRepository<TEntity>
    where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetOneByFilterAsync(Expression<Func<TEntity, bool>> filterExpression);
    Task<IEnumerable<TEntity>> GetAllByFilterAsync(Expression<Func<TEntity, bool>> filterExpression);
    Task UpdateAsync(string id, TEntity entity);
    Task ReplaceAsync(string id, TEntity entity);
    Task DeleteAsync(string id);
}
