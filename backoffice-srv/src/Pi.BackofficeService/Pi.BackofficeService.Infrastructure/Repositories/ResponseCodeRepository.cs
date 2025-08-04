using Microsoft.EntityFrameworkCore;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.SeedWork;
using Pi.BackofficeService.Infrastructure.Extensions;

namespace Pi.BackofficeService.Infrastructure.Repositories;

public class ResponseCodeRepository : IResponseCodeRepository
{
    private readonly BackofficeDbContext _context;

    public ResponseCodeRepository(BackofficeDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<List<ResponseCode>> GetAll()
    {
        return await _context.ResponseCodes.ToListAsync();
    }

    public async Task<List<ResponseCode>> Get(ProductType?[] productTypes)
    {
        return await _context.ResponseCodes
            .Where(q => productTypes.Contains(q.ProductType))
            .ToListAsync();
    }

    public async Task<List<ResponseCode>> Get(IQueryFilter<ResponseCode> filter)
    {
        return await _context.ResponseCodes.Include(q => q.Actions)
            .OrderBy(q => q.Description)
            .WhereByFilters(filter)
            .ToListAsync();
    }

    public async Task<ResponseCode?> Get(Guid id)
    {
        return await _context.ResponseCodes.Where(q => q.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ResponseCode>> Get(Guid[] ids)
    {
        return await _context.ResponseCodes.Where(q => ids.Contains(q.Id)).ToListAsync();
    }

    public async Task<ResponseCode?> GetByStateMachine(Machine machine, string state, ProductType? productType)
    {
        return await _context.ResponseCodes
            .Where(q => q.State.ToLower() == state.ToLower() && q.Machine == machine)
            .Where(q => q.ProductType == productType)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ResponseCode>> GetByStates(Machine machine, string[] state)
    {
        return await _context.ResponseCodes.Where(q => q.Machine == machine)
            .Where(q => state.Select(s => s.ToLower()).Contains(q.State.ToLower()))
            .ToListAsync();
    }

    public async Task<List<ResponseCode>> GetAllByStates(string[] state)
    {
        return await _context.ResponseCodes.Where(q => state.Select(s => s.ToLower()).Contains(q.State.ToLower()))
            .ToListAsync();
    }
}
