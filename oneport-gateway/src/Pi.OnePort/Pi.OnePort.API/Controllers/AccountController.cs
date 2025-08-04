using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.OnePort.Db2.Models;
using Pi.OnePort.Db2.Repositories;

namespace Pi.OnePort.API.Controllers;

[ApiController]
[Route("internal/accounts/{accountNo}")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly ISetRepo _setRepo;

    public AccountController(ILogger<AccountController> logger, ISetRepo setRepo)
    {
        _logger = logger;
        _setRepo = setRepo;
    }

    /// <summary>
    /// Get account available
    /// </summary>
    [HttpGet("available", Name = "GetAccountsAvailable")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<AccountAvailable>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAccountsAvailable(string accountNo, [FromQuery] int page = 1)
    {
        try
        {
            var result = await _setRepo.GetAccountsAvailable(accountNo, page);
            return Ok(new ApiResponse<List<AccountAvailable>>(result));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetAccountsAvailable via controller failed");
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message, title: "Get accountss available failed");
        }
    }

    /// <summary>
    /// Get account position
    /// </summary>
    [HttpGet("positions", Name = "GetAccountPosition")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<AccountPosition>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAccountPosition(string accountNo, [FromQuery] int page = 1)
    {
        try
        {
            var result = await _setRepo.GetAccountPositions(accountNo, page);
            return Ok(new ApiResponse<List<AccountPosition>>(result));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetAccountPosition via controller failed");
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message, title: "Get account positions failed");
        }
    }

    /// <summary>
    /// Get credit balance account available
    /// </summary>
    [HttpGet("credit-balance/available", Name = "GetCreditBalanceAccountsAvailable")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<AccountAvailableCreditBalance>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetCreditBalanceAccountsAvailable(string accountNo, [FromQuery] int page = 1)
    {
        try
        {
            var result = await _setRepo.GetCreditBalanceAccountsAvailable(accountNo, page);
            return Ok(new ApiResponse<List<AccountAvailableCreditBalance>>(result));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetCreditBalanceAccountsAvailable via controller failed");
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message, title: "Get credit balance available failed");
        }
    }

    /// <summary>
    /// Get credit balance account position
    /// </summary>
    [HttpGet("credit-balance/positions", Name = "GetAccountCreditBalancePosition")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<AccountPositionCreditBalance>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAccountCreditBalancePosition(string accountNo, [FromQuery] int page = 1)
    {
        try
        {
            var result = await _setRepo.GetCreditBalanceAccountPositions(accountNo, page);
            return Ok(new ApiResponse<List<AccountPositionCreditBalance>>(result));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetAccountCreditBalancePosition via controller failed");
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message, title: "Get credit balance positions failed");
        }
    }
}
