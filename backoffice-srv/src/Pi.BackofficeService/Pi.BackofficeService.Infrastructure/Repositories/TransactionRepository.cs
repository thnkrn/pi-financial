using Microsoft.EntityFrameworkCore;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly BackofficeDbContext _context;

    public TransactionRepository(BackofficeDbContext context)
    {
        _context = context;
    }

    public async Task<Bank?> GetBankByAbbr(string abbr)
    {
        return await _context.Banks.Where(q => q.Abbreviation == abbr).FirstOrDefaultAsync();
    }

    public async Task<Bank?> GetBankByBankCode(string bankCode)
    {
        return await _context.Banks.Where(q => q.Code == bankCode).FirstOrDefaultAsync();
    }

    public async Task<List<Bank>> GetBanks()
    {
        return await _context.Banks.ToListAsync();
    }

    public async Task<List<Bank>> GetBanksByChannelAsync(string channel)
    {
        var bankCodes = await _context.BankChannels
            .Where(q => q.Channel.ToLower() == channel.ToLower())
            .Select(q => q.BankCode)
            .ToListAsync();

        return await _context.Banks.Where(q => bankCodes.Contains(q.Code)).ToListAsync();
    }
}
