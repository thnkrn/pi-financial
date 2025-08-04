using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.TfexService.Application.Commands.Account;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Domain.Models.InitialMargin;

namespace Pi.TfexService.Application.Tests.Commands.Account;

public class UpsertInitialMarginConsumerTests
{
    private readonly Mock<IInitialMarginRepository> _mockRepository;
    private readonly Mock<ILogger<UpsertInitialMarginConsumer>> _mockLogger;
    private readonly UpsertInitialMarginConsumer _consumer;

    public UpsertInitialMarginConsumerTests()
    {
        _mockRepository = new Mock<IInitialMarginRepository>();
        _mockLogger = new Mock<ILogger<UpsertInitialMarginConsumer>>();
        _consumer = new UpsertInitialMarginConsumer(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Consume_ShouldUpsertDataAndReturnSuccess()
    {
        // Arrange
        var upsertInitialMargin = new UpsertInitialMargin(
            [
                new InitialMarginData("AAPL", "FUT", 100.0m),
                new InitialMarginData("MSFT", "FUT", 200.0m)
            ],
            DateTime.UtcNow
        );

        var mockContext = new Mock<ConsumeContext<UpsertInitialMargin>>();
        mockContext.Setup(c => c.Message).Returns(upsertInitialMargin);
        mockContext.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        // Act
        await _consumer.Consume(mockContext.Object);

        // Assert
        _mockRepository.Verify(r => r.UpsertInitialMargin(It.Is<List<InitialMargin>>(list =>
            list.Count == 2 &&
            list[0].Symbol == "AAPL" &&
            list[0].ImOutright == 100.0m * 1.75m &&
            list[0].ImSpread == 100.0m * 1.75m * 0.25m &&
            list[1].Symbol == "MSFT" &&
            list[1].ImOutright == 200.0m * 1.75m &&
            list[1].ImSpread == 200.0m * 1.75m * 0.25m
        ), It.IsAny<CancellationToken>()), Times.Once);

        mockContext.Verify(c => c.RespondAsync(It.Is<UpsertInitialMarginResponse>(response =>
            response.IsSuccess == true
        )), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldUpsertDataAndReturnSuccess_When_ModifiedSymbol()
    {
        // Arrange
        var upsertInitialMargin = new UpsertInitialMargin(
            [
                new InitialMarginData("SET50", "PHY", 100.0m),
            ],
            DateTime.UtcNow
        );

        var mockContext = new Mock<ConsumeContext<UpsertInitialMargin>>();
        mockContext.Setup(c => c.Message).Returns(upsertInitialMargin);
        mockContext.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        // Act
        await _consumer.Consume(mockContext.Object);

        // Assert
        _mockRepository.Verify(r => r.UpsertInitialMargin(It.Is<List<InitialMargin>>(list =>
            list.Count == 1 &&
            list[0].Symbol == "S50" &&
            list[0].ImOutright == 100.0m * 1.75m &&
            list[0].ImSpread == 100.0m * 1.75m * 0.25m &&
            list[0].ProductType == "FUT"
        ), It.IsAny<CancellationToken>()), Times.Once);

        mockContext.Verify(c => c.RespondAsync(It.Is<UpsertInitialMarginResponse>(response =>
            response.IsSuccess == true
        )), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldLogErrorAndReturnFailure_WhenExceptionIsThrown()
    {
        // Arrange
        var upsertInitialMargin = new UpsertInitialMargin(
            [
                new InitialMarginData("AAPL", "FUT", 100.0m),
            ],
            DateTime.UtcNow
        );

        var mockContext = new Mock<ConsumeContext<UpsertInitialMargin>>();
        mockContext.Setup(c => c.Message).Returns(upsertInitialMargin);
        mockContext.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        _mockRepository.Setup(r => r.UpsertInitialMargin(It.IsAny<List<InitialMargin>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        await Assert.ThrowsAsync<Exception>(() => _consumer.Consume(mockContext.Object));

        // Assert
        mockContext.Verify(c => c.RespondAsync(It.Is<UpsertInitialMarginResponse>(response =>
            response.IsSuccess == true
        )), Times.Never);
    }

    [Fact]
    public async Task Consume_ShouldPassCancellationTokenToRepository()
    {
        // Arrange
        var upsertInitialMargin = new UpsertInitialMargin(
            [
                new InitialMarginData("AAPL", "FUT", 100.0m),
            ],
            DateTime.UtcNow
        );

        var mockContext = new Mock<ConsumeContext<UpsertInitialMargin>>();
        var cancellationToken = new CancellationTokenSource().Token;
        mockContext.Setup(c => c.Message).Returns(upsertInitialMargin);
        mockContext.Setup(c => c.CancellationToken).Returns(cancellationToken);

        // Act
        await _consumer.Consume(mockContext.Object);

        // Assert
        _mockRepository.Verify(r => r.UpsertInitialMargin(It.IsAny<List<InitialMargin>>(), cancellationToken), Times.Once);
    }
}