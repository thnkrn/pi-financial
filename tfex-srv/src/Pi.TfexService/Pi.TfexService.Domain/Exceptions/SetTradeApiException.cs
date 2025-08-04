using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Pi.TfexService.Domain.Exceptions;

[Serializable]
public class SetTradeApiException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public SetTradeApiException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public SetTradeApiException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected SetTradeApiException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class SetTradeInvalidDataException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public SetTradeInvalidDataException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public SetTradeInvalidDataException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected SetTradeInvalidDataException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class SetTradeNotFoundException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public SetTradeNotFoundException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public SetTradeNotFoundException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected SetTradeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class SetTradePriceOutOfRangeException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public SetTradePriceOutOfRangeException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public SetTradePriceOutOfRangeException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected SetTradePriceOutOfRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class SetTradePriceOutOfRangeFromLastDoneException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public SetTradePriceOutOfRangeFromLastDoneException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public SetTradePriceOutOfRangeFromLastDoneException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected SetTradePriceOutOfRangeFromLastDoneException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class SetTradePlaceOrderBothSideException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public SetTradePlaceOrderBothSideException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public SetTradePlaceOrderBothSideException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected SetTradePlaceOrderBothSideException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class SetTradeOutsideTradingHoursException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public SetTradeOutsideTradingHoursException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public SetTradeOutsideTradingHoursException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected SetTradeOutsideTradingHoursException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class SetTradeNotEnoughPositionException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public SetTradeNotEnoughPositionException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public SetTradeNotEnoughPositionException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected SetTradeNotEnoughPositionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class SetTradeNotEnoughExcessEquityException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public SetTradeNotEnoughExcessEquityException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public SetTradeNotEnoughExcessEquityException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected SetTradeNotEnoughExcessEquityException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class SetTradeSeriesNotFoundException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public SetTradeSeriesNotFoundException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public SetTradeSeriesNotFoundException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected SetTradeSeriesNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class SetTradeUpdateOrderNoValueChangedException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public SetTradeUpdateOrderNoValueChangedException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public SetTradeUpdateOrderNoValueChangedException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected SetTradeUpdateOrderNoValueChangedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class SetTradeNotEnoughLineAvailableException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public SetTradeNotEnoughLineAvailableException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public SetTradeNotEnoughLineAvailableException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected SetTradeNotEnoughLineAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class ApiResponseException(string? message, string? code = null)
{
    [JsonProperty("message")]
    public string? ErrorMessage { get; set; } = message;

    [JsonProperty("code")]
    public string? ErrorCode { get; set; } = code;
}

public abstract class ApiExceptionHelper
{
    public static ApiResponseException DeserializeApiException(Object errorContent)
    {
        // Deserialize the JSON error message
        var error = JsonConvert.DeserializeObject<ApiResponseException>(errorContent.ToString() ?? string.Empty);

        // Create and return the custom exception
        return error ?? new ApiResponseException("Internal Server Error");
    }
}

