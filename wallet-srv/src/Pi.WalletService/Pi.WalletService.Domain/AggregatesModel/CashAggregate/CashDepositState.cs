using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.SeedWork;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.CashAggregate;

public class CashDepositState : BaseEntity, SagaStateMachineInstance
{
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
    public DateTime? PaymentReceivedDateTime { get; set; }
    public string? BankName { get; set; }
    public string? FailedReason { get; set; }
}
