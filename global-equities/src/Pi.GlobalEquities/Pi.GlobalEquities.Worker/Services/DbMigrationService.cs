using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Pi.GlobalEquities.Repositories.Configs;
using Pi.GlobalEquities.Repositories.Migration;
using Pi.GlobalEquities.Repositories.Models;

namespace Pi.GlobalEquities.Worker.Services;

public class DbMigrationService : BackgroundService
{
    private readonly IMongoClient _mongoClient;
    private readonly IOptions<MongoDbConfig> _mongoOptions;
    private MongoDbConfig MongoConfig => _mongoOptions.Value;
    private readonly ILogger<DbMigrationService> _logger;

    public DbMigrationService(
        IMongoClient mongoClient,
        IOptions<MongoDbConfig> mongoOptions,
        ILogger<DbMigrationService> logger)
    {
        _mongoClient = mongoClient;
        _mongoOptions = mongoOptions;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (true)
        {
            try
            {
                _logger.LogInformation("Start DbMigrationService.");

                var db = _mongoClient.GetDatabase(MongoConfig.Database);

                if (HasIncorrectAccountVersion(db))
                    await MigrateAccountCollection(db, ct);

                if (HasIncorrectOrderVersion(db))
                    await MigrateOrderCollection(db, ct);

                _logger.LogInformation("Stop DbMigrationService");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when ExecuteAsync and will rerun again");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromMinutes(30), ct);
            }
        }
    }

    private async Task MigrateAccountCollection(IMongoDatabase db, CancellationToken ct)
    {
        using var session = await _mongoClient.StartSessionAsync(cancellationToken: ct);
        try
        {
            _logger.LogInformation("Start Account Migration");
            session.StartTransaction();

            await AccountMigration.MigrateToV0(db, session, ct);
            await AccountMigration.MigrateToV1(db, session, ct);

            await session.CommitTransactionAsync(ct);
            _logger.LogInformation("Stop Account Migration");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when MigrateAccountCollection and will rerun again");
            await session.AbortTransactionAsync(ct);
        }
    }

    private async Task MigrateOrderCollection(IMongoDatabase db, CancellationToken ct)
    {
        using var session = await _mongoClient.StartSessionAsync(cancellationToken: ct);
        try
        {
            _logger.LogInformation("Start Order Migration");
            session.StartTransaction();

            await OrderMigration.MigrateToV1(db, session, ct);
            await OrderMigration.MigrateToV2(db, session, ct);

            await session.CommitTransactionAsync(ct);
            _logger.LogInformation("Stop Order Migration");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when MigrateOrderCollection and will rerun again");
            await session.AbortTransactionAsync(ct);
        }
    }

    private bool HasIncorrectAccountVersion(IMongoDatabase db)
    {
        var accountQueryable = db.GetCollection<OrderEntity>(MongoDbConfig.AccountCollection.Name).AsQueryable();
        var hasIncorrectAccountVersion = accountQueryable.Any(x => x.Version != 1);
        return hasIncorrectAccountVersion;
    }

    private bool HasIncorrectOrderVersion(IMongoDatabase db)
    {
        var orderQueryable = db.GetCollection<OrderEntity>(MongoDbConfig.OrderCollection.Name).AsQueryable();
        var hasIncorrectOrderVersion = orderQueryable.Any(x => x.Version != 2);
        return hasIncorrectOrderVersion;
    }
}
