using System.ComponentModel.DataAnnotations;
using MassTransit;
using Pi.Common.SeedWork;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Domain.AggregatesModel.TradingAggregate;

public class EquityOrderState : SagaStateMachineInstance, IAuditableEntity
{
    public EquityOrderState(Guid correlationId, Guid tradingAccountId, string tradingAccountNo, TradingAccountType tradingAccountType, string customerCode, string secSymbol)
    {
        CorrelationId = correlationId;
        TradingAccountId = tradingAccountId;
        TradingAccountNo = tradingAccountNo;
        TradingAccountType = tradingAccountType;
        CustomerCode = customerCode;
        SecSymbol = secSymbol;
    }

    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public Guid TradingAccountId { get; set; }
    public string TradingAccountNo { get; set; }
    public TradingAccountType TradingAccountType { get; set; }
    public string CustomerCode { get; set; }
    public string? EnterId { get; set; }
    public string? OrderNo { get; set; }
    public string? BrokerOrderId { get; set; }
    public ConditionPrice ConditionPrice { get; set; }
    public OrderStatus? OrderStatus { get; set; }
    public string SecSymbol { get; set; }
    public decimal? Price { get; set; }
    public decimal? MatchedPrice { get; set; }
    public int Volume { get; set; }
    public int PubVolume { get; set; }
    public int? MatchedVolume { get; set; }
    public int? CancelledVolume { get; set; }
    public OrderSide OrderSide { get; set; }
    public OrderAction OrderAction { get; set; }
    public OrderType? OrderType { get; set; }
    public Condition Condition { get; set; }
    public ServiceType? ServiceType { get; set; }
    public Ttf? Ttf { get; set; }
    public bool? Lending { get; set; }
    public string? StockType { get; set; }
    public bool? TodaySell { get; set; }
    public DateTime? OrderDateTime { get; set; }
    public string? IpAddress { get; set; }
    public string? FailedReason { get; set; }
    public string? ResponseAddress { get; set; }
    public Guid? RequestId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateOnly CreatedAtDate { get; set; }
    public DateTime? UpdatedAt { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; protected set; }
}
