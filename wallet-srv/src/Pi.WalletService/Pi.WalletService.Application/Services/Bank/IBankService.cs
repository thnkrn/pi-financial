using System.Runtime.Serialization;
namespace Pi.WalletService.Application.Services.Bank;

public interface IBankService
{
    Task<QRPaymentResponse> GenerateQR(
        string transactionNo,
        decimal amount,
        string transactionRefCode,
        string customerCode,
        string product,
        int expiredTimeInMinute);

    Task<DDPaymentResponse> WithdrawViaAts(
        string transactionNo,
        string transactionRefCode,
        string accountNo,
        string destinationBankCode,
        decimal amount,
        string customerCode,
        string product);

    Task<OnlineDirectDebitRegistrationResult> RegisterOnlineDirectDebit(
        string citizenId,
        string refCode,
        string oddBank,
        CancellationToken cancellationToken);
}

[Serializable]
public class UnableToQrPaymentException : Exception
{
    public UnableToQrPaymentException()
    {
    }

    public UnableToQrPaymentException(string message)
        : base(message)
    {
    }

    public UnableToQrPaymentException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected UnableToQrPaymentException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

[Serializable]
public class UnableToWithdrawViaAtsException : Exception
{
    public UnableToWithdrawViaAtsException()
    {
    }

    public UnableToWithdrawViaAtsException(string message)
        : base(message)
    {
    }

    public UnableToWithdrawViaAtsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected UnableToWithdrawViaAtsException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}