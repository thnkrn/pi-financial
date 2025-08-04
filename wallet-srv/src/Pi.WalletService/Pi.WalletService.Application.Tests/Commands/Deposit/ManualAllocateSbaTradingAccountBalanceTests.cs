using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.CustomerService;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
namespace Pi.WalletService.Application.Tests.Commands.Deposit;

public class ManualAllocateSbaTradingAccountBalanceTests : ConsumerTest
{
    private readonly Mock<ITransactionQueries> _transactionQueries;
    private readonly Mock<ICustomerService> _customerService;

    public ManualAllocateSbaTradingAccountBalanceTests()
    {
        Mock<IDepositEntrypointRepository> depositEntrypointRepository = new Mock<IDepositEntrypointRepository>();
        Mock<IWithdrawEntrypointRepository> withdrawEntrypointRepository = new Mock<IWithdrawEntrypointRepository>();
        _transactionQueries = new Mock<ITransactionQueries>();
        _customerService = new Mock<ICustomerService>();

        Provider = new ServiceCollection()
            .AddScoped<ITransactionQueries>(_ => _transactionQueries.Object)
            .AddScoped<ICustomerService>(_ => _customerService.Object)
            .AddScoped<IDepositEntrypointRepository>(_ => depositEntrypointRepository.Object)
            .AddScoped<IWithdrawEntrypointRepository>(_ => withdrawEntrypointRepository.Object)
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<ManualAllocateSbaTradingAccountBalance>();
                cfg.AddConsumer<UpdateTradingAccountBalance>();
            })
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void ShouldBeAbleToUpdateTradingAccountBalanceSuccessfully()
    {
        const string transactionNo = "transaction";
        // Arrange
        _transactionQueries
            .Setup(t => t.GetCashDepositTransaction(transactionNo))
            .ReturnsAsync(new CashDepositState
            {
                CorrelationId = Guid.NewGuid(),
                TransactionNo = transactionNo,
                CustomerCode = "1234",
                AccountCode = "12345",
                BankName = "SCB",
                Channel = Channel.QR,
                RequestedAmount = 1000
            });

        _customerService.Setup(c => c.DepositCashAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<decimal>(),
            It.IsAny<Purpose>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>())).ReturnsAsync(new ICustomerService.CustomerServiceResponse(transactionNo, "_000", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

        var client = Harness.GetRequestClient<ManualAllocateSbaTradingAccountBalanceRequest>();

        // Act
        var response = await client.GetResponse<ManualAllocateSbaTradingAccountBalanceSuccess>(new ManualAllocateSbaTradingAccountBalanceRequest(transactionNo));

        // Assert
        Assert.True(await Harness.Consumed.Any<ManualAllocateSbaTradingAccountBalanceSuccess>());
        Assert.Equal(transactionNo, response.Message.TransactionNo);
    }

    [Fact]
    public async void ShouldThrowTransactionNotFoundExceptionWhenTransactionNotExisted()
    {
        const string transactionNo = "transaction";
        // Arrange
        _transactionQueries
            .Setup(t => t.GetCashDepositTransaction(transactionNo))
            .ReturnsAsync((CashDepositState?)null);

        _customerService.Setup(c => c.DepositCashAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<decimal>(),
            It.IsAny<Purpose>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>())).ReturnsAsync(new ICustomerService.CustomerServiceResponse(transactionNo, "000", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

        var client = Harness.GetRequestClient<ManualAllocateSbaTradingAccountBalanceRequest>();

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () => await client.GetResponse<ManualAllocateSbaTradingAccountBalanceSuccess>(new ManualAllocateSbaTradingAccountBalanceRequest(transactionNo)));

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(TransactionNotFoundException).ToString())));
    }
}
