using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Repositories;

public class CommonDataRepository : ICommonDataRepository
{
    private MongoDbConfig _config;
    private IMongoClient _client;
    private ILogger<CommonDataRepository> _logger;
    public CommonDataRepository(IMongoClient client, IOptions<MongoDbConfig> options, ILogger<CommonDataRepository> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
    }

    public async Task<IList<FundCategory>> GetFundCategories(CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var query = db.GetCollection<FundCategory>(MongoDbConfig.FundCategoriesColName).AsQueryable();

        return await query.ToListAsync(ct);
    }

    public async Task<IList<ExtFundCategory>> GetExtFundCategories(CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var query = db.GetCollection<ExtFundCategory>(MongoDbConfig.ExtFundCategoriesColName).AsQueryable();

        return await query.ToListAsync(ct);
    }

    public async Task<BusinessCalendar> GetBusinessCalendar(CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var query = db.GetCollection<BusinessHoliday>(MongoDbConfig.BusinessHolidaysColName).AsQueryable();

        var holidays = await query.Select(x => x.Date).ToListAsync(ct);

        return new() { BusinessHolidays = holidays };
    }

    public async Task UpsertBusinessCalendar(IEnumerable<DateTime> businessHolidays, CancellationToken ct)
    {
        if (businessHolidays == null)
            throw new ArgumentNullException(nameof(businessHolidays));

        var holidays = businessHolidays.Select(x => new BusinessHoliday { Date = x }).ToArray();
        if (!holidays.Any())
            return;

        var db = _client.GetDatabase(_config.Database);
        var col = db.GetCollection<BusinessHoliday>(MongoDbConfig.BusinessHolidaysColName);

        using var session = await _client.StartSessionAsync(cancellationToken: ct);
        try
        {
            session.StartTransaction();

            await col.DeleteManyAsync(session, new BsonDocument(), cancellationToken: ct);
            await col.InsertManyAsync(session, holidays, new InsertManyOptions(), cancellationToken: ct);

            await session.CommitTransactionAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when UpsertBusinessCalendar.");
            await session.AbortTransactionAsync(ct);
        }
    }
}
