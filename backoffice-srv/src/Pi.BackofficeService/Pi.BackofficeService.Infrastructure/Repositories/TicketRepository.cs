using Microsoft.EntityFrameworkCore;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.SeedWork;
using Pi.BackofficeService.Infrastructure.Extensions;

namespace Pi.BackofficeService.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly TicketDbContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public TicketRepository(TicketDbContext context)
    {
        _context = context;
    }

    public async Task<List<TicketState>> Get(int pageNum, int pageSize, string? orderBy, string? orderDir,
        IQueryFilter<TicketState>? filters)
    {
        var query = _context.Set<TicketState>().WhereByFilters(filters);

        if (!string.IsNullOrEmpty(orderBy) && !string.IsNullOrEmpty(orderDir))
        {
            query = query.OrderByProperty(orderBy, orderDir);
        }
        else
        {
            query = query.OrderByDescending(q => q.CreatedAt);
        }

        return await query.Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> Count()
    {
        return await _context
            .Set<TicketState>()
            .CountAsync();
    }

    public async Task<int> Count(IQueryFilter<TicketState>? filters)
    {
        return await _context
            .Set<TicketState>()
            .WhereByFilters(filters)
            .CountAsync();
    }

    public async Task<string?> GetLatestTicketNo()
    {
        return await _context.Set<TicketState>()
            .OrderByDescending(q => q.TicketNo)
            .Where(q => q.TicketNo != null)
            .Select(q => q.TicketNo)
            .FirstOrDefaultAsync();
    }

    public async Task<TicketState?> GetByTicketNo(string ticketNo)
    {
        return await _context.Set<TicketState>()
            .OrderByDescending(q => q.CreatedAt)
            .Where(q => q.TicketNo == ticketNo)
            .FirstOrDefaultAsync();
    }

    public async Task<TicketState?> GetById(Guid correlationId)
    {
        return await _context.Set<TicketState>().Where(q => q.CorrelationId == correlationId).FirstOrDefaultAsync();
    }

    public async Task<List<TicketState>> GetByTransactionId(Guid transactionId)
    {
        return await _context.Set<TicketState>()
            .Where(q => q.TransactionId == transactionId)
            .OrderBy(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<TicketState>> GetByTransactionNo(string transactionNo)
    {
        return await _context.Set<TicketState>()
            .Where(q => q.TransactionNo == transactionNo)
            .OrderBy(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task<TicketState> Create(TicketState ticketState)
    {
        var record = await _context.AddAsync(ticketState);
        await _context.SaveChangesAsync();

        return record.Entity;
    }

    public async Task UpdateTicketNo(Guid correlationId, string ticketNo)
    {
        var result = await _context.Set<TicketState>().SingleOrDefaultAsync(d => d.CorrelationId == correlationId);

        if (result == null)
        {
            throw new KeyNotFoundException();
        }

        result.TicketNo = ticketNo;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null && ex.InnerException.Message.ToLower().Contains("duplicate entry"))
            {
                throw new DuplicateTicketNoException();
            }
        }
    }

    public void Update(TicketState ticketState)
    {
        _context.Update(ticketState);
    }
}
