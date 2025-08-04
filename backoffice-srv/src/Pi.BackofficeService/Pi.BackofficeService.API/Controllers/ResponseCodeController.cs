using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Pi.BackofficeService.API.Factories;
using Pi.BackofficeService.API.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Application.Queries.Filters;
using Pi.Common.Http;

namespace Pi.BackofficeService.API.Controllers;

[ApiController]
[Route("response_codes")]
public class ResponseCodeController : ControllerBase
{
    private readonly IBackofficeQueries _backofficeQueries;

    public ResponseCodeController(IBackofficeQueries backofficeQueries)
    {
        _backofficeQueries = backofficeQueries;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ResponseCodeResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> ResponseCode([FromQuery] ResponseCodeFilter filter)
    {
        var records = await _backofficeQueries.GetResponseCodes(filter);
        var responseCodeResponses = records.Select(DtoFactory.NewResponseCodeResponse);

        return Ok(new ApiResponse<List<ResponseCodeResponse>>(responseCodeResponses.ToList()));
    }

    [HttpGet("{responseCodeId}/actions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ResponseCodeActionsResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> ResponseCodeActions([Required] Guid responseCodeId)
    {
        var records = await _backofficeQueries.GetResponseCodeAction(responseCodeId);

        if (records.IsNullOrEmpty()) return NotFound();

        var responseCodeResponses = records.Select(DtoFactory.NewResponseCodeActionsResponse);

        return Ok(new ApiResponse<List<ResponseCodeActionsResponse>>(responseCodeResponses.ToList()));
    }
}
