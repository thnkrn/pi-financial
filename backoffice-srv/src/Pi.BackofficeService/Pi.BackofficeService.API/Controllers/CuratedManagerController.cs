using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Models.CuratedManager;
using Pi.BackofficeService.Application.Services.CuratedManagerService;
using Pi.Common.Http;

namespace Pi.BackofficeService.API.Controllers;
[Authorize(Policy = "CuratedManagerAccess")]
[ApiController]
[Route("curated-manager")]
public class CuratedManagerController : ControllerBase
{
    private readonly ICuratedManagerService _curatedManagerService;

    public CuratedManagerController(ICuratedManagerService curatedManagerService)
    {
        _curatedManagerService = curatedManagerService;
    }

    private string? GetAuthorizationToken()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authHeader))
            return null;

        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return null;

        var token = authHeader.Replace("Bearer ", "");
        return string.IsNullOrWhiteSpace(token) ? null : token;
    }


    [HttpGet("list")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CuratedListResponse>))]
    public async Task<IActionResult> GetCuratedList()
    {
        try
        {
            var authToken = GetAuthorizationToken();
            if (authToken == null)
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    detail: "Unauthorized"
                );

            var result = await _curatedManagerService.GetCuratedList(authToken);
            return Ok(new ApiResponse<CuratedListResponse>(result));
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: e.Message ?? "An unexpected error occurred"
            );
        }
    }

    [HttpPost("list")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<CuratedListResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadCuratedManualListCSV(IFormFile file, [FromQuery] string dataSource)
    {
        if (file == null)
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "File is required"
            );

        if (!file.ContentType.Equals("text/csv") && !file.ContentType.Equals("application/csv") &&
            !file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "Only CSV files are allowed"
            );

        if (string.IsNullOrWhiteSpace(dataSource))
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "dataSource is required query parameter"
            );

        if (!Enum.TryParse<CuratedListSource>(dataSource, out _))
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "dataSource must be either 'SET' or 'GE'"
            );

        try
        {
            var authToken = GetAuthorizationToken();
            if (authToken == null)
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    detail: "Unauthorized"
                );

            var result = await _curatedManagerService.UploadCuratedManualList(authToken, file, dataSource);

            return Created("", new ApiResponse<CuratedListResponse>(result));
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: e.Message ?? "An unexpected error occurred"
            );
        }
    }

    [HttpPatch("list/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CuratedListItem>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCuratedListById(string id, [FromBody] CuratedListUpdateRequest curatedListUpdateRequest, [FromQuery] string dataSource)
    {
        if (curatedListUpdateRequest == null)
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "Request body is required"
            );

        if (string.IsNullOrWhiteSpace(curatedListUpdateRequest.Name) && string.IsNullOrWhiteSpace(curatedListUpdateRequest.Hashtag))
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "Either name or hashtag must be provided"
            );

        if (string.IsNullOrWhiteSpace(dataSource))
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "dataSource is required query parameter"
            );

        if (!Enum.TryParse<CuratedListSource>(dataSource, out _))
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "dataSource must be either 'SET' or 'GE'"
            );

        try
        {
            var authToken = GetAuthorizationToken();
            if (authToken == null)
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    detail: "Unauthorized"
                );

            var result = await _curatedManagerService.UpdateCuratedListById(authToken, id, curatedListUpdateRequest.Name, curatedListUpdateRequest.Hashtag, dataSource);
            return Ok(new ApiResponse<CuratedListItem>(result));
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: e.Message ?? "An unexpected error occurred"
            );
        }
    }

    [HttpDelete("list/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCuratedListById(string id, [FromQuery] string dataSource)
    {
        if (string.IsNullOrWhiteSpace(dataSource))
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "dataSource is required query parameter"
            );

        if (!Enum.TryParse<CuratedListSource>(dataSource, out _))
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "dataSource must be either 'SET' or 'GE'"
            );

        try
        {
            var authToken = GetAuthorizationToken();
            if (authToken == null)
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    detail: "Unauthorized"
                );

            await _curatedManagerService.DeleteCuratedListById(authToken, id, dataSource);

            return NoContent();
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: e.Message ?? "An unexpected error occurred"
            );
        }
    }

    [HttpGet("filters")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<CuratedFilterGroup>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ResponseCache(Duration = 30)]
    public async Task<IActionResult> GetCuratedFilters([FromQuery] string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "groupName is required parameter"
            );

        try
        {
            var authToken = GetAuthorizationToken();
            if (authToken == null)
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    detail: "Unauthorized"
                );

            var groupNameUnescaped = Uri.UnescapeDataString(groupName);

            var result = await _curatedManagerService.GetCuratedFilters(authToken, groupNameUnescaped);

            return Ok(new ApiResponse<List<CuratedFilterGroup>>(result));
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: e.Message ?? "An unexpected error occurred"
            );
        }
    }

    [HttpPost("filters")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<List<CuratedFilterGroup>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadCuratedFiltersCSV(IFormFile file, [FromQuery] string dataSource)
    {
        if (file == null)
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "File is required"
            );

        if (!file.ContentType.Equals("text/csv") && !file.ContentType.Equals("application/csv") &&
            !file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "Only CSV files are allowed"
            );

        if (string.IsNullOrWhiteSpace(dataSource))
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "dataSource is required query parameter"
            );

        if (!Enum.TryParse<CuratedListSource>(dataSource, out _))
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "dataSource must be either 'SET' or 'GE'"
            );

        try
        {
            var authToken = GetAuthorizationToken();
            if (authToken == null)
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    detail: "Unauthorized"
                );

            var result = await _curatedManagerService.UploadCuratedFilters(authToken, file, dataSource);

            return Created("", new ApiResponse<List<CuratedFilterGroup>>(result));
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: e.Message ?? "An unexpected error occurred"
            );
        }
    }

    [HttpGet("members/{curatedListId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<TransformedCuratedMemberItem>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ResponseCache(Duration = 30)]
    public async Task<IActionResult> GetCuratedMembersByCuratedListId(string curatedListId)
    {
        try
        {
            var authToken = GetAuthorizationToken();
            if (authToken == null)
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    detail: "Unauthorized"
                );

            var result = await _curatedManagerService.GetCuratedMembersByCuratedListId(authToken, curatedListId);
            return Ok(new ApiResponse<List<TransformedCuratedMemberItem>>(result));
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: e.Message ?? "An unexpected error occurred"
            );
        }
    }
}