using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.User.API.Models;
using Pi.User.Application.Models;
using Pi.User.Application.Queries;

namespace Pi.User.API.Controllers;

[ApiController]
public class UserTradingAccountController : ControllerBase
{
    private readonly IUserTradingAccountQueries _userTradingAccountQueries;

    public UserTradingAccountController(IUserTradingAccountQueries userTradingAccountQueries)
    {
        _userTradingAccountQueries = userTradingAccountQueries;
    }

    [HttpGet("internal/user/trading-accounts", Name = "GetTradingAccountsByCustomerCode")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<string>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<ApiResponse<IEnumerable<string>>>> GetTradingAccountsByCustomerCode(
        [FromQuery(Name = "custCode")] [Required]
        string custCode)
    {
        try
        {
            var tradingAccounts = await _userTradingAccountQueries.GetTradingAccountNoListAsync(custCode);
            return Ok(new ApiResponse<IEnumerable<string>>(tradingAccounts));
        }
        catch (InvalidDataException e)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: ErrorCodes.Usr0001.ToString().ToUpper(),
                detail: e.Message);
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: e.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserTradingAccountInfo>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [HttpGet("internal/trading-accounts/v2", Name = "GetUserTradingAccountInfoByUserId")]
    public async Task<IActionResult> InternalGetTradingAccountInfo([FromQuery(Name = "userId")] Guid userId,
        [FromQuery(Name = "customerCode")] string customerCode, CancellationToken cancellationToken)
    {
        try
        {
            var userTradingAccountInfo =
                await _userTradingAccountQueries.GetUserTradingAccountInfoAsync(userId, customerCode,
                    cancellationToken);
            if (userTradingAccountInfo is null)
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: ErrorCodes.Usr0001.ToString().ToUpper(),
                    detail:
                    $"User trading account info not found for user id: {userId} and customer code: {customerCode}");

            return Ok(new ApiResponse<UserTradingAccountInfo>(userTradingAccountInfo));
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: e.Message);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserTradingAccountInfo>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [HttpGet("secure/trading-accounts/v2", Name = "GetUserTradingAccountInfoByCustomerId")]
    public async Task<IActionResult> SecureGetTradingAccountInfo([FromHeader(Name = "user-id")] Guid userId,
        [FromQuery(Name = "customerCode")] string customerCode, CancellationToken cancellationToken)
    {
        return await InternalGetTradingAccountInfo(userId, customerCode, cancellationToken);
    }

    /// <summary>
    /// Returns list of customer code and hasPin flag belonging to <paramref name="userId"/>, grouped by customer code.
    /// </summary>
    /// <param name="userId">Guid user id to query trading accounts for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of trading accounts, grouped by customer code.</returns>
    [HttpGet("internal/check-has-pin", Name = "InternalGetCheckHasPin")]
    [HttpGet("secure/check-has-pin", Name = "SecureGetGetCheckHasPin")]
    [ProducesResponseType(StatusCodes.Status200OK,
        Type = typeof(ApiResponse<List<CustomerCodeHasPin>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> CheckHasPin(
        [FromHeader(Name = "user-id")] [Required]
        Guid userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var checkHasPinList = await _userTradingAccountQueries.CheckHasPin(userId, cancellationToken);

            return Ok(new ApiResponse<List<CustomerCodeHasPin>>(checkHasPinList));
        }
        catch (InvalidDataException e)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: e.Message);
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: e.Message);
        }
    }
}