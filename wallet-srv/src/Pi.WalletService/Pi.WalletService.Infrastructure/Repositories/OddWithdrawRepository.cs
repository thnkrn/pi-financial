using Pi.WalletService.Domain.AggregatesModel.OddWithdrawAggregate;

namespace Pi.WalletService.Infrastructure.Repositories;

public class OddWithdrawRepository : GenericRepository<OddWithdrawState>, IOddWithdrawRepository
{
    private readonly WalletDbContext _walletDbContext;

    public OddWithdrawRepository(WalletDbContext walletDbContext) : base(walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }
}
