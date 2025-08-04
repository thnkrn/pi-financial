using Pi.Common.Generators.Number;
using Pi.SetService.Application.Utils;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Application.Services.NumberGeneratorService;

public class EquityNumberGeneratorService : Common.Generators.Number.NumberGeneratorService, IEquityNumberGeneratorService
{
    private readonly INumberGeneratorRepository _numberGeneratorRepository;
    private readonly IEquityOrderStateRepository _equityOrderStateRepository;
    private const string Module = "set_order";
    private readonly string _timeZone = "Asia/Bangkok";

    public EquityNumberGeneratorService(INumberGeneratorRepository numberGeneratorRepository,
        IEquityOrderStateRepository equityOrderStateRepository) : base(numberGeneratorRepository)
    {
        _numberGeneratorRepository = numberGeneratorRepository;
        _equityOrderStateRepository = equityOrderStateRepository;
        _timeZone = DateTimeHelper.ThTimeZone;
    }

    public async Task<string> GenerateOnlineOrderNoAsync(CancellationToken ct = default)
    {
        var generatorSettings = new NumberGeneratorSettings(Module, "SO", true, TimeZoneInfo.FindSystemTimeZoneById(_timeZone));
        return await GenerateStringNumberAsync(generatorSettings, ct);
    }

    public async Task<ulong> GenerateOfflineOrderNoAsync(CancellationToken ct = default)
    {
        var generatorSettings = new NumberGeneratorSettings(Module, "SOF", false, TimeZoneInfo.FindSystemTimeZoneById(_timeZone));
        return await GenerateAsNumber(generatorSettings, ct);
    }

    public async Task<string> GenerateAndUpdateOnlineOrderNoAsync(Guid correlationId, CancellationToken cancellationToken = default)
    {
        return await GenerateAndUpdateAsync((orderNo,
                ct) => UpdateOrderNoAsync(correlationId,
                orderNo,
                ct),
            async ct => await GenerateOnlineOrderNoAsync(ct),
            cancellationToken);
    }

    public async Task<ulong> GenerateAndUpdateOfflineOrderNoAsync(Guid correlationId, CancellationToken cancellationToken = default)
    {
        return await GenerateAndUpdateAsync((orderNo,
                ct) => UpdateOrderNoAsync(correlationId,
                orderNo,
                ct),
            async ct => await GenerateOfflineOrderNoAsync(ct),
            cancellationToken);
    }

    public async Task<ulong> GenerateSblOrderOrderIdAsync(CancellationToken cancellationToken = default)
    {
        var generatorSettings = new NumberGeneratorSettings(Module, "SBL", false, TimeZoneInfo.FindSystemTimeZoneById(_timeZone));
        return await GenerateAsNumber(generatorSettings, cancellationToken);
    }

    private async Task<ulong> GenerateAsNumber(NumberGeneratorSettings generatorSettings, CancellationToken cancellationToken)
    {
        var numberGenerator = await _numberGeneratorRepository.GetAsync(generatorSettings.Module, generatorSettings.Prefix, generatorSettings.Daily, cancellationToken);

        if (numberGenerator == null)
        {
            numberGenerator = await _numberGeneratorRepository.CreateAsync(new NumberGenerator(generatorSettings.Module, generatorSettings.Prefix, generatorSettings.Daily), cancellationToken);
            await _numberGeneratorRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            if (!IsSameMonth(generatorSettings, numberGenerator))
            {
                numberGenerator.CurrentCounter = 0;
            }
        }

        var number = numberGenerator.Generate(generatorSettings.TimeZoneInfo);
        _numberGeneratorRepository.Update(numberGenerator);
        await _numberGeneratorRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return (ulong)number;
    }

    private async Task<T> GenerateAndUpdateAsync<T>(
        Func<string, CancellationToken, Task> updateFunc,
        Func<CancellationToken, Task<T>> generateFunc,
        CancellationToken cancellationToken = default)
    {
        bool con;
        T recordNumber;
        do
        {
            con = false;
            recordNumber = await generateFunc(cancellationToken);
            try
            {
                await updateFunc(recordNumber?.ToString() ?? throw new InvalidOperationException(), cancellationToken);
            }
            catch (DuplicateRecordNoException)
            {
                con = true;
            }
        }
        while (con);
        return recordNumber;
    }

    private async Task UpdateOrderNoAsync(Guid correlationId, string orderNo, CancellationToken cancellationToken = default)
    {
        await _equityOrderStateRepository.UpdateOrderNoAsync(correlationId, orderNo, cancellationToken);
    }

    private static bool IsSameMonth(NumberGeneratorSettings generatorSettings, NumberGenerator numberGenerator)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, generatorSettings.TimeZoneInfo!).Month.Equals(TimeZoneInfo.ConvertTimeFromUtc(numberGenerator.UpdatedAt, generatorSettings.TimeZoneInfo!).Month);
    }
}
