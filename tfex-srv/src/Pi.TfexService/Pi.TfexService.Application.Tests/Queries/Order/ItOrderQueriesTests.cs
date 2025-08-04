using Microsoft.Extensions.Logging;
using Moq;
using Pi.Client.ItBackOffice.Model;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Queries.Order;
using Pi.TfexService.Application.Services.It;

namespace Pi.TfexService.Application.Tests.Queries.Order;

public class ItOrderQueriesTests
{
    private readonly ItOrderTradeQueries _itOrderQueries;
    private readonly Mock<IItService> _itService;

    public ItOrderQueriesTests()
    {
        Mock<ILogger<ItOrderTradeQueries>> logger = new();
        _itService = new Mock<IItService>();
        _itOrderQueries = new ItOrderTradeQueries(_itService.Object, logger.Object);
    }

    [Theory]
    [InlineData("2024-01-01", "2024-09-30", "The date range must not exceed 90 days")]
    [InlineData("2024-09-02", "2024-09-01", "Date from must less than date to")]
    public async void GetOrderTrade_ShouldThrowExceptionWhenInvalidDate(string dateFrom, string dateTo,
        string errorMessage)
    {
        // Arrange
        var requestModel = new GetTradeDetailRequestModel(
            "1234567890",
            1,
            2,
            DateOnly.Parse(dateFrom),
            DateOnly.Parse(dateTo),
            Side.Long,
            Position.Auto);

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _itOrderQueries.GetOrderTrade(
                requestModel,
                CancellationToken.None));

        // Assert
        Assert.Equal(errorMessage, exception.Message);
    }

    [Fact]
    public async void GetOrderTrade_ShouldThrowExceptionWhenInvalidAccountNo()
    {
        // Arrange
        var requestModel = new GetTradeDetailRequestModel(
            "1234567890",
            1,
            2,
            DateOnly.Parse("2024-09-01"),
            DateOnly.Parse("2024-09-30"),
            Side.Long,
            Position.Auto);

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _itOrderQueries.GetOrderTrade(
                requestModel,
                CancellationToken.None));

        // Assert
        Assert.Equal("The account code must contain format xxxxxxx0 or xxxxxxx-0", exception.Message);
    }

    [Theory]
    [InlineData("08002800", "2024-09-01", "2024-09-01")]
    [InlineData("0800280-0", "2024-09-01", "2024-09-01")]
    [InlineData("08002800", "2024-09-01", "2024-09-30")]
    [InlineData("0800280-0", "2024-09-01", "2024-09-30")]
    public async void GetOrderTrade_ShouldReturnItTradeDetail(string accountNo, string dateFrom, string dateTo)
    {
        // Arrange
        var requestModel = new GetTradeDetailRequestModel(
            accountNo,
            1,
            2,
            DateOnly.Parse(dateFrom),
            DateOnly.Parse(dateTo),
            Side.Long,
            Position.Open);

        _itService.Setup(s => s.GetTradeDetail(
                It.IsAny<GetTradeDetailRequestModel>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedOrderTrade(MockTradeDetail(), false));

        // Act
        var result = await _itOrderQueries.GetOrderTrade(requestModel, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    private static List<TradeDetail> MockTradeDetail()
    {
        return
        [
            new TradeDetail(shareCode: "AAAZ24",
                refType: "BU",
                buysellsorter: "C",
                price: 12.34,
                unit: 5,
                amt: 10,
                commSub: 1,
                vatSub: 2,
                refDate: DateTime.Now,
                confirmTime: new TradeDetailConfirmTime(hours: 10, minutes: 5, seconds: 10)
            ),
            new TradeDetail(shareCode: "BBBZ24",
                refType: "BH",
                buysellsorter: "O",
                price: 23.45,
                unit: 6,
                amt: 11,
                commSub: 1.5,
                vatSub: 2.5,
                refDate: DateTime.Now,
                confirmTime: new TradeDetailConfirmTime(hours: 11, minutes: 10, seconds: 50)
            ),
            new TradeDetail(shareCode: "CCCZ24",
                refType: "SE",
                buysellsorter: "C",
                price: 85.44,
                unit: 8,
                amt: 90,
                commSub: 2,
                vatSub: 3,
                refDate: DateTime.Now,
                confirmTime: new TradeDetailConfirmTime(hours: 4, minutes: 6, seconds: 10)
            ),
            new TradeDetail(shareCode: "CCCZ24",
                refType: "SH",
                buysellsorter: "O",
                price: 55.42,
                unit: 1,
                amt: 1,
                commSub: 0.1,
                vatSub: 0.1,
                refDate: DateTime.Now,
                confirmTime: new TradeDetailConfirmTime(hours: 5, minutes: 7, seconds: 10)
            )
        ];
    }
}