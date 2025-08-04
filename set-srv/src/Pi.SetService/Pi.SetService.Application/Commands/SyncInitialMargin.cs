using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.SetService.Application.Extensions;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Services.SbaService;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Application.Commands;

public record SyncInitialMarginRequest
{
    public required string BucketName { get; init; }
    public required string FileKey { get; init; }
}

public class SyncInitialMarginConsumer(ISbaService sbaService, IInstrumentRepository instrumentRepository, ILogger<SyncInitialMarginConsumer> logger) : IConsumer<SyncInitialMarginRequest>
{
    private const int ChuckSize = 100;

    public async Task Consume(ConsumeContext<SyncInitialMarginRequest> context)
    {
        logger.LogInformation("Start Sync: {FileKey} (bucket={Bucket})", context.Message.FileKey, context.Message.BucketName);

        var result = context.Message.FileKey switch
        {
            "osec_ctrl.dat" => await SyncMarginInstrumentInfos(context),
            "omrg_tbl.dat" => await SyncMarginRates(context),
            _ => throw new NotSupportedException("Unsupported File")
        };

        logger.LogInformation("Sync Result of \"{FileKey}\": {@Result}", context.Message.FileKey, result);
        logger.LogInformation("Sync Finished: {FileKey} (bucket={Bucket})", context.Message.FileKey, context.Message.BucketName);

        if (context.ResponseAddress != null)
        {
            await context.RespondAsync(result);
        }
    }

    private async Task<SyncProcessResult> SyncMarginInstrumentInfos(ConsumeContext<SyncInitialMarginRequest> context)
    {
        var infos = sbaService.GetMarginInstrumentInfoFromStorage(context.Message.BucketName, context.Message.FileKey, context.CancellationToken);
        var result = new SyncProcessResult();
        await foreach (var instrumentInfos in infos.ProcessBatch(ChuckSize, context.CancellationToken))
        {
            var equityInfos = await instrumentRepository.GetEquityInfos(instrumentInfos.Select(q => q.Symbol).Distinct(), context.CancellationToken);
            var dicts = equityInfos.DistinctBy(q => q.Symbol).ToDictionary(q => q.Symbol, q => q);

            foreach (var instrumentInfo in instrumentInfos)
            {
                if (dicts.TryGetValue(instrumentInfo.Symbol, out var info))
                {
                    if (info.MarginCode == instrumentInfo.MarginCode && info.IsTurnoverList == instrumentInfo.IsTurnoverList)
                    {
                        result.Skip += 1;
                        continue;
                    }

                    info.MarginCode = instrumentInfo.MarginCode;
                    info.IsTurnoverList = instrumentInfo.IsTurnoverList;
                    instrumentRepository.UpdateEquityInfo(info);
                    result.Update += 1;
                }
                else
                {
                    instrumentRepository.CreateEquityInfo(new EquityInfo(Guid.NewGuid(), instrumentInfo.Symbol, instrumentInfo.MarginCode, instrumentInfo.IsTurnoverList));
                    result.Create += 1;
                }
            }

            var executions = await instrumentRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
            result.Execution += executions;
        }

        return result;
    }

    private async Task<SyncProcessResult> SyncMarginRates(ConsumeContext<SyncInitialMarginRequest> context)
    {
        var marginRates = sbaService.GetMarginRatesFromStorage(context.Message.BucketName, context.Message.FileKey, context.CancellationToken);
        var result = new SyncProcessResult();
        await foreach (var marginRatesBatch in marginRates.ProcessBatch(ChuckSize, context.CancellationToken))
        {
            var uniqueMarginRates = marginRatesBatch.DistinctBy(q => q.MarginCode).ToArray();
            var equityInfos = await instrumentRepository.GetEquityInitialMargins(uniqueMarginRates.Select(q => q.MarginCode), context.CancellationToken);
            var dicts = equityInfos.ToDictionary(q => q.MarginCode, q => q);

            foreach (var marginRate in uniqueMarginRates)
            {
                if (dicts.TryGetValue(marginRate.MarginCode, out var info))
                {
                    if (info.Rate == marginRate.MarginRate)
                    {
                        result.Skip += 1;
                        continue;
                    }

                    info.Rate = marginRate.MarginRate;
                    instrumentRepository.UpdateEquityInitialMargin(info);
                    result.Update += 1;
                }
                else
                {
                    instrumentRepository.CreateEquityInitialMargin(new EquityInitialMargin(Guid.NewGuid(), marginRate.MarginCode, marginRate.MarginRate));
                    result.Create += 1;
                }
            }

            var executions = await instrumentRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
            result.Execution += executions;
        }

        return result;
    }
}
