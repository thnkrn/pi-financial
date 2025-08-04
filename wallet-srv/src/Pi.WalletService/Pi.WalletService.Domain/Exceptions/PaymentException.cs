using System.Runtime.Serialization;

namespace Pi.WalletService.Domain.Exceptions;

public class FinnetInsufficientBalanceException : Exception
{
    public FinnetInsufficientBalanceException()
    {
    }

    public FinnetInsufficientBalanceException(string message)
        : base(message)
    {
    }

    public FinnetInsufficientBalanceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected FinnetInsufficientBalanceException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

}
