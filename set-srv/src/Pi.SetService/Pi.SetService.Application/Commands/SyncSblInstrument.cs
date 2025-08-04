using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.SetService.Application.Extensions;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Services.SblService;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Application.Commands;

public record SyncSblInstrument
{
    public required string BucketName { get; init; }
    public required string FileKey { get; init; }
}

public class SyncSblInstrumentConsumer(
    ISblService sblService,
    IInstrumentRepository instrumentRepository,
    ILogger<SyncSblInstrumentConsumer> logger) : IConsumer<SyncSblInstrument>
{
    private const int ChuckSize = 500;

    public async Task Consume(ConsumeContext<SyncSblInstrument> context)
    {
        logger.LogInformation("Start Sync: {FileKey} (bucket={Bucket})", context.Message.FileKey, context.Message.BucketName);

        var result = await Sync(context);

        logger.LogInformation("Sync Result of \"{FileKey}\": {@Result}", context.Message.FileKey, result);
        logger.LogInformation("Sync Finished: {FileKey} (bucket={Bucket})", context.Message.FileKey, context.Message.BucketName);

        if (context.ResponseAddress != null)
        {
            await context.RespondAsync(result);
        }
    }

    private async Task<SyncProcessResult> Sync(ConsumeContext<SyncSblInstrument> context)
    {
        var instruments = sblService.GetSblInstrumentInfoFromStorage(context.Message.BucketName, context.Message.FileKey, context.CancellationToken);
        var result = new SyncProcessResult();

        await instrumentRepository.ClearSblInstrumentsAsync(context.CancellationToken);
        await foreach (var sblInstrumentInfos in instruments.ProcessBatch(ChuckSize, context.CancellationToken))
        {
            var uniqueInfos = sblInstrumentInfos.DistinctBy(q => q.Symbol).ToArray();

            foreach (var instrumentInfo in uniqueInfos)
            {
                instrumentRepository.CreateSblInstrument(new SblInstrument(Guid.NewGuid(), instrumentInfo.Symbol, instrumentInfo.InterestRate, instrumentInfo.RetailLender));
                result.Create += 1;
            }

            var executions = await instrumentRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
            result.Execution += executions;
        }

        return result;
    }
}
