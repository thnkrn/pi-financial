using Microsoft.EntityFrameworkCore;
using Pi.Common.SeedWork;
using Pi.WalletService.Domain.AggregatesModel.LogAggregate;

namespace Pi.WalletService.Infrastructure.Repositories;

public class FreewillRequestLogRepository : IFreewillRequestLogRepository
{
    private readonly WalletDbContext _walletDbContext;

    public IUnitOfWork UnitOfWork => _walletDbContext;

    public FreewillRequestLogRepository(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task<FreewillRequestLog?> Get(string transId, FreewillRequestType requestType)
    {
        return await _walletDbContext.FreewillRequestLogs
            .Where(l => l.TransId == transId && l.Type == requestType)
            .SingleOrDefaultAsync();
    }

    public FreewillRequestLog Create(FreewillRequestLog freewillRequestLog)
    {
        return _walletDbContext.FreewillRequestLogs.Add(freewillRequestLog).Entity;
    }

    public async Task<List<FreewillRequestLog>> Get(string? transId, FreewillRequestType? type, DateTime? createdAtFrom, DateTime? createdAtTo)
    {
        IQueryable<FreewillRequestLog> query = _walletDbContext.FreewillRequestLogs;

        if (transId != null)
            query = query.Where(t => t.TransId == transId);

        if (type != null)
            query = query.Where(t => t.Type == type);

        if (createdAtFrom != null && createdAtTo != null)
            query = query.Where(t => t.CreatedAt >= createdAtFrom && t.CreatedAt < createdAtTo);

        var freewillRequestLogs = await query.ToListAsync();
        return freewillRequestLogs;
    }
}