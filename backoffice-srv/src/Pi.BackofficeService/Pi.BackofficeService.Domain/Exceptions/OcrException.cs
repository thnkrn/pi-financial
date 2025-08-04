namespace Pi.BackofficeService.Domain.Exceptions;

[Serializable]
public class OcrException : Exception
{
    public OcrException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public OcrException()
    {
    }

    public OcrException(string message)
        : base(message)
    {
    }
}
