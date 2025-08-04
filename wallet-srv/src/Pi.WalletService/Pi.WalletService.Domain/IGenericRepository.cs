using System.Linq.Expressions;
namespace Pi.WalletService.Domain;

public interface IGenericRepository<T> where T : class
{
    void Add(T entity);
    Task AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));
    T? Get(Expression<Func<T, bool>> expression);
    Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default(CancellationToken));
}
