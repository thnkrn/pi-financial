using System.Runtime.Serialization;
namespace Pi.WalletService.Domain.Exceptions;

[Serializable]
public class DepositWithdrawDisabledException : Exception
{
    public DepositWithdrawDisabledException()
    {
    }

    public DepositWithdrawDisabledException(string message)
        : base(message)
    {
    }

    public DepositWithdrawDisabledException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected DepositWithdrawDisabledException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}