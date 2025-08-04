using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Application.Services.GlobalEquities;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.ForeignExchange;
namespace Pi.WalletService.Application.Tests.Commands.GlobalWalletTransfer;

public class TransferUsdMoneyTests : ConsumerTest
{
    private readonly Mock<IGlobalTradeService> _globalTradeService;
    private readonly Mock<IGlobalUserManagementService> _globalUserManagementService;
    private const string MainAccount = "exante_account_id";
    public TransferUsdMoneyTests()
    {
        Mock<IDepositEntrypointRepository> depositEntrypointRepository = new Mock<IDepositEntrypointRepository>();
        Mock<IWithdrawEntrypointRepository> withdrawEntrypointRepository = new Mock<IWithdrawEntrypointRepository>();
        _globalTradeService = new Mock<IGlobalTradeService>();
        _globalUserManagementService = new Mock<IGlobalUserManagementService>();
        Mock<IConfiguration> configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c["Exante:MainAccountId"]).Returns(MainAccount);

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<TransferUsdMoneyConsumer>(); })
            .AddScoped<IDepositEntrypointRepository>(_ => depositEntrypointRepository.Object)
            .AddScoped<IWithdrawEntrypointRepository>(_ => withdrawEntrypointRepository.Object)
            .AddScoped<IGlobalTradeService>(_ => _globalTradeService.Object)
            .AddScoped<IGlobalUserManagementService>(_ => _globalUserManagementService.Object)
            .AddScoped<IConfiguration>(_ => configuration.Object)
            .AddScoped<ILogger<TransferUsdMoneyConsumer>>(_ => NullLogger<TransferUsdMoneyConsumer>.Instance)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Able_To_TransferUSDFromSubAccountToMainAccount_Correctly()
    {
        // Arrange
        const string accountId = "account_id";
        _globalTradeService
            .Setup(g => g.GetAccountSummary(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new AccountSummaryResponse(
                accountId,
                "USD",
                12345,
                "20000",
                new List<Position>()));
        var client = Harness.GetRequestClient<TransferUsdMoneyFromSubAccountToMainAccount>();

        // Act
        var response = await client.GetResponse<TransferUsdMoneyToMainSucceeded>(
            new TransferUsdMoneyFromSubAccountToMainAccount(
                "traction_no",
                accountId,
                Currency.USD,
                200,
                0
                ));

        // Assert
        Assert.Equal(accountId, response.Message.FromAccount);
        Assert.Equal(MainAccount, response.Message.ToAccount);
    }

    [Fact]
    public async void Should_Not_TransferUSDFromSubAccountToMainAccount_When_Available_Balance_Is_Not_Enough()
    {
        // Arrange
        const string accountId = "account_id";
        _globalTradeService
            .Setup(g => g.GetAccountSummary(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new AccountSummaryResponse(
                accountId,
                "USD",
                12345,
                "100",
                new List<Position>()));
        var client = Harness.GetRequestClient<TransferUsdMoneyFromSubAccountToMainAccount>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<TransferUsdMoneyToMainSucceeded>(
            new TransferUsdMoneyFromSubAccountToMainAccount(
                "traction_no",
                accountId,
                Currency.USD,
                200,
                0
                )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(TransferInsufficientBalanceException).ToString())));
    }

    [Fact]
    public async void Should_Handle_TransferUSDFromSubAccountToMainAccount_When_Unable_To_Transfer()
    {
        // Arrange
        const string accountId = "account_id";
        _globalTradeService
            .Setup(g => g.GetAccountSummary(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new AccountSummaryResponse(
                accountId,
                "USD",
                12345,
                "2000",
                new List<Position>()));
        _globalUserManagementService
            .Setup(g => g.TransferMoney(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
            .ThrowsAsync(new Exception("Something wrong"));

        var client = Harness.GetRequestClient<TransferUsdMoneyFromSubAccountToMainAccount>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<TransferUsdMoneyToMainSucceeded>(
                new TransferUsdMoneyFromSubAccountToMainAccount(
                    "traction_no",
                    accountId,
                    Currency.USD,
                    200,
                    0
                )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(TransferMoneyException).ToString())));
    }

    [Fact]
    public async void Should_Able_To_TransferUsdMoneyFromMainAccountToSubAccount_Correctly()
    {
        // Arrange
        const string accountId = "account_id";
        _globalTradeService
            .Setup(g => g.GetAccountSummary(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new AccountSummaryResponse(
                accountId,
                "USD",
                12345,
                "20000",
                new List<Position>()));
        var client = Harness.GetRequestClient<TransferUsdMoneyFromMainAccountToSubAccount>();

        // Act
        var response = await client.GetResponse<TransferUsdMoneyToSubSucceeded>(
            new TransferUsdMoneyFromMainAccountToSubAccount(
                "traction_no",
                accountId,
                Currency.USD,
                200,
                0
                ));

        // Assert
        Assert.Equal(accountId, response.Message.ToAccount);
        Assert.Equal(MainAccount, response.Message.FromAccount);
    }

    [Fact]
    public async void Should_Not_TransferUsdMoneyFromMainAccountToSubAccount_When_Available_Balance_Is_Not_Enough()
    {
        // Arrange
        const string accountId = "account_id";
        _globalTradeService
            .Setup(g => g.GetAccountSummary(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new AccountSummaryResponse(
                accountId,
                "USD",
                12345,
                "100",
                new List<Position>()));
        _globalUserManagementService
            .Setup(g => g.TransferMoney(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
            .ThrowsAsync(new TransferInsufficientBalanceException("Something wrong"));

        var client = Harness.GetRequestClient<TransferUsdMoneyFromMainAccountToSubAccount>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<TransferUsdMoneyToSubSucceeded>(
            new TransferUsdMoneyFromMainAccountToSubAccount(
                "traction_no",
                accountId,
                Currency.USD,
                200,
                0
                )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(TransferInsufficientBalanceException).ToString())));
    }

    [Fact]
    public async void Should_Handle_TransferUsdMoneyFromMainAccountToSubAccount_When_Unable_To_Transfer()
    {
        // Arrange
        const string accountId = "account_id";
        _globalTradeService
            .Setup(g => g.GetAccountSummary(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new AccountSummaryResponse(
                accountId,
                "USD",
                12345,
                "2000",
                new List<Position>()));
        _globalUserManagementService
            .Setup(g => g.TransferMoney(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
            .ThrowsAsync(new Exception("Something wrong"));

        var client = Harness.GetRequestClient<TransferUsdMoneyFromMainAccountToSubAccount>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<TransferUsdMoneyToSubSucceeded>(
                new TransferUsdMoneyFromMainAccountToSubAccount(
                    "traction_no",
                    accountId,
                    Currency.USD,
                    200,
                    0
                )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(TransferMoneyException).ToString())));
    }
}
