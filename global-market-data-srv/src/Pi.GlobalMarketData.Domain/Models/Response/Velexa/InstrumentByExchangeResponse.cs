namespace Pi.GlobalMarketData.Domain.Models.Response.Velexa;

public class VelexaInstrument
{
    public string SymbolId { get; set; }
    public string Ticker { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Group { get; set; }
    public string UnderlyingSymbolId { get; set; }
    public string Exchange { get; set; }
    public string Expiration { get; set; }
    public string Country { get; set; }
    public string SymbolType { get; set; }
    public OptionData OptionData { get; set; }
    public string MinPriceIncrement { get; set; }
    public string Currency { get; set; }
    public Identifiers Identifiers { get; set; }
    public string Icon { get; set; }
}

public class VelexaInstrumentSpecification
{
    public string ContractMultiplier { get; set; }
    public string Leverage { get; set; }
    private string _lotSize;
    public string LotSize
    {
        get => _lotSize;
        set => _lotSize = decimal.TryParse(value, out var lotSize) ? lotSize.ToString("G29") : value;
    }
    public string PriceUnit { get; set; }
    public string Units { get; set; }
}

public class OptionData
{
    public string OptionGroupId { get; set; }
    public string OptionRight { get; set; }
    public string StrikePrice { get; set; }
}

public class Identifiers
{
    public string ISIN { get; set; }
    public string FIGI { get; set; }
    public string RIC { get; set; }
    public string SEDOL { get; set; }
}