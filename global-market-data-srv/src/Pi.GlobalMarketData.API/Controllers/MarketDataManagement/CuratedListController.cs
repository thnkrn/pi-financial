using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.GlobalMarketData.API.Infrastructure.Services;
using Pi.GlobalMarketData.API.Validator;
using Pi.GlobalMarketData.Application.Commands;
using Pi.GlobalMarketData.Application.Helper;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Pi.GlobalMarketData.API.Controllers.MarketDataManagement;

[ApiController]
[Route("market-data-management/curated-lists")]
public class CuratedListController : ControllerBase
{
    private readonly IControllerRequestHelper _requestHelper;
    private readonly IFileUtils _fileUtils;

    /// <summary>
    /// </summary>
    /// <param name="requestHelper"></param>
    /// <param name="fileUtils"></param>
    public CuratedListController
    (
        IControllerRequestHelper requestHelper,
        IFileUtils fileUtils
    )
    {
        _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
        _fileUtils = fileUtils ?? throw new ArgumentNullException(nameof(fileUtils));
    }

    [HttpGet]
    [SwaggerOperation(Tags = new[] { "MarketDataManagement" })]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<CuratedList>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllCuratedList()
    {
        var response = await _requestHelper.ExecuteMarketDataManagementRequest<
            GetCuratedListRequest,
            GetCuratedListResponse
        >(new GetCuratedListRequest(), _ => ModelState.IsValid);

        return Ok(response);
    }

    [HttpGet("{id}", Name = "GetCuratedListById")]
    [SwaggerOperation(Tags = new[] { "MarketDataManagement" })]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CuratedList>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetCuratedListById([FromRoute] string id)
    {
        var response = await _requestHelper.ExecuteMarketDataManagementRequest<
            GetByIdCuratedListRequest,
            GetByIdCuratedListResponse
        >(new GetByIdCuratedListRequest(id), _ => ModelState.IsValid);

        return Ok(response);
    }

    [HttpPost(Name = "CreateCuratedList")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ValidateCsvFile(allowedExtensions: [".csv"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CreateCuratedListResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateCuratedList(IFormFile file)
    {
        var curatedList = CsvHelperExtensions.ReadCsvData<CuratedList>(file.OpenReadStream());
        if (curatedList == null || !curatedList.Any())
        {
            return BadRequest(new ProblemDetails { Status = 400, Title = "No valid data found in the CSV file" });
        }
        _fileUtils.SaveFile(file);
        var response = await _requestHelper.ExecuteMarketDataManagementRequest<CreateCuratedListRequest, CreateCuratedListResponse>(
            new CreateCuratedListRequest(curatedList),
            _ => ModelState.IsValid
        );

        return Ok(response);
    }

    [HttpPatch("{id}", Name = "UpdateCuratedList")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UpdateCuratedListResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateCuratedList([FromRoute] string id, [FromBody] CuratedList CuratedList)
    {
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<UpdateCuratedListRequest, UpdateCuratedListResponse>(
                new UpdateCuratedListRequest(id, CuratedList),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpDelete("{id}", Name = "DeleteCuratedList")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<DeleteCuratedListResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteCuratedList([FromRoute] string id)
    {
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<DeleteCuratedListRequest, DeleteCuratedListResponse>(
                new DeleteCuratedListRequest(id),
                _ => ModelState.IsValid
            );

        if (!((DeleteCuratedListResponse)response).Success)
        {
            return ((DeleteCuratedListResponse)response).Message switch
            {
                "BadRequest" => BadRequest(new ProblemDetails { Status = 400, Title = "Invalid ID format" }),
                "NotFound" => NotFound(new ProblemDetails { Status = 404, Title = "Curated list not found" }),
                _ => StatusCode(500, new ProblemDetails { Status = 500, Title = "Internal server error" }),
            };
        }

        return Ok(response);
    }
}
