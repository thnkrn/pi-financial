using Microsoft.EntityFrameworkCore;
using Pi.Common.Database.Extensions;
using Pi.Common.Domain;
using Pi.Common.Generators.Number;
using Pi.Common.SeedWork;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Infrastructure.Repositories;

public class FundOrderRepository : IFundOrderRepository
{
    private readonly FundDbContext _context;
    private readonly DbContextOptions<FundDbContext> _dbContextOptions;
    public IUnitOfWork UnitOfWork => _context;

    public FundOrderRepository(FundDbContext fundDbContext, DbContextOptions<FundDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
        _context = fundDbContext;
    }

    public async Task<FundOrderState?> GetAsync(Guid correlationId, CancellationToken cancellationToken = default)
    {
        return await _context.FundOrderState
            .SingleOrDefaultAsync(q => q.CorrelationId == correlationId, cancellationToken);
    }

    public async Task<List<FundOrderState>> GetAsync(IQueryFilter<FundOrderState> filters, CancellationToken cancellationToken = default)
    {
        return await _context.FundOrderState
            .WhereByFilters(filters)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<DateOnly>> GetEffectiveDates(IQueryFilter<FundOrderState> filters, CancellationToken cancellationToken = default)
    {
        return await _context.FundOrderState
            .WhereByFilters(filters)
            .Where(q => q.EffectiveDate != null)
            .Select(q => (DateOnly)q.EffectiveDate!)
            .Distinct()
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task UpdateOrderNoAsync(Guid correlationId, string orderNo, CancellationToken cancellationToken = default)
    {
        var result = await _context.FundOrderState
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

    public async Task<FundOrderState?> GetByBrokerOrderIdAndOrderTypeAsync(string brokerOrderId, FundOrderType orderType,
        CancellationToken cancellationToken = default)
    {
        return await _context.FundOrderState.SingleOrDefaultAsync(
            q => q.BrokerOrderId == brokerOrderId && q.OrderType == orderType, cancellationToken: cancellationToken);
    }

    public async Task<List<FundOrderState>> GetByBrokerOrderIds(string[] brokerOrderIds,
        CancellationToken cancellationToken = default)
    {
        return await _context.FundOrderState.Where(q => brokerOrderIds.Contains(q.BrokerOrderId))
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<FundOrderState>> GetByBrokerOrderIdAndOrderSideAsync(string brokerOrderId, OrderSide orderSide,
        CancellationToken cancellationToken = default)
    {
        return await _context.FundOrderState.Where(
            q => q.BrokerOrderId == brokerOrderId && q.OrderSide == orderSide).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<FundOrderState>> GetByOrderNoAsync(string[] orderNos, CancellationToken cancellationToken = default)
    {
        return await _context.FundOrderState
            .Where(q => orderNos.Contains(q.OrderNo))
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task UpdateUnitHolderIdAsync(string oldUnitHolderId, string newUnitHolderId,
        CancellationToken cancellationToken = default)
    {
        var fundOrderStates = await _context.FundOrderState.Where(q => q.UnitHolderId == oldUnitHolderId).ToListAsync(cancellationToken: cancellationToken);
        fundOrderStates.ForEach(q =>
        {
            q.UnitHolderId = newUnitHolderId;
        });
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(FundOrderState fundOrderState)
    {
        using (var context = new FundDbContext(_dbContextOptions))
        {
            _context.Update(fundOrderState);
            await context.SaveChangesAsync();
        }
    }

    public async Task<FundOrderState> CreateAsync(FundOrderState fundOrderState, CancellationToken cancellationToken = default)
    {
        using (var context = new FundDbContext(_dbContextOptions))
        {
            var result = await context.FundOrderState.AddAsync(fundOrderState, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return result.Entity;
        }
    }
}
