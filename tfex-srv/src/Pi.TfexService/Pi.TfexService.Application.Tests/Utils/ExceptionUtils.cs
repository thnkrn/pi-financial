using MassTransit;
using MassTransit.Events;
using Microsoft.AspNetCore.Http;
using Moq;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Utils;
using Pi.TfexService.Domain.Exceptions;

namespace Pi.TfexService.Application.Tests.Utils;

public class ExceptionUtilsTests
{
    [Fact]
    public void HandleException_Should_Return_Correctly_When_Exception()
    {
        // Arrange
        var exception = new Exception("Exception");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, result.statusCode);
        Assert.Equal(ErrorCodes.SetTradeInternalError, result.title);
        Assert.Equal("Exception", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_ArgumentException()
    {
        // Arrange
        var exception = new ArgumentException("ArgumentException");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, result.statusCode);
        Assert.Equal(ErrorCodes.InvalidData, result.title);
        Assert.Equal("ArgumentException", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_SetTradeInvalidDataException()
    {
        // Arrange
        var exception = new SetTradeInvalidDataException("SetTradeInvalidDataException");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, result.statusCode);
        Assert.Equal(ErrorCodes.InvalidData, result.title);
        Assert.Equal("SetTradeInvalidDataException", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_SetTradePriceOutOfRangeException()
    {
        // Arrange
        var exception = new SetTradePriceOutOfRangeException("SetTradePriceOutOfRangeException");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, result.statusCode);
        Assert.Equal(ErrorCodes.PriceOutOfRange, result.title);
        Assert.Equal("SetTradePriceOutOfRangeException", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_SetTradePriceOutOfRangeFromLastDoneException()
    {
        // Arrange
        var exception = new SetTradePriceOutOfRangeFromLastDoneException("SetTradePriceOutOfRangeFromLastDoneException");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, result.statusCode);
        Assert.Equal(ErrorCodes.PriceOutOfRangeFromLastDone, result.title);
        Assert.Equal("SetTradePriceOutOfRangeFromLastDoneException", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_SetTradePlaceOrderBothSideException()
    {
        // Arrange
        var exception = new SetTradePlaceOrderBothSideException("SetTradePlaceOrderBothSideException");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, result.statusCode);
        Assert.Equal(ErrorCodes.PlaceOrderBothSide, result.title);
        Assert.Equal("SetTradePlaceOrderBothSideException", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_SetTradeNotEnoughPositionException()
    {
        // Arrange
        var exception = new SetTradeNotEnoughPositionException("SetTradeNotEnoughPositionException");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, result.statusCode);
        Assert.Equal(ErrorCodes.NotEnoughPosition, result.title);
        Assert.Equal("SetTradeNotEnoughPositionException", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_SetTradeNotEnoughExcessEquityException()
    {
        // Arrange
        var exception = new SetTradeNotEnoughExcessEquityException("SetTradeNotEnoughExcessEquityException");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, result.statusCode);
        Assert.Equal(ErrorCodes.NotEnoughExcessEquity, result.title);
        Assert.Equal("SetTradeNotEnoughExcessEquityException", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_SetTradeOutsideTradingHoursException()
    {
        // Arrange
        var exception = new SetTradeOutsideTradingHoursException("SetTradeOutsideTradingHoursException");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, result.statusCode);
        Assert.Equal(ErrorCodes.OutsideTradingHours, result.title);
        Assert.Equal("SetTradeOutsideTradingHoursException", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_SetTradeUpdateOrderNoValueChangedException()
    {
        // Arrange
        var exception = new SetTradeUpdateOrderNoValueChangedException("SetTradeUpdateOrderNoValueChangedException");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, result.statusCode);
        Assert.Equal(ErrorCodes.UpdateOrderNoValueChanged, result.title);
        Assert.Equal("SetTradeUpdateOrderNoValueChangedException", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_SetTradeAuthException()
    {
        // Arrange
        var exception = new SetTradeAuthException("SetTradeAuthException");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, result.statusCode);
        Assert.Equal(ErrorCodes.SetTradeInternalError, result.title);
        Assert.Equal("SetTradeAuthException", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_UnauthorizedAccessException()
    {
        // Arrange
        var exception = new UnauthorizedAccessException("UnauthorizedAccessException");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, result.statusCode);
        Assert.Equal(ErrorCodes.SetTradeInternalError, result.title);
        Assert.Equal("UnauthorizedAccessException", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_SetTradeNotFoundException()
    {
        // Arrange
        var exception = new SetTradeNotFoundException("SetTradeNotFoundException");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, result.statusCode);
        Assert.Equal(ErrorCodes.InvalidData, result.title);
        Assert.Equal("SetTradeNotFoundException", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_SetTradeSeriesNotFoundException()
    {
        // Arrange
        var exception = new SetTradeSeriesNotFoundException("SetTradeSeriesNotFoundException");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, result.statusCode);
        Assert.Equal(ErrorCodes.SeriesNotFound, result.title);
        Assert.Equal("SetTradeSeriesNotFoundException", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_SetTradeApiException()
    {
        // Arrange
        var exception = new SetTradeApiException("SetTradeApiException");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, result.statusCode);
        Assert.Equal(ErrorCodes.SetTradeInternalError, result.title);
        Assert.Equal("SetTradeApiException", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_RequestFaultException()
    {
        // Arrange
        var innerException = new ArgumentException("RequestFaultException");
        var faultInstance = new MockFault<ArgumentException>(innerException);

        var exception = new RequestFaultException("RequestFaultException", faultInstance);

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, result.statusCode);
        Assert.Equal(ErrorCodes.InvalidData, result.title);
        Assert.Equal("RequestFaultException", result.detail);
    }

    [Fact]
    public void HandleException_Should_Return_Correctly_When_SetTradeNotEnoughLineAvailableException()
    {
        // Arrange
        var exception = new SetTradeNotEnoughLineAvailableException("SetTradeNotEnoughLineAvailableException");

        // Act
        var result = ExceptionUtils.HandleException(exception);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, result.statusCode);
        Assert.Equal(ErrorCodes.NotEnoughLineAvailable, result.title);
        Assert.Equal("SetTradeNotEnoughLineAvailableException", result.detail);
    }
}

public class MockFault<T>(T faultException) : Fault<T>
    where T : Exception
{
    public Guid FaultId { get; } = Guid.NewGuid();
    public Guid? FaultedMessageId { get; } = Guid.NewGuid();
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    public ExceptionInfo[] Exceptions { get; } =
    [
        new FaultExceptionInfo
        {
            ExceptionType = faultException.GetType().FullName!,
            Message = faultException.Message!,
            StackTrace = faultException.StackTrace!
        }
    ];

    public HostInfo Host { get; } = null!;
    public string[] FaultMessageTypes { get; } = null!;
    public T Message { get; } = null!;
}