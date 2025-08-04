using System.Runtime.Serialization;
namespace Pi.WalletService.Domain.Exceptions;

[Serializable]
public class UserOtpRequestLimitReachedException : Exception
{
    public UserOtpRequestLimitReachedException()
    {
    }

    public UserOtpRequestLimitReachedException(string message)
        : base(message)
    {
    }

    public UserOtpRequestLimitReachedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected UserOtpRequestLimitReachedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}