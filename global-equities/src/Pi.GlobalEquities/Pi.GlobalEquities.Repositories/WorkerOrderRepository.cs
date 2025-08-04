using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Repositories.Configs;
using Pi.GlobalEquities.Repositories.Models;

namespace Pi.GlobalEquities.Repositories;

public class WorkerOrderRepository : OrderRepository, IWorkerOrderRepository
{
    readonly MongoDbConfig _config;
    readonly IMongoClient _client;

    public WorkerOrderRepository(IMongoClient client, IOptions<MongoDbConfig> options) : base(client, options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<IOrder> GetOrder(string orderId, CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var orderColName = MongoDbConfig.OrderCollection.Name;
        var query = db.GetCollection<OrderEntity>(orderColName).AsQueryable();

        var result = await query.Where(x => x.Id == orderId).FirstOrDefaultAsync(ct);
        var order = result.ToDomainModel();
        return order;
    }
}
