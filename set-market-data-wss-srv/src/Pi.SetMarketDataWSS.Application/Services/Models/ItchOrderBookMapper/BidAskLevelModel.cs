namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;

public class BidAskLevelModel(decimal price, long quantity, int level)
{
    public decimal Price { get; private set; } = price;
    public long Quantity { get; private set; } = quantity;
    public int Level { get; private set; } = level;
}