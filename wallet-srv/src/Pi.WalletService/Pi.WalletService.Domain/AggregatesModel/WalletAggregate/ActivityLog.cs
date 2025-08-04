using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.SeedWork;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
namespace Pi.WalletService.Domain.AggregatesModel.WalletAggregate;

public class ActivityLog : BaseEntity, SagaStateMachineInstance
{
    public ActivityLog(
        Guid id,
        Guid correlationId,
        string transactionNo,
        TransactionType transactionType,
        string userId,
        string accountCode,
        string customerCode,
        Channel channel,
        Product product,
        Purpose purpose,
        string stateMachine,
        string state,
        decimal requestedAmount,
        DateTime? paymentReceivedDateTime,
        decimal? paymentReceivedAmount,
        DateTime? paymentDisbursedDateTime,
        decimal? paymentDisbursedAmount,
        decimal? paymentConfirmedAmount,
        string? otpRequestRef,
        string? otpRequestId,
        DateTime? otpConfirmedDateTime,
        decimal? fee,
        string? customerName,
        string? bankAccountNo,
        string? bankAccountName,
        string? bankAccountTaxId,
        string? bankName,
        string? bankCode,
        string? bankBranchCode,
        DateTime? depositGeneratedDateTime,
        string? qrTransactionNo,
        string? qrTransactionRef,
        string? qrValue,
        Currency? requestedCurrency,
        string? requestedAmountWithCurrency,
        decimal? requestedFxAmount,
        Currency? requestedFxCurrency,
        string? requestedFxAmountWithCurrency,
        Currency? paymentReceivedCurrency,
        decimal? transferFee,
        string? fxTransactionId,
        decimal? exchangeAmount,
        Currency? exchangeCurrency,
        DateTime? fxInitiateRequestDateTime,
        DateTime? fxConfirmedDateTime,
        decimal? fxConfirmedExchangeRate,
        decimal? fxConfirmedAmount,
        Currency? fxConfirmedCurrency,
        string? fxConfirmedAmountWithCurrency,
        string? transferFromAccount,
        decimal? transferAmount,
        string? transferToAccount,
        Currency? transferCurrency,
        string? transferAmountWithCurrency,
        DateTime? transferRequestTime,
        DateTime? transferCompleteTime,
        string? failedReason,
        string? requestId,
        string? requesterDeviceId
    )
    {
        Id = id;
        CorrelationId = correlationId;
        TransactionNo = transactionNo;
        TransactionType = transactionType;
        UserId = userId;
        AccountCode = accountCode;
        CustomerCode = customerCode;
        Channel = channel;
        Product = product;
        Purpose = purpose;
        StateMachine = stateMachine;
        State = state;
        RequestedAmount = requestedAmount;
        PaymentReceivedDateTime = paymentReceivedDateTime;
        PaymentReceivedAmount = paymentReceivedAmount;
        PaymentDisbursedDateTime = paymentDisbursedDateTime;
        PaymentDisbursedAmount = paymentDisbursedAmount;
        PaymentConfirmedAmount = paymentConfirmedAmount;
        OtpRequestRef = otpRequestRef;
        OtpRequestId = otpRequestId;
        OtpConfirmedDateTime = otpConfirmedDateTime;
        Fee = fee;
        CustomerName = customerName;
        BankAccountNo = bankAccountNo;
        BankAccountName = bankAccountName;
        BankAccountTaxId = bankAccountTaxId;
        BankName = bankName;
        BankCode = bankCode;
        BankBranchCode = bankBranchCode;
        DepositGeneratedDateTime = depositGeneratedDateTime;
        QrTransactionNo = qrTransactionNo;
        QrTransactionRef = qrTransactionRef;
        QrValue = qrValue;
        RequestedCurrency = requestedCurrency;
        RequestedAmountWithCurrency = requestedAmountWithCurrency;
        RequestedFxAmount = requestedFxAmount;
        RequestedFxCurrency = requestedFxCurrency;
        RequestedFxAmountWithCurrency = requestedFxAmountWithCurrency;
        PaymentReceivedCurrency = paymentReceivedCurrency;
        TransferFee = transferFee;
        FxTransactionId = fxTransactionId;
        FxInitiateRequestDateTime = fxInitiateRequestDateTime;
        FxConfirmedDateTime = fxConfirmedDateTime;
        FxConfirmedExchangeRate = fxConfirmedExchangeRate;
        FxConfirmedAmount = fxConfirmedAmount;
        FxConfirmedCurrency = fxConfirmedCurrency;
        FxConfirmedAmountWithCurrency = fxConfirmedAmountWithCurrency;
        TransferFromAccount = transferFromAccount;
        TransferAmount = transferAmount;
        TransferToAccount = transferToAccount;
        TransferCurrency = transferCurrency;
        TransferAmountWithCurrency = transferAmountWithCurrency;
        TransferRequestTime = transferRequestTime;
        TransferCompleteTime = transferCompleteTime;
        FailedReason = failedReason;
        RequestId = requestId;
        RequesterDeviceId = requesterDeviceId;
        ExchangeAmount = exchangeAmount;
        ExchangeCurrency = exchangeCurrency;
    }

