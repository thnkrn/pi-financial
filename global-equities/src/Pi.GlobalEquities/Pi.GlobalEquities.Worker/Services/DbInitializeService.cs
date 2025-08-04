using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Repositories;
using Pi.GlobalEquities.Repositories.Configs;
using Pi.GlobalEquities.Repositories.Migration;

namespace Pi.GlobalEquities.Worker.Services;

public class DbInitializeService
{
    private MongoDbConfig _config;
    private IMongoClient _client;

    public DbInitializeService(IMongoClient client, IOptions<MongoDbConfig> options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task InitDatabase(CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);

        await CreateAccountIndex(db, ct);
        await CreateOrderIndex(db, ct);
    }

    async Task CreateAccountIndex(IMongoDatabase db, CancellationToken ct)
    {
        var accountCol = db.GetCollection<Account>(MongoDbConfig.AccountCollection.Name);

        var indexes = await accountCol.Indexes.ListAsync(ct);
        var indexList = await indexes.ToListAsync(ct);

        var userIdIndex = MongoDbConfig.AccountCollection.Indexes.UserId;
        if (!IsIndexExists(indexList, userIdIndex))
            await CreateIndex(userIdIndex, x => x.UserId, accountCol, ct);

        var velexaAccountIndex = MongoDbConfig.AccountCollection.Indexes.VelexaAccount;
        if (!IsIndexExists(indexList, velexaAccountIndex))
            await CreateIndex(velexaAccountIndex, x => x.VelexaAccount, accountCol, ct);
    }

    async Task CreateOrderIndex(IMongoDatabase db, CancellationToken ct)
    {
        var orderCol = db.GetCollection<Order>(MongoDbConfig.OrderCollection.Name);

        var indexes = await orderCol.Indexes.ListAsync(ct);
        var indexList = await indexes.ToListAsync(ct);

        var statusIndex = MongoDbConfig.OrderCollection.Indexes.OrderStatus;
        if (!IsIndexExists(indexList, statusIndex))
            await CreateIndex(statusIndex, x => x.Status, orderCol, ct);

        var lastUpdateIndex = MongoDbConfig.OrderCollection.Indexes.OrderLastUpdate;
        if (!IsIndexExists(indexList, lastUpdateIndex))
            await CreateIndex(lastUpdateIndex, x => x.UpdatedAt, orderCol, ct);
    }

    private async Task CreateIndex<T>(
        string name,
        Expression<Func<T, object>> expression,
        IMongoCollection<T> col,
        CancellationToken ct)
    {
        var indexKeysDefinition = Builders<T>.IndexKeys.Ascending(expression);
        var indexOptions = new CreateIndexOptions { Background = true, Name = name };
        var indexModel = new CreateIndexModel<T>(indexKeysDefinition, indexOptions);
        await col.Indexes.CreateOneAsync(indexModel, cancellationToken: ct);
    }

    private static bool IsIndexExists(IEnumerable<BsonDocument> indexes, string name)
    {
        var indexExists = indexes.Any(index => index["name"] == name);
        return indexExists;
    }
}


