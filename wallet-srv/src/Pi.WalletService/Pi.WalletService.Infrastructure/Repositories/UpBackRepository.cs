using Microsoft.EntityFrameworkCore;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.Repositories;

public class UpBackRepository : GenericRepository<UpBackState>, IUpBackRepository
{
    private readonly WalletDbContext _walletDbContext;

    public UpBackRepository(WalletDbContext walletDbContext) : base(walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task<UpBackState?> Get(Guid correlationId)
    {
        return await _walletDbContext
            .Set<UpBackState>()
            .SingleOrDefaultAsync(r => r.CorrelationId == correlationId);
    }
}
