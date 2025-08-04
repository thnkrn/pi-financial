using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.Domain.Exceptions;
namespace Pi.WalletService.Application.Tests.Commands.Deposit;

public class ValidatePaymentSourceTests : ConsumerTest
{
    public ValidatePaymentSourceTests()
    {
        Mock<IDepositEntrypointRepository> depositEntrypointRepository = new Mock<IDepositEntrypointRepository>();
        Mock<IWithdrawEntrypointRepository> withdrawEntrypointRepository = new Mock<IWithdrawEntrypointRepository>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<ValidatePaymentSourceConsumer>(); })
            .AddScoped<IDepositEntrypointRepository>(_ => depositEntrypointRepository.Object)
            .AddScoped<IWithdrawEntrypointRepository>(_ => withdrawEntrypointRepository.Object)
            .BuildServiceProvider(true);
    }

    [Theory]
    [InlineData("bank_name", true)]
    [InlineData("KTB G-WALLET", false)]
    [InlineData("บจก.ทรู มันนี่ เพื่อเก็บรักษาเงินรับล่วง", false)]
    public async void Should_Be_Able_To_Validate_Payment_Source_Correctly(string bankName, bool shouldPass)
    {
        // Arrange
        const string transactionNo = "transaction_no";

        var client = Harness.GetRequestClient<ValidatePaymentSource>();
        var request = new ValidatePaymentSource(
            transactionNo,
            bankName);

        // Act
        if (shouldPass)
        {
            var response = await client.GetResponse<DepositValidatePaymentSourceSucceed>(request);

            // Assert
            Assert.Equal(transactionNo, response.Message.TransactionNo);
        }
        else
        {
            var exception = await Assert.ThrowsAsync<RequestFaultException>(async () => await client.GetResponse<DepositValidatePaymentSourceSucceed>(request));
            Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(InvalidBankSourceException).ToString())));
        }
    }
}
