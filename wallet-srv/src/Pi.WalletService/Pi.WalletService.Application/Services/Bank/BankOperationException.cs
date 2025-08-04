using System.Runtime.Serialization;
namespace Pi.WalletService.Application.Services.Bank;

[Serializable]
public class BankOperationException : Exception
{
    public BankOperationException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public BankOperationException()
    {
    }

    public BankOperationException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected BankOperationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}