using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.Common.Features;
using Pi.TfexService.Application.Commands.Order;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Application.Services.UserService;
using Pi.TfexService.Domain.Exceptions;

namespace Pi.TfexService.Application.Tests.Commands.Order;

public class PatchOrderConsumerTests : ConsumerTest
{

    private readonly Mock<ISetTradeService> _setTradeService = new();
    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<IUserV2Service> _userV2Service = new();
    private readonly Mock<IFeatureService> _featureService = new();
    public PatchOrderConsumerTests()
    {
        Provider = new ServiceCollection()
            .AddScoped<ISetTradeService>(_ => _setTradeService.Object)
            .AddScoped<IUserService>(_ => _userService.Object)
            .AddScoped<IUserV2Service>(_ => _userV2Service.Object)
            .AddScoped<IFeatureService>(_ => _featureService.Object)
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<PatchOrderConsumer>();
            })
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Cancel_Should_Work_Correctly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new SetTradePatchOrderRequest(
            PatchOrderType.Cancel,
            userId.ToString(),
            "AccountCode",
            1234,
            2,
            3
        );

        _setTradeService.Setup(s => s.GetOrderByNo(
            It.IsAny<string>(),
            It.IsAny<long>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(new SetTradeOrder(OrderNo: request.OrderNo, CanCancel: true));

        _setTradeService.Setup(s => s.CancelOrder(
            request.AccountCode,
            request.OrderNo,
            default)).ReturnsAsync(true);

        _userService.Setup(u => u.GetUserById(request.UserId))
            .ReturnsAsync(new User(userId, [""], [request.AccountCode], "", "", "", "", "", ""));

        // Act
        var client = Harness.GetRequestClient<SetTradePatchOrderRequest>();
        var response = await client.GetResponse<SetTradePatchOrderSuccess>(request);

        // Assert
        Assert.True(await Harness.Consumed.Any<SetTradePatchOrderRequest>());
        Assert.NotNull(response.Message);
    }

    [Fact]
    public async void Cancel_Should_Throw_Exception_When_Cannot_Cancel()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new SetTradePatchOrderRequest(
            PatchOrderType.Cancel,
            userId.ToString(),
            "AccountCode",
            1234,
            2,
            3
        );

        _setTradeService.Setup(s => s.GetOrderByNo(
            It.IsAny<string>(),
            It.IsAny<long>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(new SetTradeOrder(OrderNo: request.OrderNo, CanCancel: false));

        _userService.Setup(u => u.GetUserById(request.UserId))
            .ReturnsAsync(new User(userId, [""], [request.AccountCode], "", "", "", "", "", ""));
        _userV2Service.Setup(u => u.GetUserById(request.UserId))
            .ReturnsAsync(new User(userId, [""], [request.AccountCode], "", "", "", "", "", ""));

        // Act
        var client = Harness.GetRequestClient<SetTradePatchOrderRequest>();
        var requestFault = await Assert.ThrowsAsync<RequestFaultException>(() =>
            client.GetResponse<SetTradePatchOrderSuccess>(request));

        // Assert
        Assert.True(await Harness.Consumed.Any<SetTradePatchOrderRequest>());
        Assert.NotNull(requestFault.Fault);
        Assert.NotEmpty(requestFault.Fault.Exceptions);
        Assert.Equal(typeof(ArgumentException).FullName, requestFault.Fault.Exceptions.First().ExceptionType);
    }

    [Fact]
    public async void Update_Should_Work_Correctly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new SetTradePatchOrderRequest(
            PatchOrderType.Update,
            userId.ToString(),
            "AccountCode",
            123,
            100,
            100
        );

        _setTradeService.Setup(s => s.GetOrderByNo(
            It.IsAny<string>(),
            It.IsAny<long>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(new SetTradeOrder(OrderNo: request.OrderNo, Qty: request.Volume!.Value + 1, Price: request.Price!.Value + 1, CanChange: true));

        _setTradeService.Setup(s => s.UpdateOrder(
                request.AccountCode,
                request.OrderNo,
                request.Price ?? 0,
                request.Volume ?? 0,
                false,
                default)).ReturnsAsync(true);
        _userService.Setup(u => u.GetUserById(request.UserId))
            .ReturnsAsync(new User(userId, [""], [request.AccountCode], "", "", "", "", "", ""));

        // Act
        var client = Harness.GetRequestClient<SetTradePatchOrderRequest>();
        var response = await client.GetResponse<SetTradePatchOrderSuccess>(request);

        // Assert
        Assert.True(await Harness.Consumed.Any<SetTradePatchOrderRequest>());
        Assert.NotNull(response.Message);
    }

    [Fact]
    public async void Update_Should_Throws_ArgumentException_When_PriceOrVolume_Invalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new SetTradePatchOrderRequest(
            PatchOrderType.Update,
            userId.ToString(),
            "AccountCode",
            123,
            0,
            0
        );
        _setTradeService.Setup(s => s.UpdateOrder(
            request.AccountCode,
            request.OrderNo,
            request.Price ?? 0,
            request.Volume ?? 0,
            false,
            default)).ReturnsAsync(true);
        _setTradeService.Setup(s => s.GetOrderByNo(
            It.IsAny<string>(),
            It.IsAny<long>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(new SetTradeOrder(OrderNo: request.OrderNo, Qty: request.Volume!.Value + 1, Price: request.Price!.Value + 1, CanChange: true));
        _userService.Setup(u => u.GetUserById(request.UserId))
            .ReturnsAsync(new User(userId, [""], [request.AccountCode], "", "", "", "", "", ""));

        // Act
        var client = Harness.GetRequestClient<SetTradePatchOrderRequest>();
        var requestFault = await Assert.ThrowsAsync<RequestFaultException>(() =>
                                         client.GetResponse<SetTradePatchOrderSuccess>(request));

        // Assert
        Assert.True(await Harness.Consumed.Any<SetTradePatchOrderRequest>());
        Assert.NotNull(requestFault.Fault);
        Assert.NotEmpty(requestFault.Fault.Exceptions);
        Assert.Equal(typeof(ArgumentException).FullName, requestFault.Fault.Exceptions.First().ExceptionType);
    }

    [Fact]
    public async void Update_Should_Throws_ArgumentException_When_Cannot_Change()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new SetTradePatchOrderRequest(
            PatchOrderType.Update,
            userId.ToString(),
            "AccountCode",
            123,
            0,
            0
        );
        _setTradeService.Setup(s => s.UpdateOrder(
            request.AccountCode,
            request.OrderNo,
            request.Price ?? 0,
            request.Volume ?? 0,
            false,
            default)).ReturnsAsync(true);
        _setTradeService.Setup(s => s.GetOrderByNo(
            It.IsAny<string>(),
            It.IsAny<long>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(new SetTradeOrder(OrderNo: request.OrderNo, Qty: request.Volume!.Value + 1, Price: request.Price!.Value + 1, CanChange: false));
        _userService.Setup(u => u.GetUserById(request.UserId))
            .ReturnsAsync(new User(userId, [""], [request.AccountCode], "", "", "", "", "", ""));

        // Act
        var client = Harness.GetRequestClient<SetTradePatchOrderRequest>();
        var requestFault = await Assert.ThrowsAsync<RequestFaultException>(() =>
            client.GetResponse<SetTradePatchOrderSuccess>(request));

        // Assert
        Assert.True(await Harness.Consumed.Any<SetTradePatchOrderRequest>());
        Assert.NotNull(requestFault.Fault);
        Assert.NotEmpty(requestFault.Fault.Exceptions);
        Assert.Equal(typeof(ArgumentException).FullName, requestFault.Fault.Exceptions.First().ExceptionType);
    }

    [Fact]
    public async void Update_Should_Throws_ArgumentException_When_Price_Volume_The_Same()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new SetTradePatchOrderRequest(
            PatchOrderType.Update,
            userId.ToString(),
            "AccountCode",
            123,
            2,
            3
        );
        _setTradeService.Setup(s => s.UpdateOrder(
            request.AccountCode,
            request.OrderNo,
            request.Price ?? 0,
            request.Volume ?? 0,
            false,
            default)).ReturnsAsync(true);
        _setTradeService.Setup(s => s.GetOrderByNo(
            It.IsAny<string>(),
            It.IsAny<long>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(new SetTradeOrder(OrderNo: request.OrderNo, Qty: request.Volume!.Value, Price: request.Price!.Value, CanChange: true));
        _userService.Setup(u => u.GetUserById(request.UserId))
            .ReturnsAsync(new User(userId, [""], [request.AccountCode], "", "", "", "", "", ""));

        // Act
        var client = Harness.GetRequestClient<SetTradePatchOrderRequest>();
        var requestFault = await Assert.ThrowsAsync<RequestFaultException>(() =>
            client.GetResponse<SetTradePatchOrderSuccess>(request));

        // Assert
        Assert.True(await Harness.Consumed.Any<SetTradePatchOrderRequest>());
        Assert.NotNull(requestFault.Fault);
        Assert.NotEmpty(requestFault.Fault.Exceptions);
        Assert.Equal(typeof(ArgumentException).FullName, requestFault.Fault.Exceptions.First().ExceptionType);
    }

    [Fact]
    public async void Update_Should_Throws_ArgumentException_When_Order_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new SetTradePatchOrderRequest(
            PatchOrderType.Update,
            userId.ToString(),
            "AccountCode",
            123,
            0,
            0
        );
        _setTradeService.Setup(s => s.UpdateOrder(
            request.AccountCode,
            request.OrderNo,
            request.Price ?? 0,
            request.Volume ?? 0,
            false,
            default)).ReturnsAsync(true);
        _setTradeService.Setup(s => s.GetOrderByNo(
            It.IsAny<string>(),
            It.IsAny<long>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(() => null!);
        _userService.Setup(u => u.GetUserById(request.UserId))
            .ReturnsAsync(new User(userId, [""], [request.AccountCode], "", "", "", "", "", ""));

        // Act
        var client = Harness.GetRequestClient<SetTradePatchOrderRequest>();
        var requestFault = await Assert.ThrowsAsync<RequestFaultException>(() =>
            client.GetResponse<SetTradePatchOrderSuccess>(request));

        // Assert
        Assert.True(await Harness.Consumed.Any<SetTradePatchOrderRequest>());
        Assert.NotNull(requestFault.Fault);
        Assert.NotEmpty(requestFault.Fault.Exceptions);
        Assert.Equal(typeof(SetTradeNotFoundException).FullName, requestFault.Fault.Exceptions.First().ExceptionType);
    }

    [Fact]
    public async void Update_Should_Throws_UnauthorizedAccessException_When_AccountCode_NotMatch_UserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new SetTradePatchOrderRequest(
            PatchOrderType.Update,
            userId.ToString(),
            "AccountCode",
            123,
            0,
            0
        );
        _setTradeService.Setup(s => s.UpdateOrder(
            request.AccountCode,
            request.OrderNo,
            request.Price ?? 0,
            request.Volume ?? 0,
            false,
            default)).ReturnsAsync(true);
        _userService.Setup(u => u.GetUserById(request.UserId))
            .ReturnsAsync(new User(userId, [""], [""], "", "", "", "", "", ""));

        // Act
        var client = Harness.GetRequestClient<SetTradePatchOrderRequest>();
        var requestFault = await Assert.ThrowsAsync<RequestFaultException>(() =>
            client.GetResponse<SetTradePatchOrderSuccess>(request));

        // Assert
        Assert.True(await Harness.Consumed.Any<SetTradePatchOrderRequest>());
        Assert.NotNull(requestFault.Fault);
        Assert.NotEmpty(requestFault.Fault.Exceptions);
        Assert.Equal(typeof(UnauthorizedAccessException).FullName, requestFault.Fault.Exceptions.First().ExceptionType);
    }

    [Theory]
    [InlineData(PatchOrderType.Update)]
    [InlineData(PatchOrderType.Cancel)]
    public async void Update_Cancel_Should_ByPassWarning(PatchOrderType patchOrderType)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new SetTradePatchOrderRequest(
            patchOrderType,
            userId.ToString(),
            "AccountCode",
            123,
            0,
            0
        );
        _setTradeService.Setup(s => s.UpdateOrder(
            request.AccountCode,
            request.OrderNo,
            request.Price ?? 0,
            request.Volume ?? 0,
            true,
            default)).ReturnsAsync(true);
        _userService.Setup(u => u.GetUserById(request.UserId))
            .ReturnsAsync(new User(userId, [""], [""], "", "", "", "", "", ""));

        // Act
        var client = Harness.GetRequestClient<SetTradePatchOrderRequest>();
        var requestFault = await Assert.ThrowsAsync<RequestFaultException>(() =>
            client.GetResponse<SetTradePatchOrderSuccess>(request));

        // Assert
        Assert.True(await Harness.Consumed.Any<SetTradePatchOrderRequest>());
        Assert.NotNull(requestFault.Fault);
        Assert.NotEmpty(requestFault.Fault.Exceptions);
        Assert.Equal(typeof(UnauthorizedAccessException).FullName, requestFault.Fault.Exceptions.First().ExceptionType);
    }
}