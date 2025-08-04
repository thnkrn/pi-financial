using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.Domain;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Tests.Commands;

public class SyncUnitHolderConsumerTest : ConsumerTest
{
    private readonly Mock<IFundConnextService> _fundConnextService;
    private readonly Mock<IUnitHolderRepository> _unitHolderRepository;
    private readonly Mock<IFundOrderRepository> _fundOrderRepository;

    public SyncUnitHolderConsumerTest()
    {
        _fundConnextService = new Mock<IFundConnextService>();
        _fundOrderRepository = new Mock<IFundOrderRepository>();
        _unitHolderRepository = new Mock<IUnitHolderRepository>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<SyncUnitHolderConsumer>(); })
            .AddScoped<IFundConnextService>(_ => _fundConnextService.Object)
            .AddScoped<IFundOrderRepository>(_ => _fundOrderRepository.Object)
            .AddScoped<IUnitHolderRepository>(_ => _unitHolderRepository.Object)
            .AddScoped<ILogger<SyncUnitHolderConsumer>>(_ => Mock.Of<ILogger<SyncUnitHolderConsumer>>())
            .BuildServiceProvider(true);
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_UpdateUnitHolderId_As_Expected_When_SyncUnitHolderFired()
    {
        // Arrange
        var msg = new SyncUnitHolder();
        var fundOrderStates = new List<FundOrderState>()
        {
            FakeFundOrderState(unitHolderId: "UnitA", brokerOrderId: "20240001"),
            FakeFundOrderState(unitHolderId: "UnitB", brokerOrderId: "20240002"),
            FakeFundOrderState(unitHolderId: "DM20240208", brokerOrderId: "20240003")
        };
        var fundOrders = new List<FundOrder>()
        {
            FakeFundOrder(unitHolderId: "UnitA", brokerOrderId: "20240001"),
            FakeFundOrder(unitHolderId: "UnitB", brokerOrderId: "20240002"),
            FakeFundOrder(unitHolderId: "DM20240208", brokerOrderId: "20240003"),
        };
        _fundOrderRepository.Setup(q =>
                q.GetAsync(It.IsAny<IQueryFilter<FundOrderState>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrderStates);
        _fundConnextService.Setup(q =>
                q.GetFundOrdersAsync(It.IsAny<DateOnly>(), It.IsAny<FundOrderStatus>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrders);
        _fundOrderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _unitHolderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncUnitHolderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SyncUnitHolder>());
        _fundOrderRepository.Verify(
            q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
        _unitHolderRepository.Verify(
            q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_UpdateUnitHolderId_As_Expected_When_SyncUnitHolderFired_With_MisMatchedFundOrder()
    {
        // Arrange
        var msg = new SyncUnitHolder();
        var fundOrderStates = new List<FundOrderState>()
        {
            FakeFundOrderState(unitHolderId: "UnitA", brokerOrderId: "20240001"),
            FakeFundOrderState(unitHolderId: "UnitB", brokerOrderId: "20240002")
        };
        var fundOrders = new List<FundOrder>()
        {
            FakeFundOrder(unitHolderId: "UnitA", brokerOrderId: "20240001"),
            FakeFundOrder(unitHolderId: "UnitB", brokerOrderId: "20240004"),
        };
        _fundOrderRepository.Setup(q =>
                q.GetAsync(It.IsAny<IQueryFilter<FundOrderState>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrderStates);
        _fundConnextService.Setup(q =>
                q.GetFundOrdersAsync(It.IsAny<DateOnly>(), It.IsAny<FundOrderStatus>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrders);
        _fundOrderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _unitHolderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncUnitHolderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SyncUnitHolder>());
        _fundOrderRepository.Verify(
            q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _unitHolderRepository.Verify(
            q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_UpdateUnitHolderIdOnce_When_SyncUnitHolderFired_And_FundOrderStatesHaveDuplicatedUnitHolder()
    {
        // Arrange
        var msg = new SyncUnitHolder();
        var fundOrderStates = new List<FundOrderState>()
        {
            FakeFundOrderState(unitHolderId: "UnitA", brokerOrderId: "20240001"),
            FakeFundOrderState(unitHolderId: "UnitA", brokerOrderId: "20240002")
        };
        var fundOrders = new List<FundOrder>()
        {
            FakeFundOrder(unitHolderId: "UnitA", brokerOrderId: "20240001"),
            FakeFundOrder(unitHolderId: "UnitA", brokerOrderId: "20240002"),
        };
        _fundOrderRepository.Setup(q =>
                q.GetAsync(It.IsAny<IQueryFilter<FundOrderState>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrderStates);
        _fundConnextService.Setup(q =>
                q.GetFundOrdersAsync(It.IsAny<DateOnly>(), It.IsAny<FundOrderStatus>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrders);
        _fundOrderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _unitHolderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncUnitHolderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SyncUnitHolder>());
        _fundOrderRepository.Verify(
            q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _unitHolderRepository.Verify(
            q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_Not_UpdateUnitHolderId_When_SyncUnitHolderFired_And_UnitHolderIsNull()
    {
        // Arrange
        var msg = new SyncUnitHolder();
        var fundOrderStates = new List<FundOrderState>()
        {
            FakeFundOrderState(unitHolderId: null, brokerOrderId: "20240001"),
        };
        var fundOrders = new List<FundOrder>()
        {
            FakeFundOrder(unitHolderId: "UnitA", brokerOrderId: "20240001"),
        };
        _fundOrderRepository.Setup(q =>
                q.GetAsync(It.IsAny<IQueryFilter<FundOrderState>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrderStates);
        _fundConnextService.Setup(q =>
                q.GetFundOrdersAsync(It.IsAny<DateOnly>(), It.IsAny<FundOrderStatus>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrders);
        _fundOrderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _unitHolderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncUnitHolderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SyncUnitHolder>());
        _fundOrderRepository.Verify(
            q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _unitHolderRepository.Verify(
            q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_Not_UpdateUnitHolderId_When_SyncUnitHolderFired_Without_FundOrderState()
    {
        // Arrange
        var msg = new SyncUnitHolder();
        _fundOrderRepository.Setup(q =>
                q.GetAsync(It.IsAny<IQueryFilter<FundOrderState>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundOrderState>());
        _fundOrderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _unitHolderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncUnitHolderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SyncUnitHolder>());
        _fundOrderRepository.Verify(
            q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _unitHolderRepository.Verify(
            q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_Not_UpdateUnitHolderId_When_SyncUnitHolderFired_Without_FundOrder()
    {
        // Arrange
        var msg = new SyncUnitHolder();
        var fundOrderStates = new List<FundOrderState>() { FakeFundOrderState() };
        _fundOrderRepository.Setup(q =>
                q.GetAsync(It.IsAny<IQueryFilter<FundOrderState>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrderStates);
        _fundConnextService.Setup(q =>
                q.GetFundOrdersAsync(It.IsAny<DateOnly>(), It.IsAny<FundOrderStatus>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundOrder>());
        _fundOrderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _unitHolderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncUnitHolderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SyncUnitHolder>());
        _fundOrderRepository.Verify(
            q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _unitHolderRepository.Verify(
            q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task? Should_CalledGetFundOrdersByStatusAsyncAsExpected_When_SyncUnitHolderFired_And_FetchFundOrderFailed()
    {
        // Arrange
        var msg = new SyncUnitHolder();
        var fundOrderStates = new List<FundOrderState>()
        {
            FakeFundOrderState(unitHolderId: "UnitA", brokerOrderId: "20240001"),
        };
        var fundOrders = new List<FundOrder>()
        {
            FakeFundOrder(unitHolderId: "UnitA", brokerOrderId: "20240001"),
        };
        _fundOrderRepository.Setup(q =>
                q.GetAsync(It.IsAny<IQueryFilter<FundOrderState>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrderStates);
        _fundConnextService.SetupSequence(q =>
                q.GetFundOrdersAsync(It.IsAny<DateOnly>(), It.IsAny<FundOrderStatus>(),
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception())
            .ThrowsAsync(new Exception())
            .ReturnsAsync(fundOrders);
        _fundOrderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
        _unitHolderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncUnitHolderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SyncUnitHolder>());
        _fundConnextService.Verify(
            q =>
                q.GetFundOrdersAsync(It.IsAny<DateOnly>(), It.IsAny<FundOrderStatus>(),
                    It.IsAny<CancellationToken>()),
            Times.Exactly(3)
        );
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task? Should_Failed_When_SyncUnitHolderFired_And_FetchFundOrderFailedExceedMaxRetry()
    {
        // Arrange
        var msg = new SyncUnitHolder();
        var fundOrderStates = new List<FundOrderState>()
        {
            FakeFundOrderState(unitHolderId: "UnitA", brokerOrderId: "20240001", effectiveDate: new DateOnly(2024, 2, 11)),
            FakeFundOrderState(unitHolderId: "UnitB", brokerOrderId: "20240002", effectiveDate: new DateOnly(2024, 2, 12)),
        };
        _fundOrderRepository.Setup(q =>
                q.GetAsync(It.IsAny<IQueryFilter<FundOrderState>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrderStates);
        _fundConnextService.Setup(q =>
                q.GetFundOrdersAsync(It.IsAny<DateOnly>(), It.IsAny<FundOrderStatus>(),
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        _fundOrderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
        _unitHolderRepository.Setup(q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncUnitHolderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SyncUnitHolder>());
        _fundConnextService.Verify(
            q =>
                q.GetFundOrdersAsync(It.IsAny<DateOnly>(), It.IsAny<FundOrderStatus>(),
                    It.IsAny<CancellationToken>()),
            Times.Exactly(6)
        );
        _fundOrderRepository.Verify(
            q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _unitHolderRepository.Verify(
            q => q.UpdateUnitHolderIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    private static FundOrderState FakeFundOrderState(string? brokerOrderId = "2002402070001", string? unitHolderId = "DM1280C0M1YE0W", DateOnly effectiveDate = default)
    {
        return new(Guid.NewGuid(), "77991131-M", "KF-OIL", Channel.MOB)
        {
            CustomerCode = "77991131",
            BrokerOrderId = brokerOrderId,
            UnitHolderId = unitHolderId,
            EffectiveDate = effectiveDate,
        };
    }

    private static FundOrder FakeFundOrder(string brokerOrderId = "2002402070001", string unitHolderId = "77991131", DateOnly effectiveDate = default)
    {
        return new()
        {
            AllottedAmount = (decimal)50.123,
            AllottedUnit = (decimal)50.123,
            AllottedNav = (decimal)50.123,
            SellAllUnit = true,
            CounterFundCode = "ABC-D",
            PaymentType = PaymentType.AtsAmc,
            AccountType = FundAccountType.OMN,
            RedemptionType = RedemptionType.Amount,
            OrderSide = OrderSide.Buy,
            Channel = Channel.MOB,
            AmcOrderId = "7787484784",
            NavDate = new DateOnly(),
            AllottedDate = new DateOnly(),
            SettlementDate = new DateOnly(),
            RejectReason = null,
            SettlementBankAccount = null,
            SettlementBankCode = null,
            SaleLicense = "hdk",
            BrokerOrderId = brokerOrderId,
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
            AccountId = "1321321",
            OrderType = FundOrderType.Subscription,
            Status = FundOrderStatus.Allotted,
            TransactionLastUpdated = DateTime.Now,
            TransactionDateTime = DateTime.Now,
            OrderNo = "SOME",
        };
    }
}
