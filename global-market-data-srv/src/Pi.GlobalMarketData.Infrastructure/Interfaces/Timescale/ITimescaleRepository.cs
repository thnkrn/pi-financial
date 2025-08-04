namespace Pi.GlobalMarketData.Infrastructure.Interfaces.Timescale;

public interface ITimescaleRepository<TEntity>
    where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task CreateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(int id);

    Task UpsertAsync(TEntity entity, params string[] conflictColumns);
}