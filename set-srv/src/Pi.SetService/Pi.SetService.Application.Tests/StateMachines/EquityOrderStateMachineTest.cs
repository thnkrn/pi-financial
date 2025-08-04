using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Pi.SetService.Application.Commands;
using Pi.SetService.Application.StateMachines;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using Pi.SetService.Domain.Events;
using Pi.SetService.IntegrationEvents;
using Condition = Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate.Condition;
using ConditionPrice = Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate.ConditionPrice;
using OrderSide = Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate.OrderSide;
using OrderStatus = Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate.OrderStatus;
using OrderType = Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate.OrderType;

namespace Pi.SetService.Application.Tests.StateMachines;

public class EquityOrderStateMachineTest : IAsyncLifetime
{
    private ISagaRepository<EquityOrderState> _sagaRepository = null!;
    private ITestHarness Harness { get; set; } = null!;
    private ServiceProvider Provider { get; set; } = null!;

    public async Task InitializeAsync()
    {
        await InitProvider();
    }

    public async Task DisposeAsync()
    {
        await Harness.Stop();
        await Provider.DisposeAsync();
    }

    private async Task InitProvider(Action<IBusRegistrationConfigurator>? configure = null)
    {
        EquityOrderStateMachine.CleanUp();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                configure?.Invoke(cfg);

                cfg.AddSagaStateMachine<EquityOrderStateMachine, EquityOrderState>(typeof(EquityOrderStateDefinition));
            })
            .BuildServiceProvider(true);
        _sagaRepository = Provider.GetRequiredService<ISagaRepository<EquityOrderState>>();
        Harness = Provider.GetRequiredService<ITestHarness>();
        await Harness.Start();
    }

    [Fact]
    public async Task Should_TransitionToSendingOrderToBroker_And_ublishSendOrderRequest_When_OrderRequestReceived()
    {
        // Arrange
        await InitProvider(_ => { });
        var msg = new OrderRequestReceived
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            TradingAccountNo = "771931-8",
            CustomerCode = "771931",
            TradingAccountType = TradingAccountType.Cash,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            Type = OrderType.Normal,
            SecSymbol = "EA",
            Condition = Condition.None
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Consumed.Any<OrderRequestReceived>());
        Assert.True(await Harness.Published.Any<SendOrderRequest>());

        var sagaHarness = Harness.GetSagaStateMachineHarness<EquityOrderStateMachine, EquityOrderState>();
        Assert.True(await sagaHarness.Consumed.Any<OrderRequestReceived>());
        Assert.True(await sagaHarness.Sagas.Any(x => x.CorrelationId == msg.CorrelationId));
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness,
            EquityOrderStateMachine.SendingOrderToBroker!);
    }

    [Fact]
    public async Task Should_Init_And_TransitionToOrderPlaced_When_SyncCreateOrderReceived()
    {
        // Arrange
        await InitProvider(_ => { });
        var msg = new SyncCreateOrderReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            TradingAccountNo = "771931-8",
            CustomerCode = "771931",
            TradingAccountType = TradingAccountType.Cash,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            SecSymbol = "EA",
            Condition = Condition.None,
            EnterId = "9009",
            BrokerOrderId = "4",
            OrderStatus = OrderStatus.Pending,
            OrderSide = OrderSide.Buy,
            OrderAction = OrderAction.Cover
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Consumed.Any<SyncCreateOrderReceived>());

        var sagaHarness = Harness.GetSagaStateMachineHarness<EquityOrderStateMachine, EquityOrderState>();
        Assert.True(await sagaHarness.Consumed.Any<SyncCreateOrderReceived>());
        Assert.True(await sagaHarness.Sagas.Any(x => x.CorrelationId == msg.CorrelationId));
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness, EquityOrderStateMachine.OrderPlaced!);
    }

    [Fact]
    public async Task Should_TransitionToOrderPlaced_And_PublishSetOrderPlaced_When_SendOrderSuccess()
    {
        // Arrange
        var sendOrderResponse = new SendOrderResponse
        {
            OrderNo = "SomeOrderNo",
            BrokerOrderNo = "SomeBrokerOrderNo",
            EnterId = "9009"
        };
        var msg = new OrderRequestReceived
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            TradingAccountNo = "771931-8",
            CustomerCode = "771931",
            TradingAccountType = TradingAccountType.Cash,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            Type = OrderType.Normal,
            SecSymbol = "EA",
            Condition = Condition.None
        };
        await InitProvider(cfg => { cfg.AddHandler<SendOrderRequest>(ctx => ctx.RespondAsync(sendOrderResponse)); });

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        await Harness.Consumed.Any<ReceivedMessage<SendOrderRequest>>();

        Assert.True(await Harness.Published.Any<SetOrderPlaced>());
        var sagaHarness = Harness.GetSagaStateMachineHarness<EquityOrderStateMachine, EquityOrderState>();
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness, EquityOrderStateMachine.OrderPlaced!);
    }

    [Fact]
    public async Task Should_ResponsePlaceOrderSuccess_When_SendOrderSuccess()
    {
        // Arrange
        var msg = new OrderRequestReceived
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            TradingAccountNo = "771931-8",
            CustomerCode = "771931",
            TradingAccountType = TradingAccountType.Cash,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            Type = OrderType.Normal,
            SecSymbol = "EA",
            Condition = Condition.None
        };
        var sendOrderResponse = new SendOrderResponse
        {
            OrderNo = "SomeOrderNo",
            BrokerOrderNo = "SomeBrokerOrderNo",
            EnterId = "9009"
        };
        await InitProvider(cfg => { cfg.AddHandler<SendOrderRequest>(ctx => ctx.RespondAsync(sendOrderResponse)); });
        var client = Harness.GetRequestClient<OrderRequestReceived>();

        // Act
        var response = await client.GetResponse<PlaceOrderSuccess>(msg);

        // Assert
        Assert.IsType<PlaceOrderSuccess>(response.Message);
        Assert.Equal(sendOrderResponse.OrderNo, response.Message.OrderNo);
        Assert.Equal(sendOrderResponse.BrokerOrderNo, response.Message.BrokerOrderId);
    }

    [Fact]
    public async Task Should_TransitionToPlaceOrderFailed_When_SendOrderFailed()
    {
        // Arrange
        var msg = new OrderRequestReceived
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            TradingAccountNo = "771931-8",
            CustomerCode = "771931",
            TradingAccountType = TradingAccountType.Cash,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            Type = OrderType.Normal,
            SecSymbol = "EA",
            Condition = Condition.None
        };
        await InitProvider(cfg =>
        {
            cfg.AddHandler((ConsumeContext<SendOrderRequest> _) => { throw new Exception(); });
        });

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        await Harness.Consumed.Any<ReceivedMessage<SendOrderRequest>>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<EquityOrderStateMachine, EquityOrderState>();
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness,
            EquityOrderStateMachine.PlaceOrderFailed!);
    }

    [Fact]
    public async Task Should_ResponsePlaceOrderFailed_When_SendOrderFailed()
    {
        // Arrange
        var msg = new OrderRequestReceived
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            TradingAccountNo = "771931-8",
            CustomerCode = "771931",
            TradingAccountType = TradingAccountType.Cash,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            Type = OrderType.Normal,
            SecSymbol = "EA",
            Condition = Condition.None
        };
        await InitProvider(cfg =>
        {
            cfg.AddHandler((ConsumeContext<SendOrderRequest> _) => { throw new Exception(); });
        });
        var client = Harness.GetRequestClient<OrderRequestReceived>();

        // Act
        var response = await client.GetResponse<PlaceOrderFailed>(msg);

        // Assert
        Assert.IsType<PlaceOrderFailed>(response.Message);
        Assert.Equal(msg.CorrelationId, response.Message.CorrelationId);
        Assert.Equal(SetErrorCode.SE201, response.Message.SetErrorCode);
    }

    [Fact]
    public async Task Should_TransitionToOrderCancelled_When_OrderPlaced_And_OrderCancelledReceived()
    {
        // Arrange
        var msg = new OrderCancelled
        {
            CorrelationId = Guid.NewGuid(),
            CancelledVolume = 100,
            Symbol = "EA",
            Source = Source.Fis,
            TransactionTime = default
        };
        await InitProvider(cfg =>
        {
            cfg.AddHandler<SendOrderRequest>(ctx => ctx.RespondAsync(new SendOrderResponse
            {
                OrderNo = "SomeOrderNo",
                BrokerOrderNo = "SomeBrokerOrderNo",
                EnterId = "9009"
            }));
        });
        var client = Harness.GetRequestClient<OrderRequestReceived>();
        await client.GetResponse<PlaceOrderSuccess>(new OrderRequestReceived
        {
            UserId = Guid.NewGuid(),
            CorrelationId = msg.CorrelationId,
            TradingAccountId = Guid.NewGuid(),
            TradingAccountNo = "771931-8",
            CustomerCode = "771931",
            TradingAccountType = TradingAccountType.Cash,
            ConditionPrice = ConditionPrice.Limit,
            Volume = (int)msg.CancelledVolume,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            Type = OrderType.Normal,
            SecSymbol = "EA",
            Condition = Condition.None
        });

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Published.Any<OrderCancelled>());

        var sagaHarness = Harness.GetSagaStateMachineHarness<EquityOrderStateMachine, EquityOrderState>();
        Assert.True(await sagaHarness.Consumed.Any<OrderCancelled>());
        Assert.True(await sagaHarness.Sagas.Any(x => x.CorrelationId == msg.CorrelationId));
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness, EquityOrderStateMachine.OrderCancelled!);
    }

    [Fact]
    public async Task Should_TransitionToOrderPlaced_When_OrderPlaced_And_OrderChangedReceived()
    {
        // Arrange
        var msg = new OrderChanged
        {
            CorrelationId = Guid.NewGuid(),
            Price = 999.99m,
            Volume = 100000,
            TransactionTime = default
        };
        await InitProvider(cfg =>
        {
            cfg.AddHandler<SendOrderRequest>(ctx => ctx.RespondAsync(new SendOrderResponse
            {
                OrderNo = "SomeOrderNo",
                BrokerOrderNo = "SomeBrokerOrderNo",
                EnterId = "9009"
            }));
        });
        var client = Harness.GetRequestClient<OrderRequestReceived>();
        await client.GetResponse<PlaceOrderSuccess>(new OrderRequestReceived
        {
            UserId = Guid.NewGuid(),
            CorrelationId = msg.CorrelationId,
            TradingAccountId = Guid.NewGuid(),
            TradingAccountNo = "771931-8",
            CustomerCode = "771931",
            TradingAccountType = TradingAccountType.Cash,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            Type = OrderType.Normal,
            SecSymbol = "EA",
            Condition = Condition.None
        });

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Published.Any<OrderChanged>());

        var sagaHarness = Harness.GetSagaStateMachineHarness<EquityOrderStateMachine, EquityOrderState>();
        Assert.True(await sagaHarness.Consumed.Any<OrderChanged>());
        Assert.True(await sagaHarness.Sagas.Any(x => x.CorrelationId == msg.CorrelationId));
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness, EquityOrderStateMachine.OrderPlaced!);
    }

    [Fact]
    public async Task Should_TransitionToOrderRejected_When_OrderPlaced_And_OrderRejected()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var msg = new OrderRejected
        {
            Source = Source.Fis,
            CorrelationId = orderId,
            TransactionTime = default,
        };
        await InitProvider(cfg =>
        {
            cfg.AddHandler<SendOrderRequest>(ctx => ctx.RespondAsync(new SendOrderResponse
            {
                OrderNo = "SomeOrderNo",
                BrokerOrderNo = "12",
                EnterId = "9009"
            }));
        });
        var client = Harness.GetRequestClient<OrderRequestReceived>();
        await client.GetResponse<PlaceOrderSuccess>(new OrderRequestReceived
        {
            UserId = Guid.NewGuid(),
            CorrelationId = orderId,
            TradingAccountId = Guid.NewGuid(),
            TradingAccountNo = "771931-8",
            CustomerCode = "771931",
            TradingAccountType = TradingAccountType.Cash,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            Type = OrderType.Normal,
            SecSymbol = "EA",
            Condition = Condition.None
        });

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Published.Any<OrderRejected>());

        var sagaHarness = Harness.GetSagaStateMachineHarness<EquityOrderStateMachine, EquityOrderState>();
        Assert.True(await sagaHarness.Consumed.Any<OrderRejected>());
        Assert.True(await sagaHarness.Sagas.Any(x => x.CorrelationId == orderId));
        await VerifySagaExistWithCorrectState(orderId, sagaHarness, EquityOrderStateMachine.OrderRejected!);
    }

    [Fact]
    public async Task Should_TransitionToOrderMatched_When_OrderPlaced_And_OrderMatched()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var msg = new OrderMatched
        {
            Symbol = "EAA",
            Volume = 100,
            CorrelationId = orderId,
            Price = 100,
            TransactionTime = default,
        };
        await InitProvider(cfg =>
        {
            cfg.AddHandler<SendOrderRequest>(ctx => ctx.RespondAsync(new SendOrderResponse
            {
                OrderNo = "SomeOrderNo",
                BrokerOrderNo = "12",
                EnterId = "9009"
            }));
        });
        var client = Harness.GetRequestClient<OrderRequestReceived>();
        await client.GetResponse<PlaceOrderSuccess>(new OrderRequestReceived
        {
            UserId = Guid.NewGuid(),
            CorrelationId = orderId,
            TradingAccountId = Guid.NewGuid(),
            TradingAccountNo = "771931-8",
            CustomerCode = "771931",
            TradingAccountType = TradingAccountType.Cash,
            ConditionPrice = ConditionPrice.Limit,
            Volume = (int)msg.Volume,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            Type = OrderType.Normal,
            SecSymbol = msg.Symbol,
            Condition = Condition.None
        });

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Published.Any<OrderMatched>());

        var sagaHarness = Harness.GetSagaStateMachineHarness<EquityOrderStateMachine, EquityOrderState>();
        Assert.True(await sagaHarness.Consumed.Any<OrderMatched>());
        Assert.True(await sagaHarness.Sagas.Any(x => x.CorrelationId == orderId));
        await VerifySagaExistWithCorrectState(orderId, sagaHarness, EquityOrderStateMachine.OrderMatched!);
    }

    [Fact]
    public async Task Should_TransitionToOrderPlaced_When_OrderPlaced_And_OrderMatched_Is_PartialMatched()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var msg = new OrderMatched
        {
            Symbol = "EAA",
            Volume = 100,
            CorrelationId = orderId,
            Price = 10,
            TransactionTime = default,
        };
        await InitProvider(cfg =>
        {
            cfg.AddHandler<SendOrderRequest>(ctx => ctx.RespondAsync(new SendOrderResponse
            {
                OrderNo = "SomeOrderNo",
                BrokerOrderNo = "12",
                EnterId = "9009"
            }));
        });
        var client = Harness.GetRequestClient<OrderRequestReceived>();
        await client.GetResponse<PlaceOrderSuccess>(new OrderRequestReceived
        {
            UserId = Guid.NewGuid(),
            CorrelationId = orderId,
            TradingAccountId = Guid.NewGuid(),
            TradingAccountNo = "771931-8",
            CustomerCode = "771931",
            TradingAccountType = TradingAccountType.Cash,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 30000,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            Type = OrderType.Normal,
            SecSymbol = msg.Symbol,
            Condition = Condition.None
        });

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Published.Any<OrderMatched>());

        var sagaHarness = Harness.GetSagaStateMachineHarness<EquityOrderStateMachine, EquityOrderState>();
        Assert.True(await sagaHarness.Consumed.Any<OrderMatched>());
        Assert.True(await sagaHarness.Sagas.Any(x => x.CorrelationId == orderId));
        await VerifySagaExistWithCorrectState(orderId, sagaHarness, EquityOrderStateMachine.OrderPlaced!);
    }

    [Fact]
    public async Task Should_TransitionToOrderPlaced_When_OrderPlaced_And_OrderMatched_Is_FullyMatched()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var msg = new OrderMatched
        {
            Symbol = "EAA",
            Volume = 300,
            CorrelationId = orderId,
            Price = 10,
            TransactionTime = default,
        };
        await InitProvider(cfg =>
        {
            cfg.AddHandler<SendOrderRequest>(ctx => ctx.RespondAsync(new SendOrderResponse
            {
                OrderNo = "SomeOrderNo",
                BrokerOrderNo = "12",
                EnterId = "9009"
            }));
        });
        var client = Harness.GetRequestClient<OrderRequestReceived>();
        await client.GetResponse<PlaceOrderSuccess>(new OrderRequestReceived
        {
            UserId = Guid.NewGuid(),
            CorrelationId = orderId,
            TradingAccountId = Guid.NewGuid(),
            TradingAccountNo = "771931-8",
            CustomerCode = "771931",
            TradingAccountType = TradingAccountType.Cash,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 1000,
            Action = OrderAction.Buy,
            Side = OrderSide.Buy,
            Type = OrderType.Normal,
            SecSymbol = msg.Symbol,
            Condition = Condition.None
        });

        // Act
        await Harness.Bus.Publish(msg);
        await Harness.Bus.Publish(msg with
        {
            Price = 7,
            Volume = 200
        });
        await Harness.Bus.Publish(msg with
        {
            Price = 12,
            Volume = 500
        });

        // Assert
        Assert.True(await Harness.Published.Any<OrderMatched>());

        var sagaHarness = Harness.GetSagaStateMachineHarness<EquityOrderStateMachine, EquityOrderState>();
        Assert.True(await sagaHarness.Consumed.Any<OrderMatched>());
        Assert.True(await sagaHarness.Sagas.Any(x => x.CorrelationId == orderId));
        await VerifySagaExistWithCorrectState(orderId, sagaHarness, EquityOrderStateMachine.OrderMatched!);
    }

    private async Task VerifySagaExistWithCorrectState(Guid sagaId,
        ISagaStateMachineTestHarness<EquityOrderStateMachine, EquityOrderState> sagaHarness, State state)
    {
        var existsId = await _sagaRepository.ShouldContainSagaInState(sagaId, sagaHarness.StateMachine,
            x => x.GetState(state.Name), Harness.TestTimeout);
        Assert.True(existsId.HasValue, "Saga was not created using the MessageId");
    }
}
