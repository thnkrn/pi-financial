using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Pi.MarketData.Search.API.Interfaces;
using Pi.MarketData.Search.Application.Services;
using Pi.MarketData.Search.Domain.Models;
using Pi.MarketData.Search.Domain.Models.Responses;
using Pi.MarketData.Search.Infrastructure.Interfaces.Redis;

namespace Pi.MarketData.Search.API.Controllers;

[ApiController]
[Route("/cgs/v2/user")]
public class UserFavoriteAndPositionController : ControllerBase
{
    private readonly IUserFavoriteAndPositionService _userFavoriteAndPositionService;
    private readonly ICacheService _cacheService;
    private readonly IOrderBookIdMapperService _orderBookIdMapperService;
    public UserFavoriteAndPositionController
    (
        IUserFavoriteAndPositionService userFavoriteAndPositionService,
        ICacheService cacheService,
        IOrderBookIdMapperService orderBookIdMapperService
    )
    {
        _userFavoriteAndPositionService = userFavoriteAndPositionService;
        _cacheService = cacheService;
        _orderBookIdMapperService = orderBookIdMapperService;
    }

    [HttpPost("instrument/favourite")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserInstrumentFavoriteResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserFavorites(
        [FromHeader(Name = "user-id")][Required] string userId,
        [FromQuery] bool useMockWatchList = false,
        CancellationToken ct = default)
    {
        var favorites = await _userFavoriteAndPositionService.GetUserFavoritesAsyncAndEnhanceWithStreamingData(userId, useMockWatchList, ct);
        var (setKeys, geKeys, symbolMap) = await _orderBookIdMapperService.MapCacheKeysFromSymbols(favorites);
        var setPriceResponse = _cacheService.GetBatchAsync<PriceResponse>(setKeys);
        var gePriceResponse = _cacheService.GetBatchAsync<PriceResponse>(geKeys);
        Task.WaitAll(setPriceResponse, gePriceResponse);
        favorites = _userFavoriteAndPositionService.MapPrice(favorites, Merge(await setPriceResponse, await gePriceResponse), symbolMap!);
        return Ok(favorites);
    }

    [HttpGet("position/market")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserInstrumentFavoriteResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Obsolete]
    public async Task<IActionResult> GetUserPositions(
        [FromHeader(Name = "user-id")][Required] string userId,
        [FromQuery] bool useMockWatchList = false)
    {
        var positions = await _userFavoriteAndPositionService.GetUserPositionsAsyncAndEnhanceWithStreamingData(userId);
        return Ok(positions);
    }

    public static IDictionary<TKey, TValue> Merge<TKey, TValue>(
        IDictionary<TKey, TValue> first,
        IDictionary<TKey, TValue> second)
        where TKey : notnull
    {
        if (first == null) throw new ArgumentNullException(nameof(first));
        if (second == null) throw new ArgumentNullException(nameof(second));

        // Create a new dictionary with the contents of the first dictionary
        var result = new Dictionary<TKey, TValue>(first);

        // Add or update entries from the second dictionary
        foreach (var kvp in second)
        {
            result[kvp.Key] = kvp.Value;
        }

        return result;
    }
}

