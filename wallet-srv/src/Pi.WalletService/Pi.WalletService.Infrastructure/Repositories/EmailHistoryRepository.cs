using Microsoft.EntityFrameworkCore;
using Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;

namespace Pi.WalletService.Infrastructure.Repositories;

public class EmailHistoryRepository : IEmailHistoryRepository
{
    private readonly WalletDbContext _walletDbContext;

    public EmailHistoryRepository(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task<EmailHistory> Create(EmailHistory emailHistory)
    {
        if (emailHistory == null)
        {
            throw new ArgumentNullException(nameof(emailHistory));
        }
        await _walletDbContext.Set<EmailHistory>().AddAsync(emailHistory);
        await _walletDbContext.SaveChangesAsync();

        return emailHistory;
    }

    public async Task<List<EmailHistory>> GetByTransactionNo(string transactionNo)
    {
        return await _walletDbContext
            .Set<EmailHistory>()
            .Where(r => r.TransactionNo == transactionNo)
            .ToListAsync();
    }
}