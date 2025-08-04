using Pi.SetService.Domain.Utils;

namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public record AccountPosition
{
    public AccountPosition(string secSymbol, Ttf ttf)
    {
        Ttf = ttf;
        SecSymbol = SetHelper.CleanSymbol(secSymbol, ttf);
    }

    public required string TradingAccountNo { get; init; }
    public required string AccountNo { get; init; }
    public required StockType StockType { get; init; }
    public required StockTypeChar StockTypeChar { get; init; }
    public required decimal StartVolume { get; init; }
    public required decimal StartPrice { get; init; }
    public required decimal AvailableVolume { get; init; }
    public required decimal ActualVolume { get; init; }
    public required decimal AvgPrice { get; init; }
    public required decimal Amount { get; init; }
    public Ttf Ttf { get; }
    public string SecSymbol { get; }

    public decimal? TodayRealize { get; init; }

    public OrderAction Action
    {
        get
        {
            return StockType switch
            {
                StockType.Short => OrderAction.Short,
                // StockType.Borrow => null, // TODO: Confirm Action is null when borrow
                _ => OrderAction.Buy
            };
        }
    }

    public OrderSide Side
    {
        get
        {
            return StockType switch
            {
                StockType.Short => OrderSide.Sell,
                // StockType.Borrow => null, // TODO: Confirm Side is null when borrow
                _ => OrderSide.Buy
            };
        }
    }

    public bool IsNvdr()
    {
        return SetHelper.IsNvdr(Ttf);
    }
}
