using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pi.WalletService.Domain;
namespace Pi.WalletService.Infrastructure;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly WalletDbContext _dbContext;
    private readonly DbSet<T> _entitySet;

    protected GenericRepository(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
        _entitySet = _dbContext.Set<T>();
    }

    public void Add(T entity)
        => _dbContext.Add(entity);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
        => await _dbContext.AddAsync(entity, cancellationToken);

    public T? Get(Expression<Func<T, bool>> expression)
        => _entitySet.FirstOrDefault(expression);

    public async Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default(CancellationToken))
        => await _entitySet.FirstOrDefaultAsync(expression, cancellationToken);


}
