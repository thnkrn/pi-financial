using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.Domain.Exceptions;
namespace Pi.WalletService.Application.Tests.Commands.Deposit;

public class ValidatePaymentAmountTests : ConsumerTest
{
    public ValidatePaymentAmountTests()
    {
        Mock<IDepositEntrypointRepository> depositEntrypointRepository = new Mock<IDepositEntrypointRepository>();
        Mock<IWithdrawEntrypointRepository> withdrawEntrypointRepository = new Mock<IWithdrawEntrypointRepository>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<ValidatePaymentAmountConsumer>(); })
            .AddScoped<IDepositEntrypointRepository>(_ => depositEntrypointRepository.Object)
            .AddScoped<IWithdrawEntrypointRepository>(_ => withdrawEntrypointRepository.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Be_Able_To_Validate_Payment_Amount_Correctly()
    {
        // Arrange
        var client = Harness.GetRequestClient<ValidatePaymentAmount>();

        // Act
        var response = await client.GetResponse<DepositValidatePaymentAmountSucceed>(new ValidatePaymentAmount(200, 200));

        // Assert
        Assert.Equal(200, response.Message.PaymentAmount);
    }

    [Fact]
    public async void Should_Throw_InvalidDepositAmountException_When_Amount_Mismatch_Correctly()
    {
        // Arrange
        var client = Harness.GetRequestClient<ValidatePaymentAmount>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(
            async () => await client.GetResponse<DepositValidatePaymentAmountSucceed>(
                new ValidatePaymentAmount(
                    200,
                    100)));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(InvalidDepositAmountException).ToString())));
    }
}
