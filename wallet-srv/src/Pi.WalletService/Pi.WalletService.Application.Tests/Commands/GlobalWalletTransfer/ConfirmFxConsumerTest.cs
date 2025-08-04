using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.SeedWork;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Application.Services.FxService;
using Pi.WalletService.Domain.AggregatesModel.WalletAggregate;
using Pi.WalletService.Domain.Events.ForeignExchange;
namespace Pi.WalletService.Application.Tests.Commands.GlobalWalletTransfer;

public class ConfirmFxConsumerTest : ConsumerTest
{
    private readonly Mock<IFxService> _fxService;

    public ConfirmFxConsumerTest()
    {
        _fxService = new Mock<IFxService>();
        Mock<IGlobalWalletTransactionHistoryRepository> transactionHistoryRepository = new();
        transactionHistoryRepository
            .Setup(r => r.UnitOfWork)
            .Returns(new Mock<IUnitOfWork>().Object);
        Mock<ILogger<ConfirmFxConsumer>> logger = new();

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<ConfirmFxConsumer>(); })
            .AddScoped<IGlobalWalletTransactionHistoryRepository>(_ => transactionHistoryRepository.Object)
            .AddScoped<IFxService>(_ => _fxService.Object)
            .AddScoped<ILogger<ConfirmFxConsumer>>(_ => logger.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Update_Transaction_Status_To_ConfirmFx()
    {
        // Arrange
        _fxService
            .Setup(r => r.Confirm(It.IsAny<ConfirmRequest>()));
        var transactionId = Guid.NewGuid().ToString();

        var client = Harness.GetRequestClient<ConfirmFxRequest>();
        var payload = new ConfirmFxRequest(transactionId);

        // Act
        var resp = await client.GetResponse<FxConfirmed>(payload);

        // Assert
        Assert.True(await Harness.Consumed.Any<ConfirmFxRequest>());
        Assert.Equal(resp.Message.TransactionId, transactionId);
    }

    [Fact]
    public async void Should_Save_Fail_Reason_When_ConfirmFx_Failed()
    {
        // Arrange
        _fxService
            .Setup(r => r.Confirm(It.IsAny<ConfirmRequest>()))
            .Throws<Exception>();
        var transactionId = Guid.NewGuid().ToString();

        var client = Harness.GetRequestClient<ConfirmFxRequest>();
        var payload = new ConfirmFxRequest(transactionId);

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () => await client.GetResponse<FxConfirmed>(payload));

        // Assert
        Assert.True(await Harness.Consumed.Any<ConfirmFxRequest>());
        Assert.Equal(
            $"The Pi.WalletService.Application.Commands.GlobalWalletTransfer.ConfirmFxRequest request faulted: Cannot Confirm Fx Rate",
            exception.Message);
    }
}