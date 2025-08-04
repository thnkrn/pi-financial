using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.Common.Domain;
using Pi.OnePort.IntegrationEvents;
using Pi.OnePort.IntegrationEvents.Models;
using Pi.SetService.Application.Commands;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using Pi.SetService.Domain.Events;
using ExecutionTransType = Pi.OnePort.IntegrationEvents.Models.ExecutionTransType;
using OrderSide = Pi.OnePort.IntegrationEvents.Models.OrderSide;
using Source = Pi.OnePort.IntegrationEvents.Models.Source;
using Ttf = Pi.OnePort.IntegrationEvents.Models.Ttf;

namespace Pi.SetService.Application.Tests.Commands;

public class BrokerOrderConsumerTest : ConsumerTest
{
    private readonly Mock<IEquityOrderStateRepository> _equityOrderStateRepository;

    public BrokerOrderConsumerTest()
    {
        _equityOrderStateRepository = new Mock<IEquityOrderStateRepository>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<BrokerOrderConsumer>(); })
            .AddScoped<IEquityOrderStateRepository>(_ => _equityOrderStateRepository.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async Task Should_PublishExpectedEvent_When_OnePortOrderMatched()
    {
        // Arrange
        var payload = new OnePortOrderMatched
        {
            ExecutionTransType = ExecutionTransType.New,
            Source = Source.Fis,
            Symbol = "EA",
            Side = OrderSide.Buy,
            Volume = 1000,
            Price = 3.50m,
            FisOrderId = "random",
            TransactionDateTime = default
        };
        var equityOrderState = new EquityOrderState(Guid.NewGuid(), Guid.NewGuid(), "0800782", TradingAccountType.Cash, "0800782", "EA")
        {
            BrokerOrderId = payload.FisOrderId,
            Volume = (int)payload.Volume
        };
        _equityOrderStateRepository.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync([equityOrderState]);

        // Act
        await Harness.Bus.Publish(payload);

        // Assert
        Assert.True(await Harness.Published.Any<OrderMatched>());
    }

    [Fact]
    public async Task Should_PublishExpectedEvent_When_OnePortOrderChanged()
    {
        // Arrange
        var payload = new OnePortOrderChanged
        {
            Volume = 100,
            Price = 3.50m,
            FisOrderId = "random",
            TransactionDateTime = default,
            AccountId = "0900432",
            EnterId = "9009",
            Channel = OrderChannel.Ifise,
            Ttf = Ttf.None
        };
        var equityOrderState = new EquityOrderState(Guid.NewGuid(), Guid.NewGuid(), "0800782", TradingAccountType.Cash, "0800782", "EA")
        {
            BrokerOrderId = payload.FisOrderId,
            Volume = (int)payload.Volume + 100
        };
        _equityOrderStateRepository.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync([equityOrderState]);

        // Act
        await Harness.Bus.Publish(payload);

        // Assert
        Assert.True(await Harness.Published.Any<OrderChanged>());
    }

    [Fact]
    public async Task Should_PublishExpectedEvent_When_OnePortOrderCancelled_Is_FullCancel()
    {
        // Arrange
        var payload = new OnePortOrderCanceled
        {
            FisOrderId = "random",
            TransactionDateTime = default,
            ExecutionTransType = ExecutionTransType.New,
            Source = Source.Fis,
            Symbol = "EA",
            Side = OrderSide.Buy,
            CancelVolume = 100,
        };
        var equityOrderState = new EquityOrderState(Guid.NewGuid(), Guid.NewGuid(), "0800782", TradingAccountType.Cash, "0800782", "EA")
        {
            BrokerOrderId = payload.FisOrderId,
            Volume = (int)payload.CancelVolume
        };
        _equityOrderStateRepository.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync([equityOrderState]);

        // Act
        await Harness.Bus.Publish(payload);

        // Assert
        Assert.True(await Harness.Published.Any<OrderCancelled>());
    }

    [Fact]
    public async Task Should_PublishExpectedEvent_When_OnePortOrderCancelled_Is_PartialCancel()
    {
        // Arrange
        var payload = new OnePortOrderCanceled
        {
            FisOrderId = "random",
            TransactionDateTime = default,
            ExecutionTransType = ExecutionTransType.New,
            Source = Source.Fis,
            Symbol = "EA",
            Side = OrderSide.Buy,
            CancelVolume = 100,
        };
        var equityOrderState = new EquityOrderState(Guid.NewGuid(), Guid.NewGuid(), "0800782", TradingAccountType.Cash, "0800782", "EA")
        {
            BrokerOrderId = payload.FisOrderId,
            Volume = (int)payload.CancelVolume + 100,
            MatchedVolume = 100,
            MatchedPrice = 3.50m,
        };
        _equityOrderStateRepository.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync([equityOrderState]);

        // Act
        await Harness.Bus.Publish(payload);

        // Assert
        Assert.True(await Harness.Published.Any<OrderCancelled>());
    }

    [Fact]
    public async Task Should_PublishExpectedEvent_When_OnePortOrderRejected()
    {
        // Arrange
        var payload = new OnePortOrderRejected
        {
            FisOrderId = "random",
            TransactionDateTime = default,
            ExecutionTransType = ExecutionTransType.New,
            Source = Source.Fis
        };
        var equityOrderState = new EquityOrderState(Guid.NewGuid(), Guid.NewGuid(), "0800782", TradingAccountType.Cash, "0800782", "EA")
        {
            BrokerOrderId = payload.FisOrderId,
        };
        _equityOrderStateRepository.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync([equityOrderState]);

        // Act
        await Harness.Bus.Publish(payload);

        // Assert
        Assert.True(await Harness.Published.Any<OrderRejected>());
    }
}
