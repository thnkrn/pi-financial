using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.Generators.Number;
using Pi.SetService.Application.Commands;
using Pi.SetService.Application.Services.NumberGeneratorService;
using Pi.SetService.Application.Services.OneportService;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using Pi.SetService.Domain.Events;

namespace Pi.SetService.Application.Tests.Commands;

public class CreateSblOrderConsumerTest : ConsumerTest
{
    private readonly Mock<IInstrumentRepository> _instrumentRepository;
    private readonly Mock<ISblOrderRepository> _sblOrderRepository;
    private readonly Mock<IEquityNumberGeneratorService> _equityNumberGeneratorService;
    private readonly Mock<IOnePortService> _onePortService;

    public CreateSblOrderConsumerTest()
    {
        _instrumentRepository = new Mock<IInstrumentRepository>();
        _sblOrderRepository = new Mock<ISblOrderRepository>();
        _equityNumberGeneratorService = new Mock<IEquityNumberGeneratorService>();
        _onePortService = new Mock<IOnePortService>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<CreateSblOrderConsumer>(); })
            .AddScoped<IInstrumentRepository>(_ => _instrumentRepository.Object)
            .AddScoped<ISblOrderRepository>(_ => _sblOrderRepository.Object)
            .AddScoped<IEquityNumberGeneratorService>(_ => _equityNumberGeneratorService.Object)
            .AddScoped<IOnePortService>(_ => _onePortService.Object)
            .AddScoped<ILogger<CreateSblOrderConsumer>>(_ => Mock.Of<ILogger<CreateSblOrderConsumer>>())
            .BuildServiceProvider(true);
    }

    [Fact]
    public async Task Should_ReturnPlaceOrderSuccess_When_Borrow_Success()
    {
        // Arrange
        var client = Harness.GetRequestClient<CreateSblOrder>();
        var payload = new CreateSblOrder
        {
            Id = Guid.NewGuid(),
            TradingAccount = FakeTradingAccount(),
            Symbol = "EA",
            Volume = 1000000,
            Type = SblOrderType.Borrow
        };
        var sblInstrument = new SblInstrument(Guid.NewGuid(), payload.Symbol, 5.00m, 2000000, 1000000, 1000000);
        const ulong orderId = 123456;
        _instrumentRepository.Setup(q => q.GetSblInstrument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sblInstrument);
        _equityNumberGeneratorService.Setup(q => q.GenerateSblOrderOrderIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderId);
        _sblOrderRepository.Setup(q => q.CreateAsync(It.IsAny<SblOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SblOrder(Guid.NewGuid(), Guid.NewGuid(), payload.TradingAccount.TradingAccountNo, payload.TradingAccount.CustomerCode, orderId, payload.Symbol, SblOrderStatus.Pending, payload.Volume, payload.Type, null, null));
        _instrumentRepository.Setup(q => q.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var actual = await client.GetResponse<PlaceOrderSuccess>(payload);

        // Assert
        Assert.Equal(orderId.ToString(), actual.Message.BrokerOrderId);
        Assert.Equal(orderId.ToString(), actual.Message.OrderNo);
        Assert.Equal(0, sblInstrument.AvailableLending);
        Assert.Equal(2000000, sblInstrument.BorrowOutstanding);
    }

    [Fact]
    public async Task Should_ReturnPlaceOrderSuccess_When_Return_Success_With_CreditAccount()
    {
        // Arrange
        var client = Harness.GetRequestClient<CreateSblOrder>();
        var payload = new CreateSblOrder
        {
            Id = Guid.NewGuid(),
            TradingAccount = FakeTradingAccount(),
            Symbol = "EA",
            Volume = 1000000,
            Type = SblOrderType.Return
        };
        var sblInstrument = new SblInstrument(Guid.NewGuid(), payload.Symbol, 5.00m, 2000000, 1000000, 1000000);
        var position = new AccountPosition(payload.Symbol, Ttf.None)
        {
            TradingAccountNo = payload.TradingAccount.TradingAccountNo,
            AccountNo = payload.TradingAccount.AccountNo,
            StockType = StockType.Borrow,
            StockTypeChar = StockTypeChar.None,
            StartVolume = 0,
            StartPrice = 0,
            AvailableVolume = payload.Volume,
            ActualVolume = 0,
            AvgPrice = 0,
            Amount = 0
        };
        var positions = new List<AccountPosition> {
            position with { StockType = StockType.Borrow },
            position with { StockType = StockType.Short },
            position with { StockType = StockType.Normal },
            new ("random", Ttf.None)
            {
                TradingAccountNo = payload.TradingAccount.TradingAccountNo,
                AccountNo = payload.TradingAccount.AccountNo,
                StockType = StockType.Borrow,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = payload.Volume,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            }
        };
        const ulong orderId = 123456;
        _instrumentRepository.Setup(q => q.GetSblInstrument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sblInstrument);
        _equityNumberGeneratorService.Setup(q => q.GenerateSblOrderOrderIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderId);
        _sblOrderRepository.Setup(q => q.CreateAsync(It.IsAny<SblOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SblOrder(Guid.NewGuid(), Guid.NewGuid(), payload.TradingAccount.TradingAccountNo, payload.TradingAccount.CustomerCode, orderId, payload.Symbol, SblOrderStatus.Pending, payload.Volume, payload.Type, null, null));
        _instrumentRepository.Setup(q => q.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _onePortService.Setup(q => q.GetPositions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(positions);

        // Act
        var actual = await client.GetResponse<PlaceOrderSuccess>(payload);

        // Assert
        Assert.Equal(orderId.ToString(), actual.Message.BrokerOrderId);
        Assert.Equal(orderId.ToString(), actual.Message.OrderNo);
        Assert.Equal(2000000, sblInstrument.AvailableLending);
        Assert.Equal(0, sblInstrument.BorrowOutstanding);
    }

    [Fact]
    public async Task Should_ReturnPlaceOrderSuccess_With_ExpectedOrderId_When_Borrow_Success()
    {

        // Arrange
        var client = Harness.GetRequestClient<CreateSblOrder>();
        var payload = new CreateSblOrder
        {
            Id = Guid.NewGuid(),
            TradingAccount = FakeTradingAccount(),
            Symbol = "EA",
            Volume = 1000000,
            Type = SblOrderType.Borrow
        };
        var sblInstrument = new SblInstrument(Guid.NewGuid(), payload.Symbol, 5.00m, 1000000, 0, 1000000);
        ulong orderId = 1234444;
        _instrumentRepository.Setup(q => q.GetSblInstrument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sblInstrument);
        _equityNumberGeneratorService.SetupSequence(q => q.GenerateSblOrderOrderIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((ulong)99999)
            .ReturnsAsync((ulong)88888)
            .ReturnsAsync(orderId);
        _sblOrderRepository.SetupSequence(q => q.CreateAsync(It.IsAny<SblOrder>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DuplicateRecordNoException())
            .ThrowsAsync(new DuplicateRecordNoException())
            .ReturnsAsync(new SblOrder(Guid.NewGuid(), Guid.NewGuid(), payload.TradingAccount.TradingAccountNo, payload.TradingAccount.CustomerCode, orderId, payload.Symbol, SblOrderStatus.Pending, payload.Volume, payload.Type, null, null));
        _instrumentRepository.Setup(q => q.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var actual = await client.GetResponse<PlaceOrderSuccess>(payload);

        // Assert
        Assert.Equal(orderId.ToString(), actual.Message.BrokerOrderId);
        Assert.Equal(orderId.ToString(), actual.Message.OrderNo);
    }

    [Fact]
    public async Task Should_ReturnPlaceOrderSuccess_With_ExpectedOrderId_When_Return_Success()
    {

        // Arrange
        var client = Harness.GetRequestClient<CreateSblOrder>();
        var payload = new CreateSblOrder
        {
            Id = Guid.NewGuid(),
            TradingAccount = FakeTradingAccount(),
            Symbol = "EA",
            Volume = 1000000,
            Type = SblOrderType.Borrow
        };
        var positions = new List<AccountPosition> {
            new (payload.Symbol, Ttf.None)
            {
                TradingAccountNo = payload.TradingAccount.TradingAccountNo,
                AccountNo = payload.TradingAccount.AccountNo,
                StockType = StockType.Borrow,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = payload.Volume,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            }
        };

        var sblInstrument = new SblInstrument(Guid.NewGuid(), payload.Symbol, 5.00m, 1000000, 0, 1000000);
        ulong orderId = 1234444;
        _instrumentRepository.Setup(q => q.GetSblInstrument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sblInstrument);
        _equityNumberGeneratorService.SetupSequence(q => q.GenerateSblOrderOrderIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((ulong)99999)
            .ReturnsAsync((ulong)88888)
            .ReturnsAsync(orderId);
        _sblOrderRepository.SetupSequence(q => q.CreateAsync(It.IsAny<SblOrder>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DuplicateRecordNoException())
            .ThrowsAsync(new DuplicateRecordNoException())
            .ReturnsAsync(new SblOrder(Guid.NewGuid(), Guid.NewGuid(), payload.TradingAccount.TradingAccountNo, payload.TradingAccount.CustomerCode, orderId, payload.Symbol, SblOrderStatus.Pending, payload.Volume, payload.Type, null, null));
        _instrumentRepository.Setup(q => q.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _onePortService.Setup(q => q.GetPositions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(positions);

        // Act
        var actual = await client.GetResponse<PlaceOrderSuccess>(payload);

        // Assert
        Assert.Equal(orderId.ToString(), actual.Message.BrokerOrderId);
        Assert.Equal(orderId.ToString(), actual.Message.OrderNo);
    }

    [Fact]
    public async Task Should_ReturnPlaceOrderFailed_When_UpdateSblInstrumentError()
    {
        // Arrange
        var client = Harness.GetRequestClient<CreateSblOrder>();
        var payload = new CreateSblOrder
        {
            Id = Guid.NewGuid(),
            TradingAccount = FakeTradingAccount(),
            Symbol = "EA",
            Volume = 1000000,
            Type = SblOrderType.Borrow
        };
        var sblInstrument = new SblInstrument(Guid.NewGuid(), payload.Symbol, 5.00m, 2000000, 1000000, 1000000);
        ulong orderId = 123456;
        _instrumentRepository.Setup(q => q.GetSblInstrument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sblInstrument);
        _equityNumberGeneratorService.Setup(q => q.GenerateSblOrderOrderIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderId);
        _sblOrderRepository.Setup(q => q.CreateAsync(It.IsAny<SblOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SblOrder(Guid.NewGuid(), Guid.NewGuid(), payload.TradingAccount.TradingAccountNo, payload.TradingAccount.CustomerCode, orderId, payload.Symbol, SblOrderStatus.Pending, payload.Volume, payload.Type, null, null));
        _instrumentRepository.Setup(q => q.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        _sblOrderRepository.Setup(q => q.DeleteAsync(It.IsAny<SblOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Verifiable();

        // Act
        var actual = await client.GetResponse<PlaceOrderFailed>(payload);

        // Assert
        Assert.Equal(SetErrorCode.SE113, actual.Message.SetErrorCode);
        _sblOrderRepository.Verify(q => q.DeleteAsync(It.IsAny<SblOrder>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnPlaceOrderFailed_When_CreateSblOrderError()
    {
        // Arrange
        var client = Harness.GetRequestClient<CreateSblOrder>();
        var payload = new CreateSblOrder
        {
            Id = Guid.NewGuid(),
            TradingAccount = FakeTradingAccount(),
            Symbol = "EA",
            Volume = 1000000,
            Type = SblOrderType.Borrow
        };
        var sblInstrument = new SblInstrument(Guid.NewGuid(), payload.Symbol, 5.00m, 2000000, 1000000, 1000000);
        ulong orderId = 123456;
        _instrumentRepository.Setup(q => q.GetSblInstrument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sblInstrument);
        _equityNumberGeneratorService.Setup(q => q.GenerateSblOrderOrderIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderId);
        _sblOrderRepository.Setup(q => q.CreateAsync(It.IsAny<SblOrder>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var actual = await client.GetResponse<PlaceOrderFailed>(payload);

        // Assert
        Assert.Equal(SetErrorCode.SE113, actual.Message.SetErrorCode);
    }

    [Fact]
    public async Task Should_ReturnPlaceOrderFailed_When_SblInstrumentNotFound()
    {
        // Arrange
        var client = Harness.GetRequestClient<CreateSblOrder>();
        var payload = new CreateSblOrder
        {
            Id = Guid.NewGuid(),
            TradingAccount = FakeTradingAccount(),
            Symbol = "EA",
            Volume = 1000000,
            Type = SblOrderType.Borrow
        };
        _instrumentRepository.Setup(q => q.GetSblInstrument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SblInstrument?)null);

        // Act
        var actual = await client.GetResponse<PlaceOrderFailed>(payload);

        // Assert
        Assert.Equal(SetErrorCode.SE111, actual.Message.SetErrorCode);
    }

    [Fact]
    public async Task Should_ReturnPlaceOrderFailed_When_RequestedVolume_MoreThan_AvailableLending()
    {
        // Arrange
        var client = Harness.GetRequestClient<CreateSblOrder>();
        var payload = new CreateSblOrder
        {
            Id = Guid.NewGuid(),
            TradingAccount = FakeTradingAccount(),
            Symbol = "EA",
            Volume = 1000000,
            Type = SblOrderType.Borrow
        };
        var sblInstrument = new SblInstrument(Guid.NewGuid(), payload.Symbol, 5.00m, 2000000, 2000000, 0);
        _instrumentRepository.Setup(q => q.GetSblInstrument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sblInstrument);

        // Act
        var actual = await client.GetResponse<PlaceOrderFailed>(payload);

        // Assert
        Assert.Equal(SetErrorCode.SE112, actual.Message.SetErrorCode);
    }

    [Fact]
    public async Task Should_ReturnPlaceOrderFailed_When_PositionInsufficientBalance()
    {
        // Arrange
        var client = Harness.GetRequestClient<CreateSblOrder>();
        var payload = new CreateSblOrder
        {
            Id = Guid.NewGuid(),
            TradingAccount = FakeTradingAccount(),
            Symbol = "EA",
            Volume = 1000000,
            Type = SblOrderType.Return
        };
        var sblInstrument = new SblInstrument(Guid.NewGuid(), payload.Symbol, 5.00m, 2000000, 1000000, 1000000);
        var positions = new List<AccountPosition> {
            new (payload.Symbol, Ttf.None)
            {
                TradingAccountNo = payload.TradingAccount.TradingAccountNo,
                AccountNo = payload.TradingAccount.AccountNo,
                StockType = StockType.Borrow,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = 100,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            }
        };
        const ulong orderId = 123456;
        _instrumentRepository.Setup(q => q.GetSblInstrument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sblInstrument);
        _equityNumberGeneratorService.Setup(q => q.GenerateSblOrderOrderIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderId);
        _sblOrderRepository.Setup(q => q.CreateAsync(It.IsAny<SblOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SblOrder(Guid.NewGuid(), Guid.NewGuid(), payload.TradingAccount.TradingAccountNo, payload.TradingAccount.CustomerCode, orderId, payload.Symbol, SblOrderStatus.Pending, payload.Volume, payload.Type, null, null));
        _instrumentRepository.Setup(q => q.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _onePortService.Setup(q => q.GetPositions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(positions);

        // Act
        var actual = await client.GetResponse<PlaceOrderFailed>(payload);

        // Assert
        Assert.Equal(SetErrorCode.SE106, actual.Message.SetErrorCode);
    }

    [Fact]
    public async Task Should_ReturnPlaceOrderFailed_When_PositionNotFound()
    {
        // Arrange
        var client = Harness.GetRequestClient<CreateSblOrder>();
        var payload = new CreateSblOrder
        {
            Id = Guid.NewGuid(),
            TradingAccount = FakeTradingAccount(),
            Symbol = "EA",
            Volume = 1000000,
            Type = SblOrderType.Return
        };
        var sblInstrument = new SblInstrument(Guid.NewGuid(), payload.Symbol, 5.00m, 2000000, 1000000, 1000000);
        const ulong orderId = 123456;
        _instrumentRepository.Setup(q => q.GetSblInstrument(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sblInstrument);
        _equityNumberGeneratorService.Setup(q => q.GenerateSblOrderOrderIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderId);
        _sblOrderRepository.Setup(q => q.CreateAsync(It.IsAny<SblOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SblOrder(Guid.NewGuid(), Guid.NewGuid(), payload.TradingAccount.TradingAccountNo, payload.TradingAccount.CustomerCode, orderId, payload.Symbol, SblOrderStatus.Pending, payload.Volume, payload.Type, null, null));
        _instrumentRepository.Setup(q => q.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _onePortService.Setup(q => q.GetPositions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var actual = await client.GetResponse<PlaceOrderFailed>(payload);

        // Assert
        Assert.Equal(SetErrorCode.SE114, actual.Message.SetErrorCode);
    }

    [Theory]
    [InlineData(SblOrderType.Borrow)]
    [InlineData(SblOrderType.Return)]
    public async Task Should_ReturnPlaceOrderFailed_When_AccountSblDisabled(SblOrderType type)
    {
        // Arrange
        var client = Harness.GetRequestClient<CreateSblOrder>();
        var tradingAccount = FakeTradingAccount();
        tradingAccount.SblRegistered = false;
        var payload = new CreateSblOrder
        {
            Id = Guid.NewGuid(),
            TradingAccount = tradingAccount,
            Symbol = "EA",
            Volume = 1000000,
            Type = type
        };

        // Act
        var actual = await client.GetResponse<PlaceOrderFailed>(payload);

        // Assert
        Assert.Equal(SetErrorCode.SE115, actual.Message.SetErrorCode);
    }

    private static TradingAccount FakeTradingAccount()
    {
        return new TradingAccount(Guid.NewGuid(), "0803174", "0803174-6", TradingAccountType.CreditBalance)
        {
            SblRegistered = true
        };
    }
}
