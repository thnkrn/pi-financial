using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;
using Pi.GlobalEquities.Repositories.Configs;
using Pi.GlobalEquities.Repositories.Models;

namespace Pi.GlobalEquities.Repositories;

public class WorkerAccountRepository : AccountRepository, IWorkerAccountRepository
{
    readonly MongoDbConfig _config;
    readonly IMongoClient _client;

    public WorkerAccountRepository(IMongoClient client, IOptions<MongoDbConfig> options) : base(client, options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<IAccount> GetAccountByProviderAccount(
        Provider provider,
        string providerAccountId, CancellationToken ct)
    {
        if (provider != Provider.Velexa)
            return null;

        var db = _client.GetDatabase(_config.Database);
        var accountColName = MongoDbConfig.AccountCollection.Name;
        var query = db.GetCollection<AccountEntity>(accountColName).AsQueryable();

        var result = await query.Where(x => x.VelexaAccount == providerAccountId).FirstOrDefaultAsync(ct);
        return result.ToDomainModel();
    }
}
