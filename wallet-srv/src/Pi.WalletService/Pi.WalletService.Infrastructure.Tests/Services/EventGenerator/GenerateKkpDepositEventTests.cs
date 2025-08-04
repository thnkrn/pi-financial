using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.Infrastructure.Services;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.Tests.Services.EventGenerator;

public class GenerateKkpDepositEventTests
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

    public GenerateKkpDepositEventTests()
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
    public async Task Should_Return_KkpDeposit_When_Mock_DepositCompleted()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";
        var eventName = MockQrDepositEventState.DepositCompleted;
        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>(),
            "Unit",
            "Test",
            "Unit",
            "Test",
            string.Empty,
            string.Empty,
            string.Empty);
        var deposit = new DepositState()
        {
            CustomerCode = "1234567890",
            Product = Product.Cash,
            QrTransactionNo = "QR1234567890",
            RequestedAmount = (decimal)100.00
        };

        _userService.Setup(u => u.GetUserInfoByCustomerCode(It.IsAny<string>())).ReturnsAsync(user);
        _depositRepository.Setup(u => u.GetByTransactionNo(It.IsAny<string>())).ReturnsAsync(deposit);

        var result = await _eventGeneratorService.GenerateKkpDepositEvent(transactionNo, eventName, _cancellation);

        Assert.True(result.IsSuccess);
        Assert.Equal($"{user.FirstnameEn} {user.LastnameEn}", result.PayerName);
        Assert.Equal(deposit.QrTransactionNo, result.TransactionNo);
    }

    [Fact]
    public async Task Should_Return_KkpDeposit_When_Mock_DepositFailed()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";
        var eventName = MockQrDepositEventState.DepositFailed;
        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>(),
            "Unit",
            "Test",
            "Unit",
            "Test",
            string.Empty,
            string.Empty,
            string.Empty);
        var deposit = new DepositState()
        {
            CustomerCode = "1234567890",
            Product = Product.Cash,
            QrTransactionNo = "QR1234567890",
            RequestedAmount = (decimal)100.00
        };

        _userService.Setup(u => u.GetUserInfoByCustomerCode(It.IsAny<string>())).ReturnsAsync(user);
        _depositRepository.Setup(u => u.GetByTransactionNo(It.IsAny<string>())).ReturnsAsync(deposit);

        var result = await _eventGeneratorService.GenerateKkpDepositEvent(transactionNo, eventName, _cancellation);

        Assert.False(result.IsSuccess);
        Assert.Equal($"{user.FirstnameEn} {user.LastnameEn}", result.PayerName);
        Assert.Equal(deposit.QrTransactionNo, result.TransactionNo);
    }

    [Fact]
    public async Task Should_Return_KkpDeposit_When_Mock_DepositFailedNameMismatch()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";
        var eventName = MockQrDepositEventState.DepositFailedNameMismatch;
        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>(),
            "Unit",
            "Test",
            "Unit",
            "Test",
            string.Empty,
            string.Empty,
            string.Empty);
        var deposit = new DepositState()
        {
            CustomerCode = "1234567890",
            Product = Product.Cash,
            QrTransactionNo = "QR1234567890",
            RequestedAmount = (decimal)100.00
        };

        _userService.Setup(u => u.GetUserInfoByCustomerCode(It.IsAny<string>())).ReturnsAsync(user);
        _depositRepository.Setup(u => u.GetByTransactionNo(It.IsAny<string>())).ReturnsAsync(deposit);

        var result = await _eventGeneratorService.GenerateKkpDepositEvent(transactionNo, eventName, _cancellation);

        Assert.True(result.IsSuccess);
        Assert.Equal("wrong name", result.PayerName);
        Assert.Equal(deposit.QrTransactionNo, result.TransactionNo);
    }

    [Fact]
    public async Task Should_Return_KkpDeposit_When_Mock_DepositFailedInvalidSource()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";
        var eventName = MockQrDepositEventState.DepositFailedInvalidSource;
        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>(),
            "Unit",
            "Test",
            "Unit",
            "Test",
            string.Empty,
            string.Empty,
            string.Empty);
        var deposit = new DepositState()
        {
            CustomerCode = "1234567890",
            Product = Product.Cash,
            QrTransactionNo = "QR1234567890",
            RequestedAmount = (decimal)100.00
        };

        _userService.Setup(u => u.GetUserInfoByCustomerCode(It.IsAny<string>())).ReturnsAsync(user);
        _depositRepository.Setup(u => u.GetByTransactionNo(It.IsAny<string>())).ReturnsAsync(deposit);

        var result = await _eventGeneratorService.GenerateKkpDepositEvent(transactionNo, eventName, _cancellation);

        Assert.True(result.IsSuccess);
        Assert.Equal("บจก.ทรู มันนี่ เพื่อเก็บรักษาเงินรับล่วง", result.PayerName);
        Assert.Equal(deposit.QrTransactionNo, result.TransactionNo);
    }

    [Fact]
    public async Task Should_Return_KkpDeposit_When_Mock_InvalidPaymentDateTime()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";
        var eventName = MockQrDepositEventState.InvalidPaymentDateTime;
        var user = new User(
            Guid.NewGuid(),
            new List<string>(),
            new List<string>(),
            "Unit",
            "Test",
            "Unit",
            "Test",
            string.Empty,
            string.Empty,
            string.Empty);
        var deposit = new DepositState()
        {
            CustomerCode = "1234567890",
            Product = Product.Cash,
            QrTransactionNo = "QR1234567890",
            RequestedAmount = (decimal)100.00
        };
        var paymentDate = DateUtils.GetThDateTimeNow().AddDays(1).Day;
        _userService.Setup(u => u.GetUserInfoByCustomerCode(It.IsAny<string>())).ReturnsAsync(user);
        _depositRepository.Setup(u => u.GetByTransactionNo(It.IsAny<string>())).ReturnsAsync(deposit);

        // Act
        var result = await _eventGeneratorService.GenerateKkpDepositEvent(transactionNo, eventName, _cancellation);

        // Asserts
        Assert.True(result.IsSuccess);
        Assert.Equal(paymentDate, DateTime.ParseExact(result.PaymentDateTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture).Day);
        Assert.Equal(deposit.QrTransactionNo, result.TransactionNo);
    }

    [Fact]
    public async Task Should_Throws_InvalidDataException_When_Transaction_NotExists()
    {
        //Arrange
        var transactionNo = "DHDW00000000000001";
        var eventName = MockQrDepositEventState.DepositCompleted;

        await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await _eventGeneratorService.GenerateKkpDepositEvent(transactionNo, eventName, _cancellation));
    }
}
