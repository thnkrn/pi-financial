using System.Runtime.Serialization;

namespace Pi.WalletService.Application.Models;

[Serializable]
public class TransactionNotFoundException : Exception
{
    public TransactionNotFoundException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public TransactionNotFoundException()
    {
    }

    public TransactionNotFoundException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected TransactionNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}