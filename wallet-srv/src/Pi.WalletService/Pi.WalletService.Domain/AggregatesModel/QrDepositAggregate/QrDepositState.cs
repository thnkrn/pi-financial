using MassTransit;
using Pi.WalletService.Domain.SeedWork;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;

public class QrDepositState : BaseEntity, SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public string? TransactionNo { get; set; }
    public Product? Product { get; set; }
    public Channel? Channel { get; set; }
    public decimal? PaymentReceivedAmount { get; set; }
    public DateTime? PaymentReceivedDateTime { get; set; }
    public decimal? Fee { get; set; }
    public DateTime? DepositQrGenerateDateTime { get; set; }
    public int QrCodeExpiredTimeInMinute { get; set; }
    public string? QrTransactionNo { get; set; }
    public string? QrValue { get; set; }
    public string? QrTransactionRef { get; set; }
    public string? FailedReason { get; set; }
    public string ResponseAddress { get; set; } = string.Empty;
    public Guid? QrExpireTokenId { get; set; }
}
