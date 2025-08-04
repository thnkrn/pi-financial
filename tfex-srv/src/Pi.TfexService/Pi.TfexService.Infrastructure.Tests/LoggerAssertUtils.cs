using Microsoft.Extensions.Logging;
using Moq;

namespace Pi.TfexService.Infrastructure.Tests;

public static class LoggerAssertUtils
{
    public static void VerifyLogError<T>(Mock<ILogger<T>> logger)
    {
        logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!),
            Times.Once);
    }
}