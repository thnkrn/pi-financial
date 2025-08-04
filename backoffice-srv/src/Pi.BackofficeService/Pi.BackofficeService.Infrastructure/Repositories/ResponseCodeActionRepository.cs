using Microsoft.EntityFrameworkCore;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.SeedWork;

namespace Pi.BackofficeService.Infrastructure.Repositories;

public class ResponseCodeActionRepository : IResponseCodeActionRepository
{
    private readonly BackofficeDbContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public ResponseCodeActionRepository(BackofficeDbContext context)
    {
        _context = context;
    }

    public async Task<List<ResponseCodeAction>> GetByGuid(Guid id)
    {
        return await _context.Set<ResponseCodeAction>().Where(q => q.ResponseCodeId == id).ToListAsync();
    }
}
