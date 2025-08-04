using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;

namespace Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;

public class GlobalWithdrawTransaction
{
    public required WithdrawState WithdrawState { get; set; }
    public GlobalWalletTransferState? GlobalWalletTransferState { get; set; }
}
