using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Helper;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.DeprecateCleaner.Services;

public class DeprecateCleaner(IMongoContext context, ILogger<DeprecateCleaner> logger)
{
    public async Task CleanAll(CancellationToken cancellationToken = default)
    {
        try
        {
            var collection = context.GetCollection<Instrument>();
            var tradingSignCollection = context.GetCollection<TradingSign>();
            var options = new FindOptions<Instrument>
            {
                BatchSize = 100
            };
            var cursor = await collection.FindAsync(
                q => q.Deprecated != true,
                options,
                cancellationToken: cancellationToken);

            var documentCount = 0;
            while (await cursor.MoveNextAsync(cancellationToken))
            {
                var batch = cursor.Current;
                var ids = new List<ObjectId>();

                foreach (var document in batch)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    if (document.SecurityType is InstrumentConstants.CS or InstrumentConstants.PS or InstrumentConstants.ETF
                        or InstrumentConstants.DR)
                    {
                        var tradingSign = await tradingSignCollection.Find(q => q.OrderBookId == document.OrderBookId && q.Sign != null && q.Sign != "")
                            .ToListAsync(cancellationToken: cancellationToken);
                        if (tradingSign != null)
                        {
                            document.TradingSigns = tradingSign;
                        }
                    }

                    if (!InstrumentHelper.IsDeprecated(document)) continue;

                    document.Deprecated = true;
                    ids.Add(document.Id);
                }

                
                var updateBuilder = Builders<Instrument>.Update;
                var update = updateBuilder.Set(x => x.Deprecated, true);
                await collection.UpdateManyAsync(q => ids.Contains(q.Id), update, cancellationToken: cancellationToken);

                documentCount += ids.Count;
            }
            
            logger.LogInformation("Updated {DocumentCount} documents", documentCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Something went wrong");
            throw;
        }
    }
}
