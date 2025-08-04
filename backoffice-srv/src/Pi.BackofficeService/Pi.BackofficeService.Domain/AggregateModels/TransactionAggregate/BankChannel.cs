namespace Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

public class BankChannel
{
    public BankChannel(string bankCode, string channel)
    {
        BankCode = bankCode;
        Channel = channel;
    }

    public string BankCode { get; private set; }
    public string Channel { get; private set; }
}
