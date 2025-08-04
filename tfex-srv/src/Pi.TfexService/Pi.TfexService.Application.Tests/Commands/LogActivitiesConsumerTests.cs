using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.SeedWork;
using Pi.TfexService.Application.Commands;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Domain.Exceptions;
using Pi.TfexService.Domain.Models;
using Pi.TfexService.Domain.Models.ActivitiesLog;

namespace Pi.TfexService.Application.Tests.Commands;

public class LogActivitiesConsumerTests
{
    private readonly Mock<IActivitiesLogRepository> _activitiesLogRepository;
    private readonly LogActivitiesConsumer _logActivitiesConsumer;
    private readonly Mock<ILogger<LogActivitiesConsumer>> _logger;
    public LogActivitiesConsumerTests()
    {
        _logger = new Mock<ILogger<LogActivitiesConsumer>>();
        _activitiesLogRepository = new Mock<IActivitiesLogRepository>();
        _activitiesLogRepository
            .Setup(r => r.UnitOfWork)
            .Returns(new Mock<IUnitOfWork>().Object);
        _logActivitiesConsumer = new LogActivitiesConsumer(_activitiesLogRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task LogActivitiesConsumer_Should_Log_Request_When_RequestBody_IsNotNullOrEmpty()
    {
        // Arrange
        var request = new LogActivitiesRequest(
            "userId",
            "customerCode",
            "accountCode",
            RequestType.PlaceOrder,
            "requestBody",
            "1",
            "responseBody",
            new DateTime(),
            new DateTime(),
            true,
            null,
            null
        );

        var consumeContext = new Mock<ConsumeContext<LogActivitiesRequest>>();
        consumeContext
            .SetupGet(c => c.Message)
            .Returns(request);

        // act
        await _logActivitiesConsumer.Consume(consumeContext.Object);

        // assert
        _activitiesLogRepository.Verify(
            r => r.AddAsync(
                It.IsAny<ActivitiesLog>(),
                It.IsAny<CancellationToken>()),
            Times.Once
        );

        _activitiesLogRepository.Verify(
            r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    // [Fact]
    // public async Task LogActivitiesConsumer_ShouldNot_Log_Request_When_RequestBody_IsNullOrEmpty()
    // {
    //     // Arrange
    //     var request = new LogActivitiesRequest(
    //         "userId",
    //         "customerCode",
    //         "accountCode",
    //         RequestType.PlaceOrder,
    //         null,
    //         "1",
    //         "responseBody",
    //         new DateTime(),
    //         new DateTime(),
    //         false,
    //         "failedReason",
    //         null
    //     );
    //
    //     var consumeContext = new Mock<ConsumeContext<LogActivitiesRequest>>();
    //     consumeContext.SetupGet(c => c.Message).Returns(request);
    //
    //     // act
    //     await _logActivitiesConsumer.Consume(consumeContext.Object);
    //
    //     // assert
    //     _activitiesLogRepository.Verify(
    //         r => r.AddAsync(It.IsAny<ActivitiesLog>(), It.IsAny<CancellationToken>()),
    //         Times.Never
    //     );
    //
    //     _activitiesLogRepository.Verify(
    //         r => r.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
    //         Times.Never
    //     );
    // }

    [Fact]
    public async Task LogActivitiesConsumer_Should_OmitException_When_ExceptionIsThrown()
    {
        // arrange
        var request = new LogActivitiesRequest(
            "userId",
            "customerCode",
            "accountCode",
            RequestType.PlaceOrder,
            "requestBody",
            "1",
            "responseBody",
            new DateTime(),
            new DateTime(),
            true,
            null,
            null
        );

        var exception = new Exception("Mock exception");

        var consumeContext = new Mock<ConsumeContext<LogActivitiesRequest>>();
        consumeContext.SetupGet(c => c.Message).Returns(request);

        _activitiesLogRepository
            .Setup(r => r.AddAsync(It.IsAny<ActivitiesLog>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // act
        await _logActivitiesConsumer.Consume(consumeContext.Object);

        // assert
        _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@o, @t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}