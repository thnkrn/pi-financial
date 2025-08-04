using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.Domain;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Bank;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Tests.Commands.SyncFundOrderConsumerTests;

public class SyncFundOrderConsumerTest : ConsumerTest
{
    private readonly Mock<IFundConnextService> _fundConnextService;
    private readonly Mock<IFundOrderRepository> _fundOrderRepository;
    private readonly Mock<IOnboardService> _onboardService;
    private readonly Mock<IUnitHolderRepository> _unitHolderRepository;

    public SyncFundOrderConsumerTest()
    {
        _fundConnextService = new Mock<IFundConnextService>();
        _fundOrderRepository = new Mock<IFundOrderRepository>();
        _onboardService = new Mock<IOnboardService>();
        _unitHolderRepository = new Mock<IUnitHolderRepository>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<SyncFundOrderConsumer>(); })
            .AddScoped<IFundConnextService>(_ => _fundConnextService.Object)
            .AddScoped<IFundOrderRepository>(_ => _fundOrderRepository.Object)
            .AddScoped<IUnitHolderRepository>(_ => _unitHolderRepository.Object)
            .AddScoped<IOnboardService>(_ => _onboardService.Object)
            .AddScoped<ILogger<SyncFundOrderConsumer>>(_ => Mock.Of<ILogger<SyncFundOrderConsumer>>())
            .AddMemoryCache()
            .BuildServiceProvider(true);
    }

    [Fact]
    public async Task Should_UpdateOrderStatus_As_Expected_When_SyncFundOrderFired()
    {
        // Arrange
        var msg = new SyncFundOrder
        {
            EffectiveDate = DateOnly.FromDateTime(DateTime.Now.Subtract(TimeSpan.FromDays(1)))
        };
        var fundOrders = new List<FundOrder>()
        {
            FakeFundOrder(unitHolderId: "UnitA", brokerOrderId: "0001")
        };

        _fundOrderRepository.Setup(q =>
                q.GetEffectiveDates(It.IsAny<IQueryFilter<FundOrderState>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DateOnly>() { DateOnly.FromDateTime(DateTime.Now) });
        _unitHolderRepository.Setup(q =>
                q.CountUnitHolderAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);
        _fundOrderRepository.Setup(q =>
                q.GetByBrokerOrderIds(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundOrderState>() { FakeFundOrderState(unitHolderId: "UnitA", brokerOrderId: "0001") });
        _onboardService.Setup(service =>
                service.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeTradingAccountInfo);
        _fundConnextService.Setup(q =>
                q.GetFundOrdersAsync(It.IsAny<DateOnly>(), null,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrders);
        _fundOrderRepository.Setup(q => q.Update(It.IsAny<FundOrderState>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncFundOrderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SyncFundOrder>());
        _fundOrderRepository.Verify(
            q => q.Update(It.IsAny<FundOrderState>()),
            Times.Exactly(2)
        );
    }

    [Fact]
    public async Task Should_CreateOfflineOrder_As_Expected_When_SyncFundOrderFired()
    {
        // Arrange
        var msg = new SyncFundOrder
        {
            EffectiveDate = DateOnly.FromDateTime(DateTime.Now.Subtract(TimeSpan.FromDays(1)))
        };
        var fundOrders = new List<FundOrder>()
        {
            FakeFundOrder(unitHolderId: "UnitA", brokerOrderId: "0001"),
            FakeFundOrder(unitHolderId: "UnitB", brokerOrderId: "0004"),
        };
        _fundOrderRepository.Setup(q =>
                q.GetEffectiveDates(It.IsAny<IQueryFilter<FundOrderState>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DateOnly>() { DateOnly.FromDateTime(DateTime.Now) });
        _onboardService.Setup(service =>
                service.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeTradingAccountInfo);
        _unitHolderRepository.Setup(q =>
                q.CountUnitHolderAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);
        _fundOrderRepository.Setup(q =>
                q.GetByBrokerOrderIds(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new List<FundOrderState>());
        _fundConnextService.Setup(q =>
                q.GetFundOrdersAsync(It.IsAny<DateOnly>(), null,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrders);
        _fundOrderRepository.Setup(q => q.CreateAsync(It.IsAny<FundOrderState>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FundOrderState(It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Channel>())).Verifiable();

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncFundOrderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SyncFundOrder>());
        _fundOrderRepository.Verify(
            q => q.CreateAsync(It.IsAny<FundOrderState>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
    }

    [Fact]
    public async Task Should_Not_CreateOfflineOrder_When_SyncFundOrderFired_And_CustCodeIsNull()
    {
        // Arrange
        var msg = new SyncFundOrder
        {
            EffectiveDate = DateOnly.FromDateTime(DateTime.Now.Subtract(TimeSpan.FromDays(1)))
        };
        var fundOrders = new List<FundOrder>()
        {
            FakeFundOrder("0001", "UnitA", new DateOnly(), "1321321")
        };

        _fundOrderRepository.Setup(q =>
                q.GetEffectiveDates(It.IsAny<IQueryFilter<FundOrderState>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DateOnly>() { DateOnly.FromDateTime(DateTime.Now) });
        _onboardService.Setup(service =>
                service.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeTradingAccountInfo);
        _unitHolderRepository.Setup(q =>
                q.CountUnitHolderAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);
        _fundOrderRepository.Setup(q =>
            q.GetByBrokerOrderIds(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new List<FundOrderState>());
        _fundConnextService.Setup(q =>
                q.GetFundOrdersAsync(It.IsAny<DateOnly>(), null,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrders);
        _fundOrderRepository.Setup(q => q.CreateAsync(It.IsAny<FundOrderState>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FundOrderState(It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Channel>())).Verifiable();

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncFundOrderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SyncFundOrder>());
        _fundOrderRepository.Verify(
            q => q.CreateAsync(It.IsAny<FundOrderState>(), It.IsAny<CancellationToken>()),
            Times.Exactly(1)
        );
    }

    [Fact]
    public async Task Should_CreateOfflineOrder_When_SyncFundOrderFired_And_CreateFailed()
    {

        // Arrange
        var msg = new SyncFundOrder
        {
            EffectiveDate = DateOnly.FromDateTime(DateTime.Now.Subtract(TimeSpan.FromDays(1)))
        };
        var fundOrders = new List<FundOrder>()
        {
            FakeFundOrder(unitHolderId: "UnitA", brokerOrderId: "0001"),
            FakeFundOrder(unitHolderId: "UnitA", brokerOrderId: "0002"),
            FakeFundOrder(unitHolderId: "UnitB", brokerOrderId: "0003"),
            FakeFundOrder(unitHolderId: "UnitB", brokerOrderId: "0004"),
            FakeFundOrder(unitHolderId: "UnitB", brokerOrderId: "0005")
        };

        _fundOrderRepository.Setup(q =>
                q.GetEffectiveDates(It.IsAny<IQueryFilter<FundOrderState>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DateOnly>() { DateOnly.FromDateTime(DateTime.Now) });
        _onboardService.Setup(service =>
                service.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeTradingAccountInfo);
        _fundOrderRepository.Setup(q =>
            q.GetByBrokerOrderIds(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new List<FundOrderState>());
        _unitHolderRepository.Setup(q =>
                q.CountUnitHolderAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);
        _fundConnextService.Setup(q =>
                q.GetFundOrdersAsync(It.IsAny<DateOnly>(), null,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrders);
        _fundOrderRepository.Setup(q => q.CreateAsync(It.IsAny<FundOrderState>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncFundOrderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SyncFundOrder>());
        _fundOrderRepository.Verify(
            q => q.CreateAsync(It.IsAny<FundOrderState>(), It.IsAny<CancellationToken>()),
            Times.Exactly(5)
        );
    }

    [Fact]
    public async Task Should_CreateOfflineOrder_As_Expected_When_SyncFundOrderFired_And_EffectiveDate_NotEqual_MessageEffectiveDate()
    {
        // Arrange
        var msg = new SyncFundOrder
        {
            EffectiveDate = DateOnly.FromDateTime(DateTime.Now)
        };
        var fundOrders = new List<FundOrder>()
        {
            FakeFundOrder(unitHolderId: "UnitA", brokerOrderId: "0001"),
            FakeFundOrder(unitHolderId: "UnitB", brokerOrderId: "0004"),
        };
        _fundOrderRepository.Setup(q =>
                q.GetEffectiveDates(It.IsAny<IQueryFilter<FundOrderState>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DateOnly>()
            {
                DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now.Subtract(TimeSpan.FromDays(1))),
                DateOnly.FromDateTime(DateTime.Now.Subtract(TimeSpan.FromDays(2))),
            });
        _onboardService.Setup(service =>
                service.GetMutualFundTradingAccountByCustCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeTradingAccountInfo);
        _unitHolderRepository.Setup(q =>
                q.CountUnitHolderAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);
        _fundOrderRepository.Setup(q =>
                q.GetByBrokerOrderIds(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new List<FundOrderState>());
        _fundConnextService.Setup(q =>
                q.GetFundOrdersAsync(It.IsAny<DateOnly>(), null,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrders);
        _fundOrderRepository.Setup(q => q.CreateAsync(It.IsAny<FundOrderState>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FundOrderState(It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Channel>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncFundOrderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SyncFundOrder>());
        _fundConnextService.Verify(
            q => q.GetFundOrdersAsync(It.IsAny<DateOnly>(), null,
                It.IsAny<CancellationToken>()),
            Times.Exactly(3)
        );
        _fundOrderRepository.Verify(
            q => q.CreateAsync(It.IsAny<FundOrderState>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
    }

    private static FundOrderState FakeFundOrderState(string? brokerOrderId = "2002402070001", string? unitHolderId = "DM1280C0M1YE0W", DateOnly effectiveDate = default)
    {
        return new(Guid.NewGuid(), "77991131-M", "KF-OIL", Channel.MOB)
        {
            BrokerOrderId = brokerOrderId,
            OrderNo = "FOSUB2002402070001",
            CustomerCode = "77991131",
            UnitHolderId = unitHolderId,
            EffectiveDate = effectiveDate,
            OrderStatus = FundOrderStatus.Submitted,
            OrderType = FundOrderType.Subscription
        };
    }

    private static FundOrder FakeFundOrder(string brokerOrderId = "2002402070001", string unitHolderId = "77991131", DateOnly effectiveDate = default, string? accountId = null)
    {
        return new()
        {
            BrokerOrderId = brokerOrderId,
            OrderNo = "FOSUB2002402070001",
            AllottedAmount = (decimal)50.123,
            AllottedUnit = (decimal)50.123,
            AllottedNav = (decimal)50.123,
            SellAllUnit = true,
            CounterFundCode = "ABC-D",
            PaymentType = PaymentType.AtsAmc,
            AccountType = FundAccountType.OMN,
            RedemptionType = RedemptionType.Amount,
            OrderSide = OrderSide.Buy,
            AmcOrderId = "7787484784",
            NavDate = new DateOnly(),
            Channel = Channel.MKT,
            AllottedDate = new DateOnly(),
            SettlementDate = new DateOnly(),
            RejectReason = null,
            SettlementBankAccount = null,
            SettlementBankCode = null,
            SaleLicense = "hdk",
            UnitHolderId = unitHolderId,
            EffectiveDate = effectiveDate,
            FundCode = "F-FUND-123",
            Unit = (decimal)50.123,
            Amount = (decimal)500.50,
            PaymentMethod = "TRN",
            BankAccount = "bank account",
            Edd = null,
            SwitchIn = null,
            SwitchTo = null,
            BankCode = "034",
            AccountId = accountId ?? "1321321-M",
            OrderType = FundOrderType.Subscription,
            Status = FundOrderStatus.Allotted,
            TransactionLastUpdated = DateTime.Now,
            TransactionDateTime = DateTime.Now,
        };
    }

    private static TradingAccountInfo FakeTradingAccountInfo()
    {
        return new TradingAccountInfo
        {
            Id = Guid.NewGuid(),
            AccountNo = "someAccountNo",
            SaleLicense = "123"
        };
    }
}
