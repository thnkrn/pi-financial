using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;

namespace Pi.WalletService.Domain.AggregatesModel.DepositAggregate;

public class GlobalDepositTransaction
{
    public required DepositState DepositState { get; set; }
    public GlobalWalletTransferState? GlobalWalletTransferState { get; set; }
}

