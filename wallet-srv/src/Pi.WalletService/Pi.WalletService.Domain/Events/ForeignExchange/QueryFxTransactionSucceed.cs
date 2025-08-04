using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
namespace Pi.WalletService.Domain.Events.ForeignExchange;

public record QueryFxTransactionSucceed(
    string Id,
    DateTime ValueDate,
    decimal ContractAmount,
    Currency ContractCurrency,
    decimal ExchangeRate,
    DateTime TransactionDateTime,
    decimal PreMarkUpRequestedFxRate
);