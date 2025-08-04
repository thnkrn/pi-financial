using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;
using Pi.GlobalEquities.Repositories.Configs;
using Pi.GlobalEquities.Repositories.Models;

namespace Pi.GlobalEquities.Repositories;

public class OrderRepository : IOrderRepository
{
    readonly MongoDbConfig _config;
    readonly IMongoClient _client;

    public OrderRepository(IMongoClient client, IOptions<MongoDbConfig> options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task CreateOrder(IOrder order, CancellationToken ct)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        var entity = order.ToDbModel();
        var db = _client.GetDatabase(_config.Database);
        var orderColName = MongoDbConfig.OrderCollection.Name;
        var col = db.GetCollection<OrderEntity>(orderColName);
        await col.InsertOneAsync(entity, null, ct);
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

    public async Task<IOrder> GetOrder(string userId, string orderId, CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var orderColName = MongoDbConfig.OrderCollection.Name;
        var query = db.GetCollection<OrderEntity>(orderColName).AsQueryable();

        var result = await query.Where(x => x.UserId == userId && x.Id == orderId).FirstOrDefaultAsync(ct);
        return result.ToDomainModel();
    }
}
