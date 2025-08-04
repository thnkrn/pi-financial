using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.Common.Features;
using Pi.WalletService.Application.Commands.Withdraw;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.Bank;
using Pi.WalletService.Application.Services.FxService;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.Withdraw;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Commands.Withdraw;

public class WithdrawRequestTests : ConsumerTest
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IOnboardService> _mockOnboardService;
    private readonly Mock<IBankInfoService> _mockBankInfoService;
    private readonly Mock<IWalletQueries> _mockWalletQueries;
    private readonly Mock<IFeatureService> _mockFeatureService;
    private readonly Mock<IFxService> _mockFxService;
    private readonly Mock<IDateQueries> _mockDateQueries;

    public WithdrawRequestTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockBankInfoService = new Mock<IBankInfoService>();
        _mockWalletQueries = new Mock<IWalletQueries>();
        _mockOnboardService = new Mock<IOnboardService>();
        _mockFeatureService = new Mock<IFeatureService>();
        _mockFxService = new Mock<IFxService>();
        _mockDateQueries = new Mock<IDateQueries>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<WithdrawRequestConsumer>(); })
            .AddScoped<IUserService>(_ => _mockUserService.Object)
            .AddScoped<IBankInfoService>(_ => _mockBankInfoService.Object)
            .AddScoped<IWalletQueries>(_ => _mockWalletQueries.Object)
            .AddScoped<IOnboardService>(_ => _mockOnboardService.Object)
            .AddScoped<IFeatureService>(_ => _mockFeatureService.Object)
            .AddScoped<IFxService>(_ => _mockFxService.Object)
            .AddScoped<IDateQueries>(_ => _mockDateQueries.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Publish_CashWithdrawRequestReceived()
    {
        // arrange
        const string accountCode = "77114961";
        const decimal requestAmount = (decimal)500.01;

        _mockWalletQueries
            .Setup(x => x.GetAvailableWithdrawalAmount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Product>()))
            .ReturnsAsync(1_000_000);
        _mockWalletQueries
            .Setup(x => x.GetBankAccount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Product>(), It.IsAny<TransactionType>(), It.IsAny<User>()))
            .ReturnsAsync(new BankAccountInfo(
                "",
                "Bank Account",
                "B",
                "014",
                "",
                "0000"
                ));
        _mockBankInfoService.Setup(x => x.GetByBankCode(It.IsAny<string>())).ReturnsAsync(new BankInfo("", "", "", ""));
        _mockUserService
            .Setup(u => u.GetUserInfoById(It.IsAny<string>()))
            .ReturnsAsync(new User(
                Guid.NewGuid(),
                new List<string>
                {
                    "7711496"
                },
                new List<string>
                {
                    accountCode
                },
                "Unit",
                "Test",
                "Unit",
                "Test",
                string.Empty,
                string.Empty,
                string.Empty));

        var payload = new WithdrawRequest(Guid.NewGuid(),
            "",
            "",
            Guid.NewGuid(),
            Product.Cash,
            requestAmount,
            false,
            0);

        // act
        await Harness.Bus.Publish(payload);

        // assert
        Assert.True(await Harness.Consumed.Any<WithdrawRequest>());
        Assert.True(await Harness.Published.Any<CashWithdrawRequestReceived>());

        var response = Harness.Published.Select<CashWithdrawRequestReceived>().First().Context;
        Assert.NotNull(response.Message);
        Assert.Equal(accountCode, response.Message.AccountCode);
        Assert.Equal(requestAmount, response.Message.Amount);
    }

    [Fact]
    public async void Should_Return_InvalidRequestFaulted_When_ProductIs_GlobalEquities()
    {
        // arrange
        var client = Harness.GetRequestClient<WithdrawRequest>();
        var payload = new WithdrawRequest(Guid.NewGuid(),
            "",
            "",
            Guid.NewGuid(),
            Product.GlobalEquities,
            100,
            false,
            0);

        // act
        var response = await client.GetResponse<BusRequestFailed>(payload);

        // assert
        Assert.True(await Harness.Consumed.Any<WithdrawRequest>());
        Assert.True(await Harness.Sent.Any<BusRequestFailed>());
        Assert.NotNull(response.Message);
        Assert.Equal(ErrorCodes.InvalidData, response.Message.ErrorCode);
    }

    [Fact]
    public async void Should_Return_InvalidRequestFaulted_When_RequestAmount_MoreThan_2M()
    {
        // arrange
        var client = Harness.GetRequestClient<WithdrawRequest>();
        var payload = new WithdrawRequest(Guid.NewGuid(),
            "",
            "",
            Guid.NewGuid(),
            Product.Cash,
            (decimal)2_000_000.01,
            false,
            0);

        // act
        var response = await client.GetResponse<BusRequestFailed>(payload);

        // assert
        Assert.True(await Harness.Consumed.Any<WithdrawRequest>());
        Assert.True(await Harness.Sent.Any<BusRequestFailed>());
        Assert.NotNull(response.Message);
        Assert.Equal(ErrorCodes.InvalidData, response.Message.ErrorCode);
    }

    [Fact]
    public async void Should_Return_InvalidRequestFaulted_When_RequestAmount_Exceed_AvailableBalance()
    {
        // arrange
        _mockWalletQueries
            .Setup(x => x.GetAvailableWithdrawalAmount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Product>()))
            .ReturnsAsync(100);
        var client = Harness.GetRequestClient<WithdrawRequest>();
        var payload = new WithdrawRequest(Guid.NewGuid(),
            "",
            "",
            Guid.NewGuid(),
            Product.Cash,
            (decimal)100.01,
            false,
            0);

        // act
        var response = await client.GetResponse<BusRequestFailed>(payload);

        // assert
        Assert.True(await Harness.Consumed.Any<WithdrawRequest>());
        Assert.True(await Harness.Sent.Any<BusRequestFailed>());
        Assert.NotNull(response.Message);
        Assert.Equal(ErrorCodes.InvalidData, response.Message.ErrorCode);
    }

    [Fact]
    public async void Should_Return_InvalidRequestFaulted_When_TradingAccount_NotFound()
    {
        // arrange
        const string accountCode = "77114969";
        _mockWalletQueries
            .Setup(x => x.GetAvailableWithdrawalAmount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Product>()))
            .ReturnsAsync(1_000_000);
        _mockUserService
            .Setup(u => u.GetUserInfoById(It.IsAny<string>()))
            .ReturnsAsync(new User(
                Guid.NewGuid(),
                new List<string>
                {
                    "7711496"
                },
                new List<string>
                {
                    accountCode
                },
                "Unit",
                "Test",
                "Unit",
                "Test",
                string.Empty,
                string.Empty,
                string.Empty));
        var client = Harness.GetRequestClient<WithdrawRequest>();
        var payload = new WithdrawRequest(Guid.NewGuid(),
            "",
            "",
            Guid.NewGuid(),
            Product.Cash,
            (decimal)100.01,
            false,
            0);

        // act
        var response = await client.GetResponse<BusRequestFailed>(payload);

        // assert
        Assert.True(await Harness.Consumed.Any<WithdrawRequest>());
        Assert.True(await Harness.Sent.Any<BusRequestFailed>());
        Assert.NotNull(response.Message);
        Assert.Equal(ErrorCodes.InvalidData, response.Message.ErrorCode);
    }

    [Fact]
    public async void Should_Return_InvalidRequestFaulted_When_Ge_V2_RequestedCurrency_NotSupported()
    {
        // arrange
        var client = Harness.GetRequestClient<WithdrawRequest>();
        var payload = new WithdrawRequest(Guid.NewGuid(),
            "",
            "",
            Guid.NewGuid(),
            Product.GlobalEquities,
            100,
            true,
            0_55,
            null,
            null,
            0,
            "THB");

        // act
        var response = await client.GetResponse<BusRequestFailed>(payload);

        // assert
        Assert.True(await Harness.Consumed.Any<WithdrawRequest>());
        Assert.True(await Harness.Sent.Any<BusRequestFailed>());
        Assert.NotNull(response.Message);
        Assert.Equal(ErrorCodes.InvalidData, response.Message.ErrorCode);
    }

    [Fact]
    public async void Should_Return_InvalidRequestFaulted_When_Ge_V2_FxTransactionId_Is_Null()
    {
        // arrange
        var client = Harness.GetRequestClient<WithdrawRequest>();
        var payload = new WithdrawRequest(
            Guid.NewGuid(),
            "",
            "",
            Guid.NewGuid(),
            Product.GlobalEquities,
            100,
            true,
            0_55,
            null,
            null,
            0,
            "USD"
        );

        // act
        var response = await client.GetResponse<BusRequestFailed>(payload);

        // assert
        Assert.True(await Harness.Consumed.Any<WithdrawRequest>());
        Assert.True(await Harness.Sent.Any<BusRequestFailed>());
        Assert.NotNull(response.Message);
        Assert.Equal(ErrorCodes.InvalidData, response.Message.ErrorCode);
    }

    [Fact]
    public async void Should_Return_InvalidRequestFaulted_When_Ge_V2_FxTransaction_Is_Null()
    {
        // arrange
        var client = Harness.GetRequestClient<WithdrawRequest>();
        var payload = new WithdrawRequest(
            Guid.NewGuid(),
            "",
            "",
            Guid.NewGuid(),
            Product.GlobalEquities,
            0.001m,
            true,
            0_55,
            null,
            "123",
            0,
            "USD"
        );

        _mockFxService
            .Setup(u => u.GetTransaction(payload.FxTransactionId!))!
            .ReturnsAsync((GetTransactionResponse?)null);

        // act
        var response = await client.GetResponse<BusRequestFailed>(payload);

        // assert
        Assert.True(await Harness.Consumed.Any<WithdrawRequest>());
        Assert.True(await Harness.Sent.Any<BusRequestFailed>());
        Assert.NotNull(response.Message);
        Assert.Equal(ErrorCodes.InvalidData, response.Message.ErrorCode);
    }

    [Fact]
    public async void Should_Return_InvalidRequestFaulted_When_Ge_V2_FxTransaction_Is_Invalid()
    {
        // arrange
        var client = Harness.GetRequestClient<WithdrawRequest>();
        var payload = new WithdrawRequest(
            Guid.NewGuid(),
            "",
            "",
            Guid.NewGuid(),
            Product.GlobalEquities,
            0.001m,
            true,
            0_55,
            null,
            "123",
            0,
            "USD"
        );

        _mockFxService
            .Setup(u => u.GetTransaction(payload.FxTransactionId!))
            .ReturnsAsync(new GetTransactionResponse(
                "123",
                FxQuoteType.Sell,
                DateTime.Now,
                10,
                "USD",
                10 * 5,
                "CHY", // Mock invalid currency = CHINESE YUAN
                5,
                DateTime.Now
            ));

        // act
        var response = await client.GetResponse<BusRequestFailed>(payload);

        // assert
        Assert.True(await Harness.Consumed.Any<WithdrawRequest>());
        Assert.True(await Harness.Sent.Any<BusRequestFailed>());
        Assert.NotNull(response.Message);
        Assert.Equal(ErrorCodes.InvalidData, response.Message.ErrorCode);
    }

    [Fact]
    public async void Should_Return_InvalidRequestFaulted_When_Ge_V2_RequestAmount_Is_Less_Than_1()
    {
        // arrange
        var client = Harness.GetRequestClient<WithdrawRequest>();
        var payload = new WithdrawRequest(
            Guid.NewGuid(),
            "",
            "",
            Guid.NewGuid(),
            Product.GlobalEquities,
            0.001m,
            true,
            0_55,
            null,
            "123",
            0,
            "USD"
        );

        _mockFxService
            .Setup(u => u.GetTransaction(payload.FxTransactionId!))
            .ReturnsAsync(new GetTransactionResponse(
                "123",
                FxQuoteType.Sell,
                DateTime.Now,
                0.001m,
                "USD",
                0.001m * 35,
                "THB",
                35,
                DateTime.Now
            ));

        // act
        var response = await client.GetResponse<BusRequestFailed>(payload);

        // assert
        Assert.True(await Harness.Consumed.Any<WithdrawRequest>());
        Assert.True(await Harness.Sent.Any<BusRequestFailed>());
        Assert.NotNull(response.Message);
        Assert.Equal(ErrorCodes.InvalidData, response.Message.ErrorCode);
    }
}
