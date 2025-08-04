namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;

// ReSharper disable UnusedAutoPropertyAccessor.Global
public class PublicTradeWrapper
{
    public int Nanos { get; set; }
    public string? DealDateTime { get; set; }
    public Aggressor? Aggressor { get; set; }
    public Quantity? Quantity { get; set; }
    public Price? Price { get; set; }
    public double PriceChange { get; set; }
}

public class Aggressor
{
    public string? Value { get; set; }
}

