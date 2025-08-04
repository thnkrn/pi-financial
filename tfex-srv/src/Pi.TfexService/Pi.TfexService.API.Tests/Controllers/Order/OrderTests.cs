using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.Common.Http;
using Pi.TfexService.API.Controllers;
using Pi.TfexService.API.Models.Order;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Domain.Exceptions;

namespace Pi.TfexService.API.Tests.Controllers.Order;

public class OrderTests : BaseOrderControllerTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async void GetOrders_Should_Return_List_Of_SetTradeOrder(int paginationCase)
    {
        // arrange
        const string accountCode = "AccountCode";
        const string userId = "91255163-175e-4b4f-b4fc-34ee0c2838d8";

        var pagination = paginationCase switch
        {
            1 => new OrderPagination
            {
                Page = 1,
                PageSize = 2,
                OrderBy = SetTradePaginationOrderBy.OrderNo,
                OrderDir = SetTradePaginationOrderDir.Asc
            },
            2 => new OrderPagination
            {
                Page = 1,
                PageSize = 2,
            },
            _ => new OrderPagination()
        };
        var orders = new List<SetTradeOrder>
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
                PriceDigit: 2),
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
                PriceDigit: 2)
        };

        SetTradeOrderQueriesMock
            .Setup(m => m.GetOrders(accountCode, pagination.Page, pagination.PageSize, pagination.GetSort(), null, null, null, default))
            .ReturnsAsync(new PaginatedSetTradeOrder(orders, false));

        // act
        var result = await OrderController.GetOrders(userId, accountCode, pagination);

        // asserts
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<ApiResponse<PaginatedSetTradeOrderDto>>(okResult.Value);
    }

    [Theory]
    [InlineData(1, "not found", 404)]
    [InlineData(2, "auth", 401)]
    [InlineData(3, "internal", 500)]
    public async void GetOrders_Should_Throw_Error(int errorCase, string errorMessage, int statusCode)
    {
        // arrange
        const string accountCode = "AccountCode";
        const string userId = "91255163-175e-4b4f-b4fc-34ee0c2838d8";
        var pagination = new OrderPagination
        {
            Page = 1,
            PageSize = 2,
            OrderBy = SetTradePaginationOrderBy.OrderNo,
            OrderDir = SetTradePaginationOrderDir.Asc
        };

        Exception exception = errorCase switch
        {
            1 => new SetTradeNotFoundException(errorMessage),
            2 => new SetTradeAuthException(errorMessage),
            _ => new Exception(errorMessage)
        };

        SetTradeOrderQueriesMock
            .Setup(m => m.GetOrders(accountCode, pagination.Page, pagination.PageSize, pagination.GetSort(), null, null, null, default))
            .ThrowsAsync(exception);

        // act
        var result = await OrderController.GetOrders(userId, accountCode, pagination);

        // asserts
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(statusCode, problemDetails.Status);
        Assert.Equal(errorMessage, problemDetails.Detail);
    }

    [Theory]
    [InlineData(0, 1, SetTradePaginationOrderBy.OrderNo, SetTradePaginationOrderDir.Asc, $"Invalid Page: 0 (Parameter 'Page')")]
    [InlineData(1, 0, SetTradePaginationOrderBy.OrderNo, SetTradePaginationOrderDir.Asc, $"Invalid PageSize: 0 (Parameter 'PageSize')")]
    [InlineData(1, 1, (SetTradePaginationOrderBy)999, SetTradePaginationOrderDir.Asc, $"Invalid OrderBy: 999 (Parameter 'OrderBy')")]
    [InlineData(1, 1, SetTradePaginationOrderBy.OrderNo, (SetTradePaginationOrderDir)999, $"Invalid OrderDir: 999 (Parameter 'OrderDir')")]
    public async void GetOrders_Pagination_Error(
        int page,
        int pageSize,
        SetTradePaginationOrderBy orderBy,
        SetTradePaginationOrderDir orderDir,
        string exceptionMessage)
    {
        // arrange
        const string accountCode = "AccountCode";
        const string userId = "91255163-175e-4b4f-b4fc-34ee0c2838d8";
        var pagination = new OrderPagination
        {
            Page = page,
            PageSize = pageSize,
            OrderBy = orderBy,
            OrderDir = orderDir
        };

        // act
        var result = await OrderController.GetOrders(userId, accountCode, pagination);

        // asserts
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(exceptionMessage, problemDetails.Detail);
    }

    [Fact]
    public async void GetActiveOrders_Should_Success()
    {
        // arrange
        const string accountCode = "AccountCode";
        const string userId = "91255163-175e-4b4f-b4fc-34ee0c2838d8";
        const string sid = "sid";

        var orderList = new List<SetTradeOrder>
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
                MatchQty: 50,
                CancelQty: 0,
                Status: "Status",
                ShowStatus: "ShowStatus",
                StatusMeaning: "StatusMeaning",
                CanCancel: true,
                CanChange: true,
                PriceDigit: 2),
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
                MatchQty: 20,
                CancelQty: 50,
                Status: "Status",
                ShowStatus: "ShowStatus",
                StatusMeaning: "StatusMeaning",
                CanCancel: true,
                CanChange: true,
                PriceDigit: 2)
        };

        SetTradeOrderQueriesMock
            .Setup(m => m.GetActiveOrders(accountCode, sid, "orderNo:desc", new CancellationToken()))
            .ReturnsAsync(orderList);

        // act
        var result = await OrderController.GetActiveOrders(userId, sid, accountCode);

        // asserts
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<ActiveOrderDto>>(okResult.Value);
        Assert.Equal(50, response.Data.Orders[0].NotMatchQty);
        Assert.Equal(30, response.Data.Orders[1].NotMatchQty);
        Assert.Equal(2, response.Data.Total);
    }

    [Fact]
    public async void GetActiveOrder_Should_Error()
    {
        // arrange
        const string accountCode = "AccountCode";
        const string userId = "91255163-175e-4b4f-b4fc-34ee0c2838d8";
        const string sid = "sid";

        SetTradeOrderQueriesMock
            .Setup(m => m.GetActiveOrders(accountCode, sid, "orderNo:desc", new CancellationToken()))
            .ThrowsAsync(new Exception("MOCK FAILED"));

        // act
        var result = await OrderController.GetActiveOrders(userId, sid, accountCode);

        // asserts
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(500, problemDetails.Status);
        Assert.Equal("MOCK FAILED", problemDetails.Detail);
    }

    [Fact]
    public async void GetOrder_Should_Return_SetTradeOrder()
    {
        // arrange
        const string accountCode = "AccountCode";
        const string orderNo = "123";
        const string userId = "91255163-175e-4b4f-b4fc-34ee0c2838d8";

        var order = new SetTradeOrder(
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
            PriceDigit: 2);

        SetTradeOrderQueriesMock
            .Setup(m => m.GetOrder(accountCode, orderNo, new CancellationToken()))
            .ReturnsAsync(order);

        // act
        var result = await OrderController.GetOrder(userId, accountCode, orderNo);

        // asserts
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<ApiResponse<SetTradeOrderDto>>(okResult.Value);
    }

    [Theory]
    [InlineData(1, "not found", 404)]
    [InlineData(2, "auth", 401)]
    [InlineData(3, "internal", 500)]
    public async void GetOrder_Should_Throw_Error(int errorCase, string errorMessage, int statusCode)
    {
        // arrange
        const string accountCode = "AccountCode";
        const string orderNo = "123";
        const string userId = "91255163-175e-4b4f-b4fc-34ee0c2838d8";

        Exception exception = errorCase switch
        {
            1 => new SetTradeNotFoundException(errorMessage),
            2 => new SetTradeAuthException(errorMessage),
            _ => new Exception(errorMessage)
        };

        SetTradeOrderQueriesMock
            .Setup(m => m.GetOrder(accountCode, orderNo, new CancellationToken()))
            .ThrowsAsync(exception);

        // act
        var result = await OrderController.GetOrder(userId, accountCode, orderNo);

        // asserts
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(statusCode, problemDetails.Status);
        Assert.Equal(errorMessage, problemDetails.Detail);
    }

    [Fact]
    public async void PlaceOrder_Should_Error_When_AccountCodeInvalid()
    {
        // arrange
        var userId = "userId";
        var accountCode = "02";
        var request = new PlaceOrderRequest(
            Series: "Symbol",
            Side: Side.Long,
            Position: Position.Open,
            PriceType: PriceType.Limit,
            Price: 100,
            Volume: 100,
            IcebergVol: 0,
            Validity.Day,
            ValidityDateCondition: null,
            StopCondition: null,
            StopSymbol: null,
            StopPrice: null,
            TriggerSession: null,
            BypassWarning: false
        );

        var result = await OrderController.PlaceOrder(userId, accountCode, request);

        // asserts
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(400, problemDetails.Status);

    }

    [Fact]
    public async void PlaceOrder_Should_Return_SetTradePlaceOrderDto()
    {
        // arrange
        const string userId = "userId";
        const string accountCode = "AccountCode";
        const long orderNo = 12345;
        var request = new PlaceOrderRequest(
            Series: "Symbol",
            Side: Side.Long,
            Position: Position.Open,
            PriceType: PriceType.Limit,
            Price: 100,
            Volume: 100,
            IcebergVol: 0,
            Validity.Day,
            ValidityDateCondition: null,
            StopCondition: null,
            StopSymbol: null,
            StopPrice: null,
            TriggerSession: null,
            BypassWarning: false
        );

        //Arrange
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddHandler<SetTradePlaceOrderRequest>(async cxt =>
                {
                    await cxt.RespondAsync(new SetTradePlaceOrderResponse(orderNo));
                });
            })
            .BuildServiceProvider(true);
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();
        var controller = new OrderController(harness.Bus, SetTradeOrderQueriesMock.Object, ItOrderQueriesMock.Object);

        // Act
        var response = await controller.PlaceOrder(userId, accountCode, request);


        // Asserts
        Assert.True(await harness.Published.Any<SetTradePlaceOrderRequest>());
        var objectResult = Assert.IsAssignableFrom<ObjectResult>(response);
        Assert.Equal(200, objectResult.StatusCode);
    }

    [Theory]
    [MemberData(nameof(PlaceOrderValidateErrorData))]
    public async void PlaceOrder_Validation_Error(string errorMessage, PlaceOrderRequest request)
    {
        // arrange
        var userId = "userId";
        var accountCode = "AccountCode";

        // act
        var result = await OrderController.PlaceOrder(userId, accountCode, request);

        // asserts
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(errorMessage, problemDetails.Detail);
        Assert.Equal(400, problemDetails.Status);
    }

    public static IEnumerable<object[]> PlaceOrderValidateErrorData =>
        new List<object[]>
        {
            new object[]
            {
                "Series is required (Parameter 'Series')",
                new PlaceOrderRequest(
                    Series: "",
                    Side: Side.Long,
                    Position: Position.Open,
                    PriceType: PriceType.Limit,
                    Price: 100,
                    Volume: 100,
                    IcebergVol: 0,
                    Validity.Day,
                    ValidityDateCondition: null,
                    StopCondition: null,
                    StopSymbol: null,
                    StopPrice: null,
                    TriggerSession: null,
                    BypassWarning: false
                )
            },
            new object[]
            {
                "Invalid Side: 999 (Parameter 'Side')",
                new PlaceOrderRequest(
                    Series: "Symbol",
                    Side: (Side)999,
                    Position: Position.Open,
                    PriceType: PriceType.Limit,
                    Price: 100,
                    Volume: 100,
                    IcebergVol: 0,
                    Validity.Day,
                    ValidityDateCondition: null,
                    StopCondition: null,
                    StopSymbol: null,
                    StopPrice: null,
                    TriggerSession: null,
                    BypassWarning: false
                )
            },
            new object[]
            {
                "Invalid Position: 999 (Parameter 'Position')",
                new PlaceOrderRequest(
                    Series: "Symbol",
                    Side: Side.Long,
                    Position: (Position)999,
                    PriceType: PriceType.Limit,
                    Price: 100,
                    Volume: 100,
                    IcebergVol: 0,
                    Validity.Day,
                    ValidityDateCondition: null,
                    StopCondition: null,
                    StopSymbol: null,
                    StopPrice: null,
                    TriggerSession: null,
                    BypassWarning: false
                )
            },
            new object[]
            {
                "Invalid PriceType: 999 (Parameter 'PriceType')",
                new PlaceOrderRequest(
                    Series: "Symbol",
                    Side: Side.Long,
                    Position: Position.Open,
                    PriceType: (PriceType)999,
                    Price: 100,
                    Volume: 100,
                    IcebergVol: 0,
                    Validity.Day,
                    ValidityDateCondition: null,
                    StopCondition: null,
                    StopSymbol: null,
                    StopPrice: null,
                    TriggerSession: null,
                    BypassWarning: false
                )
            },
            new object[]
            {
                "Price must greater than 0 when PriceType is Limit",
                new PlaceOrderRequest(
                    Series: "Symbol",
                    Side: Side.Long,
                    Position: Position.Open,
                    PriceType: PriceType.Limit,
                    Price: 0,
                    Volume: 100,
                    IcebergVol: 0,
                    Validity.Day,
                    ValidityDateCondition: null,
                    StopCondition: null,
                    StopSymbol: null,
                    StopPrice: null,
                    TriggerSession: null,
                    BypassWarning: false
                )
            },
            new object[]
            {
                "Invalid Volume: 0 (Parameter 'Volume')",
                new PlaceOrderRequest(
                    Series: "Symbol",
                    Side: Side.Long,
                    Position: Position.Open,
                    PriceType: PriceType.Ato,
                    Price: 0,
                    Volume: 0,
                    IcebergVol: 0,
                    Validity.Day,
                    ValidityDateCondition: null,
                    StopCondition: null,
                    StopSymbol: null,
                    StopPrice: null,
                    TriggerSession: null,
                    BypassWarning: false
                )
            },
            new object[]
            {
                "Invalid IcebergVol: -100 (Parameter 'IcebergVol')",
                new PlaceOrderRequest(
                    Series: "Symbol",
                    Side: Side.Long,
                    Position: Position.Open,
                    PriceType: PriceType.Ato,
                    Price: 0,
                    Volume: 100,
                    IcebergVol: -100,
                    Validity.Day,
                    ValidityDateCondition: null,
                    StopCondition: null,
                    StopSymbol: null,
                    StopPrice: null,
                    TriggerSession: null,
                    BypassWarning: false
                ),

            },
            new object[]
            {
                "Invalid ValidityType: 999 (Parameter 'ValidityType')",
                new PlaceOrderRequest(
                    Series: "Symbol",
                    Side: Side.Long,
                    Position: Position.Open,
                    PriceType: PriceType.Ato,
                    Price: 0,
                    Volume: 100,
                    IcebergVol: 0,
                    (Validity)999,
                    ValidityDateCondition: null,
                    StopCondition: null,
                    StopSymbol: null,
                    StopPrice: null,
                    TriggerSession: null,
                    BypassWarning: false
                )
            },
            new object[]
            {
                $"Invalid ValidityDateCondition:  (Parameter 'ValidityDateCondition')",
                new PlaceOrderRequest(
                    Series: "Symbol",
                    Side: Side.Long,
                    Position: Position.Open,
                    PriceType: PriceType.Ato,
                    Price: 0,
                    Volume: 100,
                    IcebergVol: 0,
                    Validity.Date,
                    ValidityDateCondition: null,
                    StopCondition: null,
                    StopSymbol: null,
                    StopPrice: null,
                    TriggerSession: null,
                    BypassWarning: false
                )
            },
            new object[]
            {
                "Invalid StopCondition: 999 (Parameter 'StopCondition')",
                new PlaceOrderRequest(
                    Series: "Symbol",
                    Side: Side.Long,
                    Position: Position.Open,
                    PriceType: PriceType.Ato,
                    Price: 0,
                    Volume: 100,
                    IcebergVol: 0,
                    Validity.Day,
                    ValidityDateCondition: null,
                    StopCondition: (TriggerCondition)999,
                    StopSymbol: null,
                    StopPrice: null,
                    TriggerSession: null,
                    BypassWarning: false
                )
            },
            new object[]
            {
                "StopSymbol is required (Parameter 'StopSymbol')",
                new PlaceOrderRequest(
                    Series: "Symbol",
                    Side: Side.Long,
                    Position: Position.Open,
                    PriceType: PriceType.Ato,
                    Price: 0,
                    Volume: 100,
                    IcebergVol: 0,
                    Validity.Day,
                    ValidityDateCondition: null,
                    StopCondition: TriggerCondition.AskOrHigher,
                    StopSymbol: null,
                    StopPrice: null,
                    TriggerSession: null,
                    BypassWarning: false
                )
            },
            new object[]
            {
                "Invalid StopPrice:  (Parameter 'StopPrice')",
                new PlaceOrderRequest(
                    Series: "Symbol",
                    Side: Side.Long,
                    Position: Position.Open,
                    PriceType: PriceType.Ato,
                    Price: 0,
                    Volume: 100,
                    IcebergVol: 0,
                    Validity.Day,
                    ValidityDateCondition: null,
                    StopCondition: TriggerCondition.AskOrHigher,
                    StopSymbol: "StopSymbol",
                    StopPrice: null,
                    TriggerSession: null,
                    BypassWarning: false
                )
            },
            new object[]
            {
                "Invalid TriggerSession: 999 (Parameter 'TriggerSession')",
                new PlaceOrderRequest(
                    Series: "Symbol",
                    Side: Side.Long,
                    Position: Position.Open,
                    PriceType: PriceType.Ato,
                    Price: 0,
                    Volume: 100,
                    IcebergVol: 0,
                    Validity.Day,
                    ValidityDateCondition: null,
                    StopCondition: TriggerCondition.Session,
                    StopSymbol: "StopSymbol",
                    StopPrice: 100,
                    TriggerSession: (TriggerSession)999,
                    BypassWarning: false
                )
            }
        };
}
