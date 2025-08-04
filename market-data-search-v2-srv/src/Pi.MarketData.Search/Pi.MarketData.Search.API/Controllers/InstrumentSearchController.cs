using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Pi.MarketData.Search.Application.Services;
using Pi.MarketData.Search.Domain.Models;

namespace Pi.MarketData.Search.API.Controllers;

[ApiController]
[Route("secure/instrument")]
public class InstrumentSearchController : ControllerBase
{
    private readonly IInstrumentSearchService _searchService;
    private readonly IUserFavoriteAndPositionService _userFavoriteAndPositionService;
    private readonly ILogger<InstrumentSearchController> _logger;
    public InstrumentSearchController(
        IInstrumentSearchService searchService,
        IUserFavoriteAndPositionService userFavoriteAndPositionService,
        ILogger<InstrumentSearchController> logger)
    {
        _searchService = searchService;
        _userFavoriteAndPositionService = userFavoriteAndPositionService;
        _logger = logger;
    }

    /// <summary>
    /// Search for instruments based on keyword and optional instrument type
    /// </summary>
    /// <param name="keyword">Search keyword</param>
    /// /// <param name="instrumentType">Optional instrument type filter. One of ["GlobalEquity", "Equity", "Derivative", "MutualFund", "all"]</param>
    /// <param name="useMockWatchList">Optional. If true, use mock watch list</param>
    /// <param name="userId">Required. User ID from 'user-id' header</param>
    /// <param name="ct"></param>
    /// <returns>List of matching instruments</returns>
    /// <response code="200">Returns the list of matching instruments</response>
    /// <response code="400">If the keyword, session ID, or user ID is missing</response>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchInstrumentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchInstruments(
        [FromHeader(Name = "user-id")][Required] string userId,
        [FromQuery] InstrumentType? instrumentType,
        [FromQuery][Required] string keyword,
        [FromQuery] bool useMockWatchList = false,
        CancellationToken ct = default)
    {
        _logger.LogDebug(
            "Searching instruments with keyword: {Keyword}, type: {InstrumentType}",
            keyword,
            instrumentType);

        if (string.IsNullOrEmpty(keyword))
            return BadRequest("Keyword is required");

        if (string.IsNullOrEmpty(userId))
            return BadRequest("User ID is required");

        try
        {
            UserFavoriteResponse? favoritesResult = null;
            if (userId != null)
            {
                try
                {
                    favoritesResult = await _userFavoriteAndPositionService.GetPiWatchListAsync(userId, useMockWatchList);
                    _logger.LogInformation("Favorites result: {FavoritesResult}", favoritesResult);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while getting user favorites. User ID: {UserId}", userId);
                }
            }
            var searchResults = await _searchService.SearchInstrumentsAndEnhanceWithFavoritesAndStreamingDataAsync(keyword, instrumentType, favoritesResult, ct);
            return Ok(searchResults);

            // TODO: 
            // - To manipulate the search results from OpenSearch with Mutual Fund
            // - By adding the Mutual Fund data as another category in the search response
            // Example:
            // searchResults.data.Add(new SearchInstrumentGroupResponse
            // {
            //     Type = "MutualFund",
            //     Category = "Mutual Funds",
            //     Order = 0,
            //     Instruments = new List<SearchInstrumentItemResponse>() 
            // });
            // 
            // Example of the search response:
            // {
            //     "data": [
            //         {
            //             "type": "GlobalEquity",
            //             "category": "Global Stocks",
            //             "order": 0,
            //             "instruments": [
            //                 {
            //                     "venue": "NASDAQ",
            //                     "symbol": "AAPL",
            //                     "name": "Apple",
            //                     "friendlyName": null,
            //                     "logo": "https://d34vubfbkpay0j.cloudfront.net/NASDAQ/AAPL.svg",
            //                     "price": "252.45",
            //                     "priceChange": "0.25",
            //                     "priceChangeRatio": "0.10",
            //                     "isFavorite": false,
            //                     "unit": "USD",
            //                     "type": "GlobalEquity",
            //                     "category": "Global Stocks",
            //                     "orderBookId": ""
            //                 }
            //             ]
            //         },
            //         {
            //             "type": "Equity",
            //             "category": "Thai Stocks",
            //             "order": 0,
            //             "instruments": [
            //                 {
            //                     "venue": "SET",
            //                     "symbol": "CPALL",
            //                     "name": "CPALL_CP ALL",
            //                     "friendlyName": "CPALL_CP ALL",
            //                     "logo": "https://d34vubfbkpay0j.cloudfront.net/SET/CPALL.svg",
            //                     "price": "57.50",
            //                     "priceChange": "0.00",
            //                     "priceChangeRatio": "0.00",
            //                     "isFavorite": false,
            //                     "unit": "THB",
            //                     "type": "Equity",
            //                     "category": "Thai Stocks",
            //                     "orderBookId": "65804"
            //                 }
            //             ]
            //         }
            //     ]
            // }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching instruments. Keyword: {Keyword}, Type: {InstrumentType}",
                keyword,
                instrumentType);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching instruments");
        }
    }
}

