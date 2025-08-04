using Microsoft.EntityFrameworkCore;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
namespace Pi.WalletService.Infrastructure.Repositories;

public class GlobalTransferRepository : GenericRepository<GlobalTransferState>, IGlobalTransferRepository
{
    private readonly WalletDbContext _walletDbContext;

    public GlobalTransferRepository(WalletDbContext walletDbContext) : base(walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task<GlobalTransferState?> Get(Guid correlationId)
    {
        return await _walletDbContext
            .Set<GlobalTransferState>()
            .SingleOrDefaultAsync(r => r.CorrelationId == correlationId);
    }

    public async Task UpdateExchangeData(Guid correlationId, decimal exchangeAmount, Currency exchangeCurrency)
    {
        var globalTransferState = await _walletDbContext.Set<GlobalTransferState>().SingleOrDefaultAsync(r => r.CorrelationId == correlationId);
        if (globalTransferState is null)
        {
            throw new Exception($"GlobalTransferState not found, CorrelationId: {correlationId}");
        }

        globalTransferState.ExchangeAmount = exchangeAmount;
        globalTransferState.ExchangeCurrency = exchangeCurrency;

        await _walletDbContext.SaveChangesAsync();
    }

    public async Task UpdateRequestedFxAmountById(Guid correlationId, decimal requestedFxAmount)
    {
        var result = await _walletDbContext.Set<GlobalTransferState>()
            .Where(r => r.CorrelationId == correlationId)
            .SingleOrDefaultAsync();

        if (result == null)
        {
            throw new KeyNotFoundException();
        }

        result.RequestedFxRate = requestedFxAmount;
        await _walletDbContext.SaveChangesAsync();
    }

    public async Task UpdateGlobalAccountById(Guid correlationId, string globalAccount)
    {
        var result = await _walletDbContext.Set<GlobalTransferState>()
            .Where(r => r.CorrelationId == correlationId)
            .SingleOrDefaultAsync();

        if (result == null)
        {
            throw new KeyNotFoundException();
        }

        result.GlobalAccount = globalAccount;
        await _walletDbContext.SaveChangesAsync();
    }

    public async Task<bool> UpdateGlobalAccountByIdAndState(Guid correlationId, string state, string globalAccount)
    {
        var result = await _walletDbContext.Set<GlobalTransferState>()
            .Where(r => r.CorrelationId == correlationId && r.CurrentState == state)
            .SingleOrDefaultAsync();

        if (result == null)
        {
            return false;
        }

        result.GlobalAccount = globalAccount;
        await _walletDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateCurrencyAndState(Guid correlationId, string state, Currency requestedCurrency, Currency requestedFxCurrency)
    {
        var result = await _walletDbContext.Set<GlobalTransferState>()
            .Where(r => r.CorrelationId == correlationId && r.CurrentState == state)
            .SingleOrDefaultAsync();

        if (result == null)
        {
            return false;
        }

        result.RequestedCurrency = requestedCurrency;
        result.RequestedFxCurrency = requestedFxCurrency;
        await _walletDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateFxTransactionIdAndState(Guid correlationId, string state, string fxTransactionId)
    {
        var result = await _walletDbContext.Set<GlobalTransferState>()
            .Where(r => r.CorrelationId == correlationId && r.CurrentState == state)
            .SingleOrDefaultAsync();

        if (result == null)
        {
            return false;
        }

        result.FxTransactionId = fxTransactionId;
        await _walletDbContext.SaveChangesAsync();
        return true;
    }
}
