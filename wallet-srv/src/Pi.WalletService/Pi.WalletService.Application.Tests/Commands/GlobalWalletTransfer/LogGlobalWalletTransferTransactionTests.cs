using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.SeedWork;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.WalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.Models;
using GlobalWalletTransferState = Pi.WalletService.IntegrationEvents.Models.GlobalWalletTransferState;

namespace Pi.WalletService.Application.Tests.Commands.GlobalWalletTransfer;

public class LogGlobalWalletTransferTransactionTests : ConsumerTest
{
    private readonly Mock<IGlobalWalletTransactionHistoryRepository> _globalWalletTransactionHistoryRepository;
    private readonly GlobalWalletTransferTransactionSnapshot _globalWalletTransferTransactionSnapshot;
    private readonly Mock<ILogger<LogGlobalWalletTransferTransactionConsumer>> _logger;

    public LogGlobalWalletTransferTransactionTests()
    {
        _logger = new Mock<ILogger<LogGlobalWalletTransferTransactionConsumer>>();
        _globalWalletTransactionHistoryRepository = new Mock<IGlobalWalletTransactionHistoryRepository>();
        _globalWalletTransactionHistoryRepository
            .Setup(t => t.UnitOfWork)
            .Returns(new Mock<IUnitOfWork>().Object);

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<LogGlobalWalletTransferTransactionConsumer>(); })
            .AddScoped<IGlobalWalletTransactionHistoryRepository>(_ => _globalWalletTransactionHistoryRepository.Object)
            .AddScoped<ILogger<LogGlobalWalletTransferTransactionConsumer>>(_ => _logger.Object)
            .BuildServiceProvider(true);

        _globalWalletTransferTransactionSnapshot = new GlobalWalletTransferTransactionSnapshot(
            Guid.NewGuid().ToString(),
            GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.DepositProcessing),
            123456,
            "7711431",
            "global_account",
            Guid.NewGuid(),
            "transaction_no",
            TransactionType.Deposit,
            200,
            Currency.THB,
            25,
            Currency.USD,
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
        var client = Harness.GetRequestClient<LogGlobalWalletTransferTransaction>();

        var response = await client.GetResponse<LogGlobalWalletTransferTransactionSuccess>(new LogGlobalWalletTransferTransaction(_globalWalletTransferTransactionSnapshot));

        Assert.Equal(_globalWalletTransferTransactionSnapshot.TransactionNo, response.Message.TransactionNo);
        _globalWalletTransactionHistoryRepository.Verify(u => u.Create(It.IsAny<GlobalWalletTransactionHistory>()), Times.Once);
        _globalWalletTransactionHistoryRepository.Verify(u => u.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async void Should_Able_To_Log_Correctly_When_Call_LogDepositTransaction_Transaction()
    {
        // Arrange
        var client = Harness.GetRequestClient<LogGlobalWalletTransferTransaction>();

        // Act
        var response = await client.GetResponse<LogGlobalWalletTransferTransactionSuccess>(new LogGlobalWalletTransferTransaction(_globalWalletTransferTransactionSnapshot));

        // Assert
        Assert.Equal("transaction_no", response.Message.TransactionNo);
    }

    [Fact]
    public async void Should_Able_Handle_Exception()
    {
        // Arrange
        _globalWalletTransactionHistoryRepository
            .Setup(t => t.Create(It.IsAny<GlobalWalletTransactionHistory>()))
            .Throws(new Exception("Some Exception"));

        var client = Harness.GetRequestClient<LogGlobalWalletTransferTransaction>();

        // Act
        var response = await client.GetResponse<LogGlobalWalletTransferTransactionSuccess>(new LogGlobalWalletTransferTransaction(_globalWalletTransferTransactionSnapshot));

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