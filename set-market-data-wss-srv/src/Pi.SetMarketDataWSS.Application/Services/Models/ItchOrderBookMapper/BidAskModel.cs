namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;

public class BidAskModel
{
    public BidAskModel(decimal price, int quantity)
    {
        Price = price;
        Quantity = quantity;
    }

    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
}