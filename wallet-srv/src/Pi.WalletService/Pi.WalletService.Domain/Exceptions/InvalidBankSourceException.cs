using System.Runtime.Serialization;
namespace Pi.WalletService.Domain.Exceptions;

[Serializable]
public class InvalidBankSourceException : Exception
{
    public InvalidBankSourceException()
    {
    }

    public InvalidBankSourceException(string message)
        : base(message)
    {
    }

    public InvalidBankSourceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected InvalidBankSourceException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

}