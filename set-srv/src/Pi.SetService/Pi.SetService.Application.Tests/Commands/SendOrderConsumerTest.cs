using System.Globalization;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Pi.SetService.Application.Commands;
using Pi.SetService.Application.Exceptions;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Options;
using Pi.SetService.Application.Services.MarketService;
using Pi.SetService.Application.Services.NumberGeneratorService;
using Pi.SetService.Application.Services.OnboardService;
using Pi.SetService.Application.Services.OneportService;
using Pi.SetService.Application.Utils;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using InstrumentProfile = Pi.SetService.Domain.AggregatesModel.InstrumentAggregate.InstrumentProfile;

namespace Pi.SetService.Application.Tests.Commands;

public class SendOrderConsumerTest : ConsumerTest
{
    private const string EnterId = "9009";
    private readonly Mock<IMarketService> _marketService;
    private readonly Mock<IEquityNumberGeneratorService> _numberGeneratorService;
    private readonly Mock<IOnboardService> _onboardService;
    private readonly Mock<IOnePortService> _onePortService;
    private readonly Mock<IOptions<SetTradingOptions>> _options;

    public SendOrderConsumerTest()
    {
        _onboardService = new Mock<IOnboardService>();
        _marketService = new Mock<IMarketService>();
        _onePortService = new Mock<IOnePortService>();
        _numberGeneratorService = new Mock<IEquityNumberGeneratorService>();
        _options = new Mock<IOptions<SetTradingOptions>>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<SendOrderConsumer>(); })
            .AddScoped<IMarketService>(_ => _marketService.Object)
            .AddScoped<IOnePortService>(_ => _onePortService.Object)
            .AddScoped<IOnboardService>(_ => _onboardService.Object)
            .AddScoped<IEquityNumberGeneratorService>(_ => _numberGeneratorService.Object)
            .AddScoped<IOptions<SetTradingOptions>>(_ => _options.Object)
            .BuildServiceProvider(true);

        _options.Setup(o => o.Value).Returns(new SetTradingOptions
        {
            EnterId = EnterId,
            NormalStartTime = DateTimeHelper.ThNow().AddHours(1).ToString("HH:mm:ss"),
            NormalEndTime = DateTimeHelper.ThNow().AddHours(2).ToString("HH:mm:ss")
        });

        SetDefaultCalendar();
    }

    private static void SetDefaultCalendar()
    {
        var cultureInfo = new CultureInfo("en-US")
        {
            DateTimeFormat = { Calendar = new GregorianCalendar() }
        };
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
    }

    [Theory]
    [InlineData(MarketStatus.Closed)]
    [InlineData(MarketStatus.OffHour)]
    public async Task Should_SendOfflineOrder_When_MarketClosed_Or_OffHour(MarketStatus marketStatus)
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Buy,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        const ulong orderNo = 123;
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _numberGeneratorService.Setup(q =>
                q.GenerateAndUpdateOfflineOrderNoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderNo);
        _onePortService.Setup(q => q.CreateNewOfflineOrder(It.IsAny<NewOfflineOrder>()))
            .Verifiable();
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(marketStatus);
        _options.Setup(o => o.Value).Returns(new SetTradingOptions
        {
            EnterId = EnterId,
            NormalStartTime = DateTimeHelper.ThNow().AddHours(1).ToString("HH:mm:ss"),
            NormalEndTime = DateTimeHelper.ThNow().AddHours(2).ToString("HH:mm:ss")
        });

        // Act
        await client.GetResponse<SendOrderResponse>(message);

        // Assert
        _onePortService.Verify(q => q.CreateNewOfflineOrder(It.IsAny<NewOfflineOrder>()), Times.Once);
    }

    [Fact]
    public async Task Should_SendMarketOrder_When_MarketOpen()
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Buy,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        var orderNo = "SO202407160001";
        var brokerOrderNo = "123049";
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _numberGeneratorService.Setup(q =>
                q.GenerateAndUpdateOnlineOrderNoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderNo);
        _onePortService.Setup(q => q.CreateNewOrder(It.IsAny<NewOrder>()))
            .ReturnsAsync(new BrokerOrderResponse
            {
                Status = BrokerOrderStatus.Accepted,
                BrokerOrderId = brokerOrderNo,
                OrderNo = orderNo
            }).Verifiable();
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Open);
        _options.Setup(o => o.Value).Returns(new SetTradingOptions
        {
            EnterId = EnterId,
            NormalStartTime = DateTimeHelper.ThNow().AddHours(-1).ToString("HH:mm:ss"),
            NormalEndTime = DateTimeHelper.ThNow().AddHours(1).ToString("HH:mm:ss")
        });

        // Act
        await client.GetResponse<SendOrderResponse>(message);

        // Assert
        _onePortService.Verify(q => q.CreateNewOrder(It.IsAny<NewOrder>()), Times.Once);
    }

    [Fact]
    public async Task Should_SendMarketOrder_When_MarketClosed_Within_NormalStartRange()
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Buy,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        var orderNo = "SO202407160001";
        var brokerOrderNo = "123049";
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _numberGeneratorService.Setup(q =>
                q.GenerateAndUpdateOnlineOrderNoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderNo);
        _onePortService.Setup(q => q.CreateNewOrder(It.IsAny<NewOrder>()))
            .ReturnsAsync(new BrokerOrderResponse
            {
                Status = BrokerOrderStatus.Accepted,
                BrokerOrderId = brokerOrderNo,
                OrderNo = orderNo
            }).Verifiable();
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Closed);
        _options.Setup(o => o.Value).Returns(new SetTradingOptions
        {
            EnterId = EnterId,
            NormalStartTime = DateTimeHelper.ThNow().AddHours(-1).ToString("HH:mm:ss"),
            NormalEndTime = DateTimeHelper.ThNow().AddHours(1).ToString("HH:mm:ss"),
        });

        // Act
        await client.GetResponse<SendOrderResponse>(message);

        // Assert
        _onePortService.Verify(q => q.CreateNewOrder(It.IsAny<NewOrder>()), Times.Once);
    }

    [Theory]
    [InlineData(ConditionPrice.Atc, OrderAction.Buy)]
    [InlineData(ConditionPrice.Ato, OrderAction.Buy)]
    [InlineData(ConditionPrice.Mtl, OrderAction.Buy)]
    [InlineData(ConditionPrice.Mkt, OrderAction.Buy)]
    public async Task Should_Return_SendOrderResponse_When_PriceIsNull(ConditionPrice conditionPrice,
        OrderAction action)
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = conditionPrice,
            Volume = 100,
            PubVolume = 100,
            Action = action,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = null
        };
        var orderNo = "SO202407160001";
        var brokerOrderNo = "123049";
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _numberGeneratorService.Setup(q =>
                q.GenerateAndUpdateOnlineOrderNoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderNo);
        _onePortService.Setup(q => q.CreateNewOrder(It.IsAny<NewOrder>()))
            .ReturnsAsync(new BrokerOrderResponse
            {
                Status = BrokerOrderStatus.Accepted,
                BrokerOrderId = brokerOrderNo,
                OrderNo = orderNo
            }).Verifiable();
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Open);

        // Act
        var response = await client.GetResponse<SendOrderResponse>(message);

        // Assert
        _onePortService.Verify(q => q.CreateNewOrder(It.IsAny<NewOrder>()));
        Assert.IsType<SendOrderResponse>(response.Message);
        Assert.Equal(orderNo, response.Message.OrderNo);
        Assert.Equal(brokerOrderNo, response.Message.BrokerOrderNo);
        Assert.Equal(EnterId, response.Message.EnterId);
        Assert.Null(response.Message.ServiceType);
    }

    [Fact]
    public async Task Should_Return_SendOrderResponse_When_Buy_Success_Within_MarketOpen()
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Buy,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        var orderNo = "SO202407160001";
        var brokerOrderNo = "123049";
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _numberGeneratorService.Setup(q =>
                q.GenerateAndUpdateOnlineOrderNoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderNo);
        _onePortService.Setup(q => q.CreateNewOrder(It.IsAny<NewOrder>()))
            .ReturnsAsync(new BrokerOrderResponse
            {
                Status = BrokerOrderStatus.Accepted,
                BrokerOrderId = brokerOrderNo,
                OrderNo = orderNo
            }).Verifiable();
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Open);

        // Act
        var response = await client.GetResponse<SendOrderResponse>(message);

        // Assert
        _onePortService.Verify(q => q.CreateNewOrder(It.IsAny<NewOrder>()));
        Assert.IsType<SendOrderResponse>(response.Message);
        Assert.Equal(orderNo, response.Message.OrderNo);
        Assert.Equal(brokerOrderNo, response.Message.BrokerOrderNo);
        Assert.Equal(EnterId, response.Message.EnterId);
        Assert.Null(response.Message.ServiceType);
    }

    [Fact]
    public async Task Should_Return_SendOrderResponse_When_Sell_Success_Within_MarketOpen()
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Sell,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Sell,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        var orderNo = "SO202407160001";
        var brokerOrderNo = "123049";
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _numberGeneratorService.Setup(q =>
                q.GenerateAndUpdateOnlineOrderNoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderNo);
        _onePortService.Setup(q => q.CreateNewOrder(It.IsAny<NewOrder>()))
            .ReturnsAsync(new BrokerOrderResponse
            {
                Status = BrokerOrderStatus.Accepted,
                BrokerOrderId = brokerOrderNo,
                OrderNo = orderNo
            }).Verifiable();
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Open);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new TradingAccount(Guid.NewGuid(), "0800783", message.TradingAccountNo, TradingAccountType.CashBalance)
            ]);
        _onePortService.Setup(q => q.GetPositions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                FakePosition(message.SecSymbol) with
                {
                    TradingAccountNo = message.TradingAccountNo,
                    StockType = StockType.Normal
                },
                FakePosition("random") with
                {
                    TradingAccountNo = message.TradingAccountNo,
                    StockType = StockType.Normal
                }
            ]);

        // Act
        var response = await client.GetResponse<SendOrderResponse>(message);

        // Assert
        _onePortService.Verify(q => q.CreateNewOrder(It.IsAny<NewOrder>()));
        Assert.IsType<SendOrderResponse>(response.Message);
        Assert.Equal(orderNo, response.Message.OrderNo);
        Assert.Equal(brokerOrderNo, response.Message.BrokerOrderNo);
        Assert.Equal(EnterId, response.Message.EnterId);
        Assert.Null(response.Message.ServiceType);
    }

    [Theory]
    [InlineData(OrderAction.Short, OrderSide.Sell, OrderType.ShortCover)]
    [InlineData(OrderAction.Cover, OrderSide.Buy, OrderType.ShortCover)]
    public async Task Should_Return_SendOrderResponse_When_ShortOrCover_Success_Within_MarketOpen(OrderAction action,
        OrderSide side, OrderType type)
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = action,
            OrderType = type,
            OrderSide = side,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        var orderNo = "SO202407160001";
        var brokerOrderNo = "123049";
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _numberGeneratorService.Setup(q =>
                q.GenerateAndUpdateOnlineOrderNoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderNo);
        _onePortService.Setup(q => q.CreateNewOrder(It.IsAny<NewOrder>()))
            .ReturnsAsync(new BrokerOrderResponse
            {
                Status = BrokerOrderStatus.Accepted,
                BrokerOrderId = brokerOrderNo,
                OrderNo = orderNo
            }).Verifiable();
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Open);
        _onePortService.Setup(q => q.GetPositions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                FakePosition(message.SecSymbol) with
                {
                    TradingAccountNo = message.TradingAccountNo,
                    StockType = StockType.Normal
                },
                FakePosition(message.SecSymbol) with
                {
                    TradingAccountNo = message.TradingAccountNo,
                    StockType = StockType.Borrow
                },
                FakePosition(message.SecSymbol) with
                {
                    TradingAccountNo = message.TradingAccountNo,
                    StockType = StockType.Short
                },
                FakePosition("random") with
                {
                    TradingAccountNo = message.TradingAccountNo,
                    StockType = StockType.Normal
                }
            ]);

        // Act
        var response = await client.GetResponse<SendOrderResponse>(message);

        // Assert
        _onePortService.Verify(q => q.CreateNewOrder(It.IsAny<NewOrder>()));
        Assert.IsType<SendOrderResponse>(response.Message);
        Assert.Equal(orderNo, response.Message.OrderNo);
        Assert.Equal(brokerOrderNo, response.Message.BrokerOrderNo);
        Assert.Equal(EnterId, response.Message.EnterId);
        Assert.Null(response.Message.ServiceType);
    }

    [Fact]
    public async Task Should_Return_SendOrderResponse_When_Buy_Success_And_MarketClosed()
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Buy,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        const ulong orderNo = 123;
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _numberGeneratorService.Setup(q =>
                q.GenerateAndUpdateOfflineOrderNoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderNo);
        _onePortService.Setup(q => q.CreateNewOfflineOrder(It.IsAny<NewOfflineOrder>()))
            .Verifiable();
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Closed);

        // Act
        var response = await client.GetResponse<SendOrderResponse>(message);

        // Assert
        _onePortService.Verify(q => q.CreateNewOfflineOrder(It.IsAny<NewOfflineOrder>()));
        Assert.IsType<SendOrderResponse>(response.Message);
        Assert.Equal(orderNo.ToString(), response.Message.OrderNo);
        Assert.Equal(orderNo.ToString(), response.Message.BrokerOrderNo);
        Assert.Equal(EnterId, response.Message.EnterId);
        Assert.Equal(ServiceType.Vip, response.Message.ServiceType);
    }

    [Fact]
    public async Task Should_Return_SendOrderResponse_When_Sell_Success_And_MarketClosed()
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Buy,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        const ulong orderNo = 123;
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _numberGeneratorService.Setup(q =>
                q.GenerateAndUpdateOfflineOrderNoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderNo);
        _onePortService.Setup(q => q.CreateNewOfflineOrder(It.IsAny<NewOfflineOrder>()))
            .Verifiable();
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Closed);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new TradingAccount(Guid.NewGuid(), "0800783", message.TradingAccountNo, TradingAccountType.CashBalance)
            ]);
        _onePortService.Setup(q => q.GetPositions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                FakePosition(message.SecSymbol) with
                {
                    TradingAccountNo = message.TradingAccountNo,
                    StockType = StockType.Normal
                },
                FakePosition("random") with
                {
                    TradingAccountNo = message.TradingAccountNo,
                    StockType = StockType.Normal
                }
            ]);

        // Act
        var response = await client.GetResponse<SendOrderResponse>(message);

        // Assert
        _onePortService.Verify(q => q.CreateNewOfflineOrder(It.IsAny<NewOfflineOrder>()));
        Assert.IsType<SendOrderResponse>(response.Message);
        Assert.Equal(orderNo.ToString(), response.Message.OrderNo);
        Assert.Equal(orderNo.ToString(), response.Message.BrokerOrderNo);
        Assert.Equal(EnterId, response.Message.EnterId);
        Assert.Equal(ServiceType.Vip, response.Message.ServiceType);
    }

    [Theory]
    [InlineData(OrderAction.Short, OrderSide.Sell, OrderType.ShortCover)]
    [InlineData(OrderAction.Cover, OrderSide.Buy, OrderType.ShortCover)]
    public async Task Should_Return_SendOrderResponse_When_ShortOrCover_Success_And_MarketClosed(OrderAction action,
        OrderSide side, OrderType type)
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = action,
            OrderType = type,
            OrderSide = side,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        const ulong orderNo = 123;
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _numberGeneratorService.Setup(q =>
                q.GenerateAndUpdateOfflineOrderNoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderNo);
        _onePortService.Setup(q => q.CreateNewOfflineOrder(It.IsAny<NewOfflineOrder>()))
            .Verifiable();
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Closed);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new TradingAccount(Guid.NewGuid(), "0800783", message.TradingAccountNo, TradingAccountType.CashBalance)
            ]);
        _onePortService.Setup(q => q.GetPositions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                FakePosition(message.SecSymbol) with
                {
                    TradingAccountNo = message.TradingAccountNo,
                    StockType = StockType.Normal
                },
                FakePosition(message.SecSymbol) with
                {
                    TradingAccountNo = message.TradingAccountNo,
                    StockType = StockType.Borrow
                },
                FakePosition(message.SecSymbol) with
                {
                    TradingAccountNo = message.TradingAccountNo,
                    StockType = StockType.Short
                },
                FakePosition("random") with
                {
                    TradingAccountNo = message.TradingAccountNo,
                    StockType = StockType.Normal
                }
            ]);

        // Act
        var response = await client.GetResponse<SendOrderResponse>(message);

        // Assert
        _onePortService.Verify(q => q.CreateNewOfflineOrder(It.IsAny<NewOfflineOrder>()));
        Assert.IsType<SendOrderResponse>(response.Message);
        Assert.Equal(orderNo.ToString(), response.Message.OrderNo);
        Assert.Equal(orderNo.ToString(), response.Message.BrokerOrderNo);
        Assert.Equal(EnterId, response.Message.EnterId);
        Assert.Equal(ServiceType.Vip, response.Message.ServiceType);
    }

    [Fact]
    public async Task
        Should_Return_SendOrderResponse_And_OrderNoIsOfflineOrderNo_When_MarketCalendar_Not_Contain_Today()
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Buy,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        const ulong orderNo = 123;
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _numberGeneratorService.Setup(q =>
                q.GenerateAndUpdateOfflineOrderNoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderNo);
        _onePortService.Setup(q => q.CreateNewOfflineOrder(It.IsAny<NewOfflineOrder>()))
            .Verifiable();
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Closed);


        // Act
        var response = await client.GetResponse<SendOrderResponse>(message);

        // Assert
        _onePortService.Verify(q => q.CreateNewOfflineOrder(It.IsAny<NewOfflineOrder>()));
        Assert.IsType<SendOrderResponse>(response.Message);
        Assert.Equal(orderNo.ToString(), response.Message.OrderNo);
        Assert.Equal(orderNo.ToString(), response.Message.BrokerOrderNo);
        Assert.Equal(EnterId, response.Message.EnterId);
        Assert.Equal(ServiceType.Vip, response.Message.ServiceType);
    }

    [Fact]
    public async Task Should_Error_When_SendOrder_Between_MaintenanceTime()
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Buy,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Maintenance);


        // Act
        var act = async () => await client.GetResponse<SendOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE107.ToString(), code);
    }

    [Theory]
    [InlineData(OrderAction.Sell, OrderSide.Sell, OrderType.Normal, SetErrorCode.SE106)]
    [InlineData(OrderAction.Short, OrderSide.Sell, OrderType.ShortCover, SetErrorCode.SE114)]
    [InlineData(OrderAction.Cover, OrderSide.Buy, OrderType.ShortCover, SetErrorCode.SE114)]
    public async Task Should_Error_When_SellOrder_With_EmptyPosition(OrderAction action, OrderSide side, OrderType type,
        SetErrorCode expectedErrorCode)
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = action,
            OrderType = type,
            OrderSide = side,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800783", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };

        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _onePortService.Setup(q => q.GetPositions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AccountPosition>());

        // Act
        var act = async () => await client.GetResponse<SendOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(expectedErrorCode.ToString(), code);
    }

    [Theory]
    [InlineData(OrderAction.Sell, OrderSide.Sell, OrderType.Normal, SetErrorCode.SE106)]
    [InlineData(OrderAction.Short, OrderSide.Sell, OrderType.ShortCover, SetErrorCode.SE106)]
    [InlineData(OrderAction.Cover, OrderSide.Buy, OrderType.ShortCover, SetErrorCode.SE106)]
    public async Task Should_Error_When_SellOrder_With_InsufficientUnitBalance(OrderAction action, OrderSide side,
        OrderType type, SetErrorCode expectedErrorCode)
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = action,
            OrderType = type,
            OrderSide = side,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800783", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };

        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _onePortService.Setup(q => q.GetPositions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                FakePosition(message.SecSymbol) with
                {
                    AvailableVolume = 10,
                    StockType = StockType.Normal
                },
                FakePosition(message.SecSymbol) with
                {
                    AvailableVolume = 10,
                    StockType = StockType.Borrow
                },
                FakePosition(message.SecSymbol) with
                {
                    AvailableVolume = 10,
                    StockType = StockType.Short
                },
                FakePosition(message.SecSymbol) with
                {
                    AvailableVolume = 10,
                    StockType = StockType.Lending
                }
            ]);

        // Act
        var act = async () => await client.GetResponse<SendOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(expectedErrorCode.ToString(), code);
    }


    [Theory]
    [InlineData(Ttf.Nvdr, false)]
    [InlineData(Ttf.None, true)]
    [InlineData(Ttf.Nvdr, true)]
    [InlineData(null, null)]
    public async Task Should_Error_When_SellOrder_With_InsufficientBalance(Ttf? ttf, bool? lending)
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Sell,
            OrderType = lending == true ? OrderType.SellLending : OrderType.Normal,
            OrderSide = OrderSide.Sell,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m,
            Ttf = ttf,
            Lending = lending
        };
        var positions = new List<AccountPosition>
        {
            new(message.SecSymbol, Ttf.None)
            {
                TradingAccountNo = "0800783-8",
                AccountNo = "08007838",
                StockType = StockType.Normal,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 90,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new(message.SecSymbol, Ttf.None)
            {
                TradingAccountNo = "0800783-8",
                AccountNo = "08007838",
                StockType = StockType.Lending,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 90,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new(message.SecSymbol, Ttf.Nvdr)
            {
                TradingAccountNo = "0800783-8",
                AccountNo = "08007838",
                StockType = StockType.Normal,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 90,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            }
        };
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800783", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };

        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _onePortService.Setup(q => q.GetPositions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(positions);

        // Act
        var act = async () => await client.GetResponse<SendOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE106.ToString(), code);
    }

    [Fact]
    public async Task Should_Error_When_InstrumentProfile_Is_Null()
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Buy,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InstrumentProfile?)null);

        // Act
        var act = async () => await client.GetResponse<SendOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE104.ToString(), code);
    }

    [Theory]
    [InlineData(8.50, 9.20, 17.00)]
    [InlineData(17.50, 9.20, 17.00)]
    public async Task Should_Error_When_RequestedPrice_Exceed_CeilingFloor(decimal price, decimal ceiling,
        decimal floor)
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Buy,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = price
        };
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = floor,
                Ceiling = ceiling
            });

        // Act
        var act = async () => await client.GetResponse<SendOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE105.ToString(), code);
    }

    [Fact]
    public async Task Should_Error_When_CeilingFloor_Is_Null()
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Buy,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 50.00m
        };
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CeilingFloor?)null);

        // Act
        var act = async () => await client.GetResponse<SendOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE104.ToString(), code);
    }

    [Fact]
    public async Task Should_Error_When_RequestedOrderType_Is_Limit_And_PriceIsNull()
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Buy,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = null
        };

        // Act
        var act = async () => await client.GetResponse<SendOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE004.ToString(), code);
    }

    [Theory]
    [InlineData(OrderAction.Borrow)]
    [InlineData(OrderAction.Return)]
    public async Task Should_Error_When_ActionUnsupported(OrderAction action)
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = action,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = null
        };

        // Act
        var act = async () => await client.GetResponse<SendOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE003.ToString(), code);
    }

    [Theory]
    [InlineData(OrderAction.Buy, OrderSide.Sell, OrderType.Normal)]
    [InlineData(OrderAction.Buy, OrderSide.Buy, OrderType.ShortCover)]
    [InlineData(OrderAction.Sell, OrderSide.Buy, OrderType.Normal)]
    [InlineData(OrderAction.Short, OrderSide.Sell, OrderType.Normal)]
    [InlineData(OrderAction.Cover, OrderSide.Buy, OrderType.Normal)]
    public async Task Should_Error_When_OrderType_Or_OrderSide_Not_MatchedWithAction(OrderAction action, OrderSide side,
        OrderType type)
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = action,
            OrderType = type,
            OrderSide = side,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = null
        };

        // Act
        var act = async () => await client.GetResponse<SendOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE002.ToString(), code);
    }

    [Theory]
    [InlineData(ExecutionTransRejectType.Fis, null)]
    [InlineData(ExecutionTransRejectType.Set, null)]
    [InlineData(null, BrokerOrderStatus.Rejected)]
    [InlineData(ExecutionTransRejectType.Fis, BrokerOrderStatus.Rejected)]
    [InlineData(ExecutionTransRejectType.Set, BrokerOrderStatus.Rejected)]
    public async Task Should_Error_When_CreateNewOrderFailed_Within_MarketOpen(ExecutionTransRejectType? rejectType,
        BrokerOrderStatus? brokerOrderStatus)
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Buy,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        var orderNo = "SO202407160001";
        var brokerOrderNo = "123049";
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _numberGeneratorService.Setup(q =>
                q.GenerateAndUpdateOnlineOrderNoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderNo);
        _onePortService.Setup(q => q.CreateNewOrder(It.IsAny<NewOrder>()))
            .ReturnsAsync(new BrokerOrderResponse
            {
                Status = brokerOrderStatus,
                BrokerOrderId = brokerOrderNo,
                OrderNo = orderNo,
                ExecutionTransRejectType = rejectType,
                Reason = "Some Error Message"
            }).Verifiable();
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Open);

        // Act
        var act = async () => await client.GetResponse<SendOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE202.ToString(), code);
    }

    [Theory]
    [InlineData(ConditionPrice.Mtl, null, SetErrorCode.SE010)]
    [InlineData(ConditionPrice.Mkt, null, SetErrorCode.SE010)]
    [InlineData(null, Condition.Gtd, SetErrorCode.SE011)]
    [InlineData(null, Condition.Gtc, SetErrorCode.SE011)]
    [InlineData(ConditionPrice.Mtl, Condition.Gtc, SetErrorCode.SE010)]
    public async Task Should_Error_When_SendOfflineOrder_With_UnsupportedPayload(ConditionPrice? conditionPrice,
        Condition? condition, SetErrorCode errorCode)
    {
        // Arrange
        var client = Harness.GetRequestClient<SendOrderRequest>();
        var message = new SendOrderRequest
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            TradingAccountType = TradingAccountType.CashBalance,
            ConditionPrice = conditionPrice ?? ConditionPrice.Limit,
            Volume = 100,
            PubVolume = 100,
            Action = OrderAction.Buy,
            OrderType = OrderType.Normal,
            OrderSide = OrderSide.Buy,
            SecSymbol = "EA",
            Condition = condition ?? Condition.None,
            Price = 9.20m
        };
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = message.SecSymbol,
                Floor = 8.50m,
                Ceiling = 10.20m
            });
        _marketService.Setup(q => q.GetInstrumentProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstrumentProfile
            {
                FriendlyName = "Energy Absolute",
                InstrumentCategory = "ThaiStock",
                Logo = "null",
                Symbol = message.SecSymbol
            });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Closed);

        // Act
        var act = async () => await client.GetResponse<SendOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(errorCode.ToString(), code);
    }

    private static AccountPosition FakePosition(string symbol, Ttf? ttf = null)
    {
        return new AccountPosition(symbol, ttf ?? Ttf.None)
        {
            TradingAccountNo = "0800783-8",
            AccountNo = "08007838",
            StockType = StockType.Borrow,
            StockTypeChar = StockTypeChar.None,
            StartVolume = 0,
            StartPrice = 0,
            AvailableVolume = 10000,
            ActualVolume = 0,
            AvgPrice = 0,
            Amount = 0
        };
    }
}