    public Guid Id { get; set; }
    public Guid CorrelationId { get; set; }
    public string? TransactionNo { get; set; }
    public TransactionType TransactionType { get; set; }
    public string UserId { get; set; }
    public string AccountCode { get; set; }
    public string CustomerCode { get; set; }
    public Channel Channel { get; set; }
    public Product Product { get; set; }
    public Purpose Purpose { get; set; }
    public string StateMachine { get; set; }
    public string State { get; set; }
    public decimal RequestedAmount { get; set; }
    public DateTime? PaymentReceivedDateTime { get; set; }
    public decimal? PaymentReceivedAmount { get; set; }
    public DateTime? PaymentDisbursedDateTime { get; set; }
    public decimal? PaymentDisbursedAmount { get; set; }
    public decimal? PaymentConfirmedAmount { get; set; }

    public string? OtpRequestRef { get; set; }
    public string? OtpRequestId { get; set; }
    public DateTime? OtpConfirmedDateTime { get; set; }
    public decimal? Fee { get; set; }
    public string? CustomerName { get; set; }
    public string? BankAccountNo { get; set; }
    public string? BankAccountName { get; set; }
    public string? BankAccountTaxId { get; set; }
    public string? BankName { get; set; }
    public string? BankCode { get; set; }
    public string? BankBranchCode { get; set; }
    public DateTime? DepositGeneratedDateTime { get; set; }
    public string? QrTransactionNo { get; set; }
    public string? QrTransactionRef { get; set; }
    public string? QrValue { get; set; }
    public Currency? RequestedCurrency { get; set; }
    public string? RequestedAmountWithCurrency { get; set; }
    public decimal? RequestedFxAmount { get; set; }
    public Currency? RequestedFxCurrency { get; set; }
    public string? RequestedFxAmountWithCurrency { get; set; }
    public Currency? PaymentReceivedCurrency { get; set; }
    public decimal? TransferFee { get; set; }
    public string? FxTransactionId { get; set; }
    public DateTime? FxInitiateRequestDateTime { get; set; }
    public decimal? ExchangeAmount { get; set; }
    public Currency? ExchangeCurrency { get; set; }
    public DateTime? FxConfirmedDateTime { get; set; }
    public decimal? FxConfirmedExchangeRate { get; set; }
    public decimal? FxConfirmedAmount { get; set; }
    public Currency? FxConfirmedCurrency { get; set; }
    public string? FxConfirmedAmountWithCurrency { get; set; }
    public string? TransferFromAccount { get; set; }

    public decimal? TransferAmount { get; set; }
    public string? TransferToAccount { get; set; }
    public Currency? TransferCurrency { get; set; }
    public string? TransferAmountWithCurrency { get; set; }
    public DateTime? TransferRequestTime { get; set; }
    public DateTime? TransferCompleteTime { get; set; }
    public string? FailedReason { get; set; }
    public string? RequestId { get; set; }
    public string? RequesterDeviceId { get; set; }
    public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
