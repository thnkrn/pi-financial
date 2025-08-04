using Pi.Common.CommonModels;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Application.Models.Dto;

public class AccountSummaryDto
{
    public required string TradingAccountNo { get; init; }
    public required string TradingAccountNoDisplay { get; init; }
    public required decimal UpnlPercentage { get; init; }
    public required ExchangeRate ExchangeRate { get; init; }
    public required IEnumerable<AccountSummaryValueDto> Values { get; init; }
    public required IEnumerable<PositionDto> Positions { get; init; }
}

public class AccountSummaryValueDto
{
    public Currency Currency { get; init; }
    public decimal NetAssetValue { get; init; }
    public decimal MarketValue { get; init; }
    public decimal Cost { get; init; }
    public decimal Upnl { get; init; }
    public decimal UnusedCash { get; init; }
    public decimal AccountLimit { get; init; }
    public decimal LineAvailable { get; init; }
}

public class PositionDto
{
    public required string Symbol { get; init; }
    public required string Venue { get; init; }
    public required string Logo { get; init; }
    public Currency Currency { get; init; }
    public decimal EntireQuantity { get; init; }
    public decimal AvailableQuantity { get; init; }
    public decimal LastPrice { get; init; }
    public decimal MarketValue { get; init; }
    public decimal AveragePrice { get; init; }
    public decimal Upnl { get; init; }
    public decimal Cost { get; init; }
    public decimal UpnlPercentage { get; init; }
}



