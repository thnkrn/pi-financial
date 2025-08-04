using Pi.WalletService.Domain.AggregatesModel.CashAggregate;

namespace Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;

public class ThaiWithdrawTransaction
{
    public required WithdrawState WithdrawState { get; set; }
    public CashWithdrawState? CashWithdrawState { get; set; }
}
