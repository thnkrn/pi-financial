public class SecurityInfo
{
    public required string Name { get; set; }
    public required string SymbolId { get; set; }
    public required string Description { get; set; }
    public required string UnderlyingSymbolId { get; set; }
    public required string Country { get; set; }
    public required string ISIN { get; set; }
    public required string Exchange { get; set; }
    public required string SymbolType { get; set; }
    public required string Currency { get; set; }
    public decimal MinPriceIncrement { get; set; }
    public required string Ticker { get; set; }
}