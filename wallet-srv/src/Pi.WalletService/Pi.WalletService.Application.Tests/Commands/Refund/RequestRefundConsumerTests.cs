using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.WalletService.Application.Commands.Refund;
using Pi.WalletService.Application.Commands.Withdraw;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.PaymentService;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.RefundInfoAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.Refund;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Transaction = Pi.WalletService.Domain.AggregatesModel.TransactionAggregate.Transaction;

namespace Pi.WalletService.Application.Tests.Commands.Refund;

public class RequestRefundConsumerTests : ConsumerTest
{
    private readonly Mock<IDepositEntrypointRepository> _depositEntrypointRepository;
    private readonly Mock<ITransactionQueriesV2> _transactionQueriesV2;
    private readonly Mock<IRefundInfoRepository> _refundInfoRepository;
    private readonly Mock<IPaymentService> _paymentService;

    private const string MockTransactionNo = "TXN0001";

    public RequestRefundConsumerTests()
    {
        _depositEntrypointRepository = new Mock<IDepositEntrypointRepository>();
        _transactionQueriesV2 = new Mock<ITransactionQueriesV2>();
        _refundInfoRepository = new Mock<IRefundInfoRepository>();
        _paymentService = new Mock<IPaymentService>();
        Mock<IWithdrawEntrypointRepository> withdrawEntrypointRepository = new();
        Provider = new ServiceCollection()
            .AddScoped<IDepositEntrypointRepository>(_ => _depositEntrypointRepository.Object)
            .AddScoped<ITransactionQueriesV2>(_ => _transactionQueriesV2.Object)
            .AddScoped<IRefundInfoRepository>(_ => _refundInfoRepository.Object)
            .AddScoped<IPaymentService>(_ => _paymentService.Object)
            .AddScoped<IWithdrawEntrypointRepository>(_ => withdrawEntrypointRepository.Object)
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<RequestRefundConsumer>();
                cfg.AddConsumer<KkpWithdrawConsumer>();
            })
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Be_Throw_InvalidRequestException_When_DepositEntrypoint_Not_Found()
    {
        // Arrange
        var payload = new RequestRefund(MockTransactionNo);
        var client = Harness.GetRequestClient<RequestRefund>();

        _depositEntrypointRepository
            .Setup(q => q.GetByTransactionNo(payload.TransactionNo))
            .ReturnsAsync(() => null);

        // Act
        var response = await client.GetResponse<BusRequestFailed>(payload);

        // Assert
        Assert.True(await Harness.Consumed.Any<RequestRefund>());
        Assert.True(await Harness.Sent.Any<BusRequestFailed>());
        Assert.NotNull(response.Message);
        Assert.Equal(ErrorCodes.InvalidData, response.Message.ErrorCode);
        Assert.Equal("Transaction not found", response.Message.ErrorMessage);
    }

    [Fact]
    public async void Should_Be_Throw_InvalidRequestException_When_Transaction_Not_Found()
    {
        // Arrange
        var payload = new RequestRefund(MockTransactionNo);
        var client = Harness.GetRequestClient<RequestRefund>();

        var depositEntrypointState = new DepositEntrypointState
        {
            CorrelationId = new Guid(),
            TransactionNo = MockTransactionNo,
            UserId = "mockUserId",
            Product = Product.Cash
        };

        _depositEntrypointRepository
            .Setup(q => q.GetByTransactionNo(payload.TransactionNo))
            .ReturnsAsync(() => depositEntrypointState);
        _transactionQueriesV2
            .Setup(q => q.GetTransactionByTransactionNo(payload.TransactionNo, depositEntrypointState.Product, depositEntrypointState.UserId))
            .ReturnsAsync(() => null);

        // Act
        var response = await client.GetResponse<BusRequestFailed>(payload);

        // Assert
        Assert.True(await Harness.Consumed.Any<RequestRefund>());
        Assert.True(await Harness.Sent.Any<BusRequestFailed>());
        Assert.NotNull(response.Message);
        Assert.Equal(ErrorCodes.InvalidData, response.Message.ErrorCode);
        Assert.Equal("Transaction not found", response.Message.ErrorMessage);
    }

    [Theory]
    [MemberData(nameof(NotRefundableTransactionData), parameters: new object[] { 0, 4 })]
    public async void Should_Be_Throw_InvalidRequestException_When_Transaction_Is_Not_Refundable(Transaction transactionData)
    {
        // Arrange
        var payload = new RequestRefund(MockTransactionNo);
        var client = Harness.GetRequestClient<RequestRefund>();

        var depositEntrypointState = new DepositEntrypointState
        {
            CorrelationId = transactionData.CorrelationId,
            TransactionNo = MockTransactionNo,
            UserId = "mockUserId",
            Product = Product.Cash
        };

        _depositEntrypointRepository
            .Setup(q => q.GetByTransactionNo(payload.TransactionNo))
            .ReturnsAsync(() => depositEntrypointState);
        _transactionQueriesV2
            .Setup(q => q.GetTransactionByTransactionNo(payload.TransactionNo, depositEntrypointState.Product, depositEntrypointState.UserId))
            .ReturnsAsync(() => transactionData);

        // Act
        var response = await client.GetResponse<BusRequestFailed>(payload);

        // Assert
        Assert.True(await Harness.Consumed.Any<RequestRefund>());
        Assert.True(await Harness.Sent.Any<BusRequestFailed>());
        Assert.NotNull(response.Message);
        Assert.Equal(ErrorCodes.InvalidData, response.Message.ErrorCode);
        Assert.Equal("Transaction is not refundable", response.Message.ErrorMessage);
    }

    [Theory]
    [MemberData(nameof(NotRefundableTransactionData), parameters: new object[] { 4, 2 })]
    public async void Should_Be_Throw_InvalidRequestException_When_Transaction_Data_Is_Missing(Transaction transactionData)
    {
        // Arrange
        var payload = new RequestRefund(MockTransactionNo);
        var client = Harness.GetRequestClient<RequestRefund>();

        var depositEntrypointState = new DepositEntrypointState
        {
            CorrelationId = transactionData.CorrelationId,
            TransactionNo = MockTransactionNo,
            UserId = "mockUserId",
            Product = Product.Cash
        };

        _depositEntrypointRepository
            .Setup(q => q.GetByTransactionNo(payload.TransactionNo))
            .ReturnsAsync(() => depositEntrypointState);
        _transactionQueriesV2
            .Setup(q => q.GetTransactionByTransactionNo(payload.TransactionNo, depositEntrypointState.Product, depositEntrypointState.UserId))
            .ReturnsAsync(() => transactionData);

        // Act
        var response = await client.GetResponse<BusRequestFailed>(payload);

        // Assert
        Assert.True(await Harness.Consumed.Any<RequestRefund>());
        Assert.True(await Harness.Sent.Any<BusRequestFailed>());
        Assert.NotNull(response.Message);
        Assert.Equal(ErrorCodes.InvalidData, response.Message.ErrorCode);
        Assert.Equal("Transaction data is missing", response.Message.ErrorMessage);
    }

    [Theory]
    [MemberData(nameof(RefundableTransactionData))]
    public async void Should_Failed_when_Refund_Failed(Transaction transactionData)
    {
        // Arrange
        var payload = new RequestRefund(MockTransactionNo);
        var client = Harness.GetRequestClient<RequestRefund>();

        var depositEntrypointState = new DepositEntrypointState
        {
            CorrelationId = transactionData.CorrelationId,
            TransactionNo = MockTransactionNo,
            UserId = "mockUserId",
            Product = Product.Cash
        };

        var kkpWithdrawRequest = new KkpWithdrawRequest(
            transactionData.AccountCode,
            transactionData.GetPaymentReceivedAmount()!.Value,
            transactionData.BankAccountNo!,
            transactionData.BankCode!,
            transactionData.Product,
            transactionData.TransactionNo
        );

        _depositEntrypointRepository
            .Setup(q => q.GetByTransactionNo(payload.TransactionNo))
            .ReturnsAsync(() => depositEntrypointState);
        _transactionQueriesV2
            .Setup(q => q.GetTransactionByTransactionNo(payload.TransactionNo, depositEntrypointState.Product, depositEntrypointState.UserId))
            .ReturnsAsync(() => transactionData);
        _paymentService
            .Setup(p => p.TransferViaOdd(
                kkpWithdrawRequest.TransactionNo!,
                RoundingUtils.RoundTransactionValue(TransactionType.Withdraw, kkpWithdrawRequest.Amount, Currency.THB),
                TransactionType.Withdraw,
                kkpWithdrawRequest.BankCode,
                kkpWithdrawRequest.BankAccountNo,
                string.Empty,
                string.Empty,
                kkpWithdrawRequest.AccountCode,
                kkpWithdrawRequest.Product
            ))
            .ThrowsAsync(new Exception("Mocked exception"));

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<BusRequestFailed>(payload));


        // Assert
        Assert.True(await Harness.Consumed.Any<RequestRefund>());
        Assert.True(await Harness.Consumed.Any<KkpWithdrawRequest>());
        Assert.NotNull(exception.Message);
        Assert.Contains("Mocked exception", exception.Message);
    }

    [Theory]
    [MemberData(nameof(RefundableTransactionData))]
    public async void Should_Failed_When_Cannot_Create_RefundInfo(Transaction transactionData)
    {
        // Arrange
        var payload = new RequestRefund(MockTransactionNo);
        var client = Harness.GetRequestClient<RequestRefund>();

        var depositEntrypointState = new DepositEntrypointState
        {
            CorrelationId = transactionData.CorrelationId,
            TransactionNo = MockTransactionNo,
            UserId = "mockUserId",
            Product = Product.Cash
        };

        var kkpWithdrawRequest = new KkpWithdrawRequest(
            transactionData.AccountCode,
            transactionData.GetPaymentReceivedAmount()!.Value,
            transactionData.BankAccountNo!,
            transactionData.BankCode!,
            transactionData.Product,
            transactionData.TransactionNo
        );

        _depositEntrypointRepository
            .Setup(q => q.GetByTransactionNo(payload.TransactionNo))
            .ReturnsAsync(() => depositEntrypointState);
        _transactionQueriesV2
            .Setup(q => q.GetTransactionByTransactionNo(payload.TransactionNo, depositEntrypointState.Product, depositEntrypointState.UserId))
            .ReturnsAsync(() => transactionData);
        _paymentService
            .Setup(p => p.TransferViaOdd(
                kkpWithdrawRequest.TransactionNo!,
                RoundingUtils.RoundTransactionValue(TransactionType.Withdraw, kkpWithdrawRequest.Amount, Currency.THB),
                TransactionType.Withdraw,
                kkpWithdrawRequest.BankCode,
                kkpWithdrawRequest.BankAccountNo,
                string.Empty,
                string.Empty,
                kkpWithdrawRequest.AccountCode,
                kkpWithdrawRequest.Product
            ))
            .ReturnsAsync(() => "TransactionRef");
        _refundInfoRepository
            .Setup(r => r.Create(It.IsAny<RefundInfo>()))
            .ThrowsAsync(new Exception("Mocked Refund Create Exception"));

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<BusRequestFailed>(payload));


        // Assert
        Assert.True(await Harness.Consumed.Any<RequestRefund>());
        Assert.True(await Harness.Consumed.Any<KkpWithdrawRequest>());
        Assert.NotNull(exception.Message);
        Assert.Contains("Mocked Refund Create Exception", exception.Message);
    }

    [Theory]
    [MemberData(nameof(RefundableTransactionData))]
    public async void Should_Success(Transaction transactionData)
    {
        // Arrange
        var payload = new RequestRefund(MockTransactionNo);
        var client = Harness.GetRequestClient<RequestRefund>();

        var depositEntrypointState = new DepositEntrypointState
        {
            CorrelationId = transactionData.CorrelationId,
            TransactionNo = MockTransactionNo,
            UserId = "mockUserId",
            Product = Product.Cash
        };

        var kkpWithdrawRequest = new KkpWithdrawRequest(
            transactionData.AccountCode,
            transactionData.GetPaymentReceivedAmount()!.Value,
            transactionData.BankAccountNo!,
            transactionData.BankCode!,
            transactionData.Product,
            transactionData.TransactionNo
        );

        var refundInfo = new RefundInfo(
            transactionData.CorrelationId,
            transactionData.GetPaymentReceivedAmount()!.Value,
            transactionData.BankAccountNo!,
            transactionData.BankAccountName!,
            0,
            RefundStatus.RefundSuccess.ToString());

        _depositEntrypointRepository
            .Setup(q => q.GetByTransactionNo(payload.TransactionNo))
            .ReturnsAsync(() => depositEntrypointState);
        _transactionQueriesV2
            .Setup(q => q.GetTransactionByTransactionNo(payload.TransactionNo, depositEntrypointState.Product, depositEntrypointState.UserId))
            .ReturnsAsync(() => transactionData);
        _paymentService
            .Setup(p => p.TransferViaOdd(
                kkpWithdrawRequest.TransactionNo!,
                RoundingUtils.RoundTransactionValue(TransactionType.Withdraw, kkpWithdrawRequest.Amount, Currency.THB),
                TransactionType.Withdraw,
                kkpWithdrawRequest.BankCode,
                kkpWithdrawRequest.BankAccountNo,
                string.Empty,
                string.Empty,
                kkpWithdrawRequest.AccountCode,
                kkpWithdrawRequest.Product
            ))
            .ReturnsAsync(() => "TransactionRef");
        _refundInfoRepository
            .Setup(r => r.Create(It.IsAny<RefundInfo>()))
            .ReturnsAsync(() => refundInfo);

        // Act
        var response = await client.GetResponse<RefundSucceed>(payload);

        // Assert
        Assert.True(await Harness.Consumed.Any<RequestRefund>());
        Assert.True(await Harness.Consumed.Any<KkpWithdrawRequest>());
        Assert.True(await Harness.Published.Any<DepositRefundSucceed>());
        var refundMessage = Harness.Published.Select<DepositRefundSucceed>().First().Context;
        Assert.Equal(transactionData.CorrelationId, refundMessage.Message.CorrelationId);
        Assert.Equal(refundInfo.Id, refundMessage.Message.RefundId);
        Assert.NotNull(response.Message);
        Assert.Equal(refundInfo.Id, response.Message.RefundId);
    }

    public static IEnumerable<object[]> NotRefundableTransactionData(int startIndex = 0, int count = 6)
    {
        // Total cases of transaction that cannot be refunded: 4 cases + 1 case (data for refund is missing)
        // 1. Transaction's DepositEntrypoint is null
        // 2. Transaction's TransactionType is not Deposit
        // 3. Transaction's Status is not WaitingForOps or is not in one of these states { "DepositFailedNameMismatch", "DepositFailedAmountMismatch", "FxFailed", "FxRateCompareFailed" }
        // 4. Transaction already has RefundId
        // 5. Transaction's for refund is missing (PaymentReceivedAmount, BankAccountNo, BankAccountName, BankCode)

        var guid = new Guid();

        var list = new List<object[]>
        {
            new object[]
            {
                // Transaction's DepositEntrypoint is null
                new Transaction(
                    guid,
                    "Received",
                    MockTransactionNo,
                    "mockUserId",
                    "mockAccountCode",
                    "mockCustomerCode",
                    Channel.QR,
                    Product.Cash,
                    Purpose.Collateral,
                    10,
                    0,
                    "mockCustomerName",
                    "mockBankAccountName",
                    "mockBankAccountNo",
                    "mockBankName",
                    "mockBankCode",
                    "",
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    TransactionType.Deposit,
                    DateTime.MinValue,
                    DateTime.MinValue)
                {
                    DepositEntrypoint = null
                }
            },
            new object[]
            {
                // Transaction's TransactionType is not Deposit
                new Transaction(
                    guid,
                    "Received",
                    MockTransactionNo,
                    "mockUserId",
                    "mockAccountCode",
                    "mockCustomerCode",
                    Channel.QR,
                    Product.Cash,
                    Purpose.Collateral,
                    10,
                    0,
                    "mockCustomerName",
                    "mockBankAccountName",
                    "mockBankAccountNo",
                    "mockBankName",
                    "mockBankCode",
                    "",
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    TransactionType.Withdraw,
                    DateTime.MinValue,
                    DateTime.MinValue)
                {
                    DepositEntrypoint = new DepositEntrypointState()
                }
            },
            new object[]
            {
                // Transaction's Status is not WaitingForOps or is not in one of these states { "DepositFailedNameMismatch", "DepositFailedAmountMismatch", "FxFailed", "FxRateCompareFailed" }
                new Transaction(
                    guid,
                    "DepositProcessing",
                    MockTransactionNo,
                    "mockUserId",
                    "mockAccountCode",
                    "mockCustomerCode",
                    Channel.QR,
                    Product.Cash,
                    Purpose.Collateral,
                    10,
                    0,
                    "mockCustomerName",
                    "mockBankAccountName",
                    "mockBankAccountNo",
                    "mockBankName",
                    "mockBankCode",
                    "",
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    TransactionType.Deposit,
                    DateTime.MinValue,
                    DateTime.MinValue)
                {
                    DepositEntrypoint = new DepositEntrypointState
                    {
                        CorrelationId = guid,
                        CurrentState = "DepositProcessing",
                    },
                    QrDeposit = new QrDepositState
                    {
                        CorrelationId = guid,
                        CurrentState = "WaitingForPayment"
                    }
                }
            },
            new object[]
            {
                // Transaction already has RefundId
                new Transaction(
                    guid,
                    "DepositProcessing",
                    MockTransactionNo,
                    "mockUserId",
                    "mockAccountCode",
                    "mockCustomerCode",
                    Channel.QR,
                    Product.Cash,
                    Purpose.Collateral,
                    10,
                    0,
                    "mockCustomerName",
                    "mockBankAccountName",
                    "mockBankAccountNo",
                    "mockBankName",
                    "mockBankCode",
                    "",
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    TransactionType.Deposit,
                    DateTime.MinValue,
                    DateTime.MinValue)
                {
                    DepositEntrypoint = new DepositEntrypointState
                    {
                        CorrelationId = guid,
                        CurrentState = "DepositProcessing",
                        RefundId = Guid.Parse("00000000-1111-1111-1111-000000000000")
                    }
                }
            },
            new object[]
            {
                // Transaction's for refund is missing (BankAccountNo, BankAccountName, BankCode)
                new Transaction(
                    guid,
                    "GlobalTransferProcessing",
                    MockTransactionNo,
                    "mockUserId",
                    "mockAccountCode",
                    "mockCustomerCode",
                    Channel.QR,
                    Product.GlobalEquities,
                    Purpose.Collateral,
                    10,
                    0,
                    "mockCustomerName",
                    null,
                    null,
                    null,
                    null,
                    "",
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    TransactionType.Deposit,
                    DateTime.MinValue,
                    DateTime.MinValue)
                {
                    DepositEntrypoint = new DepositEntrypointState
                    {
                        CorrelationId = guid,
                        CurrentState = "DepositProcessing",
                        BankAccountNo = null,
                        BankAccountName = null,
                        BankName = null,
                        BankCode = null
                    },
                    QrDeposit = new QrDepositState
                    {
                        CorrelationId = guid,
                        CurrentState = "QrDepositCompleted",
                        PaymentReceivedAmount = 10
                    },
                    GlobalTransfer = new GlobalTransferState
                    {
                        CorrelationId = guid,
                        CurrentState = "FxFailed"
                    }
                }
            },
            new object[]
            {
                // Transaction's for refund is missing (PaymentReceivedAmount is null or <= 0)
                new Transaction(
                    guid,
                    "GlobalTransferProcessing",
                    MockTransactionNo,
                    "mockUserId",
                    "mockAccountCode",
                    "mockCustomerCode",
                    Channel.QR,
                    Product.GlobalEquities,
                    Purpose.Collateral,
                    0,
                    0,
                    "mockCustomerName",
                    "mockBankAccountName",
                    "mockBankAccountNo",
                    "mockBankName",
                    "mockBankCode",
                    "",
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    TransactionType.Deposit,
                    DateTime.MinValue,
                    DateTime.MinValue)
                {
                    DepositEntrypoint = new DepositEntrypointState
                    {
                        CorrelationId = guid,
                        CurrentState = "GlobalTransferProcessing",
                        BankAccountNo = null,
                        BankAccountName = null,
                        BankName = null,
                        BankCode = null
                    },
                    QrDeposit = new QrDepositState
                    {
                        CorrelationId = guid,
                        CurrentState = "QrDepositCompleted",
                        PaymentReceivedAmount = 0
                    },
                    GlobalTransfer = new GlobalTransferState
                    {
                        CorrelationId = guid,
                        CurrentState = "FxFailed"
                    }
                }
            }
        };

        return list.GetRange(startIndex, count);
    }

    public static IEnumerable<object[]> RefundableTransactionData()
    {
        var guid = new Guid();

        var list = new List<object[]>
        {
            new object[]
            {
                // Transaction's Status is WaitingForOps
                new Transaction(
                    guid,
                    "DepositProcessing",
                    MockTransactionNo,
                    "mockUserId",
                    "mockAccountCode",
                    "mockCustomerCode",
                    Channel.QR,
                    Product.Cash,
                    Purpose.Collateral,
                    10,
                    0,
                    "mockCustomerName",
                    "mockBankAccountName",
                    "mockBankAccountNo",
                    "mockBankName",
                    "mockBankCode",
                    "",
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    TransactionType.Deposit,
                    DateTime.MinValue,
                    DateTime.MinValue)
                {
                    DepositEntrypoint = new DepositEntrypointState
                    {
                        CorrelationId = guid,
                        CurrentState = "DepositProcessing",
                        BankAccountNo = "mockBankAccountNo",
                        BankAccountName = "mockBankAccountName",
                        BankName = "mockBankName",
                        BankCode = "mockBankCode"
                    },
                    QrDeposit = new QrDepositState
                    {
                        CorrelationId = guid,
                        CurrentState = "DepositFailedNameMismatch",
                        PaymentReceivedAmount = 10
                    }
                }
            },
            new object[]
            {
                // Transaction's Status is DepositFailedNameMismatch
                new Transaction(
                    guid,
                    "GlobalTransferProcessing",
                    MockTransactionNo,
                    "mockUserId",
                    "mockAccountCode",
                    "mockCustomerCode",
                    Channel.QR,
                    Product.Cash,
                    Purpose.Collateral,
                    10,
                    0,
                    "mockCustomerName",
                    "mockBankAccountName",
                    "mockBankAccountNo",
                    "mockBankName",
                    "mockBankCode",
                    "",
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    TransactionType.Deposit,
                    DateTime.MinValue,
                    DateTime.MinValue)
                {
                    DepositEntrypoint = new DepositEntrypointState
                    {
                        CorrelationId = guid,
                        CurrentState = "GlobalTransferProcessing",
                        BankAccountNo = "mockBankAccountNo",
                        BankAccountName = "mockBankAccountName",
                        BankName = "mockBankName",
                        BankCode = "mockBankCode"
                    },
                    QrDeposit = new QrDepositState
                    {
                        CorrelationId = guid,
                        CurrentState = "QrDepositCompleted",
                        PaymentReceivedAmount = 10
                    },
                    GlobalTransfer = new GlobalTransferState
                    {
                        CorrelationId = guid,
                        CurrentState = "FxRateCompareFailed"
                    }
                }
            },
        };

        return list;
    }
}