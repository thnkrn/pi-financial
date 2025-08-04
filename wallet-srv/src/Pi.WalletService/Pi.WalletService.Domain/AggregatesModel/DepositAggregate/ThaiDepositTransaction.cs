using Pi.WalletService.Domain.AggregatesModel.CashAggregate;

namespace Pi.WalletService.Domain.AggregatesModel.DepositAggregate;

public class ThaiDepositTransaction
{
    public required DepositState DepositState { get; set; }
    public CashDepositState? CashDepositState { get; set; }
}
