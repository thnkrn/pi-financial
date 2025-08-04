using MongoDB.Bson;
using MongoDB.Driver;
using Pi.GlobalEquities.Models;
using Pi.GlobalEquities.Repositories.Configs;
using Pi.GlobalEquities.Repositories.Models;

namespace Pi.GlobalEquities.Repositories.Migration;

public static class OrderMigration
{
    public static async Task MigrateToV1(IMongoDatabase database, IClientSessionHandle session, CancellationToken ct)
    {
        var col = database.GetCollection<OrderEntity>(MongoDbConfig.OrderCollection.Name);
        var filter = Builders<OrderEntity>.Filter.Or(
            Builders<OrderEntity>.Filter.Eq(x => x.Version, null),
            Builders<OrderEntity>.Filter.Eq(x => x.Version, 0)
        );
        var update = Builders<OrderEntity>.Update
            .Set(x => x.Version, 1);

        await col.UpdateManyAsync(session, filter, update, cancellationToken: ct);
    }

    public static async Task MigrateToV2(IMongoDatabase database, IClientSessionHandle session, CancellationToken ct)
    {
        var col = database.GetCollection<BsonDocument>(MongoDbConfig.OrderCollection.Name);

        //bump version for every document that is not queue or cancel
        var versionFiltered = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("Version", 1),
            Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Ne("Status", "Queue"),
                Builders<BsonDocument>.Filter.Ne("Status", "Canceled")
            )
        );
        var versionUpdate = Builders<BsonDocument>.Update
            .Set("Version", 2);
        await col.UpdateManyAsync(session, versionFiltered, versionUpdate, cancellationToken: ct);


        var cancelledFilter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("Version", 1),
            Builders<BsonDocument>.Filter.Eq("Status", "Canceled")
        );
        var canceledUpdate = Builders<BsonDocument>.Update
            .Set("Version", 2)
            .Set("Status", "Cancelled");
        await col.UpdateManyAsync(session, cancelledFilter, canceledUpdate, cancellationToken: ct);


        var queueFiltered = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("Version", 1),
            Builders<BsonDocument>.Filter.Eq("Status", "Queue")
        );
        var queueUpdate = Builders<BsonDocument>.Update
            .Set("Version", 2)
            .Set("Status", "Queued");
        await col.UpdateManyAsync(session, queueFiltered, queueUpdate, cancellationToken: ct);
    }
}
