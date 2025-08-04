using Pi.GlobalMarketData.Application.Constants;
using Pi.GlobalMarketData.Application.Utils;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Request;
using Pi.GlobalMarketData.Domain.Models.Response;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Services.Logging.Extensions;
using Pi.GlobalMarketData.NonRealTimeDataHandler.constants;
using Pi.GlobalMarketData.NonRealTimeDataHandler.Helpers;
using Pi.GlobalMarketData.NonRealTimeDataHandler.interfaces;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.Services;

public class GEInstrumentDataUpdaterService : BackgroundService
{
    private readonly string? _email;
    private readonly IMongoService<FullStockExchangeSecurityEntity> _fullStockExchangeSecurityEntityMongoService;
    private readonly IMongoService<MorningStarFlag> _morningStarFlagService;
    private readonly IGeDataRepository _geDataRepository;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GEInstrumentDataUpdaterService> _logger;
    private readonly string? _password;
    private readonly IHostApplicationLifetime _appLifetime;

    public GEInstrumentDataUpdaterService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IGeDataRepository geDataRepository,
        IMongoService<FullStockExchangeSecurityEntity> fullStockExchangeSecurityListMongoService,
        IMongoService<MorningStarFlag> morningStarFlagService,
        IHostApplicationLifetime appLifetime,
        ILogger<GEInstrumentDataUpdaterService> logger
    )
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientKeys.MorningStar);
        _email = configuration.GetValue<string>(ConfigurationKeys.MorningStarEmail);
        _password = configuration.GetValue<string>(ConfigurationKeys.MorningStarPassword);
        _geDataRepository = geDataRepository;
        _fullStockExchangeSecurityEntityMongoService = fullStockExchangeSecurityListMongoService;
        _morningStarFlagService = morningStarFlagService;
        _appLifetime = appLifetime;

        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        HttpRequestHelper<GEInstrumentDataUpdaterService> httpRequestHelper =
            new(_httpClient, _logger);

        MorningStarDataHelper morningStarHelper = new(httpRequestHelper);

        try
        {
            var GEVenue = await _geDataRepository.GeVenueMapping.GetAllAsync();
            List<MorningStarStockRequest> request = [];

            foreach (var instrument in GEVenue)
            {
                if (instrument == null)
                    continue;

                request.Add(
                    new MorningStarStockRequest
                    {
                        ExchangeId = instrument.ExchangeIdMs ?? string.Empty,
                        Identifier = instrument.ExchangeIdMs ?? string.Empty
                    }
                );
            }

            await CallStockExchangeSecurityList(morningStarHelper, request);
            await MorningStarService();
            await WhitelistUpdater();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during execution.");
        }
        _logger.LogDebug("GEInstrumentDataUpdater job done/");
        _appLifetime.StopApplication();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
    }

    public async Task CallStockExchangeSecurityList(
        MorningStarDataHelper helper,
        List<MorningStarStockRequest> requests
    )
    {
        var tokenEntity = await helper.Login(_email ?? "", _password ?? "");

        if (tokenEntity is { IsSuccess: true })
        {
            _logger.LogDebug(
                "Processing {Count} stock exchange security requests.",
                requests.Count
            );

            for (var index = 0; index < requests.Count; index++)
            {
                var response = await helper.GetStockExchangeSecurityList(requests[index], null);

                _logger.LogDebug(
                    "Stock Exchange List ------------------------> {Identifier}",
                    requests[index].Identifier
                );

                if (response == null)
                    continue;
                if (response.FullStockExchangeSecurityEntityList == null)
                    continue;

                foreach (var item in response.FullStockExchangeSecurityEntityList!)
                    await _fullStockExchangeSecurityEntityMongoService.UpsertAsyncByFilter(
                        target =>
                            target.ExchangeId == item.ExchangeId && target.Symbol == item.Symbol,
                        item
                    );
            }
        }
    }

    private async Task MorningStarService()
    {
        var instruments = await _geDataRepository.GeInstrument.GetAllAsync();
        var geVenueMappings = await _geDataRepository.GeVenueMapping.GetAllAsync();
        var morningStarStocks = await _geDataRepository.MorningStarStocks.GetAllAsync();
        var morningStarEtfs = await _geDataRepository.MorningStarEtfs.GetAllAsync();

        var symbols = instruments.Select(instrument => instrument.Symbol).ToHashSet();
        var isins = instruments.Select(instrument => instrument.Isin).ToHashSet();

        // Get all stock exchanges matching any symbol in a single query
        var stockExchangesBySymbol = await _fullStockExchangeSecurityEntityMongoService.GetAllByFilterAsync(target =>
            symbols.Contains(target.Symbol)
        );

        // Get all stock exchanges matching any ISIN in a single query (for fallback)
        var stockExchangesByIsin = await _fullStockExchangeSecurityEntityMongoService.GetAllByFilterAsync(target =>
            isins.Contains(target.Isin)
        );

        var symbolLookup = stockExchangesBySymbol
            .Where(se => se.Symbol != null)
            .GroupBy(se => se.Symbol!)
            .ToDictionary(g => g.Key, g => g.First());
        var isinLookup = stockExchangesByIsin
            .Where(se => se.Symbol != null)
            .GroupBy(se => se.Symbol!)
            .ToDictionary(g => g.Key, g => g.First());

        foreach (var instrument in instruments)
        {
            instrument.StandardTicker = instrument.Symbol;
            var stockExchange =
                (instrument.Symbol != null ? symbolLookup.GetValueOrDefault(instrument.Symbol) : null) ??
                (instrument.Isin != null ? isinLookup.GetValueOrDefault(instrument.Isin) : null);

            var instrumentSymbol = DataManipulation.ToMorningStarSymbol(
                instrument.Symbol,
                instrument.Exchange
            );

            if (stockExchange != null)
            {
                instrument.InvestmentType = stockExchange.InvestmentTypeId;
                instrument.StandardTicker = stockExchange.Symbol;
                instrument.MorningStarStockStatus = stockExchange.StockStatus;
                instrument.MorningStarSuspendFlag = stockExchange.SuspendedFlag;
            }

            var geVenueMapping = geVenueMappings.FirstOrDefault(target => target.VenueCode == instrument.Venue)
                ?? new GeVenueMapping();

            var morningStarStock = morningStarStocks?.FirstOrDefault(target =>
                string.Equals(target.Symbol, instrumentSymbol, StringComparison.OrdinalIgnoreCase)
                && target.ExchangeId.Equals(geVenueMapping.ExchangeIdMs, StringComparison.OrdinalIgnoreCase)
            );
            var morningStarEtf = morningStarEtfs?.FirstOrDefault(target =>
                string.Equals(target.Symbol, instrumentSymbol, StringComparison.OrdinalIgnoreCase)
                && target.ExchangeId.Equals(geVenueMapping.ExchangeIdMs, StringComparison.OrdinalIgnoreCase)
            );

            if ((instrument.InvestmentType == InvestmentType.CE.Value) || (morningStarEtf != null))
                instrument.InstrumentCategory = StockDetail.ETFs.Value;
            else if ((instrument.InvestmentType == InvestmentType.EQ.Value) || (morningStarStock != null))
                instrument.InstrumentCategory = StockDetail.Stock.Value;
        }
        await _geDataRepository.GeInstrument.UpdateManyAsync(instruments, x => x.Id);
    }

    private async Task WhitelistUpdater()
    {
        var instruments = await _geDataRepository.GeInstrument.GetAllAsync();
        var _instruments = instruments.ToList();

        _logger.LogDebug("Updated or inserted {Count} whitelist entries.", _instruments.Count);

        foreach (var instrument in _instruments)
        {
            var whitelistDB = await _geDataRepository.Whitelist.GetByFilterAsync(target =>
                target.Symbol == instrument.Symbol
                && target.Exchange == instrument.Exchange
                && target.Mic == instrument.Mic
            );

            var morningStarFlag = new MorningStarFlag
            {
                Isin = instrument.Isin,
                IsSubscribe = whitelistDB?.IsWhitelist ?? false,
                ExchangeId = instrument.ExchangeIdMs,
                StandardTicker = instrument.StandardTicker,
                StatementType = MorningStarStatementType.Quarterly.Value,
                StartDate = DateTime.Now.AddYears(-2).ToString(DataFormat.DateTimeFormat),
                EndDate = DateTime.Now.ToString(DataFormat.DateTimeFormat),
                ExcludingFrom = DateTime.Now.AddYears(-2).ToString(DataFormat.DateTimeFormat),
                ExcludingTo = DateTime.Now.ToString(DataFormat.DateTimeFormat),
                InstrumentCategory = instrument.InstrumentCategory,
            };

            try
            {
                await _morningStarFlagService.UpsertAsyncByFilter(
                target => target.StandardTicker == instrument.StandardTicker,
                morningStarFlag
            );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occur with with Isin {Isin}", morningStarFlag.Isin);
            }


            if (whitelistDB != null)
                continue;

            var whitelist = new WhiteList
            {
                Symbol = instrument.Symbol,
                Exchange = instrument.Exchange,
                Mic = instrument.Mic,
                StandardTicker = instrument.StandardTicker
            };

            await _geDataRepository.Whitelist.UpsertAsyncByFilter(
                target =>
                    target.Symbol == instrument.Symbol && target.Exchange == instrument.Exchange,
                whitelist
            );
        }
    }
}
