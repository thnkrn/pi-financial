using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.Domain.Exceptions;
namespace Pi.WalletService.Application.Tests.Commands.GlobalWalletTransfer;

public class ValidateGlobalTransferPaymentTests : ConsumerTest
{
    public ValidateGlobalTransferPaymentTests()
    {
        Mock<IDepositEntrypointRepository> depositEntrypointRepository = new Mock<IDepositEntrypointRepository>();
        Mock<IWithdrawEntrypointRepository> withdrawEntrypointRepository = new Mock<IWithdrawEntrypointRepository>();
        Mock<IQrDepositRepository> qrDepositRepository = new Mock<IQrDepositRepository>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<ValidateGlobalTransferPaymentConsumer>(); })
            .AddScoped<IDepositEntrypointRepository>(_ => depositEntrypointRepository.Object)
            .AddScoped<IWithdrawEntrypointRepository>(_ => withdrawEntrypointRepository.Object)
            .AddScoped<IQrDepositRepository>(_ => qrDepositRepository.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Able_To_ValidateGlobalTransferPayment_Correctly()
    {
        // Arrange
        const string transactionNo = "transaction_no";
        var client = Harness.GetRequestClient<ValidateGlobalTransferDepositPayment>();

        // Act
        var response = await client.GetResponse<GlobalTransferDepositPaymentValidationCompleted>(
            new ValidateGlobalTransferDepositPayment(
                transactionNo,
                DateTime.Now.AddSeconds(-30),
                DateTime.Now,
                60
                ));

        // Arrange
        Assert.Equal(transactionNo, response.Message.TransactionNo);
    }

    [Fact]
    public async void Should_Handle_ValidateGlobalTransfer_Payment_Correctly_When_Validation_Failed()
    {
        // Arrange
        const string transactionNo = "transaction_no";
        var client = Harness.GetRequestClient<ValidateGlobalTransferDepositPayment>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
            await client.GetResponse<GlobalTransferDepositPaymentValidationCompleted>(
            new ValidateGlobalTransferDepositPayment(
                transactionNo,
                DateTime.Now.AddMinutes(-30),
                DateTime.Now,
                15
            )));

        // Arrange
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(QrCodeExpiredException).ToString())));
    }
}
