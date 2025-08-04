using System.ComponentModel;
using Pi.FundMarketData.API.Attributes;

namespace Pi.FundMarketData.API.Models.Filters;

public enum FundType
{
    [FilterDefault]
    [Description("All")]
    All,
    [Description("Money Market")] MoneyMarket,
    [Description("Fixed Income")] FixedIncome,
    [Description("Global Fixed Income")] GlobalFixedIncome,
    [Description("Thailand Equity")] ThailandEquity,
    [Description("Global Equity (Emerging)")] GlobalEquityEmerging,
    [Description("Global Equity (Developed)")] GlobalEquityDeveloped,
    [Description("Allocation")] Allocation,
    [Description("REITs")] REITs,
    [Description("Commodity")] Commodity,
    [Description("Others")] Others,
}
