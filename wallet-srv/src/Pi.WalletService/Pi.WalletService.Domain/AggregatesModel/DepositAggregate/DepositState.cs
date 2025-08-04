using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.DepositAggregate;

public class DepositState : SagaStateMachineInstance, ITransactionState
{
    private string? _bankAccountNo;
    public Guid CorrelationId { get; set; }
    public string? TransactionNo { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string AccountCode { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public Channel Channel { get; set; }
    public Product Product { get; set; }
    public Purpose Purpose { get; set; }
    public string? CurrentState { get; set; }
    public decimal RequestedAmount { get; set; }
    public decimal? BankFee { get; set; }
    public DateTime? PaymentReceivedDateTime { get; set; }
    public decimal? PaymentReceivedAmount { get; set; }
    // Amount = PaymentReceivedAmount - BankFee;
    public decimal? Amount { get; set; }
    public string? CustomerName { get; set; }
    public string? BankAccountName { get; set; }
    public string? BankName { get; set; }
    public string? BankCode { get; set; }
    public string? BankAccountNo
    {
        get => _bankAccountNo;
        set => _bankAccountNo = value?.Trim(Convert.ToChar(" ")).Replace(" ", "").Replace("-", "");
    }
    public DateTime? DepositQrGenerateDateTime { get; set; }
    public int QrCodeExpiredTimeInMinute { get; set; }
    public string? QrTransactionNo { get; set; }
    public string? QrValue { get; set; }
    public string? QrTransactionRef { get; set; }
    public string? FailedReason { get; set; }
    public byte[]? RowVersion { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string ResponseAddress { get; set; } = string.Empty;
    public Guid? RequestId { get; set; }

    public Guid? RequesterDeviceId { get; set; }
}
