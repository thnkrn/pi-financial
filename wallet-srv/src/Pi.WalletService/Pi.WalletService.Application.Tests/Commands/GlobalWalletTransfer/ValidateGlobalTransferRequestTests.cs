using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Commands.GlobalWalletTransfer;

public class ValidateGlobalTransferRequestTests : ConsumerTest
{

    public ValidateGlobalTransferRequestTests()
    {
        Mock<IWalletQueries> walletQueries = new();
        Mock<IDepositEntrypointRepository> depositEntrypointRepository = new();
        Mock<IWithdrawEntrypointRepository> withdrawEntrypointRepository = new();

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<ValidateGlobalTransferRequestConsumer>(); })
            .AddScoped<IWalletQueries>(_ => walletQueries.Object)
            .AddScoped<IDepositEntrypointRepository>(_ => depositEntrypointRepository.Object)
            .AddScoped<IWithdrawEntrypointRepository>(_ => withdrawEntrypointRepository.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Able_To_Handle_Deposit_ValidateGlobalTransferRequest_Correctly()
    {
        // Arrange
        const string transactionNo = "transaction_no";
        var client = Harness.GetRequestClient<ValidateGlobalTransferRequest>();

        // Act
        var response = await client.GetResponse<GlobalTransferRequestValidationCompleted>(
            new ValidateGlobalTransferRequest(
                transactionNo,
                TransactionType.Deposit,
                Guid.NewGuid().ToString(),
                "7711496",
                decimal.Parse("25.0"),
                2400,
                Currency.THB,
                Currency.USD
                ));

        // Assert
        Assert.Equal(transactionNo, response.Message.TransactionNo);
    }

    [Fact]
    public async void Should_Able_To_Handle_Deposit_ValidateGlobalTransferRequest_Correctly_When_Currency_Is_Invalid()
    {
        // Arrange
        const string transactionNo = "transaction_no";
        var client = Harness.GetRequestClient<ValidateGlobalTransferRequest>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<GlobalTransferRequestValidationCompleted>(
                new ValidateGlobalTransferRequest(
                    transactionNo,
                    TransactionType.Deposit,
                    Guid.NewGuid().ToString(),
                    "7711496",
                    decimal.Parse("25.0"),
                    1000,
                    Currency.USD,
                    Currency.USD
                )));

        // Assert
        Assert.Equal($"Invalid currency pair, Got USD and USD", exception.Fault?.Exceptions.First().Message);
    }

    [Theory]
    [InlineData(2_000_001)]
    [InlineData(0)]
    public async void Should_Able_To_Handle_Deposit_ValidateGlobalTransferRequest_Correctly_When_Amount_Exceed_Or_Too_Low(decimal amount)
    {
        // Arrange
        const string transactionNo = "transaction_no";
        var client = Harness.GetRequestClient<ValidateGlobalTransferRequest>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<GlobalTransferRequestValidationCompleted>(
            new ValidateGlobalTransferRequest(
                transactionNo,
                TransactionType.Deposit,
                Guid.NewGuid().ToString(),
                "7711496",
                decimal.Parse("25.0"),
                amount,
                Currency.THB,
                Currency.USD
            )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(InvalidDataException).ToString())));
    }

    [Fact]
    public async void Should_Able_To_Handle_Withdraw_ValidateGlobalTransferRequest_Correctly()
    {

        // Arrange
        const string transactionNo = "transaction_no";
        var client = Harness.GetRequestClient<ValidateGlobalTransferRequest>();

        // Act
        var response = await client.GetResponse<GlobalTransferRequestValidationCompleted>(
            new ValidateGlobalTransferRequest(
                transactionNo,
                TransactionType.Withdraw,
                Guid.NewGuid().ToString(),
                "7711496",
                decimal.Parse("25.0"),
                2400,
                Currency.THB,
                Currency.USD
            ));

        // Assert
        Assert.Equal(transactionNo, response.Message.TransactionNo);
    }

    [Fact]
    public async void Should_Able_To_Handle_ValidateGlobalTransferRequest_Correctly_When_Get_Unknown_TransactionType()
    {
        // Arrange
        const string transactionNo = "transaction_no";
        var client = Harness.GetRequestClient<ValidateGlobalTransferRequest>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<GlobalTransferRequestValidationCompleted>(
            new ValidateGlobalTransferRequest(
                transactionNo,
                (TransactionType)3,
                Guid.NewGuid().ToString(),
                "7711496",
                decimal.Parse("25.0"),
                2400,
                Currency.THB,
                Currency.USD
            )));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(InvalidDataException).ToString())));
    }
}
