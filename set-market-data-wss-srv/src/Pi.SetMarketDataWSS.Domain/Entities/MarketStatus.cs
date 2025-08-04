namespace Pi.SetMarketDataWSS.Domain.Entities;

public class MarketStatus
{
    public long MarketStatusId { get; set; }
    public string? Status { get; set; }
    public string? Description { get; set; }
    public DateTime Timestamp { get; set; }
}