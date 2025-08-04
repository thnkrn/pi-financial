using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.RefundAggregate;

public class RefundState : SagaStateMachineInstance, ITransactionState
{
    public Guid CorrelationId { get; set; }
    public string? DepositTransactionNo { get; set; }
    public string? TransactionNo { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string AccountCode { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public Channel Channel { get; set; }
    public Product Product { get; set; }
    public string? CurrentState { get; set; }
    public decimal Amount { get; set; }
    public string? BankAccountNo { get; set; }
    public string? BankName { get; set; }
    public string? BankCode { get; set; }
    public decimal? BankFee { get; set; }
    public DateTime? RefundedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ResponseAddress { get; set; }
    public Guid? RequestId { get; set; }
    public string? FailedReason { get; set; }
}
