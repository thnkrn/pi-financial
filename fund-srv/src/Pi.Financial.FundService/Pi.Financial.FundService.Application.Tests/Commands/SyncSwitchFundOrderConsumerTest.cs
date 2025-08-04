using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Domain.Events;

namespace Pi.Financial.FundService.Application.Tests.Commands;

public class SyncSwitchFundOrderConsumerTest : ConsumerTest
{
    private readonly Mock<IFundConnextService> _fundConnextService;
    private readonly Mock<IFundOrderRepository> _fundOrderRepository;

    public SyncSwitchFundOrderConsumerTest()
    {
        _fundConnextService = new Mock<IFundConnextService>();
        _fundOrderRepository = new Mock<IFundOrderRepository>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<SyncSwitchFundOrderConsumer>(); })
            .AddScoped<IFundConnextService>(_ => _fundConnextService.Object)
            .AddScoped<IFundOrderRepository>(_ => _fundOrderRepository.Object)
            .AddScoped<ILogger<SyncSwitchFundOrderConsumer>>(_ => Mock.Of<ILogger<SyncSwitchFundOrderConsumer>>())
            .BuildServiceProvider(true);
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_Publish_CreateSwitchInFundOrderReceived_When_ReceivedSwitchOrderPlaced()
    {
        // Arrange
        var msg = new SwitchOrderPlaced
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "TEST2024",
        };
        _fundOrderRepository.Setup(q => q.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeFundOrderState());
        _fundConnextService.Setup(q => q.GetFundOrdersByOrderNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundOrder>() { FakeFundOrder(FundOrderType.SwitchIn), FakeFundOrder(FundOrderType.SwitchOut) });

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncSwitchFundOrderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SwitchOrderPlaced>());
        Assert.True(await Harness.Published.Any<CreateSwitchInFundOrderReceived>());
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_Publish_CreateSwitchInFundOrderReceived_With_ExpectedPayload_When_ReceivedSwitchOrderPlaced()
    {
        // Arrange
        var msg = new SwitchOrderPlaced
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "TEST2024",
        };
        var fundOrderState = FakeFundOrderState();
        var fundOrder = FakeFundOrder(FundOrderType.SwitchIn);
        _fundOrderRepository.Setup(q => q.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrderState);
        _fundConnextService.Setup(q => q.GetFundOrdersByOrderNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundOrder>() { fundOrder });

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncSwitchFundOrderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SwitchOrderPlaced>());
        var payload = Harness.Published.Select<CreateSwitchInFundOrderReceived>().First().Context.Message.FundOrderState;
        Assert.NotEqual(fundOrderState.CorrelationId, payload.CorrelationId);
        Assert.Null(payload.CurrentState);
        Assert.Null(payload.CounterFundCode);
        Assert.Null(payload.RedemptionType);
        Assert.Null(payload.SellAllUnit);
        Assert.Equal(FundOrderType.SwitchIn, payload.OrderType);
        Assert.Equal(fundOrder.Amount, payload.Amount);
        Assert.Equal(fundOrder.Unit, payload.Unit);
        Assert.Equal(fundOrder.FundCode, payload.FundCode);
        Assert.Equal(fundOrder.Status, payload.OrderStatus);
        Assert.Equal(fundOrder.EffectiveDate, payload.EffectiveDate);
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_Not_Publish_CreateSwitchInFundOrderReceived_When_ReceivedSwitchOrderPlaced_And_FundOrderStateNotFound()
    {
        // Arrange
        var msg = new SwitchOrderPlaced
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "TEST2024",
        };
        var fundOrder = FakeFundOrder(FundOrderType.SwitchIn);
        _fundConnextService.Setup(q => q.GetFundOrdersByOrderNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundOrder>() { fundOrder });

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncSwitchFundOrderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SwitchOrderPlaced>());
        Assert.False(await Harness.Published.Any<CreateSwitchInFundOrderReceived>());
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_Not_Publish_CreateSwitchInFundOrderReceived_When_ReceivedSwitchOrderPlaced_And_SwitchInFundOrderNotFound()
    {
        // Arrange
        var msg = new SwitchOrderPlaced
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "TEST2024",
        };
        var fundOrderState = FakeFundOrderState();
        var fundOrder = FakeFundOrder(FundOrderType.Redemption);
        _fundOrderRepository.Setup(q => q.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrderState);
        _fundConnextService.Setup(q => q.GetFundOrdersByOrderNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundOrder>() { fundOrder });

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncSwitchFundOrderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SwitchOrderPlaced>());
        Assert.False(await Harness.Published.Any<CreateSwitchInFundOrderReceived>());
    }

    [Fact(Skip = ("Flaky on CI"))]
    public async Task Should_Not_Publish_CreateSwitchInFundOrderReceived_When_ReceivedSwitchOrderPlaced_And_FundOrderStateOrderTypeNotEqualSwitchOut()
    {
        // Arrange
        var msg = new SwitchOrderPlaced
        {
            CorrelationId = Guid.NewGuid(),
            OrderNo = "TEST2024",
        };
        var fundOrderState = FakeFundOrderState();
        var fundOrder = FakeFundOrder(FundOrderType.SwitchOut);
        _fundOrderRepository.Setup(q => q.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundOrderState);
        _fundConnextService.Setup(q => q.GetFundOrdersByOrderNoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundOrder>() { fundOrder });

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<SyncSwitchFundOrderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<SwitchOrderPlaced>());
        Assert.False(await Harness.Published.Any<CreateSwitchInFundOrderReceived>());
    }

    private static FundOrderState FakeFundOrderState(FundOrderType fundOrderType = FundOrderType.SwitchOut, string orderNo = "FOSW2002402070001", string unitHolderId = "DM1280C0M1YE0W", DateOnly effectiveDate = default)
    {
        return new(Guid.NewGuid(), "77991131-M", "KF-OIL", Channel.MOB)
        {
            OrderNo = orderNo,
            CustomerCode = "77991131",
            UnitHolderId = unitHolderId,
            EffectiveDate = effectiveDate,
            OrderType = fundOrderType
        };
    }

    private static FundOrder FakeFundOrder(FundOrderType fundOrderType, string? fundCode = null)
    {
        return new()
        {
            FundCode = fundCode ?? "F-FUND-123",
            Unit = 50.123m,
            Amount = 500.50m,
            PaymentMethod = "TRN",
            BankAccount = "bank account",
            Edd = null,
            SwitchIn = null,
            SwitchTo = null,
            BankCode = "034",
            BrokerOrderId = "123123414",
            AccountId = "1321321",
            OrderType = fundOrderType,
            Status = FundOrderStatus.Submitted,
            TransactionLastUpdated = DateTime.Now,
            EffectiveDate = new DateOnly(),
            TransactionDateTime = DateTime.Now,
            OrderNo = "FOSUB2024027",
            UnitHolderId = "77991131",
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
        };
    }
}
