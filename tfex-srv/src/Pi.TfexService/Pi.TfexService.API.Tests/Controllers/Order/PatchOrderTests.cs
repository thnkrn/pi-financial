using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.TfexService.API.Controllers;
using Pi.TfexService.API.Models.Order;
using Pi.TfexService.Application.Commands.Order;
using Pi.TfexService.Application.Models;
namespace Pi.TfexService.API.Tests.Controllers.Order;

public class PatchOrderTests : BaseOrderControllerTests
{

    private readonly Mock<IRequestClient<SetTradePatchOrderRequest>> _mockRequestClient = new();
    private readonly Mock<Response<SetTradePatchOrderSuccess>> _mockSuccessResponse = new();

    [Fact]
    public async void PatchOrder_Should_Error_When_AccountCodeInvalid()
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

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, null)]
    [InlineData(null, 0)]
    [InlineData(null, null)]
    public async void PatchOrder_Update_Should_Returns_BadRequest_When_Request_Invalid(object? priceObj, int? volume)
    {
        // arrange
        var price = priceObj != null ? Convert.ToDecimal(priceObj) : (decimal?)null;
        const string userId = "userId";
        const string accountCode = "AccountCode";
        const long orderNo = 12345;
        var request = new PatchOrderRequest(PatchOrderType.Update, price, volume);

        // act
        var result = await OrderController.PatchOrder(userId, accountCode, orderNo, request);

        // asserts
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(400, problemDetails.Status);
    }

    [Fact]
    public async void PatchOrder_Cancel_Should_Return_True()
    {
        // arrange
        const string userId = "userId";
        const string accountCode = "AccountCode";
        const long orderNo = 12345;
        var request = new PatchOrderRequest(PatchOrderType.Cancel);
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddHandler<SetTradePatchOrderRequest>(async cxt =>
                {
                    await cxt.RespondAsync(new SetTradePatchOrderSuccess());
                });
            })
            .BuildServiceProvider(true);
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();
        var controller = new OrderController(harness.Bus, SetTradeOrderQueriesMock.Object, ItOrderQueriesMock.Object);

        // act
        var response = await controller.PatchOrder(userId, accountCode, orderNo, request);

        // asserts
        Assert.True(await harness.Published.Any<SetTradePatchOrderRequest>());
        var objectResult = Assert.IsAssignableFrom<ObjectResult>(response);
        Assert.Equal(200, objectResult.StatusCode);
    }

    [Theory]
    [InlineData(100, null)]
    [InlineData(null, 100)]
    [InlineData(100, 100)]
    public async void PatchOrder_Update_Should_Return_True(object? priceObj, int? volume)
    {
        // arrange
        var price = priceObj != null ? Convert.ToDecimal(priceObj) : (decimal?)null;
        const string userId = "userId";
        const string accountCode = "AccountCode";
        const long orderNo = 12345;
        var request = new PatchOrderRequest(PatchOrderType.Update, price, volume);
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddHandler<SetTradePatchOrderRequest>(async cxt =>
                {
                    await cxt.RespondAsync(new SetTradePatchOrderSuccess());
                });
            })
            .BuildServiceProvider(true);
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();
        var controller = new OrderController(harness.Bus, SetTradeOrderQueriesMock.Object, ItOrderQueriesMock.Object);

        // act
        var response = await controller.PatchOrder(userId, accountCode, orderNo, request);

        // asserts
        Assert.True(await harness.Published.Any<SetTradePatchOrderRequest>());
        var objectResult = Assert.IsAssignableFrom<ObjectResult>(response);
        Assert.Equal(200, objectResult.StatusCode);
    }

}
