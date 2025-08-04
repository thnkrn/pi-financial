using System.Runtime.Serialization;
namespace Pi.WalletService.Domain.Exceptions;

[Serializable]
public class QrCodeExpiredException : Exception
{
    public QrCodeExpiredException()
    {
    }

    public QrCodeExpiredException(string message)
        : base(message)
    {
    }

    public QrCodeExpiredException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected QrCodeExpiredException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}