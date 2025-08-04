using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi.Common.SeedWork;
using Pi.WalletService.Domain.AggregatesModel.WalletAggregate;

namespace Pi.WalletService.Infrastructure.Repositories;

public class GlobalWalletTransactionHistoryRepository : IGlobalWalletTransactionHistoryRepository
{
    private readonly WalletDbContext _walletDbContext;
    private readonly ILogger<GlobalWalletTransactionHistoryRepository> _logger;
    public IUnitOfWork UnitOfWork => _walletDbContext;

    public GlobalWalletTransactionHistoryRepository(
        WalletDbContext walletDbContext,
        ILogger<GlobalWalletTransactionHistoryRepository> logger)
    {
        _walletDbContext = walletDbContext;
        _logger = logger;
    }

    public async Task<GlobalWalletTransactionHistory> Get(Guid id)
    {
        GlobalWalletTransactionHistory globalWalletTransactionHistory;
        try
        {
            globalWalletTransactionHistory = await _walletDbContext.GlobalWalletTransactionHistories
                .Where(n => n.Id == id)
                .FirstAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(
                "Unable to queries transaction. Transaction ID: {ID}. Exception: {E}",
                id,
                (object?)e ?? string.Empty);
            throw;
        }

        return globalWalletTransactionHistory;
    }

    public async Task<GlobalWalletTransactionHistory?> GetByTransactionNo(string transactionNo, string userId)
    {
        GlobalWalletTransactionHistory globalWalletTransactionHistory;
        try
        {
            globalWalletTransactionHistory = await _walletDbContext.GlobalWalletTransactionHistories
                .Where(n => n.TransactionNo == transactionNo && n.UserId == userId)
                .FirstAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(
                "Unable to queries transaction. Transaction No: {TransactionNo}. Exception: {E}",
                transactionNo,
                (object?)e ?? string.Empty);
            throw;
        }

        return globalWalletTransactionHistory;
    }

    public GlobalWalletTransactionHistory Create(GlobalWalletTransactionHistory globalWalletTransactionHistory)
    {
        return _walletDbContext.GlobalWalletTransactionHistories.Add(globalWalletTransactionHistory).Entity;
    }
}