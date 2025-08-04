using Pi.WalletService.Domain.SeedWork;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.WalletAggregate;

public class GlobalWalletTransactionHistory : BaseEntity
{
    public GlobalWalletTransactionHistory(
        Guid id,
        Guid correlationId,
        string userId,
        string currentState,
        long customerId,
        string customerCode,
        string globalAccount,
        string transactionNo,
        TransactionType transactionType,
        decimal requestedAmount,
        string requestedCurrency,
        string requestedAmountWithCurrency,
        decimal requestedFxAmount,
        string requestedFxCurrency,
        string requestedFxAmountWithCurrency,
        decimal? paymentReceivedAmount,
        string? paymentReceivedCurrency,
        DateTime? fxInitiateRequestDateTime,
        string? fxTransactionId,
        decimal? fxConfirmedExchangeRate,
        DateTime? fxConfirmedDateTime,
        decimal? fxConfirmedAmount,
        string? fxConfirmedCurrency,
        string? fxConfirmedAmountWithCurrency,
        string? transferFromAccount,
        decimal? transferAmount,
        string? transferToAccount,
        string? transferCurrency,
        string? transferAmountWithCurrency,
        DateTime? transferRequestTime,
        DateTime? transferCompleteTime,
        decimal? transactionFee,
        decimal? refundAmount,
        decimal? netAmount,
        Guid? requesterDeviceId
    )
    {
        Id = id;
        CorrelationId = correlationId;
        UserId = userId;
        TransactionFee = transactionFee;
        CurrentState = currentState;
        CustomerId = customerId;
        CustomerCode = customerCode;
        GlobalAccount = globalAccount;
        TransactionNo = transactionNo;
        TransactionType = transactionType;
        RequestedAmount = requestedAmount;
        RequestedCurrency = requestedCurrency;
        RequestedAmountWithCurrency = requestedAmountWithCurrency;
        RequestedFxAmount = requestedFxAmount;
        RequestedFxCurrency = requestedFxCurrency;
        RequestedFxAmountWithCurrency = requestedFxAmountWithCurrency;
        PaymentReceivedAmount = paymentReceivedAmount;
        PaymentReceivedCurrency = paymentReceivedCurrency;
        FxInitiateRequestDateTime = fxInitiateRequestDateTime;
        FxTransactionId = fxTransactionId;
        FxConfirmedDateTime = fxConfirmedDateTime;
        FxConfirmedExchangeRate = fxConfirmedExchangeRate;
        FxConfirmedAmount = fxConfirmedAmount;
        FxConfirmedCurrency = fxConfirmedCurrency;
        FxConfirmedAmountWithCurrency = fxConfirmedAmountWithCurrency;
        TransferFromAccount = transferFromAccount;
        TransferAmount = transferAmount;
        TransferToAccount = transferToAccount;
        TransferCurrency = transferCurrency;
        TransferRequestTime = transferRequestTime;
        TransferCompleteTime = transferCompleteTime;
        TransferAmountWithCurrency = transferAmountWithCurrency;
        RefundAmount = refundAmount;
        NetAmount = netAmount;
        RequesterDeviceId = requesterDeviceId;
    }

    public Guid Id { get; private set; }
    public Guid CorrelationId { get; private set; }
    public string? TransactionNo { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public string UserId { get; private set; }
    public long CustomerId { get; private set; }
    public string CustomerCode { get; private set; }
    public string GlobalAccount { get; private set; }
    public string CurrentState { get; private set; }
    public decimal RequestedAmount { get; private set; }
    public string RequestedCurrency { get; private set; }
    public string RequestedAmountWithCurrency { get; private set; }
    public decimal RequestedFxAmount { get; private set; }
    public string RequestedFxCurrency { get; private set; }
    public string RequestedFxAmountWithCurrency { get; private set; }
    public decimal? PaymentReceivedAmount { get; private set; }
    public string? PaymentReceivedCurrency { get; private set; }
    public decimal? TransactionFee { get; private set; }

    public string? FxTransactionId { get; private set; }

    // FxInitiateRequest
    public DateTime? FxInitiateRequestDateTime { get; private set; }

    // FxInitiateResponse
    public DateTime? FxConfirmedDateTime { get; private set; }
    public decimal? FxConfirmedExchangeRate { get; private set; }
    public decimal? FxConfirmedAmount { get; private set; }
    public string? FxConfirmedCurrency { get; private set; }

    public string? FxConfirmedAmountWithCurrency { get; private set; }

    // Transfer Account Request
    public string? TransferFromAccount { get; private set; }
    public decimal? TransferAmount { get; private set; }
    public string? TransferToAccount { get; private set; }
    public string? TransferCurrency { get; private set; }
    public string? TransferAmountWithCurrency { get; private set; }
    public DateTime? TransferRequestTime { get; private set; }
    public DateTime? TransferCompleteTime { get; private set; }

    public string? FailedReason { get; private set; }

    // Always in THB
    public decimal? RefundAmount { get; private set; }

    // Always in THB
    public decimal? NetAmount { get; private set; }

    public Guid? RequesterDeviceId { get; private set; }
}