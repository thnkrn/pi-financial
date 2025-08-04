namespace Pi.GlobalMarketDataRealTime.Domain.Models.Fix;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class SecurityInfo
{
    public string? Name { get; set; }
    public string? SymbolId { get; set; }
    public string? Description { get; set; }
    public string? UnderlyingSymbolId { get; set; }
    public string? Country { get; set; }
    public string? ISIN { get; set; }
    public string? Exchange { get; set; }
    public string? SymbolType { get; set; }
    public string? Currency { get; set; }
    public decimal MinPriceIncrement { get; set; }
    public string? Ticker { get; set; }
}