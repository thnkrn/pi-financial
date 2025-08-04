using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;

namespace Pi.WalletService.Infrastructure.Repositories;

public class OddDepositRepository : GenericRepository<OddDepositState>, IOddDepositRepository
{
    private readonly WalletDbContext _walletDbContext;

    public OddDepositRepository(WalletDbContext walletDbContext) : base(walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }
}
