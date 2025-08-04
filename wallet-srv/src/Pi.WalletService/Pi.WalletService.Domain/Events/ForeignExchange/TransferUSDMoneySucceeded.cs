using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
namespace Pi.WalletService.Domain.Events.ForeignExchange;

public record TransferUsdMoneyToSubSucceeded(string TransactionNo, string FromAccount, string ToAccount, decimal TransferAmount, Currency TransferCurrency, DateTime RequestedTime, DateTime CompletedTime, decimal Fee);
public record TransferUsdMoneyToMainSucceeded(string TransactionNo, string FromAccount, string ToAccount, decimal TransferAmount, Currency TransferCurrency, DateTime RequestedTime, DateTime CompletedTime, decimal Fee);