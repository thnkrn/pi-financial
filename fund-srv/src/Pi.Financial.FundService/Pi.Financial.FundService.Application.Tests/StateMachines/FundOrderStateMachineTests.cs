using System.Diagnostics.CodeAnalysis;
using MassTransit;
using MassTransit.Events;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.StateMachines;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Domain.Events;
using OrderState = Pi.Financial.FundService.IntegrationEvents.Models.FundOrderState;

[assembly: SuppressMessage("xUnit", "xUnit1004", Justification = "Masstransit StateMachine Test is flaky on CI")]

namespace Pi.Financial.FundService.Application.Tests.StateMachines;

public class FundOrderStateMachineTests : IAsyncLifetime
{
    private ISagaRepository<FundOrderState> _sagaRepository = null!;
    private ITestHarness Harness { get; set; } = null!;
    private ServiceProvider Provider { get; set; } = null!;
    private const string IgnoreSubscriptFund = "-I";
    private const string SendToBrokerFailSuffix = "-F";
    private const string NewUnitHolderSuffix = "-N";

    private async Task InitProvider(Action<IBusRegistrationConfigurator>? configure = null)
    {
        OrderState.CleanUp();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                if (configure == null)
                {
                    cfg.AddHandler<GenerateOrderNo>(ctx =>
                    {
                        return ctx.RespondAsync(new OrderNoGenerated(ctx.Message.CorrelationId, "FOSUB2024124"));
                    });
                    cfg.AddHandler<SubscriptFund>(ctx =>
                    {
                        if (ctx.Message.TradingAccountNo.EndsWith(SendToBrokerFailSuffix))
                        {
                            throw new Exception();
                        }

                        return ctx.RespondAsync(new SubscriptionFundOrder
                        {
                            TransactionId = "randomTransactionId",
                            SaOrderReferenceNo = ctx.Message.OrderNo,
                            NewUnitHolder = ctx.Message.TradingAccountNo.EndsWith(NewUnitHolderSuffix),
                            UnitHolderId = "dm20240124",
                            UnitHolderType = UnitHolderType.SEG,
                            SaleLicense = "123"
                        });
                    });
                }
                else
                {
                    configure(cfg);
                }

                cfg.AddSagaStateMachine<FundOrderStateMachine, FundOrderState>(typeof(FundOrderStateDefinition));
            })
            .BuildServiceProvider(true);
        _sagaRepository = Provider.GetRequiredService<ISagaRepository<FundOrderState>>();
        Harness = Provider.GetRequiredService<ITestHarness>();
        await Harness.Start();
    }

    public async Task InitializeAsync()
    {
        await InitProvider();
    }

    public async Task DisposeAsync()
    {
        await Harness.Stop();
        await Provider.DisposeAsync();
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_TransitionToGeneratingOrderNo_And_PublishOrderNoGenerateRequest_When_SubscriptionFundRequestReceived()
    {
        // Arrange
        await InitProvider(_ => { });
        var msg = new SubscriptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            UnitHolderId = "4343434343",
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            BankCode = "014",
            Channel = Channel.MOB,
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 0,
            PaymentType = PaymentType.AtsSa
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Consumed.Any<SubscriptionFundRequestReceived>());
        Assert.True(await Harness.Published.Any<GenerateOrderNo>());

        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        Assert.True(await sagaHarness.Consumed.Any<SubscriptionFundRequestReceived>());
        Assert.True(await sagaHarness.Sagas.Any(x => x.CorrelationId == msg.CorrelationId));
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness, OrderState.GeneratingOrderNo!);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_TransitionToGeneratingOrderNo_And_PublishOrderNoGenerateRequest_When_RedemptionFundRequestReceived()
    {
        // Arrange
        await InitProvider(_ => { });
        var msg = new RedemptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            UnitHolderId = "123456789",
            BankCode = "014",
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Channel = Channel.MOB,
            Amount = 0,
            Unit = null,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Consumed.Any<RedemptionFundRequestReceived>());
        Assert.True(await Harness.Published.Any<GenerateOrderNo>());

        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        Assert.True(await sagaHarness.Consumed.Any<RedemptionFundRequestReceived>());
        Assert.True(await sagaHarness.Sagas.Any(x => x.CorrelationId == msg.CorrelationId));
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness, OrderState.GeneratingOrderNo!);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_TransitionToGeneratingOrderNo_And_PublishOrderNoGenerateRequest_When_SwitchingFundRequestReceived()
    {
        // Arrange
        await InitProvider(_ => { });
        var msg = new SwitchingFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            UnitHolderId = "123456789",
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 100m,
            Unit = null,
            CounterFundCode = "KF-CHINA",
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = null,
            Channel = Channel.MOB,
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Consumed.Any<SwitchingFundRequestReceived>());
        Assert.True(await Harness.Published.Any<GenerateOrderNo>());

        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        Assert.True(await sagaHarness.Consumed.Any<SwitchingFundRequestReceived>());
        Assert.True(await sagaHarness.Sagas.Any(x => x.CorrelationId == msg.CorrelationId));
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness, OrderState.GeneratingOrderNo!);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_TransitionToOrderPlaced_When_CreateSwitchInFundOrderReceived()
    {
        // Arrange
        await InitProvider(_ => { });
        var fundOrderState = new FundOrderState(Guid.NewGuid(), "77991131-M", "KF-OIL", Channel.MOB)
        {
            CustomerCode = "77991131",
            OrderNo = "TESTORDERNO",
            UnitHolderId = "77991131",
            EffectiveDate = new DateOnly(),
        };
        var msg = new CreateSwitchInFundOrderReceived(fundOrderState);

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Consumed.Any<CreateSwitchInFundOrderReceived>());

        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        Assert.True(await sagaHarness.Consumed.Any<CreateSwitchInFundOrderReceived>());
        Assert.True(await sagaHarness.Sagas.Any(x => x.CorrelationId == fundOrderState.CorrelationId));
        await VerifySagaExistWithCorrectState(fundOrderState.CorrelationId, sagaHarness, OrderState.OrderPlaced!);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_TransitionToOrderCancelled_When_CancelFundOrderReceived_For_SubscriptionOrder()
    {
        // Arrange
        var msg = new SubscriptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            UnitHolderId = "4343434343",
            FundCode = "KF-Oil",
            Channel = Channel.MOB,
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            BankCode = "014",
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 100,
            PaymentType = PaymentType.AtsAmc
        };

        // Act
        await Harness.Bus.Publish(msg);
        await Harness.Published.Any<SubscriptionOrderPlaced>();
        await Harness.Bus.Publish(new CancelFundOrderReceived(msg.CorrelationId));

        // Assert
        Assert.True(await Harness.Published.Any<CancelFundOrderReceived>());
        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        Assert.True(await sagaHarness.Consumed.Any<CancelFundOrderReceived>());
        Assert.True(await sagaHarness.Sagas.Any(x => x.CorrelationId == msg.CorrelationId));
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness, OrderState.OrderCancelled!);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_TransitionToOrderCancelled_When_CancelFundOrderReceived_For_SwitchingOrder()
    {
        // Arrange
        var response = new SwitchingFundOrder
        {
            TransactionId = "TransactionId",
            UnitHolderId = "dm1110000",
            SaOrderReferenceNo = "FORED20240129",
            SellAllFlag = false,
            SaleLicense = "123"
        };
        await InitProvider(cfg =>
        {
            cfg.AddHandler<GenerateOrderNo>(ctx =>
            {
                return ctx.RespondAsync(new OrderNoGenerated(ctx.Message.CorrelationId, "FOSUB2024124"));
            });
            cfg.AddHandler<SwitchingFund>(ctx =>
            {
                return ctx.RespondAsync(response);
            });
        });
        var msg = new SwitchingFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            CounterFundCode = "KF-CHINA",
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = null,
            TradingAccountNo = "771931-M",
            UnitHolderId = "123456789",
            CustomerCode = "771931",
            Amount = 100m,
            Unit = null,
            Channel = Channel.MOB,
        };

        // Act
        await Harness.Bus.Publish(msg);
        await Harness.Published.Any<SwitchOrderPlaced>();
        await Harness.Bus.Publish(new CancelFundOrderReceived(msg.CorrelationId));

        // Assert
        Assert.True(await Harness.Published.Any<CancelFundOrderReceived>());
        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        Assert.True(await sagaHarness.Consumed.Any<CancelFundOrderReceived>());
        Assert.True(await sagaHarness.Sagas.Any(x => x.CorrelationId == msg.CorrelationId));
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness, OrderState.OrderCancelled!);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_SetFieldsAsExpected_When_SubscriptionFundRequestReceived()
    {
        // Arrange
        var msg = new SubscriptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            BankCode = "014",
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Channel = Channel.MOB,
            UnitHolderId = "4343434343",
            Amount = 10000,
            PaymentType = PaymentType.AtsSa
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Consumed.Any<SubscriptionFundRequestReceived>());

        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        var fundOrder = sagaHarness.Sagas.Select(x => x.CorrelationId == msg.CorrelationId).FirstOrDefault()?.Saga;
        Assert.NotNull(fundOrder);
        Assert.Equal(FundOrderType.Subscription, fundOrder.OrderType);
        Assert.Equal(OrderSide.Buy, fundOrder.OrderSide);
        Assert.Equal(msg.CorrelationId, fundOrder.CorrelationId);
        Assert.Equal(msg.FundCode, fundOrder.FundCode);
        Assert.Equal(msg.BankAccount, fundOrder.BankAccount);
        Assert.Equal(msg.BankCode, fundOrder.BankCode);
        Assert.Equal(msg.Amount, fundOrder.Amount);
        Assert.Equal(msg.TradingAccountNo, fundOrder.TradingAccountNo);
        Assert.Equal(msg.EffectiveDate, fundOrder.EffectiveDate);
        Assert.Equal(msg.CustomerCode, fundOrder.CustomerCode);
        Assert.Equal(msg.PaymentType, fundOrder.PaymentType);
        Assert.Null(fundOrder.Unit);
        Assert.Null(fundOrder.RedemptionType);
        Assert.Null(fundOrder.SellAllUnit);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_SetFieldsAsExpected_When_RedemptionFundRequestReceived()
    {
        // Arrange
        var msg = new RedemptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            Channel = Channel.MOB,
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            BankCode = "014",
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 10000,
            Unit = 0,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = true,
            UnitHolderId = "123456789",
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Consumed.Any<RedemptionFundRequestReceived>());

        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        var fundOrder = sagaHarness.Sagas.Select(x => x.CorrelationId == msg.CorrelationId).FirstOrDefault()?.Saga;
        Assert.NotNull(fundOrder);
        Assert.Equal(FundOrderType.Redemption, fundOrder.OrderType);
        Assert.Equal(OrderSide.Sell, fundOrder.OrderSide);
        Assert.Equal(msg.CorrelationId, fundOrder.CorrelationId);
        Assert.Equal(msg.FundCode, fundOrder.FundCode);
        Assert.Equal(msg.BankAccount, fundOrder.BankAccount);
        Assert.Equal(msg.BankCode, fundOrder.BankCode);
        Assert.Equal(msg.Amount, fundOrder.Amount);
        Assert.Equal(msg.TradingAccountNo, fundOrder.TradingAccountNo);
        Assert.Equal(msg.EffectiveDate, fundOrder.EffectiveDate);
        Assert.Equal(msg.CustomerCode, fundOrder.CustomerCode);
        Assert.Equal(msg.Unit, fundOrder.Unit);
        Assert.Equal(msg.RedemptionType, fundOrder.RedemptionType);
        Assert.Equal(msg.SellAllFlag, fundOrder.SellAllUnit);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_SetFieldsAsExpected_When_SwitchingFundRequestReceived()
    {
        // Arrange
        var msg = new SwitchingFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            TradingAccountNo = "771931-M",
            UnitHolderId = "123456789",
            CustomerCode = "771931",
            Amount = 100m,
            Unit = null,
            CounterFundCode = "KF-CHINA",
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = null,
            Channel = Channel.MOB,
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Consumed.Any<SwitchingFundRequestReceived>());

        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        var fundOrder = sagaHarness.Sagas.Select(x => x.CorrelationId == msg.CorrelationId).FirstOrDefault()?.Saga;
        Assert.NotNull(fundOrder);
        Assert.Equal(FundOrderType.SwitchOut, fundOrder.OrderType);
        Assert.Equal(OrderSide.Switch, fundOrder.OrderSide);
        Assert.Equal(msg.CorrelationId, fundOrder.CorrelationId);
        Assert.Equal(msg.FundCode, fundOrder.FundCode);
        Assert.Null(fundOrder.BankCode);
        Assert.Null(fundOrder.BankAccount);
        Assert.Equal(msg.Amount, fundOrder.Amount);
        Assert.Equal(msg.TradingAccountNo, fundOrder.TradingAccountNo);
        Assert.Equal(msg.EffectiveDate, fundOrder.EffectiveDate);
        Assert.Equal(msg.CustomerCode, fundOrder.CustomerCode);
        Assert.Equal(msg.Unit, fundOrder.Unit);
        Assert.Equal(msg.RedemptionType, fundOrder.RedemptionType);
        Assert.Equal(msg.SellAllFlag, fundOrder.SellAllUnit);
        Assert.Equal(msg.CounterFundCode, fundOrder.CounterFundCode);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_SetOrderNo_When_OrderNoGenerateRequestCompleted()
    {
        // Arrange
        var msg = new SubscriptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            Channel = Channel.MOB,
            EffectiveDate = new DateOnly(),
            UnitHolderId = "4343434343",
            BankAccount = "3612830792",
            BankCode = "014",
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 100,
            PaymentType = PaymentType.AtsSa
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        await Harness.Consumed.Any<ReceivedMessage<OrderNoGenerated>>();

        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        var updatedFundOrder = sagaHarness.Sagas.Select(x => x.CorrelationId == msg.CorrelationId).FirstOrDefault()?.Saga;
        Assert.NotNull(updatedFundOrder!.OrderNo);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_TransitionToSendingOrderToBroker_When_OrderNoGenerateRequestCompleted()
    {
        // Arrange
        await InitProvider(cfg =>
        {
            cfg.AddHandler<GenerateOrderNo>(ctx =>
            {
                return ctx.RespondAsync(new OrderNoGenerated(ctx.Message.CorrelationId, "FOSUB2024124"));
            });
        });
        var msg = new SubscriptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            Channel = Channel.MOB,
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            UnitHolderId = "4343434343",
            BankCode = "014",
            TradingAccountNo = $"771931{IgnoreSubscriptFund}",
            CustomerCode = "771931",
            Amount = 0,
            PaymentType = PaymentType.AtsSa
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        await Harness.Consumed.Any<ReceivedMessage<OrderNoGenerated>>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness, OrderState.SendingOrderToBroker!);

    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_SendSubscriptFund_When_SubscriptionFundRequestReceived_And_OrderSideIsBuy()
    {
        // Arrange
        var msg = new SubscriptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            Channel = Channel.MOB,
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            BankCode = "014",
            TradingAccountNo = "771931-M",
            UnitHolderId = "4343434343",
            CustomerCode = "771931",
            Amount = 100,
            PaymentType = PaymentType.AtsAmc
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Published.Any<SubscriptFund>());
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_SendRedeemFund_When_RedemptionFundRequestReceived_And_OrderSideIsSell()
    {
        // Arrange
        var msg = new RedemptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            UnitHolderId = "123456789",
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            Channel = Channel.MOB,
            BankCode = "014",
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 100,
            Unit = null,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Published.Any<RedeemFund>());
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_SendSwitchingFund_When_SwitchingFundRequestReceived_And_OrderSideIsSwitch()
    {
        // Arrange
        var msg = new SwitchingFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            UnitHolderId = "123456789",
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 100m,
            Unit = null,
            CounterFundCode = "KF-CHINA",
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = null,
            Channel = Channel.MOB,
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Published.Any<SwitchingFund>());
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_SetBrokerOrderId_When_SubscriptFundSuccess()
    {
        // Arrange
        var msg = new SubscriptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            UnitHolderId = "4343434343",
            Channel = Channel.MOB,
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            BankCode = "014",
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 100,
            PaymentType = PaymentType.AtsAmc
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        await Harness.Consumed.Any<ReceivedMessage<SubscriptionFundOrder>>();

        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        var updatedFundOrder = sagaHarness.Sagas.Select(x => x.CorrelationId == msg.CorrelationId).FirstOrDefault()?.Saga;
        Assert.NotNull(updatedFundOrder?.BrokerOrderId);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_SetBrokerOrderId_And_ExpectedFields_When_RedeemFundSuccess()
    {
        // Arrange
        var response = new RedemptionFundOrder
        {
            TransactionId = "TransactionId",
            SaOrderReferenceNo = "FORED20240129",
            SettlementDate = new DateOnly(),
            PaymentType = PaymentType.AtsSa,
            UnitHolderId = "dm1110000",
            SellAllFlag = false,
            SaleLicense = "123"
        };
        await InitProvider(cfg =>
        {
            cfg.AddHandler<GenerateOrderNo>(ctx =>
            {
                return ctx.RespondAsync(new OrderNoGenerated(ctx.Message.CorrelationId, "FOSUB2024124"));
            });
            cfg.AddHandler<RedeemFund>(ctx =>
            {
                return ctx.RespondAsync(response);
            });
        });
        var msg = new RedemptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            UnitHolderId = "123456789",
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            BankCode = "014",
            Channel = Channel.MOB,
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 100,
            Unit = null,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        await Harness.Consumed.Any<ReceivedMessage<RedemptionFundOrder>>();

        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        var updatedFundOrder = sagaHarness.Sagas.Select(x => x.CorrelationId == msg.CorrelationId).FirstOrDefault()?.Saga;
        Assert.Equal(response.TransactionId, updatedFundOrder?.BrokerOrderId);
        Assert.Equal(response.PaymentType, updatedFundOrder?.PaymentType);
        Assert.Equal(response.SellAllFlag, updatedFundOrder?.SellAllUnit);
        Assert.Equal(response.UnitHolderId, updatedFundOrder?.UnitHolderId);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_SetBrokerOrderId_And_ExpectedFields_When_SwitchingFundSuccess()
    {
        // Arrange
        var response = new SwitchingFundOrder
        {
            TransactionId = "TransactionId",
            SaOrderReferenceNo = "FORED20240129",
            UnitHolderId = "dm1110000",
            SellAllFlag = false,
            SaleLicense = "123"
        };
        await InitProvider(cfg =>
        {
            cfg.AddHandler<GenerateOrderNo>(ctx =>
            {
                return ctx.RespondAsync(new OrderNoGenerated(ctx.Message.CorrelationId, "FOSUB2024124"));
            });
            cfg.AddHandler<SwitchingFund>(ctx =>
            {
                return ctx.RespondAsync(response);
            });
        });
        var msg = new SwitchingFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            TradingAccountNo = "771931-M",
            UnitHolderId = "123456789",
            CustomerCode = "771931",
            Amount = 100m,
            Unit = null,
            CounterFundCode = "KF-CHINA",
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = null,
            Channel = Channel.MOB,
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        await Harness.Consumed.Any<ReceivedMessage<SwitchingFundOrder>>();

        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        var updatedFundOrder = sagaHarness.Sagas.Select(x => x.CorrelationId == msg.CorrelationId).FirstOrDefault()?.Saga;
        Assert.Equal(response.TransactionId, updatedFundOrder?.BrokerOrderId);
        Assert.Equal(response.SellAllFlag, updatedFundOrder?.SellAllUnit);
        Assert.Equal(response.UnitHolderId, updatedFundOrder?.UnitHolderId);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_TransitionToOrderPlaced_When_SubscriptFundSuccess()
    {
        // Arrange
        var msg = new SubscriptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            BankCode = "014",
            UnitHolderId = "4343434343",
            Channel = Channel.MOB,
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 100,
            PaymentType = PaymentType.AtsAmc
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        await Harness.Consumed.Any<ReceivedMessage<SubscriptionFundOrder>>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness, OrderState.OrderPlaced!);

    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_TransitionToOrderPlaced_When_RedeemFundSuccess()
    {
        // Arrange
        await InitProvider(cfg =>
        {
            cfg.AddHandler<GenerateOrderNo>(ctx =>
            {
                return ctx.RespondAsync(new OrderNoGenerated(ctx.Message.CorrelationId, "FOSUB2024124"));
            });
            cfg.AddHandler<RedeemFund>(ctx =>
            {
                return ctx.RespondAsync(new RedemptionFundOrder
                {
                    TransactionId = "TransactionId",
                    SaOrderReferenceNo = "FORED2024124",
                    SettlementDate = new DateOnly(),
                    PaymentType = PaymentType.AtsSa,
                    UnitHolderId = "dm1110000",
                    SellAllFlag = false,
                    SaleLicense = "123"
                });
            });
        });
        var msg = new RedemptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            UnitHolderId = "123456789",
            TradingAccountId = Guid.NewGuid(),
            BankAccount = "3612830792",
            BankCode = "014",
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Channel = Channel.MOB,
            Amount = 100,
            Unit = null,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        await Harness.Consumed.Any<ReceivedMessage<RedemptionFundOrder>>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        Assert.NotNull(sagaHarness.Sagas.ContainsInState(msg.CorrelationId, sagaHarness.StateMachine, OrderState.OrderPlaced));
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_TransitionToOrderPlaced_When_SwitchingFundSuccess()
    {
        // Arrange
        var response = new SwitchingFundOrder
        {
            TransactionId = "TransactionId",
            SaOrderReferenceNo = "FORED20240129",
            UnitHolderId = "dm1110000",
            SellAllFlag = false,
            SaleLicense = "123"
        };
        await InitProvider(cfg =>
        {
            cfg.AddHandler<GenerateOrderNo>(ctx =>
            {
                return ctx.RespondAsync(new OrderNoGenerated(ctx.Message.CorrelationId, "FOSUB2024124"));
            });
            cfg.AddHandler<SwitchingFund>(ctx =>
            {
                return ctx.RespondAsync(response);
            });
        });
        var msg = new SwitchingFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            UnitHolderId = "123456789",
            EffectiveDate = new DateOnly(),
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 100m,
            Unit = null,
            CounterFundCode = "KF-CHINA",
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = null,
            Channel = Channel.MOB,
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        await Harness.Consumed.Any<ReceivedMessage<RedemptionFundOrder>>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness, OrderState.OrderPlaced!);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_PublishSubscriptionCreated_When_SubscriptFundSuccess()
    {
        // Arrange
        var msg = new SubscriptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            UnitHolderId = "4343434343",
            TradingAccountId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            Channel = Channel.MOB,
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            BankCode = "014",
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 100,
            PaymentType = PaymentType.AtsAmc
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Published.Any<SubscriptionOrderPlaced>());
        Assert.False(await Harness.Published.Any<CreateUnitHolder>());
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_PublishCreateUnitHolder_When_SubscriptFundSuccess_With_NewUnitHolderResponse()
    {
        // Arrange
        var msg = new SubscriptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            TradingAccountId = Guid.NewGuid(),
            BankAccount = "3612830792",
            UnitHolderId = "4343434343",
            BankCode = "014",
            Channel = Channel.MOB,
            TradingAccountNo = $"771931{NewUnitHolderSuffix}",
            CustomerCode = "771931",
            Amount = 100,
            PaymentType = PaymentType.AtsAmc
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Published.Any<CreateUnitHolder>());
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_TransitionToPlaceOrderFailed_When_SubscriptFundFailed()
    {
        // Arrange
        var msg = new SubscriptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            TradingAccountId = Guid.NewGuid(),
            UnitHolderId = "4343434343",
            Channel = Channel.MOB,
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            BankCode = "014",
            TradingAccountNo = $"771931{SendToBrokerFailSuffix}",
            CustomerCode = "771931",
            Amount = 100,
            PaymentType = PaymentType.AtsAmc
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        await Harness.Consumed.Any<FaultEvent<SubscriptionFundOrder>>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness, OrderState.PlaceOrderFailed!);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_TransitionToPlaceOrderFailed_When_RedeemFundFailed()
    {
        // Arrange
        await InitProvider(cfg =>
        {
            cfg.AddHandler<GenerateOrderNo>(ctx =>
            {
                return ctx.RespondAsync(new OrderNoGenerated(ctx.Message.CorrelationId, "FOSUB2024124"));
            });
            cfg.AddHandler((ConsumeContext<RedeemFund> _) =>
            {
                throw new Exception();
            });
        });
        var msg = new RedemptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            TradingAccountId = Guid.NewGuid(),
            BankAccount = "3612830792",
            BankCode = "014",
            UnitHolderId = "123456789",
            Channel = Channel.MOB,
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 100,
            Unit = null,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        await Harness.Consumed.Any<ReceivedMessage<RedemptionFundOrder>>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness, OrderState.PlaceOrderFailed!);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_TransitionToPlaceOrderFailed_When_SwitchingFundFailed()
    {
        // Arrange
        await InitProvider(cfg =>
        {
            cfg.AddHandler<GenerateOrderNo>(ctx =>
            {
                return ctx.RespondAsync(new OrderNoGenerated(ctx.Message.CorrelationId, "FOSUB2024124"));
            });
            cfg.AddHandler((ConsumeContext<SwitchingFund> _) =>
            {
                throw new Exception();
            });
        });
        var msg = new SwitchingFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            TradingAccountId = Guid.NewGuid(),
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            UnitHolderId = "123456789",
            Amount = 100m,
            Unit = null,
            CounterFundCode = "KF-CHINA",
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = null,
            Channel = Channel.MOB,
        };

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        await Harness.Consumed.Any<ReceivedMessage<SwitchingFundOrder>>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<FundOrderStateMachine, FundOrderState>();
        await VerifySagaExistWithCorrectState(msg.CorrelationId, sagaHarness, OrderState.PlaceOrderFailed!);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_ResponseSubscriptionFundOrder_When_SubscriptionFundRequestReceived_And_SubscriptFundSuccess()
    {
        // Arrange
        var client = Harness.GetRequestClient<SubscriptionFundRequestReceived>();
        var msg = new SubscriptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            TradingAccountId = Guid.NewGuid(),
            BankAccount = "3612830792",
            BankCode = "014",
            Channel = Channel.MOB,
            TradingAccountNo = "771931-M",
            UnitHolderId = "4343434343",
            CustomerCode = "771931",
            Amount = 100,
            PaymentType = PaymentType.AtsAmc
        };

        // Act
        var response = await client.GetResponse<SubscriptionFundOrder>(msg);

        // Assert
        Assert.IsType<SubscriptionFundOrder>(response.Message);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_ResponseRedemptionFundOrder_When_RedemptionFundRequestReceived_And_RedeemFundSuccess()
    {
        // Arrange
        await InitProvider(cfg =>
        {
            cfg.AddHandler<GenerateOrderNo>(ctx =>
            {
                return ctx.RespondAsync(new OrderNoGenerated(ctx.Message.CorrelationId, "FORED2024124"));
            });
            cfg.AddHandler<RedeemFund>(ctx =>
            {
                return ctx.RespondAsync(new RedemptionFundOrder
                {
                    TransactionId = "TransactionId",
                    SaOrderReferenceNo = "FORED2024124",
                    SettlementDate = new DateOnly(),
                    PaymentType = PaymentType.AtsSa,
                    UnitHolderId = "dm1110000",
                    SellAllFlag = false,
                    SaleLicense = "123"
                });
            });
        });
        var client = Harness.GetRequestClient<RedemptionFundRequestReceived>();
        var msg = new RedemptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            BankCode = "014",
            TradingAccountId = Guid.NewGuid(),
            Channel = Channel.MOB,
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 100,
            Unit = null,
            UnitHolderId = "123456789",
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
        };

        // Act
        var response = await client.GetResponse<RedemptionFundOrder>(msg);

        // Assert
        Assert.IsType<RedemptionFundOrder>(response.Message);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_ResponseSwitchingFundOrder_When_SwitchingFundRequestReceived_And_SwitchingFundSuccess()
    {
        // Arrange
        await InitProvider(cfg =>
        {
            cfg.AddHandler<GenerateOrderNo>(ctx =>
            {
                return ctx.RespondAsync(new OrderNoGenerated(ctx.Message.CorrelationId, "FOSWI2024124"));
            });
            cfg.AddHandler<SwitchingFund>(ctx =>
            {
                return ctx.RespondAsync(new SwitchingFundOrder
                {
                    TransactionId = "TransactionId",
                    SaOrderReferenceNo = "FORED20240129",
                    UnitHolderId = "dm1110000",
                    SellAllFlag = false,
                    SaleLicense = "123"
                });
            });
        });
        var client = Harness.GetRequestClient<SwitchingFundRequestReceived>();
        var msg = new SwitchingFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            TradingAccountId = Guid.NewGuid(),
            EffectiveDate = new DateOnly(),
            UnitHolderId = "123456789",
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 100m,
            Unit = null,
            CounterFundCode = "KF-CHINA",
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = null,
            Channel = Channel.MOB,
        };

        // Act
        var response = await client.GetResponse<SwitchingFundOrder>(msg);

        // Assert
        Assert.IsType<SwitchingFundOrder>(response.Message);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_ResponsePlaceOrderFailed_With_FOE201_When_SubscriptionFundRequestReceived_And_SubscriptFundFailed()
    {
        // Arrange
        var client = Harness.GetRequestClient<SubscriptionFundRequestReceived>();
        var msg = new SubscriptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            Channel = Channel.MOB,
            UnitHolderId = "4343434343",
            TradingAccountId = Guid.NewGuid(),
            BankCode = "014",
            TradingAccountNo = $"771931{SendToBrokerFailSuffix}",
            CustomerCode = "771931",
            Amount = 100,
            PaymentType = PaymentType.AtsAmc
        };

        // Act
        var response = await client.GetResponse<PlaceOrderFailed>(msg);

        // Assert
        Assert.IsType<PlaceOrderFailed>(response.Message);
        Assert.Equal(msg.CorrelationId, response.Message.CorrelationId);
        Assert.Equal(FundOrderErrorCode.FOE201, response.Message.ErrorCode);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_ResponsePlaceOrderFailed_With_FOE201_When_RedemptionFundRequestReceived_And_RedeemFundFailed()
    {
        // Arrange
        await InitProvider(cfg =>
        {
            cfg.AddHandler<GenerateOrderNo>(ctx =>
            {
                return ctx.RespondAsync(new OrderNoGenerated(ctx.Message.CorrelationId, "FOSUB2024124"));
            });
            cfg.AddHandler((ConsumeContext<RedeemFund> _) =>
            {
                throw new Exception();
            });
        });
        var client = Harness.GetRequestClient<RedemptionFundRequestReceived>();
        var msg = new RedemptionFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            BankAccount = "3612830792",
            Channel = Channel.MOB,
            BankCode = "014",
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            UnitHolderId = "123456789",
            TradingAccountId = Guid.NewGuid(),
            Amount = 100,
            Unit = null,
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = false,
        };

        // Act
        var response = await client.GetResponse<PlaceOrderFailed>(msg);

        // Assert
        Assert.IsType<PlaceOrderFailed>(response.Message);
        Assert.Equal(msg.CorrelationId, response.Message.CorrelationId);
        Assert.Equal(FundOrderErrorCode.FOE201, response.Message.ErrorCode);
    }

    [Fact(Skip = "Flaky test on CI")]
    public async Task Should_ResponsePlaceOrderFailed_With_FOE201_When_RedemptionFundRequestReceived_And_SwitchingFundFailed()
    {
        // Arrange
        await InitProvider(cfg =>
        {
            cfg.AddHandler<GenerateOrderNo>(ctx =>
            {
                return ctx.RespondAsync(new OrderNoGenerated(ctx.Message.CorrelationId, "FOSUB2024124"));
            });
            cfg.AddHandler((ConsumeContext<SwitchingFund> _) =>
            {
                throw new Exception();
            });
        });
        var client = Harness.GetRequestClient<SwitchingFundRequestReceived>();
        var msg = new SwitchingFundRequestReceived
        {
            CorrelationId = Guid.NewGuid(),
            FundCode = "KF-Oil",
            EffectiveDate = new DateOnly(),
            TradingAccountNo = "771931-M",
            CustomerCode = "771931",
            Amount = 100m,
            TradingAccountId = Guid.NewGuid(),
            UnitHolderId = "123456789",
            Unit = null,
            CounterFundCode = "KF-CHINA",
            RedemptionType = RedemptionType.Amount,
            SellAllFlag = null,
            Channel = Channel.MOB,
        };

        // Act
        var response = await client.GetResponse<PlaceOrderFailed>(msg);

        // Assert
        Assert.IsType<PlaceOrderFailed>(response.Message);
        Assert.Equal(msg.CorrelationId, response.Message.CorrelationId);
        Assert.Equal(FundOrderErrorCode.FOE201, response.Message.ErrorCode);
    }

    private async Task VerifySagaExistWithCorrectState(Guid sagaId, ISagaStateMachineTestHarness<FundOrderStateMachine, FundOrderState> sagaHarness, State state)
    {
        var existsId = await _sagaRepository.ShouldContainSagaInState(sagaId, sagaHarness.StateMachine, x => x.GetState(state.Name), Harness.TestTimeout);
        Assert.True(existsId.HasValue, "Saga was not created using the MessageId");
    }
}
