using Microsoft.Extensions.Logging;
using OpenSearch.Client;
using Pi.MarketData.Search.Domain.Models;

namespace Pi.MarketData.Search.Application.Services;

public interface IOrderBookGetterService
{
    Task<string?> GetOrderBookIdBySymbol(string symbol, string venue);
    Task<SearchInstrumentDocument?> GetOrderBookBySymbol(string symbol, string venue);
}

public class OrderBookGetterService : IOrderBookGetterService
{
    private readonly IOpenSearchClient _openSearchClient;
    private readonly ILogger<OrderBookGetterService> _logger;

    public OrderBookGetterService(
        IOpenSearchClient openSearchClient,
        ILogger<OrderBookGetterService> logger)
    {
        _openSearchClient = openSearchClient;
        _logger = logger;
    }

    public async Task<string?> GetOrderBookIdBySymbol(string symbol, string venue)
    {
        _logger.LogDebug("GetOrderBookIdBySymbol: '{Symbol}', '{Venue}'", symbol, venue);
        try
        {
            var response = await _openSearchClient.SearchAsync<SearchInstrumentDocument>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            m => m.Term(t => t.Field(f => f.Symbol).Value(symbol)),
                            m => m.Term(t => t.Field(f => f.Venue).Value(venue))
                        )
                    )
                )
            );

            if (!response.IsValid)
            {
                _logger.LogError("Failed to get order book ID for symbol {Symbol} and venue {Venue}. Error: {Error}",
                    symbol, venue, response.DebugInformation);
                return null;
            }

            _logger.LogDebug("GetOrderBookIdBySymbol Response: {Response}", response.Documents.FirstOrDefault()?.OrderBookId);

            return response.Documents.FirstOrDefault()?.OrderBookId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order book ID for symbol {Symbol} and venue {Venue}", symbol, venue);
            return null;
        }
    }

    public async Task<SearchInstrumentDocument?> GetOrderBookBySymbol(string symbol, string venue)
    {
        _logger.LogDebug("GetOrderBookBySymbol: '{Symbol}', '{Venue}'", symbol, venue);
        try
        {
            var response = await _openSearchClient.SearchAsync<SearchInstrumentDocument>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            m => m.Term(t => t.Field(f => f.Symbol).Value(symbol)),
                            m => m.Term(t => t.Field(f => f.Venue).Value(venue))
                        )
                    )
                )
            );

            if (!response.IsValid)
            {
                _logger.LogError("Failed to get order book for symbol {Symbol} and venue {Venue}. Error: {Error}",
                    symbol, venue, response.DebugInformation);
                return null;
            }

            _logger.LogDebug("GetOrderBookBySymbol Response: {Response}", response.Documents.FirstOrDefault()?.OrderBookId);

            return response.Documents.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order book ID for symbol {Symbol} and venue {Venue}", symbol, venue);
            return null;
        }
    }
}
