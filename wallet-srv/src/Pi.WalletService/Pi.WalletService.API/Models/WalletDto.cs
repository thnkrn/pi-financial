using System.ComponentModel.DataAnnotations;
using Pi.WalletService.Application.Services.FxService;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.API.Models;

[Obsolete("Use DepositGenericRequest Instead")]
public record DepositGlobalRequest(
    string? CustomerCode, // Nullable for backward compat for now, remove when client param is implemented
    decimal RequestedFxRate,
    decimal DepositAmount,
    string RequestedCurrency,
    string RequestedFxCurrency
);

[Obsolete("Use WithdrawGenericRequest Instead")]
public record WithdrawGlobalRequest(
    string? CustomerCode, // Nullable for backward compat for now, remove when client param is implemented
    decimal ForeignAmount,
    string ForeignCurrency,
    string FxTransactionId
);

public record ManualAllocationRequest(string TransactionNo);

public record WithdrawResponse(Guid TicketId, string TransactionNo, string OtpRefCode);

public record ExchangeRateRequest(
    FxQuoteType FxQuoteType,
    string ContractCurrency,
    decimal ContractAmount,
    string CounterCurrency,
    string RequestedBy
);

public record CurrencyExchangeRequest(
    TransactionType TransactionType,
    string InputCurrency,
    string ExchangeCurrency
)
{
    [Range(0.01, (double)decimal.MaxValue)]
    public decimal InputAmount { get; set; }

    [Range(0.01, (double)decimal.MaxValue)]
    public decimal ExchangeRate { get; set; }
};

public record CurrencyExchangeResponse(decimal ExchangeAmount);

public record TicketResponse(Guid TicketId, string TransactionNo, string? OtpRefCode = null);

public record RefundResponse(Guid RefundId, string TransactionNo, DateTime RefundedAt);

public record DepositForeignExchangeRequest(
    string? FxTransactionId,
    decimal? RequestedFxRate,
    string RequestedCurrency,
    string RequestedFxCurrency
);

public record WithdrawForeignExchangeRequest(
    decimal ForeignAmount,
    string ForeignCurrency,
    string FxTransactionId
);

public record DepositGenericRequest(
    string? CustomerCode, // Nullable for backward compat for now, remove when client param is implemented
    Product Product,
    Channel Channel,
    decimal RequestAmount,
    DepositForeignExchangeRequest? ForeignExchangeRequest = null
);

public record WithdrawGenericRequest(
    string? CustomerCode, // Nullable for backward compat for now, remove when client param is implemented
    Product Product,
    decimal RequestAmount,
    WithdrawForeignExchangeRequest? ForeignExchangeRequest = null
);

public record AvailableBalance(decimal Amount);

public record UpdateTransactionRequest(
    TransactionType TransactionType,
    string FailedReason
);
