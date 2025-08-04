using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Domain.Tests.AggregatesModel.FinancialAssetAggregate;

public class OnlineOrderTest
{
    [Theory]
    [InlineData(Ttf.None, false)]
    [InlineData(Ttf.TrustFund, false)]
    [InlineData(Ttf.Nvdr, true)]
    public void IsNvdr_Should_Return_Expected(Ttf ttf, bool expected)
    {
        // Arrange
        var order = new OnlineOrder(123, "888888", "EA", OrderSide.Buy, ttf)
        {
            Price = 0,
            Volume = 0,
            EnterId = "9009",
            PubVolume = 0,
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            MatchVolume = 100,
            Type = OrderType.ShortCover,
            OrderState = OrderState.Pending,
            OrderAction = OrderAction.Cover,
            TradingAccountNo = "888888-1"
        };

        // Act
        var actual = order.IsNvdr();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(OrderStatus.Pending, true)]
    [InlineData(OrderStatus.PendingEx, true)]
    [InlineData(OrderStatus.PendingTg, true)]
    [InlineData(OrderStatus.Queuing, true)]
    [InlineData(OrderStatus.QueuingEx, true)]
    [InlineData(OrderStatus.Matched, false)]
    [InlineData(OrderStatus.PartialMatch, false)]
    [InlineData(OrderStatus.Matching, false)]
    [InlineData(OrderStatus.MatchedEx, false)]
    [InlineData(OrderStatus.Cancelled, false)]
    [InlineData(OrderStatus.Rejected, false)]
    [InlineData(OrderStatus.Unknown, false)]
    public void IsOpenOrder_Should_Return_Expected(OrderStatus status, bool expected)
    {
        // Arrange
        var order = new OnlineOrder(123, "888888", "EA", OrderSide.Buy, Ttf.None)
        {
            Price = 0,
            Volume = 0,
            EnterId = "9009",
            PubVolume = 0,
            OrderStatus = status,
            ConditionPrice = ConditionPrice.Limit,
            MatchVolume = 100,
            Type = OrderType.ShortCover,
            OrderState = OrderState.Pending,
            OrderAction = OrderAction.Cover,
            TradingAccountNo = "888888-1"
        };

        // Act
        var actual = order.IsOpenOrder();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SetDeals_Should_Set_Only_MatchedOrderNo()
    {
        // Arrange
        var order = new OnlineOrder(123, "888888", "EA", OrderSide.Buy, Ttf.None)
        {
            Price = 0,
            Volume = 0,
            EnterId = "9009",
            PubVolume = 0,
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            MatchVolume = 100,
            Type = OrderType.ShortCover,
            OrderState = OrderState.Pending,
            OrderAction = OrderAction.Cover,
            TradingAccountNo = "888888-1"
        };
        var deals = new List<Deal>()
        {
            new(order.OrderNo, 456)
            {
                DealVolume = 100,
                DealPrice = 25,
                SumComm = 0,
                SumVat = 0,
                SumTradingFee = 0,
                SumClearingFee = 0
            },
            new(88888, 456)
            {
                DealVolume = 200,
                DealPrice = 15,
                SumComm = 0,
                SumVat = 0,
                SumTradingFee = 0,
                SumClearingFee = 0
            },
        };

        // Act
        order.SetDeals(deals);

        // Assert
        Assert.Single(order.Deals!.ToArray());
        Assert.Equal(deals.First(), order.Deals!.First());
    }

    [Fact]
    public void AvgMatchedPrice_Should_Return_Expected()
    {
        // Arrange
        var order = new OnlineOrder(123, "888888", "EA", OrderSide.Buy, Ttf.None)
        {
            Price = 0,
            Volume = 0,
            EnterId = "9009",
            PubVolume = 0,
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            MatchVolume = 100,
            Type = OrderType.ShortCover,
            OrderState = OrderState.Pending,
            OrderAction = OrderAction.Cover,
            TradingAccountNo = "888888-1"
        };
        var deals = new List<Deal>()
        {
            new(order.OrderNo, 456)
            {
                DealVolume = 100,
                DealPrice = 25,
                SumComm = 0,
                SumVat = 0,
                SumTradingFee = 0,
                SumClearingFee = 0
            },
            new(order.OrderNo, 456)
            {
                DealVolume = 200,
                DealPrice = 15,
                SumComm = 0,
                SumVat = 0,
                SumTradingFee = 0,
                SumClearingFee = 0
            },
        };
        order.SetDeals(deals);

        // Act
        var actual = order.AvgMatchedPrice;

        // Assert
        Assert.Equal(18.3333m, decimal.Round(actual, 4));
    }

    [Fact]
    public void AvgMatchedPrice_Should_Return_Zero_When_Deals_Is_Null()
    {
        // Arrange
        var order = new OnlineOrder(123, "888888", "EA", OrderSide.Buy, Ttf.None)
        {
            Price = 0,
            Volume = 0,
            EnterId = "9009",
            PubVolume = 0,
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            MatchVolume = 100,
            Type = OrderType.ShortCover,
            OrderState = OrderState.Pending,
            OrderAction = OrderAction.Cover,
            TradingAccountNo = "888888-1"
        };

        // Act
        var actual = order.AvgMatchedPrice;

        // Assert
        Assert.Equal(0, decimal.Round(actual, 4));
    }

    [Fact]
    public void AvgMatchedPrice_Should_Return_Zero_When_Deals_Is_Empty()
    {
        // Arrange
        var order = new OnlineOrder(123, "888888", "EA", OrderSide.Buy, Ttf.None)
        {
            Price = 0,
            Volume = 0,
            EnterId = "9009",
            PubVolume = 0,
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            MatchVolume = 100,
            Type = OrderType.ShortCover,
            OrderState = OrderState.Pending,
            OrderAction = OrderAction.Cover,
            TradingAccountNo = "888888-1"
        };
        order.SetDeals(new List<Deal>());

        // Act
        var actual = order.AvgMatchedPrice;

        // Assert
        Assert.Equal(0, decimal.Round(actual, 4));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(-1000)]
    public void AvgMatchedPrice_Should_Return_Zero_When_MatchVolumeBelowEqual_Zero(int matchVolume)
    {
        // Arrange
        var order = new OnlineOrder(123, "888888", "EA", OrderSide.Buy, Ttf.None)
        {
            Price = 0,
            Volume = 0,
            EnterId = "9009",
            PubVolume = 0,
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            MatchVolume = matchVolume,
            Type = OrderType.ShortCover,
            OrderState = OrderState.Pending,
            OrderAction = OrderAction.Cover,
            TradingAccountNo = "888888-1"
        };
        var deals = new List<Deal>()
        {
            new(order.OrderNo, 456)
            {
                DealVolume = 100,
                DealPrice = 25,
                SumComm = 0,
                SumVat = 0,
                SumTradingFee = 0,
                SumClearingFee = 0
            },
            new(order.OrderNo, 456)
            {
                DealVolume = 200,
                DealPrice = 15,
                SumComm = 0,
                SumVat = 0,
                SumTradingFee = 0,
                SumClearingFee = 0
            },
        };
        order.SetDeals(deals);

        // Act
        var actual = order.AvgMatchedPrice;

        // Assert
        Assert.Equal(0, decimal.Round(actual, 4));
    }

    [Theory]
    [InlineData("NSL-R", null, "NSL-R")]
    [InlineData("NSL-R", Ttf.None, "NSL-R")]
    [InlineData("NSL-R", Ttf.TrustFund, "NSL-R")]
    [InlineData("NSL-R", Ttf.Nvdr, "NSL")]
    [InlineData("NSL-L", null, "NSL-L")]
    [InlineData("NSL-L", Ttf.None, "NSL-L")]
    [InlineData("NSL-L", Ttf.TrustFund, "NSL-L")]
    [InlineData("NSL-L", Ttf.Nvdr, "NSL-L")]
    public void SecSymbol_Should_Return_Expected(string symbol, Ttf? ttf, string expected)
    {
        // Arrange
        var order = new OnlineOrder(123, "888888", symbol, OrderSide.Buy, ttf)
        {
            Price = 0,
            Volume = 0,
            EnterId = "9009",
            PubVolume = 0,
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            MatchVolume = 100,
            Type = OrderType.ShortCover,
            OrderState = OrderState.Pending,
            OrderAction = OrderAction.Cover,
            TradingAccountNo = "888888-1"
        };

        // Act
        var actual = order.SecSymbol;

        // Assert
        Assert.Equal(expected, actual);
    }
}
