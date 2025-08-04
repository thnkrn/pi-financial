using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.TfexService.API.Filters;
using Pi.TfexService.Application.Queries.Account;
using Pi.TfexService.Application.Utils;

namespace Pi.TfexService.API.Controllers;

public class AccountController(ISetTradeAccountQueries setTradeAccountQueries)
    : ControllerBase
{
    [HttpGet("secure/{accountCode}/account-info")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<AccountInfoDto>))]
    [ServiceFilter(typeof(SecureAuthorizationFilter))]
    public async Task<IActionResult> GetAccountInfo(
        [FromHeader(Name = "user-id")][Required] string userId,
        [Required] string accountCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await setTradeAccountQueries.GetAccountInfo(userId, accountCode, cancellationToken);

            return Ok(new ApiResponse<AccountInfoDto>(resp));
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    [HttpGet("secure/{accountCode}/portfolio")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<PortfolioDto>>))]
    [ServiceFilter(typeof(SecureAuthorizationFilter))]
    public async Task<IActionResult> GetPortfolio(
        [FromHeader(Name = "user-id")][Required] string userId,
        [FromHeader(Name = "sid")] string? sid,
        [Required] string accountCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await setTradeAccountQueries.GetPortfolio(accountCode, sid, cancellationToken);

            return Ok(new ApiResponse<List<PortfolioDto>>(resp));
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    [HttpGet("secure/{userId}/portfolio/summary")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<PortfolioSummaryDto>>))]
    public async Task<IActionResult> GetPortfolioByUserId([Required] string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await setTradeAccountQueries.GetPortfolioByUserId(userId, cancellationToken);

            return Ok(new ApiResponse<List<PortfolioSummaryDto>>(resp));
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    [HttpGet("secure/{accountCode}/series-info/{series}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SeriesInfoDto>))]
    [ServiceFilter(typeof(SecureAuthorizationFilter))]
    public async Task<IActionResult> GetSeriesInfo(
        [FromHeader(Name = "user-id")][Required] string userId,
        [FromHeader(Name = "sid")] string? sid,
        [Required] string accountCode,
        [Required] string series,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await setTradeAccountQueries.GetSeriesInfo(accountCode, sid ?? string.Empty, series, cancellationToken);
            return Ok(new ApiResponse<SeriesInfoDto>(resp));
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    private ObjectResult HandleException(Exception e)
    {
        var errorResponse = ExceptionUtils.HandleException(e);
        return Problem(statusCode: errorResponse.statusCode, detail: errorResponse.detail, title: errorResponse.title);
    }
}