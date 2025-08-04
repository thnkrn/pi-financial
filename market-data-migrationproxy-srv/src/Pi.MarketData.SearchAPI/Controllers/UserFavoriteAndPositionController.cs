using Microsoft.AspNetCore.Mvc;
using Pi.MarketData.SearchAPI.Services;
using Pi.MarketData.Domain.Models;
namespace Pi.MarketData.SearchAPI.Controllers;

[ApiController]
[Route("secure/user")]
public class UserFavoriteAndPositionController : ControllerBase
{
    private readonly IUserFavoriteAndPositionService _userFavoriteAndPositionService;
    public UserFavoriteAndPositionController(
        IUserFavoriteAndPositionService userFavoriteAndPositionService)
    {
        _userFavoriteAndPositionService = userFavoriteAndPositionService;
    }


    [HttpGet("favorite")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserFavorites(
        [FromHeader(Name = "sid")] string sessionId,
        [FromHeader(Name = "user-id")] string userId,
        [FromQuery] bool useMockWatchList = false)
    {
        var favorites = await _userFavoriteAndPositionService.GetUserFavoritesAsyncAndEnhanceWithStreamingData(sessionId, useMockWatchList);
        return Ok(favorites);
    }

    [HttpGet("position")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserPositions(
        [FromHeader(Name = "sid")] string sessionId,
        [FromHeader(Name = "user-id")] string userId,
        [FromQuery] bool useMockWatchList = false)
    {
        var positions = await _userFavoriteAndPositionService.GetUserPositionsAsyncAndEnhanceWithStreamingData(sessionId, useMockWatchList);
        return Ok(positions);
    }
}

