using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.Features;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Application.Services.FxService;
using Pi.WalletService.Domain.AggregatesModel.AtsDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Tests.Commands.GlobalWalletTransfer;

public class InitiateFxRequestTests : IAsyncLifetime
{
    private readonly Mock<IFxService> _fxService;
    private readonly Mock<IConfiguration> _config;
    private readonly Mock<IQrDepositRepository> _qrDepositRepository;
    private readonly Mock<IOddDepositRepository> _oddDepositRepository;
    private readonly Mock<IAtsDepositRepository> _atsDepositRepository;
    private readonly Mock<IDepositEntrypointRepository> _depositEntrypointRepository;
    private readonly Mock<IWithdrawEntrypointRepository> _withdrawEntrypointRepository;
    private readonly Mock<ILogger<InitiateFxRequestConsumer>> _logger;
    private readonly Mock<IFeatureService> _featureService;
    private ITestHarness? _harness;
    private ServiceProvider? _provider;

    public async Task InitializeAsync()
    {
        _provider = new ServiceCollection()

            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<InitiateFxRequestConsumer>();
            })
            .AddScoped<IFxService>(_ => _fxService.Object)
            .AddScoped<IDepositEntrypointRepository>(_ => _depositEntrypointRepository.Object)
            .AddScoped<IWithdrawEntrypointRepository>(_ => _withdrawEntrypointRepository.Object)
            .AddScoped<IQrDepositRepository>(_ => _qrDepositRepository.Object)
            .AddScoped<IAtsDepositRepository>(_ => _atsDepositRepository.Object)
            .AddScoped<IOddDepositRepository>(_ => _oddDepositRepository.Object)
            .AddScoped<ILogger<InitiateFxRequestConsumer>>(_ => _logger.Object)
            .AddScoped<IConfiguration>(_ => _config.Object)
            .AddScoped<IFeatureService>(_ => _featureService.Object)
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

    public InitiateFxRequestTests()
    {
        _depositEntrypointRepository = new Mock<IDepositEntrypointRepository>();
        _withdrawEntrypointRepository = new Mock<IWithdrawEntrypointRepository>();
        _qrDepositRepository = new Mock<IQrDepositRepository>();
        _oddDepositRepository = new Mock<IOddDepositRepository>();
        _atsDepositRepository = new Mock<IAtsDepositRepository>();
        _fxService = new Mock<IFxService>();
        _logger = new Mock<ILogger<InitiateFxRequestConsumer>>();
        _config = new Mock<IConfiguration>();
        _featureService = new Mock<IFeatureService>();
        _config.Setup(x => x["Fx:AccountTHB"]).Returns("AccountTHB");
        _config.Setup(x => x["Fx:AccountUSD"]).Returns("AccountUSD");
    }

    [Fact]
    public async void Should_Return_Confirm_Fx()
    {
        // Arrange
        var fxResponse = new InitiateResponse("transactionId", 10, 350, Currency.USD, Currency.THB, 33, DateTime.Now.AddDays(1));

        _fxService
            .Setup(r => r.Initiate(It.IsAny<InitiateRequest>()).Result)
            .Returns(fxResponse);

        var client = _harness?.GetRequestClient<InitiateFxRequest>();
        var payload = new InitiateFxRequest(Guid.NewGuid().ToString(), TransactionType.Deposit, 20, Currency.USD);

        // Act
        var resp = await client?.GetResponse<GetFxInitialQuoteSucceed>(payload)!;

        // Assert
        Assert.True(await _harness?.Consumed.Any<InitiateFxRequest>()!);
        Assert.Equal(resp.Message.ConfirmedFxRate, fxResponse.ExchangeRate);
    }
}
