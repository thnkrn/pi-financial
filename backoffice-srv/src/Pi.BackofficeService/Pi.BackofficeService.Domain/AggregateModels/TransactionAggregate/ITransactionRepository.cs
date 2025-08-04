namespace Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

public interface ITransactionRepository
{
    Task<Bank?> GetBankByAbbr(string abbr);
    Task<Bank?> GetBankByBankCode(string bankCode);
    Task<List<Bank>> GetBanks();
    Task<List<Bank>> GetBanksByChannelAsync(string channel);
}
