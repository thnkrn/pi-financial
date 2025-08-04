using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;

namespace Pi.WalletService.Infrastructure.Repositories;

public class QrDepositRepository : GenericRepository<QrDepositState>, IQrDepositRepository
{
    private readonly WalletDbContext _walletDbContext;

    public QrDepositRepository(WalletDbContext walletDbContext) : base(walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    // public async Task<QrDepositState?> Get(Guid correlationId)
    // {
    //     return await _walletDbContext
    //         .Set<QrDepositState>()
    //         .SingleOrDefaultAsync(r => r.CorrelationId == correlationId);
    // }

}
