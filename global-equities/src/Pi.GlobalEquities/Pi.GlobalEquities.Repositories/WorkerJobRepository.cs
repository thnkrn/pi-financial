using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pi.GlobalEquities.Repositories.Configs;
using Pi.GlobalEquities.Repositories.Models;

namespace Pi.GlobalEquities.Repositories;

public class WorkerJobRepository : IWorkerJobRepository
{
    readonly MongoDbConfig _config;
    readonly IMongoClient _client;

    public WorkerJobRepository(IMongoClient client, IOptions<MongoDbConfig> options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<DomainModels.WorkerJob<T>> GetJobDetails<T>(string jobName, CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var workerJobColName = MongoDbConfig.WorkerJobCollection.Name;
        var query = db.GetCollection<WorkerJobEntity<T>>(workerJobColName).AsQueryable();

        var result = await query.Where(x => x.Name == jobName).FirstOrDefaultAsync(ct);
        return result.ToDomainModel();
    }

    public async Task ReplaceJobDetails<T>(DomainModels.WorkerJob<T> job, CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var workerJobColName = MongoDbConfig.WorkerJobCollection.Name;
        var col = db.GetCollection<WorkerJobEntity<T>>(workerJobColName);
        var option = new ReplaceOptions { IsUpsert = true };

        var entity = job.ToDbModel();
        var filter = Builders<WorkerJobEntity<T>>.Filter.Eq(x => x.Name, job.Name);
        await col.ReplaceOneAsync(filter, entity, option, ct);
    }
}
