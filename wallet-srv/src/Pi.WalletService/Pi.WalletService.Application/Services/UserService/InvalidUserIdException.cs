using System;
using System.Runtime.Serialization;
namespace Pi.WalletService.Application.Services.UserService;

[Serializable]
public class InvalidUserIdException : Exception
{
    public InvalidUserIdException(Guid userId, Exception? innerException) : base("Invalid user id", innerException)
    {
        UserId = userId;
    }
    public InvalidUserIdException()
    {
    }

    public InvalidUserIdException(string message)
        : base(message)
    {
    }

    public InvalidUserIdException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected InvalidUserIdException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public Guid UserId { get; }
}