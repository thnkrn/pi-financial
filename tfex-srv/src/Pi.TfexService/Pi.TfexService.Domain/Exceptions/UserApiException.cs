using System.Runtime.Serialization;

namespace Pi.TfexService.Domain.Exceptions;

[Serializable]
public class UserApiException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public UserApiException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public UserApiException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected UserApiException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}