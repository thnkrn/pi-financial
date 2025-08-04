using MassTransit;
using Pi.WalletService.Domain.SeedWork;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;

public class OddDepositState : BaseEntity, SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public Product Product { get; set; }
    public Channel? Channel { get; set; }
    public DateTime? PaymentReceivedDateTime { get; set; }
    public decimal? PaymentReceivedAmount { get; set; }
    public decimal? Fee { get; set; }
    public string? OtpRequestRef { get; set; }
    public Guid? OtpRequestId { get; set; }
    public DateTime? OtpConfirmedDateTime { get; set; }
    public string? FailedReason { get; set; }
    public Guid? RequestId { get; set; }
    public string ResponseAddress { get; set; } = string.Empty;
    public Guid? OtpValidationExpireTokenId { get; set; }
}
