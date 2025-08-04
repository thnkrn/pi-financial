using System.Runtime.Serialization;
namespace Pi.WalletService.Domain.Exceptions;

[Serializable]
public class UserOtpVerificationLimitReachedException : Exception
{
    public UserOtpVerificationLimitReachedException()
    {
    }

    public UserOtpVerificationLimitReachedException(string message)
        : base(message)
    {
    }

    public UserOtpVerificationLimitReachedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected UserOtpVerificationLimitReachedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}