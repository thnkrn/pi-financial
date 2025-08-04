using Microsoft.EntityFrameworkCore;
using Pi.Common.Database.Extensions;
using Pi.Common.Domain;
using Pi.Common.Generators.Number;
using Pi.Common.SeedWork;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Infrastructure.Repositories;

public class EquityOrderStateRepository : IEquityOrderStateRepository
{
    private readonly SetDbContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public EquityOrderStateRepository(SetDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EquityOrderState>> GetEquityOrderStates(IQueryFilter<EquityOrderState> filters)
    {
        return await _context.EquityOrderState.WhereByFilters(filters)
            .ToListAsync();
    }

    public async Task UpdateOrderNoAsync(Guid correlationId, string orderNo, CancellationToken cancellationToken = default)
    {
        var result = await _context.EquityOrderState
            .SingleOrDefaultAsync(d => d.CorrelationId == correlationId, cancellationToken: cancellationToken);

        if (result == null)
        {
            throw new KeyNotFoundException();
        }
        result.OrderNo = orderNo;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            if (ex.Message.ToLower().Contains("duplicate") || (ex.InnerException != null && ex.InnerException.Message.ToLower().Contains("duplicate")))
            {
                throw new DuplicateRecordNoException();
            }
        }
    }
}
