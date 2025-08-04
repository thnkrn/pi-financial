using System.ComponentModel.DataAnnotations;

namespace Pi.MarketData.Search.Application.Configs;

public class FundMarketDataOptions
{
    public const string Options = "FundMarketData";
    [Required]
    public required string Host { get; init; }
}