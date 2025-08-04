using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Domain.Events.ForeignExchange;
namespace Pi.WalletService.Application.Tests.Commands.GlobalWalletTransfer;

public class ValidateFxRequestTests : IAsyncLifetime
{
    private readonly Mock<IConfiguration> _config;
    private readonly Mock<ILogger<ValidateFxRequestConsumer>> _logger;
    private ITestHarness? _harness;
    private ServiceProvider? _provider;
    private const string MaxSlippage = "1";

    public async Task InitializeAsync()
    {
        _provider = new ServiceCollection()

            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<ValidateFxRequestConsumer>();
            })
            .AddScoped<ILogger<ValidateFxRequestConsumer>>(_ => _logger.Object)
            .AddScoped<IConfiguration>(_ => _config.Object)
            .BuildServiceProvider(true);

        _harness = _provider.GetRequiredService<ITestHarness>();

        await _harness.Start();
    }

    public async Task DisposeAsync()
    {
        await _harness.Stop();
        if (_provider != null)
        {
            await _provider.DisposeAsync();
        }
    }

    public ValidateFxRequestTests()
    {
        _logger = new Mock<ILogger<ValidateFxRequestConsumer>>();
        _config = new Mock<IConfiguration>();
        _config.Setup(x => x["Fx:MaxSlippage"]).Returns(MaxSlippage);
    }

    [Fact]
    public async void Should_Return_Successfully()
    {
        // Arrange
        var client = _harness?.GetRequestClient<ValidateFxRequest>();
        var txId = Guid.NewGuid().ToString();
        var payload = new ValidateFxRequest(txId, 34, 34, 1);

        // Act
        var resp = await client?.GetResponse<ValidateFxRequestSucceed>(payload)!;

        // Assert
        Assert.True(await _harness?.Consumed.Any<ValidateFxRequest>()!);
        Assert.Equal(txId, resp.Message.TransactionId);
    }
    [Theory]
    [InlineData(30, 33)]
    [InlineData(33, 36)]
    public async void Should_Return_Exception_When_Confirm_Fx_Rate_Exceed_Threshold(decimal requestedFxRate, decimal confirmedFxRate)
    {
        // Arrange
        var client = _harness?.GetRequestClient<ValidateFxRequest>();
        var payload = new ValidateFxRequest(Guid.NewGuid().ToString(), requestedFxRate, confirmedFxRate, 1);

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () => await client?.GetResponse<ValidateFxRequestSucceed>(payload)!);

        // Assert
        Assert.True(await _harness?.Consumed.Any<ValidateFxRequest>()!);
        Assert.Contains(typeof(FxRateDiffOverThresholdException).FullName, exception.Fault?.Exceptions.Select(e => e.ExceptionType) ?? Array.Empty<string>());
    }
}