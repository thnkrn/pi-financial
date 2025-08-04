using System.Runtime.Serialization;
namespace Pi.WalletService.Domain.Exceptions;

[Serializable]
public class InvalidDepositAmountException : Exception
{
    public InvalidDepositAmountException()
    {
    }

    public InvalidDepositAmountException(string message)
        : base(message)
    {
    }

    public InvalidDepositAmountException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected InvalidDepositAmountException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}