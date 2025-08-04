namespace Pi.SetMarketData.Infrastructure.Interfaces.SqlServer;

public interface ISqlServerRepository<TEntity>
    where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
}