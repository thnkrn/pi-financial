using System.Runtime.Serialization;
namespace Pi.WalletService.Domain.Exceptions;

public static class PaymentErrorCodes
{
    public const string FinnetInsufficientBalance = "PAY0001";
}

[Serializable]
public class InsufficientBalanceException : Exception
{
    public InsufficientBalanceException()
    {
    }

    public InsufficientBalanceException(string message)
        : base(message)
    {
    }

    public InsufficientBalanceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected InsufficientBalanceException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}