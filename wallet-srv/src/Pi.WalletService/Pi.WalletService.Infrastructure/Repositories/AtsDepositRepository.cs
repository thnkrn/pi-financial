using Pi.WalletService.Domain.AggregatesModel.AtsDepositAggregate;
using AtsDepositState = Pi.WalletService.Domain.AggregatesModel.AtsDepositAggregate.AtsDepositState;

namespace Pi.WalletService.Infrastructure.Repositories;

public class AtsDepositRepository : GenericRepository<AtsDepositState>, IAtsDepositRepository
{
    private readonly WalletDbContext _walletDbContext;

    public AtsDepositRepository(WalletDbContext walletDbContext) : base(walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }
}
