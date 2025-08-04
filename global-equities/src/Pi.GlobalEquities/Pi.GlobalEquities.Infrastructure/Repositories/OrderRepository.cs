using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pi.GlobalEquities.Application.Repositories;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Infrastructure.Repositories.Models;
using Pi.GlobalEquities.Repositories.Configs;

namespace Pi.GlobalEquities.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    readonly MongoDbConfig _config;
    readonly IMongoClient _client;

    public OrderRepository(IMongoClient client, IOptions<MongoDbConfig> options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task UpdateOrder(string orderId, IOrder order, CancellationToken ct)
    {
        var entity = order.ToDbModel();
        var db = _client.GetDatabase(_config.Database);
        var orderColName = MongoDbConfig.OrderCollection.Name;
        var col = db.GetCollection<OrderEntity>(orderColName);
        var filter = Builders<OrderEntity>.Filter.Eq(x => x.Id, orderId);
        var option = new ReplaceOptions { IsUpsert = true };

        await col.ReplaceOneAsync(filter, entity, option, ct);
    }

    public async Task<IEnumerable<IOrder>?> GetOrders(string refId, CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var orderColName = MongoDbConfig.OrderCollection.Name;
        var query = db.GetCollection<OrderEntity>(orderColName).AsQueryable();

        var results = await query.Where(x => x.Id == refId || x.GroupId == refId).ToListAsync(ct);

        return results?.Select(x => x.ToDomainModel());
    }
}
