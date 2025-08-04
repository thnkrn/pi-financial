using Microsoft.AspNetCore.Mvc;
using Moq;
using Pi.Client.ItBackOffice.Model;
using Pi.Common.Http;
using Pi.TfexService.API.Models.Order;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.It;
using Pi.TfexService.Domain.Exceptions;

namespace Pi.TfexService.API.Tests.Controllers.Order;

public class OrderTradeTests : BaseOrderControllerTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async void GetOrderTrade_Should_Return_List_Of_ItOrderTrade(int paginationCase)
    {
        // Arrange
        const string accountCode = "AccountCode";
        const string userId = "userId";

        var pagination = paginationCase switch
        {
            1 => new OrderTradePagination
            {
                DateFrom = DateOnly.Parse("2024-09-01"),
                DateTo = DateOnly.Parse("2024-09-30"),
                Page = 1,
                PageSize = 2,
                OrderBy = ItPaginationOrderBy.TradeDateTime,
                OrderDir = ItPaginationOrderDir.Desc
            },
            2 => new OrderTradePagination
            {
                DateFrom = DateOnly.Parse("2024-09-01"),
                DateTo = DateOnly.Parse("2024-09-30"),
                Page = 1,
                PageSize = 2,
            },
            3 => new OrderTradePagination
            {
                DateFrom = DateOnly.Parse("2024-09-01"),
                DateTo = DateOnly.Parse("2024-09-30"),
            },
            _ => new OrderTradePagination
            {
                DateFrom = default,
                DateTo = default
            }
        };

        ItOrderQueriesMock.Setup(m => m.GetOrderTrade(It.IsAny<GetTradeDetailRequestModel>(), CancellationToken.None))
            .ReturnsAsync(new PaginatedOrderTrade(MockTradeDetail(), false));

        // Act
        var result = await OrderController.GetOrderTrade(userId, accountCode, pagination);

        // Asserts
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<ApiResponse<PaginatedItOrderTradeDto>>(okResult.Value);
    }

    [Theory]
    [InlineData(1, "Data Not Found", 404)]
    [InlineData(2, "Internal Error", 500)]
    [InlineData(3, "Internal Error", 500)]
    public async void GetOrderTrade_Should_Handle_When_Have_Exception(int errorCase, string errorMessage, int statusCode)
    {
        // Arrange
        const string accountCode = "AccountCode";
        const string userId = "userId";
        var pagination = new OrderTradePagination
        {
            DateFrom = DateOnly.Parse("2024-09-01"),
            DateTo = DateOnly.Parse("2024-09-30"),
            Page = 1,
            PageSize = 2,
            OrderBy = ItPaginationOrderBy.TradeDateTime,
            OrderDir = ItPaginationOrderDir.Desc
        };

        var exception = errorCase switch
        {
            1 => new ItNotFoundException(errorMessage),
            2 => new ItApiException(errorMessage),
            _ => new Exception(errorMessage)
        };

        ItOrderQueriesMock.Setup(m => m.GetOrderTrade(It.IsAny<GetTradeDetailRequestModel>(), CancellationToken.None)).ThrowsAsync(exception);

        // Act
        var result = await OrderController.GetOrderTrade(userId, accountCode, pagination);

        // Asserts
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(statusCode, problemDetails.Status);
        Assert.Equal(errorMessage, problemDetails.Detail);
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

    private static List<ItOrderTradeDto> MockData()
    {
        return MockTradeDetail().Select(t => new ItOrderTradeDto(t)).ToList();
    }
}