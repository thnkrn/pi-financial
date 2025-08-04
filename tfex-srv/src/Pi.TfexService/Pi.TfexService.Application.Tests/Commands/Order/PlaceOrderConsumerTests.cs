using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.Common.Features;
using Pi.TfexService.Application.Commands.Order;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Application.Services.UserService;

namespace Pi.TfexService.Application.Tests.Commands.Order;

public class PlaceOrderConsumerTests : ConsumerTest
{

    private readonly Mock<ISetTradeService> _setTradeService = new();
    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<IUserV2Service> _userV2Service = new();
    private readonly Mock<IFeatureService> _featureService = new();
    public PlaceOrderConsumerTests()
    {
        Provider = new ServiceCollection()
            .AddScoped<ISetTradeService>(_ => _setTradeService.Object)
            .AddScoped<IUserService>(_ => _userService.Object)
            .AddScoped<IUserV2Service>(_ => _userV2Service.Object)
            .AddScoped<IFeatureService>(_ => _featureService.Object)
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<PlaceOrderConsumer>();
            })
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void ShouldFailed_Auth()
    {
        // Arrange
        var request = new SetTradePlaceOrderRequest(
            "userid",
            "12345",
            "123456",
            new SetTradePlaceOrderRequest.PlaceOrderInfo(
                "123456",
                Side.Long,
                Position.Auto,
                PriceType.Ato,
                1,
                1,
                0,
                Validity.Day));

        var user = new User(
            new Guid(),
            [""],
            [""],
            "",
            "",
            "",
            "",
            "",
            "");

        _userService.Setup(s => s.GetUserById(request.UserId))
            .ReturnsAsync(user);

        // Act
        var client = Harness.GetRequestClient<SetTradePlaceOrderRequest>();
        var requestFault = await Assert.ThrowsAsync<RequestFaultException>(() =>
            client.GetResponse<SetTradePlaceOrderResponse>(request));

        // Assert
        Assert.True(await Harness.Consumed.Any<SetTradePlaceOrderRequest>());
        Assert.NotNull(requestFault.Fault);
        Assert.NotEmpty(requestFault.Fault.Exceptions);
        Assert.Equal(typeof(UnauthorizedAccessException).FullName, requestFault.Fault.Exceptions.First().ExceptionType);

    }

    [Fact]
    public async void ShouldBeAbleToPlaceOrderSuccessfully()
    {
        // Arrange
        var request = new SetTradePlaceOrderRequest(
            "userid",
            "12345",
            "123456",
            new SetTradePlaceOrderRequest.PlaceOrderInfo(
                "123456",
                Side.Long,
                Position.Auto,
                PriceType.Limit,
                1,
                1,
                0,
                Validity.Day));

        var order = new SetTradeOrder(
            123
            );

        var user = new User(
            new Guid(),
            [""],
            [request.AccountCode],
            "",
            "",
            "",
            "",
            "",
            "");

        _userService.Setup(s => s.GetUserById(request.UserId))
            .ReturnsAsync(user);

        _setTradeService.Setup(s => s.PlaceOrder(
                request.UserId,
                request.CustomerCode,
                request.AccountCode,
                request.OrderInfo,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var client = Harness.GetRequestClient<SetTradePlaceOrderRequest>();
        var response = await client.GetResponse<SetTradePlaceOrderResponse>(request);

        // Assert
        Assert.True(await Harness.Consumed.Any<SetTradePlaceOrderResponse>());
        Assert.Equal(123, response.Message.OrderNo);
    }

    [Fact]
    public async void ShouldBeAbleToPlaceOrder_AndNotify()
    {
        // Arrange
        var request = new SetTradePlaceOrderRequest(
            "userid",
            "12345",
            "123456",
            new SetTradePlaceOrderRequest.PlaceOrderInfo(
                "123456",
                Side.Long,
                Position.Auto,
                PriceType.Limit,
                1,
                1,
                0,
                Validity.Day));

        var order = new SetTradeOrder(
            123
        );

        var user = new User(
            new Guid(),
            [""],
            [request.AccountCode],
            "",
            "",
            "",
            "",
            "",
            "");

        _userService.Setup(s => s.GetUserById(request.UserId))
            .ReturnsAsync(user);
        _userV2Service.Setup(s => s.GetUserById(request.UserId))
            .ReturnsAsync(user);

        _setTradeService.Setup(s => s.PlaceOrder(
                request.UserId,
                request.CustomerCode,
                request.AccountCode,
                request.OrderInfo,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var client = Harness.GetRequestClient<SetTradePlaceOrderRequest>();
        var response = await client.GetResponse<SetTradePlaceOrderResponse>(request);

        // Assert
        Assert.True(await Harness.Consumed.Any<SetTradePlaceOrderResponse>());
        Assert.Equal(123, response.Message.OrderNo);
    }

    [Theory]
    [InlineData(PriceType.Limit, Validity.Day, 5, 5)]
    [InlineData(PriceType.MpMtl, Validity.Day, 5, 0)]
    [InlineData(PriceType.Ato, Validity.Ioc, 5, 0)]
    [InlineData(PriceType.MpMkt, Validity.Ioc, 5, 0)]
    public async void ShouldBeAbleToPlaceOrderWithCorrectValidityAndPrice(PriceType priceType, Validity validity, decimal requestPrice, decimal actualPrice)
    {
        // Arrange
        var request = new SetTradePlaceOrderRequest(
            "userid",
            "12345",
            "123456",
            new SetTradePlaceOrderRequest.PlaceOrderInfo(
                "123456",
                Side.Long,
                Position.Auto,
                priceType,
                requestPrice,
                5));

        var order = new SetTradeOrder(
            123
        );

        var user = new User(
            new Guid(),
            [""],
            [request.AccountCode],
            "",
            "",
            "",
            "",
            "",
            "");

        _userService.Setup(s => s.GetUserById(request.UserId))
            .ReturnsAsync(user);

        _setTradeService.Setup(s => s.PlaceOrder(
                request.UserId,
                request.CustomerCode,
                request.AccountCode,
                It.Is<SetTradePlaceOrderRequest.PlaceOrderInfo>(orderInfo =>
                    orderInfo.ValidityType == validity
                    && orderInfo.Price == actualPrice
                    && orderInfo.BypassWarning == true
                ),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var client = Harness.GetRequestClient<SetTradePlaceOrderRequest>();
        var response = await client.GetResponse<SetTradePlaceOrderResponse>(request);

        // Assert
        Assert.True(await Harness.Consumed.Any<SetTradePlaceOrderResponse>());
        Assert.Equal(123, response.Message.OrderNo);
    }
}