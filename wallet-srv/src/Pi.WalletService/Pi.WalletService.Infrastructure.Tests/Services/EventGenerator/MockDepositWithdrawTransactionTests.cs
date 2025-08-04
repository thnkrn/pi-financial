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
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using CashWithdrawStateName = Pi.WalletService.IntegrationEvents.Models.CashWithdrawState;
using CashDepositStateName = Pi.WalletService.IntegrationEvents.Models.CashDepositState;
using WithdrawStateName = Pi.WalletService.IntegrationEvents.Models.WithdrawState;

namespace Pi.WalletService.Infrastructure.Tests.Services.EventGenerator;

public class MockDepositWithdrawTransactionTests
{
    private readonly Mock<IDepositEntrypointRepository> _depositEntrypointRepository;
    private readonly Mock<IWithdrawEntrypointRepository> _withdrawEntrypointRepository;
    private readonly Mock<IQrDepositRepository> _qrDepositRepository;
    private readonly Mock<IWithdrawRepository> _withdrawRepository;
    private readonly Mock<ICashDepositRepository> _cashDepositRepository;
    private readonly Mock<ICashWithdrawRepository> _cashWithdrawRepository;
    private readonly Mock<IGlobalTransferRepository> _globalTransferRepository;
    private readonly Mock<ITransactionRepository> _transactionRepository;
    private readonly EventGeneratorService _eventGeneratorService;
    private readonly CancellationToken _cancellation;

    public MockDepositWithdrawTransactionTests()
    {
        _depositEntrypointRepository = new();
        _withdrawEntrypointRepository = new();
        _qrDepositRepository = new();
        _cashWithdrawRepository = new();
        _cashDepositRepository = new();
        _globalTransferRepository = new();
        _transactionRepository = new();
        _cancellation = new CancellationToken();
        _withdrawRepository = new();
        Mock<IDepositRepository> depositRepository = new();
        Mock<IGlobalWalletDepositRepository> globalWalletRepository = new();
        Mock<IUserService> userService = new();
        Mock<IBus> bus = new();
        Mock<IOptions<MockOptions>> mockOptions = new();

        mockOptions.Setup(x => x.Value).Returns(new MockOptions
        {
            MaxPollingWaitingTimeInSeconds = 1,
        });

        _eventGeneratorService = new EventGeneratorService(
            bus.Object,
            _depositEntrypointRepository.Object,
            _withdrawEntrypointRepository.Object,
            _qrDepositRepository.Object,
            depositRepository.Object,
            _cashWithdrawRepository.Object,
            _cashDepositRepository.Object,
            _withdrawRepository.Object,
            globalWalletRepository.Object,
            _globalTransferRepository.Object,
            _transactionRepository.Object,
            userService.Object,
            mockOptions.Object);
    }

    [Theory]
    [InlineData(TransactionType.Withdraw, Product.Cash, MockDepositWithdrawReasons.FreewillFailed)]
    [InlineData(TransactionType.Withdraw, Product.Derivatives, MockDepositWithdrawReasons.TfexFailed)]
    public async Task Should_Return_True_When_Withdraw_And_Mock_FreewillOrTfexFailed(TransactionType transactionType, Product product, MockDepositWithdrawReasons reasons)
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";
        _cashWithdrawRepository.Setup(
            c => c.UpdateAccountCodeByTransactionNoAndState(
                It.IsAny<string>(),
                CashWithdrawStateName.GetName(() => CashWithdrawStateName.CashWithdrawWaitingForOtpValidation),
                It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await _eventGeneratorService.MockDepositWithdrawTransaction(transactionNo, transactionType, product, reasons, _cancellation);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Should_Return_True_When_Withdraw_And_Mock_KkpFailed()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";
        _withdrawRepository.Setup(
            c => c.UpdateBankAccountByTransactionNoAndState(
                It.IsAny<string>(),
                WithdrawStateName.GetName(() => WithdrawStateName.WaitingForConfirmation),
                It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await _eventGeneratorService.MockDepositWithdrawTransaction(transactionNo, TransactionType.Withdraw, Product.Cash, MockDepositWithdrawReasons.KkpFailed, _cancellation);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Should_Return_True_When_Deposit_And_Mock_FreewillFailed()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";
        _cashDepositRepository.Setup(
            c => c.UpdateAccountCodeByTransactionNoAndState(
                It.IsAny<string>(),
                CashDepositStateName.GetName(() => CashDepositStateName.Received),
                It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await _eventGeneratorService.MockDepositWithdrawTransaction(transactionNo, TransactionType.Deposit, Product.Cash, MockDepositWithdrawReasons.FreewillFailed, _cancellation);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Should_Return_True_When_Deposit_And_Mock_TfexFailed()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";
        _cashDepositRepository.Setup(
            c => c.UpdateAccountCodeByTransactionNoAndState(
                It.IsAny<string>(),
                CashDepositStateName.GetName(() => CashDepositStateName.CashDepositWaitingForGateway),
                It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await _eventGeneratorService.MockDepositWithdrawTransaction(transactionNo, TransactionType.Deposit, Product.Derivatives, MockDepositWithdrawReasons.TfexFailed, _cancellation);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(TransactionType.Withdraw, Product.Cash, MockDepositWithdrawReasons.TfexFailed)]
    [InlineData(TransactionType.Deposit, Product.Cash, MockDepositWithdrawReasons.TfexFailed)]
    public async Task Should_Throw_Exception_When_Product_IsNot_Derivatives_And_Mock_TfexFailed(TransactionType transactionType, Product product, MockDepositWithdrawReasons reasons)
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";

        // Act && Assert
        await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await _eventGeneratorService.MockDepositWithdrawTransaction(transactionNo, transactionType, product, reasons, _cancellation));
    }

    [Fact]
    public async Task Should_Return_False_When_RecordNotUpdate_Within_MaxPollingWaitingTime()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";
        _cashDepositRepository.Setup(
            c => c.UpdateAccountCodeByTransactionNoAndState(
                It.IsAny<string>(),
                CashDepositStateName.GetName(() => CashDepositStateName.Received),
                It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await _eventGeneratorService.MockDepositWithdrawTransaction(transactionNo, TransactionType.Withdraw, Product.Cash, MockDepositWithdrawReasons.KkpFailed, _cancellation);

        // Assert
        Assert.False(result);
    }


    [Fact]
    public async Task Should_Return_False_When_RecordNotUpdate_And_CancelAfter_100Ms()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";
        _cashDepositRepository.Setup(
            c => c.UpdateAccountCodeByTransactionNoAndState(
                It.IsAny<string>(),
                CashDepositStateName.GetName(() => CashDepositStateName.Received),
                It.IsAny<string>())).ReturnsAsync(false);
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);
        var newCancellationToken = cancellationTokenSource.Token;

        // Act && Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await _eventGeneratorService.MockDepositWithdrawTransaction(transactionNo, TransactionType.Deposit, Product.Cash, MockDepositWithdrawReasons.FreewillFailed, newCancellationToken));
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Deposit_And_Mock_KkpFailed()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";

        // Act && Assert
        await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await _eventGeneratorService.MockDepositWithdrawTransaction(transactionNo, TransactionType.Deposit, Product.Cash, MockDepositWithdrawReasons.KkpFailed, _cancellation));
    }

    [Fact]
    public async Task Should_Throw_Exception_When_GlobalEquities_Mock_FreewillFailed()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";

        // Act && Assert
        await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await _eventGeneratorService.MockDepositWithdrawTransaction(transactionNo, TransactionType.Deposit, Product.GlobalEquities, MockDepositWithdrawReasons.FreewillFailed, _cancellation));
    }

}
