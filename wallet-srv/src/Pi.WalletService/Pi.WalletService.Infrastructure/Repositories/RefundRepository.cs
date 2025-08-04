using Microsoft.EntityFrameworkCore;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.RefundAggregate;
using Pi.WalletService.Infrastructure.Extensions;

namespace Pi.WalletService.Infrastructure.Repositories;

public class RefundRepository : IRefundRepository
{
    private readonly WalletDbContext _walletDbContext;

    public RefundRepository(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task<List<RefundState>> Get(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<RefundState>? filters)
    {
        var query = _walletDbContext
            .Set<RefundState>()
            .AsQueryable()
            .WhereByFilters(filters);

        if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(orderDir))
        {
            query = query.OrderByProperty(orderBy, orderDir);
        }
        else
        {
            query = query.OrderByDescending(q => q.CreatedAt);
        }

        var transactions = await query.Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return transactions;
    }

    public Task<int> CountTransactions(IQueryFilter<RefundState>? filters)
    {
        return _walletDbContext
            .Set<RefundState>()
            .AsQueryable()
            .WhereByFilters(filters)
            .CountAsync();
    }
}
