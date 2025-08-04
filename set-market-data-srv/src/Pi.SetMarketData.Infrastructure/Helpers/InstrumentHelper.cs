using MongoDB.Bson;
using MongoDB.Driver;
using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Extensions;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Services.Helpers;

public class InstrumentHelper : IInstrumentHelper
{
    private readonly IMongoService<SetMonthMapping> _monthMappingService;
    private IEnumerable<SetMonthMapping> _monthMapping;
    public InstrumentHelper(IMongoService<SetMonthMapping> monthMappingService)
    {
        _monthMappingService = monthMappingService;
        _monthMapping = _monthMappingService.GetAllAsync().GetAwaiter().GetResult();
    }

    private readonly Dictionary<string, string> defaultMonthMap = new()
    {
        { "F", "Jan" }, { "G", "Feb" }, { "H", "Mar" }, { "J", "Apr" },
        { "K", "May" }, { "M", "Jun" }, { "N", "Jul" }, { "Q", "Aug" },
        { "U", "Sep" }, { "V", "Oct" }, { "X", "Nov" }, { "Z", "Dec" }
    };

    public async Task<string> GetFriendlyName(string symbol, string instrumentCategory)
    {
        if (_monthMapping == null || !_monthMapping.Any() || _monthMapping.Count() < 12) // If Month mapping is not contain whole year, then set default month mapping collection
        {
            _monthMapping = defaultMonthMap.Select(m => new SetMonthMapping
            {
                Id = ObjectId.GenerateNewId(),
                MonthCode = m.Key,
                MonthAbbr = m.Value
            });
            await SaveMonthMappingsAsync(_monthMapping);
        }

        switch (instrumentCategory)
        {
            case InstrumentConstants.SET50IndexFutures:
                //Extract Index Futures
                return ExtractS50IndexFuture(symbol);
            case InstrumentConstants.SET50IndexOptions:
                //Extract Index Options
                return ExtractS50IndexOptions(symbol);
            case InstrumentConstants.SectorIndexFutures:
                //Extract Sector Index Futures
                return ExtractSectorIndexFutures(symbol);
            case InstrumentConstants.SingleStockFutures:
                //Extract SingleStock Futures
                return ExtractSingleStockFutures(symbol);
            case InstrumentConstants.CurrencyFutures:
                //Extract Currency Futures
                return ExtractCurrencyFutures(symbol);
            case InstrumentConstants.PreciousMetalFutures:
                //Extract Precious Metal Futures
                return ExtractPreciousMetalFutures(symbol);
        }
        return symbol;
    }

