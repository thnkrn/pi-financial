using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;
using Pi.GlobalEquities.Repositories.Configs;
using Pi.GlobalEquities.Repositories.Models;

namespace Pi.GlobalEquities.Repositories;

public class AccountRepository : IAccountRepository
{
    readonly MongoDbConfig _config;
    readonly IMongoClient _client;

    public AccountRepository(IMongoClient client, IOptions<MongoDbConfig> options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<IAccount> GetAccount(string userId, string accountId, CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var accountColName = MongoDbConfig.AccountCollection.Name;
        var query = db.GetCollection<AccountEntity>(accountColName).AsQueryable();

        var result = await query.Where(x => x.UserId == userId && x.Id == accountId).FirstOrDefaultAsync(ct);
        return result.ToDomainModel();
    }

    public async Task<IEnumerable<IAccount>> GetAccounts(string userId, CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var accountColName = MongoDbConfig.AccountCollection.Name;
        var query = db.GetCollection<AccountEntity>(accountColName).AsQueryable();

        var accounts = await query.Where(x => x.UserId == userId).ToListAsync(ct);
        var results = accounts.Select(x => x.ToDomainModel());

        return results;
    }

    public async Task<IAccount> GetAccountByProviderAccount(
        string userId,
        Provider provider,
        string providerAccountId, CancellationToken ct)
    {
        if (provider != Provider.Velexa)
            return null;

        var db = _client.GetDatabase(_config.Database);
        var accountColName = MongoDbConfig.AccountCollection.Name;
        var query = db.GetCollection<AccountEntity>(accountColName).AsQueryable();

        var result = await query.Where(x => x.UserId == userId && x.VelexaAccount == providerAccountId).FirstOrDefaultAsync(ct);
        return result.ToDomainModel();
    }

    public async Task UpsertAccounts(IEnumerable<IAccount> accounts, CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var accountColName = MongoDbConfig.AccountCollection.Name;
        var col = db.GetCollection<AccountEntity>(accountColName);
        var option = new ReplaceOptions { IsUpsert = true };

        foreach (var acc in accounts)
        {
            var entity = acc.ToDbModel();
            var filter = Builders<AccountEntity>.Filter.Eq(x => x.Id, acc.Id);
            await col.ReplaceOneAsync(filter, entity, option, ct);
        }
    }
}
