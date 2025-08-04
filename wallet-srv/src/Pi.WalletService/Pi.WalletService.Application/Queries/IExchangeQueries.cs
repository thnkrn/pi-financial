using Pi.WalletService.Application.Services.FxService;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Queries;

public interface IExchangeQueries
{
    decimal ExchangeCurrency(
        TransactionType transactionType,
        string inputCurrency,
        decimal inputAmount,
        string exchangeCurrency,
        decimal exchangeRate
    );

    Task<ExchangeRate> GetExchangeRate(
        FxQuoteType quoteType,
        string contractCurrency,
        decimal contractAmount,
        string counterCurrency,
        string requestedBy
    );
}