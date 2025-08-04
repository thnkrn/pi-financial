namespace Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

public class TransactionHistoryV2
{
    public string? State { get; set; }
    public Product? Product { get; set; }
    public string? AccountCode { get; set; }
    public string? CustomerName { get; set; }
    public string? BankAccountNo { get; set; }
    public string? BankAccountName { get; set; }
    public string? BankName { get; set; }
    public DateOnly? EffectiveDate { get; set; }
    public DateTime? PaymentDateTime { get; set; }
    public string? GlobalAccount { get; set; }
    public string? TransactionNo { get; set; }
    public TransactionType TransactionType { get; set; }
    public string? RequestedAmount { get; set; }
    public Currency RequestedCurrency { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Currency? ToCurrency { get; set; }
    public string? TransferAmount { get; set; }
    public Channel? Channel { get; set; }
    public string? BankAccount { get; set; }
    public string? Fee { get; set; }
    public string? TransferFee { get; set; }
}

public class TransactionV2 : TransactionHistoryV2
{
    public Guid Id { get; set; }
    public string? CustomerCode { get; set; }
    public string? FailedReason { get; set; }
    public string? PaymentReceivedAmount { get; set; }
    public string? PaymentDisbursedAmount { get; set; }
    public string? ConfirmedAmount { get; set; }
    public QrDepositState? QrDeposit { get; set; }
    public OddDepositState? OddDeposit { get; set; }
    public AtsDepositState? AtsDeposit { get; set; }
    public OddWithdrawState? OddWithdraw { get; set; }
    public AtsWithdrawState? AtsWithdraw { get; set; }
    public GlobalTransferState? GlobalTransfer { get; set; }
    public UpBackState? UpBack { get; set; }
    public RecoveryState? Recovery { get; set; }
    public RefundInfo? Refund { get; set; }
    public BillPaymentState? BillPayment { get; set; }
}

public class QrDepositState
{
    public string? State { get; set; }
    public decimal? PaymentReceivedAmount { get; set; }
    public DateTime? PaymentReceivedDateTime { get; set; }
    public decimal? Fee { get; set; }
    public DateTime? DepositQrGenerateDateTime { get; set; }
    public string? QrTransactionNo { get; set; }
    public string? QrValue { get; set; }
    public string? QrTransactionRef { get; set; }
    public string? FailedReason { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class OddDepositState
{
    public string? State { get; set; }
    public DateTime? PaymentReceivedDateTime { get; set; }
    public decimal? PaymentReceivedAmount { get; set; }
    public decimal? Fee { get; set; }
    public string? OtpRequestRef { get; set; }
    public Guid? OtpRequestId { get; set; }
    public DateTime? OtpConfirmedDateTime { get; set; }
    public string? FailedReason { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AtsDepositState
{
    public string? State { get; set; }
    public DateTime? PaymentReceivedDateTime { get; set; }
    public decimal? PaymentReceivedAmount { get; set; }
    public decimal? Fee { get; set; }
    public string? OtpRequestRef { get; set; }
    public Guid? OtpRequestId { get; set; }
    public DateTime? OtpConfirmedDateTime { get; set; }
    public string? FailedReason { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class OddWithdrawState
{
    public string? State { get; set; }
    public DateTime? PaymentDisbursedDateTime { get; set; }
    public decimal? PaymentDisbursedAmount { get; set; }
    public decimal? Fee { get; set; }
    public string? OtpRequestRef { get; set; }
    public Guid? OtpRequestId { get; set; }
    public DateTime? OtpConfirmedDateTime { get; set; }
    public string? FailedReason { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AtsWithdrawState
{
    public string? State { get; set; }
    public DateTime? PaymentDisbursedDateTime { get; set; }
    public decimal? PaymentDisbursedAmount { get; set; }
    public decimal? Fee { get; set; }
    public string? OtpRequestRef { get; set; }
    public Guid? OtpRequestId { get; set; }
    public DateTime? OtpConfirmedDateTime { get; set; }
    public string? FailedReason { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UpBackState
{
    public string? State { get; set; }
    public string? FailedReason { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class GlobalTransferState
{
    public string? State { get; set; }
    public string? GlobalAccount { get; set; } = string.Empty;
    public Currency? RequestedCurrency { get; set; }
    public decimal? RequestedFxRate { get; set; }
    public Currency? RequestedFxCurrency { get; set; }
    public decimal? PaymentReceivedAmount { get; set; }
    public Currency? PaymentReceivedCurrency { get; set; }
    public DateTime? FxInitiateRequestDateTime { get; set; }
    public string? FxTransactionId { get; set; }
    public decimal? FxConfirmedAmount { get; set; }
    public decimal? FxConfirmedExchangeRate { get; set; }
    public Currency? FxConfirmedCurrency { get; set; }
    public DateTime? FxConfirmedDateTime { get; set; }
    public decimal? TransferAmount { get; set; }
    public Currency? TransferCurrency { get; set; }
    public decimal? TransferFee { get; set; }
    public string? TransferFromAccount { get; set; }
    public string? TransferToAccount { get; set; }
    public DateTime? TransferRequestTime { get; set; }
    public DateTime? TransferCompleteTime { get; set; }
    public string? FailedReason { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class RecoveryState
{
    public string? State { get; set; }
    public string GlobalAccount { get; set; } = string.Empty;
    public string? TransferFromAccount { get; set; }
    public decimal? TransferAmount { get; set; }
    public string? TransferToAccount { get; set; }
    public Currency? TransferCurrency { get; set; }
    public DateTime? TransferRequestTime { get; set; }
    public DateTime? TransferCompleteTime { get; set; }
    public string? FailedReason { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class RefundInfo
{
    public string? RefundId { get; set; }
    public decimal? Amount { get; set; }
    public string? TransferToAccountNo { get; set; }
    public string? TransferToAccountName { get; set; }
    public decimal? Fee { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class BillPaymentState
{
    public string? State { get; set; }
    public decimal? PaymentReceivedAmount { get; set; }
    public DateTime? PaymentReceivedDateTime { get; set; }
    public string? Reference1 { get; set; }
    public string? Reference2 { get; set; }
    public string? CustomerPaymentName { get; set; }
    public string? CustomerPaymentBankCode { get; set; }
    public decimal? Fee { get; set; }
    public string? BillPaymentTransactionRef { get; set; }
    public string? FailedReason { get; set; }
}

