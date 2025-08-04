using System.ComponentModel.DataAnnotations;
using Pi.Financial.FundService.Application.Models.Trading;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.API.Models;

public class SiriusPlaceOrderRequest
{
    [Required]
    public required string Symbol { get; init; }
    [Required]
    public required string EffectiveDate { get; init; }
    [Required]
    public required string TradingAccountNo { get; init; }
    [Required]
    public required decimal Quantity { get; init; }
}

public class SiriusPlaceBuyOrderRequest : SiriusPlaceOrderRequest
{
    [Required]
    public required PaymentMethod PaymentMethod { get; init; }
    [Required]
    public required string BankAccount { get; init; }
    [Required]
    public required string BankCode { get; init; }
    public string? UnitHolderId { get; init; }
    public bool? Recurring { get; init; }
}

public class SiriusPlaceSellOrderRequest : SiriusPlaceOrderRequest
{
    [Required]
    public required RedemptionType UnitType { get; init; }
    [Required]
    public required string UnitHolderId { get; init; }
    [Required]
    public required string BankAccount { get; init; }
    [Required]
    public required string BankCode { get; init; }
    public bool? SellAllFlag { get; init; }
}

public class SiriusPlaceSwitchOrderRequest : SiriusPlaceOrderRequest
{
    [Required]
    public required string TargetSymbol { get; init; }
    [Required]
    public required string UnitHolderId { get; init; }
    [Required]
    public required RedemptionType UnitType { get; init; }
    public bool? SellAllFlag { get; init; }
}
