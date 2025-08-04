using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.WalletService.Application.Commands.Refund;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;

namespace Pi.WalletService.Application.Tests.Commands.Refund;

public class RefundReceivedConsumerTests : ConsumerTest
{
    private readonly Mock<IDepositRepository> _depositRepository;

    public RefundReceivedConsumerTests()
    {
        _depositRepository = new Mock<IDepositRepository>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<RefundInfoConsumer>(); })
            .AddScoped<IDepositRepository>(_ => _depositRepository.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Be_Return_Response_With_Expected_RefundAmount_When_RequestRefund()
    {
        // Arrange
        var deposit = new DepositState()
        {
            TransactionNo = "SomeTransactionNo",
            PaymentReceivedAmount = 100,
            BankAccountNo = "accountNo",
            BankCode = "bankCode",
        };
        _depositRepository.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(() => deposit);
        var client = Harness.GetRequestClient<RefundInfoRequest>()!;
        var payload = new RefundInfoRequest(Guid.NewGuid(), deposit.TransactionNo);

        // Act
        var actual = await client.GetResponse<RefundInfoResponse>(payload);

        // Assert
        Assert.Equal(deposit.PaymentReceivedAmount, actual.Message.RefundAmount);
    }

    [Fact]
    public async void Should_Be_Failed_When_RequestRefund_And_Deposit_Not_Found()
    {
        // Arrange
        var transactionNo = "SomeTransactionNo";
        _depositRepository.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(() => null);
        var client = Harness.GetRequestClient<RefundInfoRequest>()!;
        var payload = new RefundInfoRequest(Guid.NewGuid(), transactionNo);

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () => await client?.GetResponse<RefundInfoResponse>(payload)!);

        // Assert
        Assert.Contains("Deposit Transaction Not Found", exception.Message);
    }

    [Theory]
    [InlineData(null, "bankAccountNo", "bankCode", "Deposit payment not complete yet")]
    [InlineData(100, null, "bankCode", "Deposit payment not complete yet")]
    [InlineData(100, "bankAccountNo", null, "Deposit payment not complete yet")]
    public async void Should_Be_Failed_When_RequestRefund_And_Deposit_PaymentNotComplete_Invalid(int? amount, string? bankAccountNo, string? bankCode, string expected)
    {
        // Arrange
        var deposit = new DepositState()
        {
            TransactionNo = "SomeTransactionNo",
            PaymentReceivedAmount = amount,
            BankAccountNo = bankAccountNo,
            BankCode = bankCode,
        };
        _depositRepository.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(() => deposit);
        var client = Harness.GetRequestClient<RefundInfoRequest>()!;
        var payload = new RefundInfoRequest(Guid.NewGuid(), deposit.TransactionNo);

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () => await client?.GetResponse<RefundInfoResponse>(payload)!);

        // Assert
        Assert.Contains(expected, exception.Message);
    }
}
