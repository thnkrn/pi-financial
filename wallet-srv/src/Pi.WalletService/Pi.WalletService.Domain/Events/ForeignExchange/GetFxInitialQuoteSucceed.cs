using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
namespace Pi.WalletService.Domain.Events.ForeignExchange;

public record GetFxInitialQuoteSucceed(
    string TransactionId,
    decimal ConfirmedFxRate,
    decimal ConfirmedAmount,
    Currency ConfirmedCurrency,
    decimal ConfirmedExchangeAmount,
    Currency ConfirmedExchangeCurrency,
    DateTime InitiateDateTime
);
