using Microsoft.Extensions.Logging;
using Moq;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Queries.Market;
using Pi.TfexService.Application.Queries.Order;
using Pi.TfexService.Application.Services.SetTrade;

namespace Pi.TfexService.Application.Tests.Queries.Order;

public class SetTradeOrderQueriesTests
{
    private readonly Mock<ISetTradeService> _setTradeService;
    private readonly SetTradeOrderQueries _setTradeOrderQueries;
    private readonly Mock<IMarketDataQueries> _marketDataQueries;

    public SetTradeOrderQueriesTests()
    {
        Mock<ILogger<SetTradeOrderQueries>> logger = new();
        _setTradeService = new Mock<ISetTradeService>();
        _marketDataQueries = new Mock<IMarketDataQueries>();
        _setTradeOrderQueries = new SetTradeOrderQueries(_setTradeService.Object, _marketDataQueries.Object, logger.Object);
    }

    [Fact]
    public async void GetOrders_ShouldReturnSetTradeOrderDtoList()
    {

        var orders = new List<SetTradeOrder>()
        {
            new(
                OrderNo: 1,
                TfxOrderNo: "1",
                AccountNo: "AccountNo",
                EntryTime: DateTime.Parse("2024-06-10T00:00:00Z"),
                CancelTime: DateTime.Parse("2024-06-10T00:00:00Z"),
                Symbol: "Symbol",
                Side: Side.Long,
                Position: Position.Open,
                PriceType: PriceType.Limit,
                Price: 100,
                Qty: 100,
                MatchQty: 100,
                CancelQty: 0,
                Status: "Status",
                ShowStatus: "ShowStatus",
                StatusMeaning: "StatusMeaning",
                CanCancel: true,
                CanChange: true,
                PriceDigit: 2
            ),
            new(
                OrderNo: 2,
                TfxOrderNo: "2",
                AccountNo: "AccountNo",
                EntryTime: DateTime.Parse("2024-06-10T00:00:00Z"),
                CancelTime: DateTime.Parse("2024-06-10T00:00:00Z"),
                Symbol: "Symbol",
                Side: Side.Long,
                Position: Position.Open,
                PriceType: PriceType.Limit,
                Price: 100,
                Qty: 100,
                MatchQty: 100,
                CancelQty: 0,
                Status: "Status",
                ShowStatus: "ShowStatus",
                StatusMeaning: "StatusMeaning",
                CanCancel: true,
                CanChange: true,
                PriceDigit: 4
            )
        };

        _setTradeService.Setup(s => s.GetOrders(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<Side?>(),
                It.IsAny<DateOnly?>(),
                It.IsAny<DateOnly?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedSetTradeOrder(orders, false));

        var result = await _setTradeOrderQueries.GetOrders("123456789", 1, 2, "orderNo:desc");

        Assert.NotNull(result);
        Assert.Equal(2, result.Orders.Count);
    }

    [Fact]
    public async void GetActiveOrders_Should_Success()
    {
        var orders = new List<SetTradeOrder>()
        {
            new(
                OrderNo: 1,
                TfxOrderNo: "1",
                AccountNo: "AccountNo",
                EntryTime: DateTime.Parse("2024-06-10T00:00:00Z"),
                CancelTime: DateTime.Parse("2024-06-10T00:00:00Z"),
                Symbol: "Symbol 1",
                Side: Side.Long,
                Position: Position.Open,
                PriceType: PriceType.Limit,
                Price: 100,
                Qty: 100,
                MatchQty: 100,
                CancelQty: 0,
                Status: "Status",
                ShowStatus: "ShowStatus",
                StatusMeaning: "StatusMeaning",
                CanCancel: true,
                CanChange: true,
                PriceDigit: 2
            ),
            new(
                OrderNo: 2,
                TfxOrderNo: "2",
                AccountNo: "AccountNo",
                EntryTime: DateTime.Parse("2024-06-10T00:00:00Z"),
                CancelTime: DateTime.Parse("2024-06-10T00:00:00Z"),
                Symbol: "Symbol 2",
                Side: Side.Long,
                Position: Position.Open,
                PriceType: PriceType.Limit,
                Price: 100,
                Qty: 100,
                MatchQty: 100,
                CancelQty: 0,
                Status: "Status",
                ShowStatus: "ShowStatus",
                StatusMeaning: "StatusMeaning",
                CanCancel: true,
                CanChange: true,
                PriceDigit: 4
            ),
            new(
                OrderNo: 3,
                TfxOrderNo: "3",
                AccountNo: "AccountNo",
                EntryTime: DateTime.Parse("2024-06-10T00:00:00Z"),
                CancelTime: DateTime.Parse("2024-06-10T00:00:00Z"),
                Symbol: null,
                Side: Side.Long,
                Position: Position.Open,
                PriceType: PriceType.Limit,
                Price: 100,
                Qty: 100,
                MatchQty: 100,
                CancelQty: 0,
                Status: "Status",
                ShowStatus: "ShowStatus",
                StatusMeaning: "StatusMeaning",
                CanCancel: true,
                CanChange: true,
                PriceDigit: 4
            )
        };

        _setTradeService.Setup(s => s.GetActiveOrders(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SetTradeOrder>(orders));

        _marketDataQueries.Setup(s => s.GetMarketData(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, MarketData>
            {
                { "Symbol 1", new MarketData(Series: "Symbol 1", InstrumentCategory: "instrumentCategory 1", Logo: "logo 1") },
                { "Symbol 2", new MarketData(Series: "Symbol 2", InstrumentCategory: "instrumentCategory 2", Logo: "logo 2") },
                { "Symbol 3", new MarketData(Series: "Symbol 3", InstrumentCategory: "instrumentCategory 3", Logo: "logo 3") }
            });


        var result = await _setTradeOrderQueries.GetActiveOrders("123456789", "sid", "orderNo:desc");

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("logo 1", result[0].Logo);
        Assert.Equal("logo 2", result[1].Logo);
        Assert.Null(result[2].Logo);
    }

    [Fact]
    public async Task GetOrder_Should_Error_When_Invalid_AccountNo()
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _setTradeOrderQueries.GetOrder("123456789", "aabbcc", new CancellationToken()));
        Assert.Equal("Invalid Page: aabbcc (Parameter 'orderNo')", exception.Message);
    }

    [Fact]
    public async Task GetOrder_Should_Return_Order()
    {
        _setTradeService.Setup(s => s.GetOrderByNo(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SetTradeOrder(
                OrderNo: 2,
                TfxOrderNo: "2",
                AccountNo: "AccountNo",
                EntryTime: DateTime.Parse("2024-06-10T00:00:00Z"),
                CancelTime: DateTime.Parse("2024-06-10T00:00:00Z"),
                Symbol: "Symbol",
                Side: Side.Long,
                Position: Position.Open,
                PriceType: PriceType.Limit,
                Price: 100,
                Qty: 100,
                MatchQty: 100,
                CancelQty: 0,
                Status: "Status",
                ShowStatus: "ShowStatus",
                StatusMeaning: "StatusMeaning",
                CanCancel: true,
                CanChange: true,
                PriceDigit: 4));
        var result = await _setTradeOrderQueries.GetOrder("123456789", "123456789", new CancellationToken());

        Assert.NotNull(result);
        Assert.Equal(2, result.OrderNo);
    }
}