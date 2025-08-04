using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.SeedWork;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.AggregatesModel.CashAggregate;

public class CashWithdrawState : BaseEntity, SagaStateMachineInstance, ITransactionState
{
    public Guid CorrelationId { get; set; }
    public string? TransactionNo { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string AccountCode { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public Channel Channel { get; set; }
    public Product Product { get; set; }
    public string? CurrentState { get; set; }
    public decimal? BankFee { get; set; }
    public decimal RequestedAmount { get; set; }
    public string? BankName { get; set; }
    public string? BankCode { get; set; }
    public string? BankAccountNo { get; set; }
    public string? OtpRequestRef { get; set; }
    public Guid? OtpRequestId { get; set; }
    public DateTime? OtpConfirmedDateTime { get; set; }
    public Guid DeviceId { get; set; }
    public string? FailedReason { get; set; }
    public string ResponseAddress { get; set; } = string.Empty;
    public Guid? RequestId { get; set; }
}