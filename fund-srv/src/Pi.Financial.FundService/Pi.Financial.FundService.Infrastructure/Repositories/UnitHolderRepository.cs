using Microsoft.EntityFrameworkCore;
using Pi.Common.SeedWork;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.Financial.FundService.Infrastructure.Repositories;

public class UnitHolderRepository : IUnitHolderRepository
{
    private readonly FundDbContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public UnitHolderRepository(FundDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<int> CountUnitHolderAsync(string unitHolderId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UnitHolders.CountAsync(q => q.UnitHolderId == unitHolderId, cancellationToken: cancellationToken);
    }

    public async Task<UnitHolder> CreateAsync(UnitHolder unitHolder, CancellationToken cancellationToken = default)
    {
        var result = await _context.UnitHolders.AddAsync(unitHolder, cancellationToken);
        return result.Entity;
    }

    public async Task UpdateUnitHolderIdAsync(string oldUnitHolderId, string newUnitHolderId,
        CancellationToken cancellationToken = default)
    {
        var unitHolders = await _context.UnitHolders.Where(q => q.UnitHolderId == oldUnitHolderId).ToListAsync(cancellationToken: cancellationToken);
        unitHolders.ForEach(q =>
        {
            q.UnitHolderId = newUnitHolderId;
        });
        await _context.SaveChangesAsync(cancellationToken);
    }
}
