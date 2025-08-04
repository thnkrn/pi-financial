namespace Pi.SetMarketData.Infrastructure.Interfaces.SqlServer;

public interface ISqlServerService<TEntity>
    where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
}