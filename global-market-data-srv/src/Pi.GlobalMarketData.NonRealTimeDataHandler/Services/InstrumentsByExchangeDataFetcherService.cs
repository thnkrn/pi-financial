using System.Net.Http.Headers;
using System.Net.Http.Json;
using Pi.GlobalMarketData.Application.Utils;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response.Velexa;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.NonRealTimeDataHandler.constants;
using Pi.GlobalMarketData.NonRealTimeDataHandler.interfaces;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.Services;

public class InstrumentsByExchangeDataFetcherService : BackgroundService
{
    private readonly IGeDataRepository _geDataRepository;
    private readonly HttpClient _httpClient;

    private readonly ILogger<InstrumentsByExchangeDataFetcherService> _logger;
    private readonly int _tokenExpireInSecond = 3600; // 1 hour

    private readonly IVelexaHttpApiJwtTokenGenerator _velexaTokenGenerator;

    private string _token = "";
    private readonly IHostApplicationLifetime _appLifetime;

    public InstrumentsByExchangeDataFetcherService(
        IHttpClientFactory httpClientFactory,
        IGeDataRepository geDataRepository,
        ILogger<InstrumentsByExchangeDataFetcherService> logger,
        IVelexaHttpApiJwtTokenGenerator velexaHttpApiJwtTokenGenerator,
        IHostApplicationLifetime appLifetime
    )
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientKeys.VelexaHttpApi);
        _geDataRepository = geDataRepository;
        _logger = logger;
        _velexaTokenGenerator = velexaHttpApiJwtTokenGenerator;
        _appLifetime = appLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var exchanges = await GetStockExchangesAsync();
            _logger.LogInformation("Got exchange list: {ExchangeList}", string.Join(",", exchanges));

            var updatedAt = DateTime.UtcNow;

            foreach (var exchange in exchanges)
            {
                _logger.LogDebug("Getting symbols of {VenueCode} {Exchange}...", exchange.VenueCode,
                    exchange.Exchange);
                try
                {
                    var instruments = await GetInstrumentsByExchangeAsync(exchange.Exchange);
                    await UpsertInstrumentsAsync(exchange, instruments, updatedAt);
                    _logger.LogDebug("Got {InstrumentCount} instruments, DB updated", instruments.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing exchange {VenueCode} {Exchange}", exchange.VenueCode,
                        exchange.Exchange);
                }
            }
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in main process");
        }
        _appLifetime.StopApplication();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
    }

    private async Task<IEnumerable<GeVenueMapping>> GetStockExchangesAsync()
    {
        try
        {
            var documents = await _geDataRepository.GeVenueMapping.GetAllAsync();
            return documents;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching stock exchanges from MongoDB");
            return Enumerable.Empty<GeVenueMapping>();
        }
    }

    protected async Task<List<VelexaInstrument>> GetInstrumentsByExchangeAsync(string exchange)
    {
        try
        {
            if (_token == "" || _velexaTokenGenerator.IsTokenExpired(_token))
            {
                _logger.LogDebug("Token not found or expired, create new one");
                _token = _velexaTokenGenerator.GenerateJwtToken(_tokenExpireInSecond);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate JWT token");
            return new List<VelexaInstrument>();
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        try
        {
            var requestUrl = $"/md/3.0/exchanges/{exchange}";
            _logger.LogDebug("Endpoint {RequestUrl}", requestUrl);

            var response = await _httpClient.GetFromJsonAsync<List<VelexaInstrument>>(requestUrl);

            if (response == null)
            {
                _logger.LogDebug("No data received for exchange {Exchange}", exchange);
                return new List<VelexaInstrument>();
            }

            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogDebug(ex, "HTTP request error for exchange {Exchange}", exchange);
            return new List<VelexaInstrument>();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error fetching instruments for exchange {Exchange}", exchange);
            return new List<VelexaInstrument>();
        }
    }

    protected async Task UpsertInstrumentsAsync(GeVenueMapping venue, List<VelexaInstrument> instruments,
        DateTime updatedAt)
    {
        try
        {
            foreach (var instrument in instruments)
            {
                var geInstrument = MapToGeInstrument(
                    venue, 
                    instrument,
                    updatedAt
                );

                try
                {
                    await _geDataRepository.GeInstrument.UpsertAsyncByFilter(
                        target => target.Symbol == geInstrument.Symbol && target.Venue == geInstrument.Venue,
                        geInstrument
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Error upserting instrument {SymbolId}", instrument.SymbolId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error upserting instruments into MongoDB");
        }
    }

    protected static GeInstrument MapToGeInstrument(
        GeVenueMapping venue,
        VelexaInstrument instrument,
        DateTime updatedAt
    )
    {
        return new GeInstrument
        {
            // From Velexa
            Symbol = instrument.Ticker,
            Exchange = instrument.Exchange,
            Name = instrument.Name,
            SymbolType = instrument.SymbolType, // Equal to Security type from Morningstar
            Currency = instrument.Currency,
            Country = instrument.Country,
            Figi = instrument.Identifiers?.FIGI,
            Isin = instrument.Identifiers?.ISIN,

            // Wait for Morningstar lookup
            InvestmentType = "",
            StandardTicker = "",
            MorningStarStockStatus = "",
            MorningStarSuspendFlag = "",

            // From Venue mapping collection
            Venue = venue.VenueCode,
            Mic = venue.Mic,
            ExchangeIdMs = venue.ExchangeIdMs,

            // Business defined value
            // All from Velexa will be "GlobalEquity"
            InstrumentType = "GlobalEquity",

            UpdatedAt = updatedAt
        };
    }
}