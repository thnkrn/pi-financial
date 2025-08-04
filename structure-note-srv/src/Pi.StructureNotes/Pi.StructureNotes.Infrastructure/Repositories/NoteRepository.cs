using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pi.StructureNotes.Infrastructure.Repositories.Entities;

namespace Pi.StructureNotes.Infrastructure.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly IServiceScopeFactory _scopeFactory;

    public NoteRepository(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async Task<AccountNotes> GetStructuredNotes(AccountInfo account, CancellationToken ct = default)
    {
        var notes = new List<Note>();
        var stocks = new List<Stock>();
        var cash = new List<Cash>();
        var accountIds = new[] { account.AccountId };
        var tasks = new List<Task>()
        {
            GetNotes(accountIds, notes, ct), GetStocks(accountIds, stocks, ct), GetCash(accountIds, cash, ct)
        };

        await Task.WhenAll(tasks);

        var result = new AccountNotes(notes, stocks, cash)
        {
            AccountId = account.AccountId,
            AccountNo = account.AccountNo,
            CustCode = account.CustCode
        };

        return result;
    }

    public async IAsyncEnumerable<AccountNotes> GetStructuredNotes(IEnumerable<AccountInfo> infos,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var notes = new List<Note>();
        var stocks = new List<Stock>();
        var cash = new List<Cash>();
        var accountIds = infos.Select(x => x.AccountId);
        var tasks = new List<Task>()
        {
            GetNotes(accountIds, notes, ct), GetStocks(accountIds, stocks, ct), GetCash(accountIds, cash, ct)
        };

        await Task.WhenAll(tasks);

        foreach (AccountInfo account in infos)
        {
            var accountId = account.AccountId;
            var tradingAccNo = account.AccountNo;
            var custCode = account.CustCode;
            var accNotes = notes.Where(x => x.AccountId == accountId);
            var accStocks = stocks.Where(x => x.AccountId == accountId);
            var accCash = cash.Where(x => x.AccountId == accountId);

            yield return new AccountNotes(accNotes, accStocks, accCash)
            {
                AccountId = accountId,
                AccountNo = tradingAccNo,
                CustCode = custCode
            };
        }
    }

    public async Task Reset(AccountEntities accEntities, CancellationToken ct = default)
    {
        var accountId = accEntities.AccountId;
        using var scope = _scopeFactory.CreateScope();
        using var ctx = scope.ServiceProvider.GetRequiredService<SnDbContext>();

        await ResetEntity(ctx, accountId, ctx.Notes, ct);
        await ResetEntity(ctx, accountId, ctx.Stocks, ct);
        await ResetEntity(ctx, accountId, ctx.Cash, ct);

        await PopulateEntity(accEntities.Notes, ctx.Notes, ct);
        await PopulateEntity(accEntities.Stocks, ctx.Stocks, ct);
        await PopulateEntity(accEntities.Cash, ctx.Cash, ct);

        await ctx.SaveChangesAsync(ct);
    }

    public async Task CleanUp(CancellationToken ct = default)
    {
        using var scope = _scopeFactory.CreateScope();
        using var ctx = scope.ServiceProvider.GetRequiredService<SnDbContext>();

        await RemoveOldEntity(ctx, ctx.Notes, ct);
        await RemoveOldEntity(ctx, ctx.Stocks, ct);
        await RemoveOldEntity(ctx, ctx.Cash, ct);

        await ctx.SaveChangesAsync(ct);
    }

    private async Task GetNotes(IEnumerable<string> accountIds, List<Note> notes, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        using var ctx = scope.ServiceProvider.GetRequiredService<SnDbContext>();
        var result = await ctx.Notes
            .Where(x => accountIds.Contains(x.AccountId))
            .Select(x => x.ToDomainModel()).ToArrayAsync(ct);

        notes.AddRange(result);
    }

    private async Task GetStocks(IEnumerable<string> accountIds, List<Stock> stocks, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        using var ctx = scope.ServiceProvider.GetRequiredService<SnDbContext>();
        var result = await ctx.Stocks
            .Where(x => accountIds.Contains(x.AccountId))
            .Select(x => x.ToDomainModel()).ToArrayAsync(ct);

        stocks.AddRange(result);
    }

    private async Task GetCash(IEnumerable<string> accountIds, List<Cash> cash, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        using var ctx = scope.ServiceProvider.GetRequiredService<SnDbContext>();
        var result = await ctx.Cash
            .Where(x => accountIds.Contains(x.AccountId))
            .Select(x => x.ToDomainModel()).ToArrayAsync(ct);

        cash.AddRange(result);
    }

    private async Task ResetEntity<T>(SnDbContext ctx, string accountId, DbSet<T> set, CancellationToken ct)
        where T : EntityBase
    {
        var count = await set.Where(x => x.AccountId == accountId).CountAsync(ct);
        var batch = 100;
        var skip = 0;
        while (skip < count)
        {
            var items = set.Where(x => x.AccountId == accountId).OrderBy(x => x.Id).Skip(skip).Take(batch);
            ctx.RemoveRange(items);
            skip += await items.CountAsync(ct);
        }
    }

    private async Task PopulateEntity<T>(IEnumerable<T> all, DbSet<T> set, CancellationToken ct)
        where T : EntityBase
    {
        var count = all.Count();
        var batch = 100;
        var skip = 0;
        while (skip < count)
        {
            var items = all.Skip(skip).Take(batch);
            set.AddRange(items);
            skip += items.Count();
        }

        await Task.CompletedTask;
    }

    private async Task RemoveOldEntity<T>(SnDbContext ctx, DbSet<T> set, CancellationToken ct)
        where T : EntityBase
    {
        var latestDateTime = await set.OrderByDescending(x => x.AsOf).Select(x => x.AsOf).FirstOrDefaultAsync(ct);
        var items = set.Where(x => x.AsOf < latestDateTime);
        ctx.RemoveRange(items);
    }
}
