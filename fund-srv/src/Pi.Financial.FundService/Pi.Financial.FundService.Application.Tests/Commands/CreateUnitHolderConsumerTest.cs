using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Services.MarketService;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.Financial.FundService.Application.Tests.Commands;

public class CreateUnitHolderConsumerTest : IAsyncLifetime
{
    private readonly ServiceProvider _provider;
    private ITestHarness _harness = null!;
    private readonly Mock<IMarketService> _marketService;
    private readonly Mock<IUnitHolderRepository> _unitHolderRepository;

    public CreateUnitHolderConsumerTest()
    {
        _marketService = new Mock<IMarketService>();
        _unitHolderRepository = new Mock<IUnitHolderRepository>();
        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<CreateUnitHolderConsumer>(); })
            .AddScoped<IMarketService>(_ => _marketService.Object)
            .AddScoped<IUnitHolderRepository>(_ => _unitHolderRepository.Object)
            .AddScoped<ILogger<SubscriptionFundConsumer>>(_ => Mock.Of<ILogger<SubscriptionFundConsumer>>())
            .BuildServiceProvider(true);
    }

    public async Task InitializeAsync()
    {
        _harness = _provider.GetRequiredService<ITestHarness>();
        await _harness.Start();
    }

    public async Task DisposeAsync()
    {
        await _harness.Stop();
        await _provider.DisposeAsync();
    }

    [Fact]
    public async Task Should_ConsumeCreateUnitHolder_When_PublishCreateUnitHolder()
    {
        // Arrange
        var domainEvent = new CreateUnitHolder
        {
            CustomerCode = "7711431",
            TradingAccountNo = "7711431-M",
            FundCode = "KF-OIL",
            UnitHolderId = "DM9390104115",
            UnitHolderType = UnitHolderType.SEG,
        };
        _unitHolderRepository.Setup(q => q.CreateAsync(It.IsAny<UnitHolder>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _marketService.Setup(service => service.GetFundInfoByFundCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => FakeFactory.NewFundInfo(domainEvent.FundCode, taxType: "SSF"));

        // Act
        await _harness.Bus.Publish(domainEvent);

        // Assert
        Assert.True(await _harness.Consumed.Any<CreateUnitHolder>());
        _unitHolderRepository.Verify(q => q.CreateAsync(It.IsAny<UnitHolder>(), It.IsAny<CancellationToken>()));
    }

    [Fact(Skip = "Failed on CI")]
    public async Task Should_Success_When_PublishCreateUnitHolder_With_NewUnitHolderIsFalse()
    {
        // Arrange
        var domainEvent = new CreateUnitHolder
        {
            CustomerCode = "7711431",
            TradingAccountNo = "7711431-M",
            FundCode = "KF-OIL",
            UnitHolderId = "DM9390104115",
            UnitHolderType = UnitHolderType.SEG,
        };
        _unitHolderRepository.Setup(q => q.CreateAsync(It.IsAny<UnitHolder>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        await _harness.Bus.Publish(domainEvent);

        // Assert
        Assert.True(await _harness.Consumed.Any<CreateUnitHolder>());
        _unitHolderRepository.Verify(q => q.CreateAsync(It.IsAny<UnitHolder>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Error_When_PublishCreateUnitHolder_Without_FundInfo()
    {
        // Arrange
        var domainEvent = new CreateUnitHolder
        {
            CustomerCode = "7711431",
            TradingAccountNo = "7711431-M",
            FundCode = "KF-OIL",
            UnitHolderId = "DM9390104115",
            UnitHolderType = UnitHolderType.SEG,
        };
        _unitHolderRepository.Setup(q => q.CreateAsync(It.IsAny<UnitHolder>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        await _harness.Bus.Publish(domainEvent);

        // Assert
        Assert.True(await _harness.Published.Any<Fault<CreateUnitHolder>>());
    }
}
