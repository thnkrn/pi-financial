using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.SetMarketData.API.Infrastructure.Services;
using Pi.SetMarketData.Application.Commands;
using Pi.SetMarketData.Application.Helper;
using Pi.SetMarketData.Application.Queries;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Pi.SetMarketData.API.Controllers.MarketDataManagement;

[ApiController]
[Route("market-data-management/curated-filters")]
public class CuratedFilterController : ControllerBase
{
    private readonly IControllerRequestHelper _requestHelper;
    private readonly IFileUtils _fileUtils;

    /// <summary>
    /// </summary>
    /// <param name="requestHelper"></param>
    /// <param name="fileUtils"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CuratedFilterController
    (
        IControllerRequestHelper requestHelper, 
        IFileUtils fileUtils
    )
    {
        _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
        _fileUtils = fileUtils ?? throw new ArgumentNullException(nameof(fileUtils));
    }

    [HttpGet(Name = "GetAllCuratedFilter")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetCuratedFilterResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllCuratedFilter(
        [FromQuery] string? groupName,
        [FromQuery] string? subGroupName
    )
    {
        var response = await _requestHelper.ExecuteMarketDataManagementRequest<GetCuratedFilterRequest, GetCuratedFilterResponse>(
            new GetCuratedFilterRequest(groupName, subGroupName),
            _ => ModelState.IsValid
        );

        return Ok(response);
    }

    [HttpGet("{id}", Name = "GetCuratedFilterById")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetByIdCuratedFilterResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetCuratedFilterById([FromRoute] string id)
    {
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<GetByIdCuratedFilterRequest, GetByIdCuratedFilterResponse>(
                new GetByIdCuratedFilterRequest(id),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpPost(Name = "CreateCuratedFilter")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CreateCuratedFilterResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateCuratedFilter(IFormFile file)
    {
        var curatedFilter = CsvHelperExtensions.ReadCsvData<CuratedFilter>(file.OpenReadStream());
        _fileUtils.SaveFile(file);
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<CreateCuratedFilterRequest, CreateCuratedFilterResponse>(
                new CreateCuratedFilterRequest(curatedFilter),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpPatch("{id}", Name = "UpdateCuratedFilter")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UpdateCuratedFilterResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateCuratedFilter([FromRoute] string id, [FromBody] CuratedFilter curatedFilter)
    {
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<UpdateCuratedFilterRequest, UpdateCuratedFilterResponse>(
                new UpdateCuratedFilterRequest(id, curatedFilter),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpDelete("{id}", Name = "DeleteCuratedFilter")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<DeleteCuratedFilterResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteCuratedFilter([FromRoute] string id)
    {
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<DeleteCuratedFilterRequest, DeleteCuratedFilterResponse>(
                new DeleteCuratedFilterRequest(id),
                _ => ModelState.IsValid
            );

        if (!((DeleteCuratedFilterResponse)response).Success)
        {
            return ((DeleteCuratedFilterResponse)response).Message switch
            {
                "BadRequest" => BadRequest(new ProblemDetails { Status = 400, Title = "Invalid ID format" }),
                "NotFound" => NotFound(new ProblemDetails { Status = 404, Title = "Curated filter not found" }),
                _ => StatusCode(500, new ProblemDetails { Status = 500, Title = "Internal server error" }),
            };
        }

        return Ok(response);
    }
}