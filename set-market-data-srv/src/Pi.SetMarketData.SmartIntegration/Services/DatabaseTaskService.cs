using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Entities.SetSmart;
using Pi.SetMarketData.SmartIntegration.Interfaces;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Services.SqlServer;
using Pi.SetMarketData.SmartIntegration.Configurations;
using Pi.SetMarketData.Application.Interfaces.BrokerIdMapperService;
using Pi.SetMarketData.Application.Helper;
using Pi.SetMarketData.Application.Constants;

namespace Pi.SetMarketData.SmartIntegration.Services;

public class DatabaseTaskService : IDatabaseTaskService
{
    private readonly SqlServerContext _context;
    private readonly IMongoService<Instrument> _instrumentService;
    private readonly IMongoService<BrokerInfo> _brokerInfoService;
    private readonly IBrokerIdMapperService _brokerIdMapperService;
    private IDictionary<string, string> _brokerIdMap;
    private readonly ILogger<DatabaseTaskService> _logger;

    public DatabaseTaskService(
        SqlServerContext context,
        IMongoService<Instrument> instrumentService,
        IBrokerIdMapperService brokerIdMapperService,
        IMongoService<BrokerInfo> brokerService,
        ILogger<DatabaseTaskService> logger
    )
    {
        _context = context;
        _instrumentService = instrumentService;
        _brokerIdMapperService = brokerIdMapperService;
        _brokerInfoService = brokerService;
        _logger = logger;

        _brokerIdMap = new Dictionary<string, string>();
    }

    public async Task PerformDatabaseTask(BatchUpdateOptions options)
    {
        options ??= new BatchUpdateOptions();
        _logger.LogInformation("Performing database update with batch size: {BatchSize}, delay: {DelayMs}ms",
            options.BatchSize, options.Delay);

        var brokers = await _brokerInfoService.GetAllAsync();
        _brokerIdMap = _brokerIdMapperService.GetBrokerIdMap(brokers);

        // Get all security details
        var securityDetails = await _context
            .Security.Join(
                _context.SecurityDetail,
                security => security.ISecurity,
                securityDetail => securityDetail.ISecurity,
                (security, securityDetail) =>
                    new SecurityDetails
                    {
                        ISecurity = security.ISecurity,
                        NSecurity = security.NSecurity,
                        NSecurityE = security.NSecurityE,
                        ZMultiplier = securityDetail.ZMultiplier,
                        ZExercise = securityDetail.ZExercise,
                        QFirstRatio = securityDetail.QFirstRatio,
                        QLastRatio = securityDetail.QLastRatio,
                        DLastExercise = securityDetail.DLastExercise,
                        QTtm = securityDetail.QTtm,
                        DFirstTrade = securityDetail.DFirstTrade,
                        DLastTrade = securityDetail.DLastTrade,
                    }
            )
            .OrderBy(e => e.ISecurity)
            .ToListAsync();

        _logger.LogInformation("SecurityDetail has: {Row} rows", securityDetails.Count);

        // Get all instruments in one query
        var symbols = securityDetails
            .Where(s => s.NSecurity != null)
            .Select(s => s.NSecurity)
            .ToList();

        var instruments = await _instrumentService.GetAllByFilterAsync(
            target => symbols.Contains(target.Symbol)
        );

        _logger.LogInformation("Instruments list has: {Document} documents", instruments.Count());

        // Update instruments in memory
        var updatedInstruments = new List<Instrument>();
        foreach (var instrument in instruments)
        {
            try
            {
                var security = securityDetails.First(target => target.NSecurity == instrument.Symbol);

                UpdateInstrumentProperties(instrument, security);
                updatedInstruments.Add(instrument);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Skipping security with null");
            }
        }

        // Process in batches
        await ProcessInBatches(updatedInstruments, options);
    }

    private void UpdateInstrumentProperties(Instrument instrument, SecurityDetails security)
    {
        instrument.FriendlyName = security.NSecurityE;

        instrument.Multiplier = security.ZMultiplier.ToString();
        instrument.ExercisePrice = security.ZExercise.ToString();

        instrument.ExerciseRatio =
            security?.QFirstRatio != null && security?.QLastRatio != null
                ? $"{security.QFirstRatio} : {security.QLastRatio}"
                : null;
        instrument.ConversionRatio =
            security?.QFirstRatio != null && security?.QLastRatio != null
                ? $"{security.QFirstRatio} : {security.QLastRatio}"
                : null;

        instrument.DaysToExercise = security?.QTtm?.ToString();
        if (instrument.InstrumentCategory == InstrumentConstants.ThaiDerivativeWarrants)
        {
            instrument.MaturityDate = security?.DLastExercise?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(security.NSecurity))
            {
                (instrument.FriendlyName, instrument.Direction) = StringHelper.DwFormat(security.NSecurity, _brokerIdMap);
                instrument.IssuerSeries = StringHelper.GetIssuerSeries(security.NSecurity, _brokerIdMap);
            }
        }
        else if (instrument.InstrumentCategory == InstrumentConstants.ThaiStockWarrants)
        {
            if (!string.IsNullOrEmpty(security.NSecurity))
            {
                instrument.FriendlyName = StringHelper.WarrantsFormat(security.NSecurity);
            }
            instrument.ExerciseDate = security?.DLastExercise?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
        instrument.LastTradingDate = security?.DLastTrade?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        instrument.FromSetSmart = true;
    }

    private async Task ProcessInBatches(List<Instrument> instruments, BatchUpdateOptions options)
    {
        int totalBatches = (int)Math.Ceiling(instruments.Count / (double)options.BatchSize);
        int processedItems = 0;

        for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
        {
            var batch = instruments
                .Skip(batchIndex * options.BatchSize)
                .Take(options.BatchSize)
                .ToList();
            try
            {
                await _instrumentService.UpdateManyAsync(batch, x => x.Id);
                processedItems += batch.Count;

                _logger.LogInformation(
                    "Processed batch {CurrentBatch}/{TotalBatches}. Progress: {ProcessedItems}/{TotalItems} items",
                    batchIndex + 1,
                    totalBatches,
                    processedItems,
                    instruments.Count
                );

                if (batchIndex < totalBatches - 1)
                {
                    _logger.LogDebug("Waiting {DelayMs}ms before processing next batch", options.Delay);
                    await Task.Delay(options.Delay);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error processing batch {CurrentBatch}/{TotalBatches}. Failed items: {FailedItems}",
                    batchIndex + 1,
                    totalBatches,
                    batch.Select(x => x.Symbol));
            }
        }
        _logger.LogInformation("Complete cronjob task");
    }
}
