using Pi.WalletService.Domain.AggregatesModel.AtsWithdrawAggregate;

namespace Pi.WalletService.Infrastructure.Repositories;

public class AtsWithdrawRepository : GenericRepository<AtsWithdrawState>, IAtsWithdrawRepository
{
    private readonly WalletDbContext _walletDbContext;

    public AtsWithdrawRepository(WalletDbContext walletDbContext) : base(walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }
}