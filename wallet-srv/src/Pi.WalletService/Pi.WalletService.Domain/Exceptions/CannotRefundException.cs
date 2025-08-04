using System.Runtime.Serialization;

namespace Pi.WalletService.Domain.Exceptions;

[Serializable]
public class CannotRefundException : Exception
{
    public CannotRefundException()
    {
    }

    public CannotRefundException(string message)
        : base(message)
    {
    }

    public CannotRefundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected CannotRefundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

}
