using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Infrastructure.Services;
using GlobalWalletTransferState = Pi.WalletService.IntegrationEvents.Models.GlobalWalletTransferState;

namespace Pi.WalletService.Infrastructure.Tests.Services.EventGenerator;

public class MockGlobalTransactionTests
{
    private readonly Mock<IDepositEntrypointRepository> _depositEntrypointRepository;
    private readonly Mock<IWithdrawEntrypointRepository> _withdrawEntrypointRepository;
    private readonly Mock<IQrDepositRepository> _qrDepositRepository;
    private readonly Mock<IDepositRepository> _depositRepository;
    private readonly Mock<IWithdrawRepository> _withdrawRepository;
    private readonly Mock<ICashDepositRepository> _cashDepositRepository;
    private readonly Mock<ICashWithdrawRepository> _cashWithdrawRepository;
    private readonly Mock<IGlobalWalletDepositRepository> _globalWalletRepository;
    private readonly Mock<IGlobalTransferRepository> _globalTransferRepository;
    private readonly Mock<ITransactionRepository> _transactionRepository;
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IBus> _bus;
    private readonly Mock<IOptions<MockOptions>> _mockOptions;
    private readonly EventGeneratorService _eventGeneratorService;
    private readonly CancellationToken _cancellation;

    public MockGlobalTransactionTests()
    {
        _depositEntrypointRepository = new();
        _withdrawEntrypointRepository = new();
        _qrDepositRepository = new();
        _cashWithdrawRepository = new();
        _cashDepositRepository = new();
        _depositRepository = new();
        _withdrawRepository = new();
        _globalWalletRepository = new();
        _globalTransferRepository = new();
        _transactionRepository = new();
        _userService = new();
        _bus = new();
        _mockOptions = new();
        _cancellation = new CancellationToken();

        _mockOptions.Setup(x => x.Value).Returns(new MockOptions
        {
            MaxPollingWaitingTimeInSeconds = 1,
        });

        _eventGeneratorService = new EventGeneratorService(
            _bus.Object,
            _depositEntrypointRepository.Object,
            _withdrawEntrypointRepository.Object,
            _qrDepositRepository.Object,
            _depositRepository.Object,
            _cashWithdrawRepository.Object,
            _cashDepositRepository.Object,
            _withdrawRepository.Object,
            _globalWalletRepository.Object,
            _globalTransferRepository.Object,
            _transactionRepository.Object,
            _userService.Object,
            _mockOptions.Object);
    }

    [Fact]
    public async Task Should_Return_True_When_Mock_FxTransferFailed()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";

        // Act
        var result = await _eventGeneratorService.MockGlobalTransaction(transactionNo, MockGlobalTransactionReasons.FxTransferFailed, _cancellation);

        // Assert
        Assert.True(result);
        _globalWalletRepository.Verify(r => r.UpdateGlobalAccountByTransactionNo(
            It.IsAny<string>(),
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Should_Return_True_When_Mock_FxRateCompareFailed()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";

        // Act
        var result = await _eventGeneratorService.MockGlobalTransaction(transactionNo, MockGlobalTransactionReasons.FxRateCompareFailed, _cancellation);

        // Assert
        Assert.True(result);
        _globalWalletRepository.Verify(r => r.UpdateRequestedFxAmountByTransactionNo(
            It.IsAny<string>(),
            It.IsAny<decimal>()), Times.Once);
    }

    [Fact]
    public async Task Should_Throws_InvalidDataException_When_Mock_FxRateCompareFailed_And_TransactionNo_Invalid()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";
        _globalWalletRepository.Setup(
            c => c.UpdateRequestedFxAmountByTransactionNo(
                It.IsAny<string>(),
                It.IsAny<decimal>())).Throws(new KeyNotFoundException("Transaction No Invalid"));

        // Act && Assert
        await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await _eventGeneratorService.MockGlobalTransaction(transactionNo, MockGlobalTransactionReasons.FxRateCompareFailed, _cancellation));
    }

    [Fact]
    public async Task Should_Return_True_When_Mock_RevertTransferFailed()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";
        _globalWalletRepository.Setup(
            c => c.UpdateGlobalAccountByTransactionNoAndState(
                It.IsAny<string>(),
                GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.WithdrawalProcessing),
                It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await _eventGeneratorService.MockGlobalTransaction(transactionNo, MockGlobalTransactionReasons.RevertTransferFailed, _cancellation);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Should_Return_True_When_Mock_TransferRequestFailed()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";
        _globalWalletRepository.Setup(
            c => c.UpdateGlobalAccountByTransactionNoAndState(
                It.IsAny<string>(),
                GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.TransferRequestValidating),
                It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await _eventGeneratorService.MockGlobalTransaction(transactionNo, MockGlobalTransactionReasons.TransferRequestFailed, _cancellation);

        // Assert
        Assert.True(result);
    }
}
