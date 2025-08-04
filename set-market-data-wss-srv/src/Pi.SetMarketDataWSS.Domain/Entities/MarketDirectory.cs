namespace Pi.SetMarketDataWSS.Domain.Entities;

public class MarketDirectory
{
    public int MarketCode { get; set; }
    public string? MarketName { get; set; }
    public string? MarketDescription { get; set; }
}