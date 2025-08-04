using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
namespace Pi.WalletService.Application.Services.FxService;

public record InitiateRequest(
    FxQuoteType FxQuoteType,
    string ContractCurrency,
    decimal? ContractAmount,
    string ContractAccountId,
    string CounterCurrency,
    string CounterAccountId,
    decimal? CounterAmount,
    string Ref1,
    string RequestedBy,
    string? Ref2 = null);

public enum FxQuoteType
{
    Buy = 'B',
    Sell = 'S'
}

public record InitiateResponse(
    string TransactionId,
    decimal ContractAmount,
    decimal CounterAmount,
    Currency ContractCurrency,
    Currency CounterCurrency,
    decimal ExchangeRate,
    DateTime ExpireAt
    );

public record ConfirmRequest(string TransactionId);

public record GetTransactionResponse(
    string Id,
    FxQuoteType QuoteType,
    DateTime? ValueDate,
    decimal? ContractAmount,
    string ContractCurrency,
    decimal? CounterAmount,
    string CounterCurrency,
    decimal? ExchangeRate,
    DateTime TransactionDateTime
);

public interface IFxService
{
    public Task<InitiateResponse> Initiate(InitiateRequest request);
    public Task Confirm(ConfirmRequest request);
    public Task<GetTransactionResponse> GetTransaction(string transactionId);
}