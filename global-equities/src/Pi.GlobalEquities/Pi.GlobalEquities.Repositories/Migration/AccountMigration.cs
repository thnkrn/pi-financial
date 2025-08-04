using MongoDB.Driver;
using Pi.GlobalEquities.Repositories.Configs;
using Pi.GlobalEquities.Repositories.Models;

namespace Pi.GlobalEquities.Repositories.Migration;

public static class AccountMigration
{
    public static async Task MigrateToV0(IMongoDatabase db, IClientSessionHandle session, CancellationToken ct)
    {
        var col = db.GetCollection<AccountEntity>(MongoDbConfig.AccountCollection.Name);
        var filter = Builders<AccountEntity>.Filter.Or(
            Builders<AccountEntity>.Filter.Eq(x => x.Version, -1),
            Builders<AccountEntity>.Filter.Eq(x => x.Version, null));
        var update = Builders<AccountEntity>.Update
            .Set(x => x.Version, 0);

        await col.UpdateManyAsync(session, filter, update, cancellationToken: ct);
    }

    public static async Task MigrateToV1(IMongoDatabase db, IClientSessionHandle session, CancellationToken ct)
    {
        var filter = Builders<AccountEntity>.Filter.Eq(x => x.Version, 0);
        var col = db.GetCollection<AccountEntity>(MongoDbConfig.AccountCollection.Name);
        var update = Builders<AccountEntity>.Update
            .Set(x => x.Version, 1);

        await col.UpdateManyAsync(session, filter, update, cancellationToken: ct);
    }

}
