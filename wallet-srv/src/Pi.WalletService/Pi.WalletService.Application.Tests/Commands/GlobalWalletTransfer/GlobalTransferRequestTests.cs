using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.Common.Features;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.ForeignExchange;
namespace Pi.WalletService.Application.Tests.Commands.GlobalWalletTransfer;

public class GlobalTransferRequestTests : ConsumerTest
{
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IOnboardService> _onboardService;
    private readonly Mock<IConfiguration> _config;
    private readonly Mock<IWalletQueries> _walletQueries;
    private readonly User _user;
    public GlobalTransferRequestTests()
    {
        _userService = new Mock<IUserService>();
        _walletQueries = new Mock<IWalletQueries>();
        _onboardService = new Mock<IOnboardService>();
        _config = new Mock<IConfiguration>();
        Mock<IFeatureService> featureService = new Mock<IFeatureService>();
        _user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>
            {
                "77114312"
            },
            "Unit",
            "Test",
            "Unit",
            "Test",
            "AABB1234",
            string.Empty,
            string.Empty
        );

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<RequestGlobalTransferConsumer>(); })
            .AddScoped<IUserService>(_ => _userService.Object)
            .AddScoped<IOnboardService>(_ => _onboardService.Object)
            .AddScoped<IFeatureService>(_ => featureService.Object)
            .AddScoped<IWalletQueries>(_ => _walletQueries.Object)
            .AddScoped<IConfiguration>(_ => _config.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Execute_Deposit_GlobalTransferRequest_Correctly()
    {
        // Arrange
        _userService
            .Setup(u => u.GetUserInfoById(It.IsAny<string>()))
            .ReturnsAsync(_user);
        const string custCode = "7711431";

        // Act
        await Harness.Bus.Publish(new RequestGlobalTransferDeposit(
            Guid.NewGuid(),
            123456,
            custCode,
            _user.Id.ToString(),
            Guid.NewGuid(),
            2_000_000,
            Currency.THB.ToString(),
            decimal.Parse("35.4"),
            Currency.USD.ToString(),
            false
        ));

        await Harness.Published.Any<GlobalTransferDepositRequestReceived>();
        var publishedEvent = Harness.Published.Select<GlobalTransferDepositRequestReceived>().First()!.Context;

        // Assert
        Assert.Equal(_user.ListTradingAccountNo.First(), publishedEvent.Message.AccountCode);
        Assert.Equal(_user.GlobalAccount, publishedEvent.Message.GlobalAccount);
        Assert.Equal(custCode, publishedEvent.Message.CustomerCode);
        Assert.Equal(Currency.USD, publishedEvent.Message.RequestedFxCurrency);
        Assert.Equal(Currency.THB, publishedEvent.Message.RequestedCurrency);
    }

    [Fact]
    public async void Should_Handle_Deposit_GlobalTransferRequest_Correctly_When_Get_Invalid_Currency()
    {
        // Arrange
        _userService
            .Setup(u => u.GetUserInfoById(It.IsAny<string>()))
            .ReturnsAsync(_user);
        var client = Harness.GetRequestClient<RequestGlobalTransferDeposit>();
        const string custCode = "7711431";

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<GlobalTransferDepositRequestReceived>(new RequestGlobalTransferDeposit(
                Guid.NewGuid(),
                123456,
                custCode,
                _user.Id.ToString(),
                Guid.NewGuid(),
                2_000_000,
                "HKD",
                decimal.Parse("35.4"),
                Currency.USD.ToString(),
                false
            )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(InvalidDataException).ToString())));
    }

    [Fact]
    public async void Should_Handle_Deposit_GlobalTransferRequest_Correctly_When_Get_Amount_Exceeded()
    {
        // Arrange
        _userService
            .Setup(u => u.GetUserInfoById(It.IsAny<string>()))
            .ReturnsAsync(_user);
        var client = Harness.GetRequestClient<RequestGlobalTransferDeposit>();
        const string custCode = "7711431";

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<GlobalTransferDepositRequestReceived>(new RequestGlobalTransferDeposit(
                Guid.NewGuid(),
                123456,
                custCode,
                _user.Id.ToString(),
                Guid.NewGuid(),
                2_000_001,
                "USD",
                decimal.Parse("35.4"),
                Currency.USD.ToString(),
                false
            )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(InvalidDataException).ToString())));
    }

    [Fact]
    public async void Should_Handle_Deposit_GlobalTransferRequest_Correctly_When_Get_Trading_Invalid()
    {
        // Arrange
        const string custCode = "7711431";
        _userService
            .Setup(u => u.GetUserInfoById(It.IsAny<string>()))
            .ReturnsAsync(new User(
                Guid.NewGuid(),
                new List<string>(),
                new List<string>(),
                "Unit",
                "Test",
                "Unit",
                "Test",
                "AABB1234",
                string.Empty,
                string.Empty
            ));
        var client = Harness.GetRequestClient<RequestGlobalTransferDeposit>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<GlobalTransferDepositRequestReceived>(new RequestGlobalTransferDeposit(
                Guid.NewGuid(),
                123456,
                custCode,
                _user.Id.ToString(),
                Guid.NewGuid(),
                2_000_000,
                "THB",
                decimal.Parse("35.4"),
                Currency.USD.ToString(),
                false
            )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(InvalidDataException).ToString())));
    }

    [Fact]
    public async void Should_Execute_Withdraw_GlobalTransferRequest_Correctly()
    {
        // Arrange
        const string custCode = "7711431";
        _userService
            .Setup(u => u.GetUserInfoById(It.IsAny<string>()))
            .ReturnsAsync(_user);

        // Act
        await Harness.Bus.Publish(
            new RequestGlobalTransferWithdraw(
                Guid.NewGuid(),
                123456,
                custCode,
                _user.Id.ToString(),
                Guid.NewGuid(),
                "fx_transaction_id",
                10,
                "USD",
                false
            ));

        await Harness.Published.Any<GlobalTransferWithdrawRequestReceived>();
        var publishedEvent = Harness.Published.Select<GlobalTransferWithdrawRequestReceived>().First()!.Context;

        // Assert
        Assert.Equal(_user.ListTradingAccountNo.First(), publishedEvent.Message.AccountCode);
        Assert.Equal(_user.GlobalAccount, publishedEvent.Message.GlobalAccount);
        Assert.Equal(custCode, publishedEvent.Message.CustomerCode);
    }

    [Fact]
    public async void Should_Handle_Withdraw_GlobalTransferRequest_Correctly_When_Get_Trading_Invalid()
    {
        // Arrange
        const string custCode = "7711431";
        _userService
            .Setup(u => u.GetUserInfoById(It.IsAny<string>()))
            .ReturnsAsync(new User(
                Guid.NewGuid(),
                new List<string>(),
                new List<string>(),
                "Unit",
                "Test",
                "Unit",
                "Test",
                "AABB1234",
                string.Empty,
                string.Empty
            ));
        var client = Harness.GetRequestClient<RequestGlobalTransferWithdraw>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<GlobalTransferWithdrawRequestReceived>(
                new RequestGlobalTransferWithdraw(
                    Guid.NewGuid(),
                    123456,
                    custCode,
                    _user.Id.ToString(),
                    Guid.NewGuid(),
                    "fx_transaction_id",
                    10,
                    "USD",
                    false
                )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(InvalidDataException).ToString())));
    }
}
