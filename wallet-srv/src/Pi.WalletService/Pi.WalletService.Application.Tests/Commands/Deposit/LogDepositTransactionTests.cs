using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.SeedWork;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.Models;
namespace Pi.WalletService.Application.Tests.Commands.Deposit;

public class LogDepositTransactionTests : ConsumerTest
{
    private readonly Mock<ITransactionHistoryRepository> _transactionHistoryRepository;
    private readonly DepositTransactionSnapshot _depositTransactionSnapshot;
    private readonly Mock<ILogger<LogDepositTransactionConsumer>> _logger;
    public LogDepositTransactionTests()
    {
        _logger = new Mock<ILogger<LogDepositTransactionConsumer>>();
        _transactionHistoryRepository = new Mock<ITransactionHistoryRepository>();
        _transactionHistoryRepository
            .Setup(t => t.UnitOfWork)
            .Returns(new Mock<IUnitOfWork>().Object);

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<LogDepositTransactionConsumer>(); })
            .AddScoped<ITransactionHistoryRepository>(_ => _transactionHistoryRepository.Object)
            .AddScoped<ILogger<LogDepositTransactionConsumer>>(_ => _logger.Object)
            .BuildServiceProvider(true);

        _depositTransactionSnapshot = new DepositTransactionSnapshot(
            Guid.NewGuid(),
            Guid.NewGuid().ToString(),
            DepositState.GetName(() => DepositState.DepositFailed),
            "7711431",
            "transaction_no",
            200,
            "77114311",
            Channel.QR,
            Product.Cash,
            Purpose.Collateral,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );
    }

    [Fact]
    public async void Should_Able_To_Log_Correctly_When_Call_LogDeposit_Transaction()
    {
        // Arrange
        var client = Harness.GetRequestClient<LogDepositTransaction>();

        // Act
        var response = await client.GetResponse<LogDepositTransactionSuccess>(new LogDepositTransaction(_depositTransactionSnapshot));

        // Assert
        Assert.Equal("transaction_no", response.Message.TransactionNo);
        _transactionHistoryRepository.Verify(u => u.Create(It.IsAny<TransactionHistory>()), Times.Once);
        _transactionHistoryRepository.Verify(u => u.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async void Should_Able_To_Log_Correctly_When_Call_LogDepositTransaction_Transaction()
    {
        // Arrange
        var client = Harness.GetRequestClient<LogDepositTransaction>();

        // Act
        var response = await client.GetResponse<LogDepositTransactionSuccess>(new LogDepositTransaction(_depositTransactionSnapshot));

        // Assert
        Assert.Equal("transaction_no", response.Message.TransactionNo);
    }

    [Fact]
    public async void Should_Able_Handle_Exception()
    {
        // Arrange
        _transactionHistoryRepository
            .Setup(t => t.Create(It.IsAny<TransactionHistory>()))
            .Throws(new Exception("Some Exception"));

        var client = Harness.GetRequestClient<LogDepositTransaction>();

        // Act
        var response = await client.GetResponse<LogDepositTransactionSuccess>(new LogDepositTransaction(_depositTransactionSnapshot));

        // Assert
        _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);

        Assert.Equal("transaction_no", response.Message.TransactionNo);
    }
}