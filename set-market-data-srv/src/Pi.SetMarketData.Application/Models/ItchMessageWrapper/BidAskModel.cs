namespace Pi.SetMarketData.Application.Models.ItchMessageWrapper;

public class BidAskModel
{
    /// <summary>
    /// </summary>
    /// <param name="price"></param>
    /// <param name="quantity"></param>
    public BidAskModel(decimal price, int quantity)
    {
        Price = price;
        Quantity = quantity;
    }

    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
}