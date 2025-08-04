using System.Runtime.Serialization;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Services.PaymentService;

public interface IPaymentService
{
    public Task<string> TransferViaOdd(
        string transactionNo,
        decimal transferAmount,
        TransactionType transactionType,
        string customerBankCode,
        string customerBankAccountNo,
        string customerBankAccountName,
        string customerBankAccountTaxId,
        string customerNo,
        Product product
    );
}

[Serializable]
public class UnableToTransferViaOddException : Exception
{
    public UnableToTransferViaOddException()
    {
    }

    public UnableToTransferViaOddException(string message)
        : base(message)
    {
    }

    public UnableToTransferViaOddException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected UnableToTransferViaOddException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
