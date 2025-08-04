using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.Common.Generators.Number;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Tests.Commands;

public class GenerateOrderNumberConsumerTest : ConsumerTest
{
    private readonly Mock<INumberGeneratorService> _numberGeneratorService;
    private readonly Mock<IFundOrderRepository> _fundRepository;

    public GenerateOrderNumberConsumerTest()
    {
        _numberGeneratorService = new Mock<INumberGeneratorService>();
        _fundRepository = new Mock<IFundOrderRepository>();

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<GenerateOrderNumberConsumer>(); })
            .AddScoped<INumberGeneratorService>(_ => _numberGeneratorService.Object)
            .AddScoped<IFundOrderRepository>(_ => _fundRepository.Object)
            .BuildServiceProvider(true);
    }

    [Theory]
    [InlineData(OrderSide.Sell, "FORED")]
    [InlineData(OrderSide.Switch, "FOSW")]
    public async Task Should_ReturnOrderNumber_When_Success(OrderSide orderSide, string expectedPrefix)
    {
        // Arrange
        var client = Harness.GetRequestClient<GenerateOrderNo>();
        var msg = new GenerateOrderNo(Guid.NewGuid(), orderSide);
        var orderNumber = "mockOrderNumber";
        _numberGeneratorService.Setup(q => q.GenerateAndUpdateAsync(
                It.IsAny<Func<string, Task>>(),
                It.Is<NumberGeneratorSettings>(m => m.Prefix == expectedPrefix),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(() => orderNumber);

        // Act
        var actual = await client.GetResponse<OrderNoGenerated>(msg);

        // Assert
        Assert.Equal(orderNumber, actual.Message.OrderNo);
    }

    [Theory]
    [InlineData(OrderSide.Buy, "FOSUB", false)]
    [InlineData(OrderSide.Buy, "FODCASUB", true)]
    public async Task Should_ReturnOrderNumber_When_Success_Buy(OrderSide orderSide, string expectedPrefix, bool recurring)
    {
        // Arrange
        var client = Harness.GetRequestClient<GenerateOrderNo>();
        var msg = new GenerateOrderNo(Guid.NewGuid(), orderSide, recurring);
        var orderNumber = "mockOrderNumber";
        _numberGeneratorService.Setup(q => q.GenerateAndUpdateAsync(
                It.IsAny<Func<string, Task>>(),
                It.Is<NumberGeneratorSettings>(m => m.Prefix == expectedPrefix),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(() => orderNumber);

        // Act
        var actual = await client.GetResponse<OrderNoGenerated>(msg);

        // Assert
        Assert.Equal(orderNumber, actual.Message.OrderNo);
    }

    [Fact]
    public async Task Should_UpdateOrderNumber_When_Success()
    {
        // Arrange
        var client = Harness.GetRequestClient<GenerateOrderNo>();
        var msg = new GenerateOrderNo(Guid.NewGuid(), OrderSide.Buy);
        var orderNumber = "FOSUB20240123";
        _fundRepository.SetupSequence(q => q.UpdateOrderNoAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _numberGeneratorService.Setup(q => q.GenerateAndUpdateAsync(It.IsAny<Func<string, Task>>(), It.IsAny<NumberGeneratorSettings>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => orderNumber);

        // Act
        var actual = await client.GetResponse<OrderNoGenerated>(msg);

        // Assert
        Assert.Equal(orderNumber, actual.Message.OrderNo);
    }

    [Fact]
    public async Task Should_ReturnOrderNumber_When_DuplicateRecordNoException_Was_Throw()
    {
        // Arrange
        var client = Harness.GetRequestClient<GenerateOrderNo>();
        var msg = new GenerateOrderNo(Guid.NewGuid(), OrderSide.Buy);
        var orderNumber = "FOSUB20240123";
        _fundRepository.SetupSequence(q => q.UpdateOrderNoAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DuplicateRecordNoException())
            .ThrowsAsync(new DuplicateRecordNoException())
            .ThrowsAsync(new DuplicateRecordNoException())
            .Returns(Task.CompletedTask);
        _numberGeneratorService.Setup(q => q.GenerateAndUpdateAsync(It.IsAny<Func<string, Task>>(), It.IsAny<NumberGeneratorSettings>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => orderNumber);

        // Act
        var actual = await client.GetResponse<OrderNoGenerated>(msg);

        // Assert
        Assert.Equal(orderNumber, actual.Message.OrderNo);
    }
}
