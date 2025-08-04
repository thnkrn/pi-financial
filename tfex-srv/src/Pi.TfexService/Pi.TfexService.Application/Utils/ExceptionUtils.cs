using MassTransit;
using Microsoft.AspNetCore.Http;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Domain.Exceptions;

namespace Pi.TfexService.Application.Utils;

public static class ExceptionUtils
{
    public static (int statusCode, string title, string detail) HandleException(Exception e)
    {
        var message = e.Message;
        var exceptionType = e.GetType().FullName;

        // Handle MassTransit RequestFaultException
        if (e is RequestFaultException { Fault.Exceptions.Length: > 0 } requestFaultException)
        {
            var innerException = requestFaultException.Fault.Exceptions[0];
            message = innerException.Message;
            exceptionType = innerException.ExceptionType;
        }

        return exceptionType switch
        {
            // 400 Bad Request
            _ when exceptionType == typeof(ArgumentException).FullName ||
                   exceptionType == typeof(SetTradeInvalidDataException).FullName
                => (StatusCodes.Status400BadRequest, ErrorCodes.InvalidData, message),

            _ when exceptionType == typeof(SetTradePriceOutOfRangeException).FullName
                => (StatusCodes.Status400BadRequest, ErrorCodes.PriceOutOfRange, message),

            _ when exceptionType == typeof(SetTradePriceOutOfRangeFromLastDoneException).FullName
                => (StatusCodes.Status400BadRequest, ErrorCodes.PriceOutOfRangeFromLastDone, message),

            _ when exceptionType == typeof(SetTradePlaceOrderBothSideException).FullName
                => (StatusCodes.Status400BadRequest, ErrorCodes.PlaceOrderBothSide, message),

            _ when exceptionType == typeof(SetTradeNotEnoughPositionException).FullName
                => (StatusCodes.Status400BadRequest, ErrorCodes.NotEnoughPosition, message),

            _ when exceptionType == typeof(SetTradeNotEnoughExcessEquityException).FullName
                => (StatusCodes.Status400BadRequest, ErrorCodes.NotEnoughExcessEquity, message),

            _ when exceptionType == typeof(SetTradeOutsideTradingHoursException).FullName
                => (StatusCodes.Status400BadRequest, ErrorCodes.OutsideTradingHours, message),

            _ when exceptionType == typeof(SetTradeUpdateOrderNoValueChangedException).FullName
                => (StatusCodes.Status400BadRequest, ErrorCodes.UpdateOrderNoValueChanged, message),

            _ when exceptionType == typeof(SetTradeNotEnoughLineAvailableException).FullName
                => (StatusCodes.Status400BadRequest, ErrorCodes.NotEnoughLineAvailable, message),

            // 401 Unauthorized
            _ when exceptionType == typeof(SetTradeAuthException).FullName ||
                   exceptionType == typeof(UnauthorizedAccessException).FullName
                => (StatusCodes.Status401Unauthorized, ErrorCodes.SetTradeInternalError, message),

            // 404 Not found
            _ when exceptionType == typeof(SetTradeNotFoundException).FullName
                => (StatusCodes.Status404NotFound, ErrorCodes.InvalidData, message),

            _ when exceptionType == typeof(SetTradeSeriesNotFoundException).FullName
                => (StatusCodes.Status404NotFound, ErrorCodes.SeriesNotFound, message),

            _ when exceptionType == typeof(ItNotFoundException).FullName
                => (StatusCodes.Status404NotFound, ErrorCodes.TradeNotFound, message),

            // 500 Internal server error
            _ when exceptionType == typeof(SetTradeApiException).FullName
                => (StatusCodes.Status500InternalServerError, ErrorCodes.SetTradeInternalError, message),

            _ when exceptionType == typeof(ItApiException).FullName
                => (StatusCodes.Status500InternalServerError, ErrorCodes.SetTradeInternalError, message),

            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.SetTradeInternalError, message)
        };
    }
}