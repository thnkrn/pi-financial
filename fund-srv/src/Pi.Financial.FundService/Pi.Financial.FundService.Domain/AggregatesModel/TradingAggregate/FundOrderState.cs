using MassTransit;
using Pi.Common.SeedWork;

namespace Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

public class FundOrderState : SagaStateMachineInstance, IAuditableEntity, IAggregateRoot
{
    public FundOrderState(Guid correlationId, string tradingAccountNo, string fundCode, Channel channel)
    {
        CorrelationId = correlationId;
        TradingAccountNo = tradingAccountNo;
        FundCode = fundCode;
        Channel = channel;
    }

    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public string? OrderNo { get; set; }
    public string? BrokerOrderId { get; set; }
    public string? AmcOrderId { get; set; }
    public string TradingAccountNo { get; set; }
    public string? UnitHolderId { get; set; }
    public string? CustomerCode { get; set; }
    public string FundCode { get; set; }
    public OrderSide OrderSide { get; set; }
    public RedemptionType? RedemptionType { get; set; }
    public FundOrderType OrderType { get; set; }
    public FundAccountType? AccountType { get; set; }
    public decimal? Unit { get; set; }
    public decimal? Amount { get; set; }
    public decimal? AllottedUnit { get; set; }
    public decimal? AllottedAmount { get; set; }
    public decimal? AllottedNav { get; set; }
    public DateOnly? AllottedDate { get; set; }
    public DateOnly? NavDate { get; set; }
    public Guid? TradingAccountId { get; set; }
    public string? CounterFundCode { get; set; }
    public FundOrderStatus OrderStatus { get; set; }
    public Channel Channel { get; set; }
    public PaymentType? PaymentType { get; set; }
    public bool? SellAllUnit { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateOnly? EffectiveDate { get; set; }
    public DateOnly? SettlementDate { get; set; }
    public string? SaleLicense { get; set; }
    public string? BankAccount { get; set; }
    public string? BankCode { get; set; }
    public string? SettlementBankAccount { get; set; }
    public string? SettlementBankCode { get; set; }
    public string? FailedReason { get; set; }
    public string? RejectReason { get; set; }
    public string? ResponseAddress { get; set; }
    public Guid? RequestId { get; set; }
}
