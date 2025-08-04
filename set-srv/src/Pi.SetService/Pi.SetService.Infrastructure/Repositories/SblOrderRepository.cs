using Microsoft.EntityFrameworkCore;
using Pi.Common.Database.Extensions;
using Pi.Common.Domain;
using Pi.Common.Generators.Number;
using Pi.Common.SeedWork;
using Pi.SetService.Domain.AggregatesModel.CommonAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Infrastructure.Repositories;

public class SblOrderRepository(SetDbContext context) : ISblOrderRepository
{
    public IUnitOfWork UnitOfWork => context;
    public async Task<PaginateResult<SblOrder>> Paginate(PaginateQuery paginateQuery, IQueryFilter<SblOrder> filters, CancellationToken ct = default)
    {
        var query = context.Set<SblOrder>().WhereByFilters(filters);

        if (!string.IsNullOrEmpty(paginateQuery.OrderBy) && !string.IsNullOrEmpty(paginateQuery.OrderDir.ToString()))
        {
            query = query.OrderByProperty(paginateQuery.OrderBy, paginateQuery.OrderDir.ToString());
        }
        else
        {
            query = query.OrderByDescending(q => q.CreatedAt);
        }

        var records = await query.Skip((paginateQuery.Page - 1) * paginateQuery.PageSize)
            .Take(paginateQuery.PageSize)
            .ToListAsync(cancellationToken: ct);
        var total = await query.CountAsync(cancellationToken: ct);

        return new PaginateResult<SblOrder>
        {
            Data = records,
            Page = paginateQuery.Page,
            PageSize = paginateQuery.PageSize,
            Total = total,
            OrderBy = paginateQuery.OrderBy,
            OrderDir = paginateQuery.OrderDir,
        };
    }

    public Task<List<SblOrder>> GetSblOrdersAsync(string tradingAccountNo, IQueryFilter<SblOrder> filters, CancellationToken ct = default)
    {
        return context.SblOrders.Where(q => q.TradingAccountNo == tradingAccountNo).WhereByFilters(filters).ToListAsync(ct);
    }

    public async Task<SblOrder?> GetSblOrder(Guid id, CancellationToken ct = default)
    {
        return await context.SblOrders.FirstOrDefaultAsync(q => q.Id == id, cancellationToken: ct);
    }

    public void Create(SblOrder sblOrder)
    {
        context.Add(sblOrder);
    }

    public void Update(SblOrder sblOrder)
    {
        context.Update(sblOrder);
    }

    public async Task<int> DeleteAsync(SblOrder sblOrder, CancellationToken ct = default)
    {
        context.SblOrders.Remove(sblOrder);
        return await context.SaveChangesAsync(ct);
    }

    public async Task<SblOrder> CreateAsync(SblOrder sblOrder, CancellationToken ct = default)
    {
        try
        {
            var result = await context.AddAsync(sblOrder, ct);
            await context.SaveChangesAsync(ct);
            return result.Entity;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("duplicate", StringComparison.CurrentCultureIgnoreCase) || (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate", StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new DuplicateRecordNoException();
            }

            throw;
        }
    }
}
