using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.BackofficeService.Application.Commands.Transaction;
using Pi.BackofficeService.Application.Models.Customer;
using Pi.BackofficeService.Application.Services.UserService;
using Pi.BackofficeService.Application.Services.WalletService;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransferCashAggregate;
using Pi.BackofficeService.Domain.Exceptions;

namespace Pi.BackofficeService.Application.Tests.Commands.Transaction;

public class FetchTransactionConsumerTest : ConsumerTest
{
    private readonly Mock<IDepositWithdrawService> _depositWithdrawService;
    private readonly Mock<ITransferCashService> _transferCashService;
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IResponseCodeRepository> _responseCodeRepository;

    public FetchTransactionConsumerTest()
    {
        _depositWithdrawService = new Mock<IDepositWithdrawService>();
        _transferCashService = new Mock<ITransferCashService>();
        _userService = new Mock<IUserService>();
        _responseCodeRepository = new Mock<IResponseCodeRepository>();

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<FetchTransactionConsumer>(); })
            .AddScoped<IDepositWithdrawService>(_ => _depositWithdrawService.Object)
            .AddScoped<ITransferCashService>(_ => _transferCashService.Object)
            .AddScoped<IResponseCodeRepository>(_ => _responseCodeRepository.Object)
            .AddScoped<IUserService>(_ => _userService.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Return_FetchTransactionResponse_When_Request_FetchTransactionMessage_With_DepositType()
    {
        // Arrange
        var transaction = new TransactionV2
        {
            Id = Guid.NewGuid(),
            Channel = Channel.AtsBatch,
            TransactionNo = "GE117700",
            BankAccount = "accountNo",
            Status = "pending",
            CustomerCode = "custCode",
            CustomerName = "custName",
            Product = Product.CashBalance
        };
        var client = Harness.GetRequestClient<FetchTransactionMessage>();
        _depositWithdrawService
            .Setup(q => q.GetTransactionV2ByTransactionNo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        // Act
        var response = await client.GetResponse<FetchTransactionResponse>(
            new FetchTransactionMessage(transaction.TransactionNo, TransactionType.Deposit)
        );

        // Assert
        Assert.Equal(transaction.Id, response.Message.TransactionId);
    }

    [Fact]
    public async void Should_Return_ExpectedCustomerName_When_Request_FetchTransactionMessage_With_DepositType()
    {
        // Arrange
        var transaction = new TransactionV2
        {
            Id = Guid.NewGuid(),
            Channel = Channel.AtsBatch,
            TransactionNo = "GE117700",
            BankAccount = "accountNo",
            Status = "pending",
            CustomerCode = "custCode",
            CustomerName = "custName",
            Product = Product.TFEX

        };
        var user = new CustomerDto(Guid.NewGuid().ToString(), new List<DeviceDto>(), new List<CustomerCodeDto>(),
            new List<TradingAccountDto>(), "FirstTh", "LastTh", "FirstEn", "LastEn", null, null, null);
        var client = Harness.GetRequestClient<FetchTransactionMessage>();
        _depositWithdrawService
            .Setup(q => q.GetTransactionV2ByTransactionNo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);
        _userService.Setup(q => q.GetUserByIdOrCustomerCode(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(user);

        // Act
        var response = await client.GetResponse<FetchTransactionResponse>(
            new FetchTransactionMessage(transaction.TransactionNo, TransactionType.Deposit)
        );

        // Assert
        Assert.Equal($"{user.FirstnameTh} {user.LastnameTh} {user.FirstnameEn} {user.LastnameEn}", response.Message.CustomerName);
    }

    [Fact]
    public async void Should_Return_FetchTransactionResponse_With_ResponseCode_When_Transaction_CurrentState_Had_ResponseCode_With_DepositType()
    {
        // Arrange
        var transaction = new TransactionV2
        {
            Id = Guid.NewGuid(),
            TransactionType = TransactionType.Deposit,
            TransactionNo = "GE117700",
            Channel = Channel.AtsBatch,
            State = "SomeErrorState",
            BankAccount = "accountNo",
            Status = "pending",
            CustomerCode = "custCode",
            CustomerName = "custName",
            Product = Product.GlobalEquity
        };
        var responseCode = new ResponseCode() { Id = Guid.NewGuid() };
        var client = Harness.GetRequestClient<FetchTransactionMessage>();
        _depositWithdrawService
            .Setup(q => q.GetTransactionV2ByTransactionNo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);
        _responseCodeRepository
            .Setup(q => q.GetByStateMachine(It.IsAny<Machine>(), It.IsAny<string>(), It.IsAny<ProductType>()))
            .ReturnsAsync(responseCode);

        // Act
        var response = await client.GetResponse<FetchTransactionResponse>(
            new FetchTransactionMessage(transaction.TransactionNo, transaction.TransactionType)
        );

        // Assert
        Assert.Equal(responseCode.Id, response.Message.ResponseCodeId);
    }

    [Fact]
    public async void Should_Throw_NotFoundException_When_Transaction_NotFound_With_DepositType()
    {
        // Arrange
        var transactionNo = "GE117700";
        var client = Harness.GetRequestClient<FetchTransactionMessage>();
        _depositWithdrawService
            .Setup(q => q.GetTransactionV2ByTransactionNo(It.IsAny<string>(), It.IsAny<CancellationToken>()));

        // Act
        var action = async () => await client.GetResponse<FetchTransactionResponse>(
            new FetchTransactionMessage(transactionNo, TransactionType.Deposit)
        );

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(action);
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(NotFoundException).ToString())));
    }

    [Fact]
    public async void Should_Return_FetchTransactionResponse_When_Request_FetchTransactionMessage_With_WithdrawType()
    {
        // Arrange
        var transaction = new TransactionV2
        {
            Id = Guid.NewGuid(),
            TransactionType = TransactionType.Withdraw,
            TransactionNo = "GE117700",
            Channel = Channel.AtsBatch,
            State = "SomeErrorState",
            BankAccount = "accountNo",
            Status = "pending",
            CustomerCode = "custCode",
            CustomerName = "custName",
            Product = Product.CashBalance
        };
        var client = Harness.GetRequestClient<FetchTransactionMessage>();
        _depositWithdrawService
            .Setup(q => q.GetTransactionV2ByTransactionNo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        // Act
        var response = await client.GetResponse<FetchTransactionResponse>(
            new FetchTransactionMessage(transaction.TransactionNo, TransactionType.Withdraw)
        );

        // Assert
        Assert.Equal(transaction.Id, response.Message.TransactionId);
    }

    [Fact]
    public async void Should_Return_FetchTransactionResponse_With_ResponseCode_When_Transaction_CurrentState_Had_ResponseCode_With_WithdrawType()
    {
        // Arrange
        var transaction = new TransactionV2
        {
            Id = Guid.NewGuid(),
            TransactionType = TransactionType.Withdraw,
            TransactionNo = "GE117700",
            Channel = Channel.AtsBatch,
            State = "SomeErrorState",
            BankAccount = "accountNo",
            Status = "pending",
            CustomerCode = "custCode",
            CustomerName = "custName",
            Product = Product.CashBalance
        };
        var responseCode = new ResponseCode() { Id = Guid.NewGuid() };
        var client = Harness.GetRequestClient<FetchTransactionMessage>();
        _depositWithdrawService
            .Setup(q => q.GetTransactionV2ByTransactionNo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);
        _responseCodeRepository
            .Setup(q => q.GetByStateMachine(It.IsAny<Machine>(), It.IsAny<string>(), It.IsAny<ProductType>()))
            .ReturnsAsync(responseCode);

        // Act
        var response = await client.GetResponse<FetchTransactionResponse>(
            new FetchTransactionMessage(transaction.TransactionNo, transaction.TransactionType)
        );

        // Assert
        Assert.Equal(responseCode.Id, response.Message.ResponseCodeId);
    }

    [Fact]
    public async void Should_Return_ExpectedCustomerName_When_Request_FetchTransactionMessage_With_WithdrawType()
    {
        // Arrange
        var transaction = new TransactionV2
        {
            Id = Guid.NewGuid(),
            TransactionType = TransactionType.Withdraw,
            TransactionNo = "GE117700",
            Channel = Channel.AtsBatch,
            State = "SomeErrorState",
            BankAccount = "accountNo",
            Status = "pending",
            CustomerCode = "custCode",
            CustomerName = "FirstTh LastTh FirstEn LastEn",
            Product = Product.CashBalance
        };
        var user = new CustomerDto(Guid.NewGuid().ToString(), new List<DeviceDto>(), new List<CustomerCodeDto>(),
            new List<TradingAccountDto>(), "FirstTh", "LastTh", "FirstEn", "LastEn", null, null, null);
        var client = Harness.GetRequestClient<FetchTransactionMessage>();
        _depositWithdrawService
            .Setup(q => q.GetTransactionV2ByTransactionNo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);
        _userService.Setup(q => q.GetUserByIdOrCustomerCode(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(user);

        // Act
        var response = await client.GetResponse<FetchTransactionResponse>(
            new FetchTransactionMessage(transaction.TransactionNo, TransactionType.Withdraw)
        );

        // Assert
        Assert.Equal($"{user.FirstnameTh} {user.LastnameTh} {user.FirstnameEn} {user.LastnameEn}", response.Message.CustomerName);
    }

    [Fact]
    public async void Should_Throw_NotFoundException_When_Transaction_NotFound_With_WithdrawType()
    {
        // Arrange
        var transactionNo = "GE117700";
        var client = Harness.GetRequestClient<FetchTransactionMessage>();
        _depositWithdrawService
            .Setup(q => q.GetTransactionV2ByTransactionNo(It.IsAny<string>(), It.IsAny<CancellationToken>()));

        // Act
        var action = async () => await client.GetResponse<FetchTransactionResponse>(
            new FetchTransactionMessage(transactionNo, TransactionType.Withdraw)
        );

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(action);
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(NotFoundException).ToString())));
    }

    [Fact]
    public async void Should_Return_ExpectedCustomerName_When_Request_FetchTransactionMessage_With_TransferCashType()
    {
        // Arrange
        var transaction = new TransferCash
        {
            Id = Guid.NewGuid(),
            State = "SomeState",
            TransactionNo = "TFC123456",
            Status = "pending",
            CustomerName = "John Doe",
            TransferFromAccountCode = "12345671",
            TransferToAccountCode = "12345678",
            TransferFromExchangeMarket = Product.Cash,
            TransferToExchangeMarket = Product.CashBalance,
            Amount = 100,
            FailedReason = null,
            OtpConfirmedDateTime = DateTime.Now,
            CreatedAt = DateTime.Now
        };
        var client = Harness.GetRequestClient<FetchTransactionMessage>();
        _transferCashService
            .Setup(q => q.GetTransferCashByTransactionNo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        // Act
        var response = await client.GetResponse<FetchTransactionResponse>(
            new FetchTransactionMessage(transaction.TransactionNo, TransactionType.TransferCash)
        );

        // Assert
        Assert.Equal(transaction.CustomerName, response.Message.CustomerName);
    }

    [Fact]
    public async void Should_Return_FetchTransactionResponse_With_ResponseCode_When_Transaction_CurrentState_Had_ResponseCode_With_TransferCashType()
    {
        // Arrange
        var transaction = new TransferCash
        {
            Id = Guid.NewGuid(),
            State = "SomeState",
            TransactionNo = "TFC123456",
            Status = "pending",
            CustomerName = "John Doe",
            TransferFromAccountCode = "12345671",
            TransferToAccountCode = "12345678",
            TransferFromExchangeMarket = Product.Cash,
            TransferToExchangeMarket = Product.CashBalance,
            Amount = 100,
            FailedReason = null,
            OtpConfirmedDateTime = DateTime.Now,
            CreatedAt = DateTime.Now
        };
        var responseCode = new ResponseCode() { Id = Guid.NewGuid() };
        var client = Harness.GetRequestClient<FetchTransactionMessage>();
        _transferCashService
            .Setup(q => q.GetTransferCashByTransactionNo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);
        _responseCodeRepository
            .Setup(q => q.GetByStateMachine(It.IsAny<Machine>(), It.IsAny<string>(), null))
            .ReturnsAsync(responseCode);

        // Act
        var response = await client.GetResponse<FetchTransactionResponse>(
            new FetchTransactionMessage(transaction.TransactionNo, TransactionType.TransferCash)
        );

        // Assert
        Assert.Equal(responseCode.Id, response.Message.ResponseCodeId);
    }

    [Fact]
    public async void Should_Throw_NotFoundException_When_Transaction_NotFound_With_TransferCashType()
    {
        // Arrange
        var transactionNo = "TFC123456";
        var client = Harness.GetRequestClient<FetchTransactionMessage>();
        _transferCashService
            .Setup(q => q.GetTransferCashByTransactionNo(It.IsAny<string>(), It.IsAny<CancellationToken>()));

        // Act
        var action = async () => await client.GetResponse<FetchTransactionResponse>(
            new FetchTransactionMessage(transactionNo, TransactionType.TransferCash)
        );

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(action);
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(NotFoundException).ToString())));
    }
}
