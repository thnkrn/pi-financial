namespace Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;

public interface IEmailHistoryRepository
{
    Task<EmailHistory> Create(EmailHistory emailHistory);
    Task<List<EmailHistory>> GetByTransactionNo(string transactionNo);
}