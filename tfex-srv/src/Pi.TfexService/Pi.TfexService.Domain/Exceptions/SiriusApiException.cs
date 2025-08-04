using System.Runtime.Serialization;

namespace Pi.TfexService.Domain.Exceptions;

[Serializable]
public class SiriusApiException : Exception
{
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public SiriusApiException(string message, string code, Exception? innerException) : base(message, innerException)
    {
        ErrorMessage = message;
        ErrorCode = code;
    }

    public SiriusApiException(string message) : base(message)
    {
        ErrorMessage = message;
    }

    // Without this constructor, deserialization will fail
    [Obsolete("Obsolete")]
    protected SiriusApiException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}