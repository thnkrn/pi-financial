using System.Runtime.Serialization;

namespace Pi.WalletService.Domain.Exceptions;

public class InvalidRequestException : Exception
{
    public string? ErrorCode { get; private set; }

    public InvalidRequestException()
    {
    }

    public InvalidRequestException(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public InvalidRequestException(string errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    // Without this constructor, deserialization will fail
    protected InvalidRequestException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}