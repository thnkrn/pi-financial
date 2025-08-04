using System.Runtime.Serialization;
namespace Pi.WalletService.Domain.Exceptions;

[Serializable]
public class CustomerNameAndBankAccountNameMismatchException : Exception
{
    public CustomerNameAndBankAccountNameMismatchException()
    {
    }

    public CustomerNameAndBankAccountNameMismatchException(string message)
        : base(message)
    {
    }

    public CustomerNameAndBankAccountNameMismatchException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected CustomerNameAndBankAccountNameMismatchException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}