    private string ExtractS50IndexFuture(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol) || symbol.Length < 3)
            throw new ArgumentException("Invalid symbol format. S50 Index Futures must contains {S50}{MonthCode}{YearCode}", nameof(symbol));

        // Extract the month code and year
        string monthCode = symbol[^3].ToString();
        string yearCode = symbol[^2..];

        // Validate and get the month abbreviation
        var monthAbbr = _monthMapping?.Where(m => m.MonthCode == monthCode).Select(m => m.MonthAbbr).FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrEmpty(monthAbbr))
            throw new ArgumentException($"Invalid month code '{monthCode}' in symbol.", nameof(symbol));

        // Construct and return the friendly name
        return $"SET50 Index {monthAbbr} {yearCode} Futures";
    }

    private string ExtractS50IndexOptions(string symbol)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Invalid symbol format.", nameof(symbol));

        // Find the last index of Option type
        int indexOfOptionType = -1;
        for (int i = symbol.Length - 1; i >= 0; i--)
        {
            if (symbol[i] == 'P' || symbol[i] == 'C')
            {
                indexOfOptionType = i;
                break;
            }
        }
        if (indexOfOptionType <= 0)
            throw new ArgumentException("Invalid symbol format. S50 Index Options must contain option type (P or C)", nameof(symbol));

        // Validate and get the month abbreviation
        string monthCode = symbol[3].ToString();
        var monthAbbr = _monthMapping?.Where(m => m.MonthCode == monthCode).Select(m => m.MonthAbbr).FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrEmpty(monthAbbr))
            throw new ArgumentException($"Invalid month code '{monthCode}' in symbol.", nameof(symbol));

        // Get Year
        var year = symbol.Substring(symbol.IndexOf(monthCode) + 1, 2);
        // Get Option type
        string optionType = symbol[indexOfOptionType] switch
        {
            'P' => "Put",
            'C' => "Call",
            _ => ""
        };

        // Get Strike price
        string strikePrice = symbol[(indexOfOptionType + 1)..];
        return $"SET50 Index {monthAbbr} {year} {optionType} {strikePrice}";
    }

    private string ExtractSectorIndexFutures(string symbol)
    {
        // Validate and get the month abbreviation
        string monthCode = symbol[^3].ToString(); // Get charAt(3) from last index
        var monthAbbr = _monthMapping?.Where(m => m.MonthCode == monthCode).Select(m => m.MonthAbbr).FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrEmpty(monthAbbr))
            throw new ArgumentException($"Invalid month code '{monthCode}' in symbol.", nameof(symbol));
        // Get year
        var year = symbol[^2..]; // Last 2 charactors is year code
        // Get Sector code
        var sectorCode = symbol[..^3]; // From first index to 3rd charactor from last is sector code

        return $"SET {sectorCode} Sector {monthAbbr} {year} Futures";
    }

    private string ExtractSingleStockFutures(string symbol)
    {
        // Validate and get the month abbreviation
        symbol = symbol.TrimEnd('x', 'X', 'y','Y','z','Z');
        string monthCode = symbol[^3].ToString(); // Get charAt(3) from last index
        var monthAbbr = _monthMapping?.Where(m => m.MonthCode == monthCode).Select(m => m.MonthAbbr).FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrEmpty(monthAbbr))
            throw new ArgumentException($"Invalid month code '{monthCode}' in symbol: {symbol}.");
        // Get year
        var year = symbol[^2..]; // Last 2 charactors is year code
        // Get symbol underlying
        var symbolUnderlying = symbol[..^3]; // From first index to 3rd charactor from last is symbol underlying

        return $"{symbolUnderlying} {monthAbbr} {year} Futures";
    }

    private string ExtractCurrencyFutures(string symbol)
    {
        // Validate and get the month abbreviation
        string monthCode = symbol[^3].ToString(); // Get charAt(3) from last index
        var monthAbbr = _monthMapping?.Where(m => m.MonthCode == monthCode).Select(m => m.MonthAbbr).FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrEmpty(monthAbbr))
            throw new ArgumentException($"Invalid month code '{monthCode}' in symbol.", nameof(symbol));
        // Get Year
        var year = symbol[^2..]; // Last 2 charactors is year code
        // Get Currency
        var currency = symbol[..^3]; // From first index to 3rd charactor from last is currency

        return $"{currency} Currency {monthAbbr} {year} Futures";
    }

    private string ExtractPreciousMetalFutures(string symbol)
    {
        // Validate and get the month abbreviation
        string monthCode = symbol[^3].ToString(); // Get charAt(3) from last index
        var monthAbbr = _monthMapping?.Where(m => m.MonthCode == monthCode).Select(m => m.MonthAbbr).FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrEmpty(monthAbbr))
            throw new ArgumentException($"Invalid month code '{monthCode}' in symbol.", nameof(symbol));

        // Get Year
        var year = symbol[^2..]; // Last 2 charactors is year code

        // Get Precious metal type
        var preciousMetalCode = symbol[..^3]; // From first index to 3rd charactor from last is precious metal description
        preciousMetalCode.TryGetDescription(out var preciousMetal);

        return $"{preciousMetal} {monthAbbr} {year} Futures";
    }

    private async Task SaveMonthMappingsAsync(IEnumerable<SetMonthMapping?> monthMappings)
    {
        if (monthMappings == null)
            throw new ArgumentNullException(nameof(monthMappings));

        var tasks = monthMappings
            .Where(m => m != null) // Filter out null values
            .Select(async m => await _monthMappingService.CreateAsync(m!)); // Non-null assertion after filtering

        await Task.WhenAll(tasks); // Wait for all tasks to complete
    }
}