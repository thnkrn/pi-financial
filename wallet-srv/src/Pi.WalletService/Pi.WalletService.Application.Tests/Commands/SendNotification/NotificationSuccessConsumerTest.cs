using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.WalletService.Application.Commands.SendNotification;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.Notification;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Transaction = Pi.WalletService.Domain.AggregatesModel.TransactionAggregate.Transaction;

namespace Pi.WalletService.Application.Tests.Commands.SendNotification;

public class NotificationSuccessConsumerTest : ConsumerTest
{
    private readonly Mock<ILogger<NotificationSuccessConsumer>> _logger;
    private readonly Mock<ITransactionQueriesV2> _transactionQueriesV2;
    private readonly Mock<INotificationService> _notificationService;

    public NotificationSuccessConsumerTest()
    {
        _logger = new Mock<ILogger<NotificationSuccessConsumer>>();
        _transactionQueriesV2 = new Mock<ITransactionQueriesV2>();
        _notificationService = new Mock<INotificationService>();

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<NotificationSuccessConsumer>(); })
            .AddScoped<ITransactionQueriesV2>(_ => _transactionQueriesV2.Object)
            .AddScoped<INotificationService>(_ => _notificationService.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Be_Throw_InvalidDataException_When_Transaction_Not_Found()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var payload = new DepositWithdrawSuccessNotification(correlationId);
        var client = Harness.GetRequestClient<DepositWithdrawSuccessNotification>();

        _transactionQueriesV2.Setup(q => q.GetTransactionById(correlationId)).ReturnsAsync(() => null);

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () => await client.GetResponse<DepositWithdrawSuccessNotificationResponse>(payload));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(InvalidDataException).ToString())));
    }

    [Theory]
    [InlineData(TransactionType.Transfer, null)]
    [InlineData(TransactionType.Refund, null)]
    [InlineData(TransactionType.Unknown, null)]
    [InlineData(TransactionType.Deposit, Product.CreditBalance)]
    [InlineData(TransactionType.Deposit, Product.Crypto)]
    [InlineData(TransactionType.Deposit, Product.Funds)]
    [InlineData(TransactionType.Deposit, Product.Unknown)]
    public async void Should_Be_Throw_ArgumentOutOfRangeException_When_TransactionType_Or_Product_Not_Support(TransactionType transactionType, Product? product)
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var payload = new DepositWithdrawSuccessNotification(correlationId);
        var client = Harness.GetRequestClient<DepositWithdrawSuccessNotification>();

        _transactionQueriesV2.Setup(q => q.GetTransactionById(correlationId)).ReturnsAsync(() => MockTransaction(transactionType, product));

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () => await client.GetResponse<DepositWithdrawSuccessNotificationResponse>(payload));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(ArgumentOutOfRangeException).ToString())));
    }

    [Theory]
    [InlineData(TransactionType.Deposit, Product.CashBalance)]
    [InlineData(TransactionType.Deposit, Product.CreditBalanceSbl)]
    [InlineData(TransactionType.Deposit, Product.Cash)]
    [InlineData(TransactionType.Deposit, Product.Derivatives)]
    [InlineData(TransactionType.Deposit, Product.GlobalEquities)]
    [InlineData(TransactionType.Withdraw, Product.CashBalance)]
    [InlineData(TransactionType.Withdraw, Product.CreditBalanceSbl)]
    [InlineData(TransactionType.Withdraw, Product.Cash)]
    [InlineData(TransactionType.Withdraw, Product.Derivatives)]
    [InlineData(TransactionType.Withdraw, Product.GlobalEquities)]
    public async void Should_Be_Success_When_TransactionType_And_Product_Correctly(TransactionType transactionType, Product? product)
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var payload = new DepositWithdrawSuccessNotification(correlationId);
        var client = Harness.GetRequestClient<DepositWithdrawSuccessNotification>();

        _transactionQueriesV2.Setup(q => q.GetTransactionById(correlationId)).ReturnsAsync(() => MockTransaction(transactionType, product));

        _notificationService.Setup(q => q.SendNotification("userId", "customerCode", 0, new List<string>(),
            new List<string>(), true, true, new CancellationToken()));

        // Act
        await client.GetResponse<DepositWithdrawSuccessNotificationResponse>(payload);

        // Assert
        Assert.True(await Harness.Consumed.Any<DepositWithdrawSuccessNotificationResponse>());
    }

    [Theory]
    [InlineData(TransactionType.Deposit, Product.CashBalance, 26)]
    [InlineData(TransactionType.Deposit, Product.CreditBalanceSbl, 27)]
    [InlineData(TransactionType.Deposit, Product.Cash, 28)]
    [InlineData(TransactionType.Deposit, Product.Derivatives, 29)]
    [InlineData(TransactionType.Deposit, Product.GlobalEquities, 48)]
    [InlineData(TransactionType.Withdraw, Product.CashBalance, 23)]
    [InlineData(TransactionType.Withdraw, Product.CreditBalanceSbl, 23)]
    [InlineData(TransactionType.Withdraw, Product.Cash, 23)]
    [InlineData(TransactionType.Withdraw, Product.Derivatives, 24)]
    [InlineData(TransactionType.Withdraw, Product.GlobalEquities, 23)]
    public void Should_Be_Retrived_Template_Id_Correctly(TransactionType transactionType, Product product, long expectedTemplateId)
    {
        // Arrange
        var consumer = new NotificationSuccessConsumer(_logger.Object, _transactionQueriesV2.Object, _notificationService.Object);

        // Act
        var templateId = consumer.GetTemplateId(transactionType, product);

        // Assert
        Assert.Equal(expectedTemplateId, templateId);
    }

    private static Transaction MockTransaction(TransactionType transactionType, Product? product)
    {
        return new Transaction(
            Guid.NewGuid(),
            "DepositProcessing",
            "transactionNo",
            "mockUserId",
            "mockAccountCode",
            "mockCustomerCode",
            Channel.QR,
            product ?? Product.CashBalance,
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
            transactionType,
            DateTime.MinValue,
            DateTime.MinValue)
        {
            DepositEntrypoint = new DepositEntrypointState
            {
                CorrelationId = Guid.NewGuid(),
                CurrentState = "DepositProcessing",
                BankAccountNo = "mockBankAccountNo",
                BankAccountName = "mockBankAccountName",
                BankName = "mockBankName",
                BankCode = "mockBankCode"
            },
            QrDeposit = new QrDepositState
            {
                CorrelationId = Guid.NewGuid(),
                CurrentState = "DepositFailedNameMismatch",
                PaymentReceivedAmount = 10
            }
        };
    }
}