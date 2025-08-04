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
[Route("market-data-management/curated-members")]
public class CuratedMemberController : ControllerBase
{
    private readonly IControllerRequestHelper _requestHelper;
    private readonly IFileUtils _fileUtils;

    /// <summary>
    /// </summary>
    /// <param name="requestHelper"></param>
    /// <param name="fileUtils"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CuratedMemberController
    (
        IControllerRequestHelper requestHelper,
        IFileUtils fileUtils
    )
    {
        _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
        _fileUtils = fileUtils ?? throw new ArgumentNullException(nameof(fileUtils));
    }

    [HttpGet(Name = "GetAllCuratedMember")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetCuratedMemberResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllCuratedMember(
        [FromQuery] int? curatedListId
    )
    {
        var response = await _requestHelper.ExecuteMarketDataManagementRequest<GetCuratedMemberRequest, GetCuratedMemberResponse>(
            new GetCuratedMemberRequest(curatedListId),
            _ => ModelState.IsValid
        );

        return Ok(response);
    }

    [HttpGet("{id}", Name = "GetCuratedMemberById")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetByIdCuratedMemberResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetCuratedMemberById([FromRoute] string id)
    {
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<GetByIdCuratedMemberRequest, GetByIdCuratedMemberResponse>(
                new GetByIdCuratedMemberRequest(id),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpPost(Name = "CreateCuratedMember")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ValidateCsvFile(allowedExtensions: [".csv"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CreateCuratedMemberResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateCuratedMember(IFormFile file)
    {
        var curatedMember = CsvHelperExtensions.ReadCsvData<CuratedMember>(file.OpenReadStream());
        if (curatedMember == null || !curatedMember.Any())
        {
            return BadRequest(new ProblemDetails { Status = 400, Title = "No valid data found in the CSV file" });
        }
        _fileUtils.SaveFile(file);
        var response = await _requestHelper.ExecuteMarketDataManagementRequest<CreateCuratedMemberRequest, CreateCuratedMemberResponse>(
            new CreateCuratedMemberRequest(curatedMember),
            _ => ModelState.IsValid
        );

        return Ok(response);
    }

    [HttpPatch("{id}", Name = "UpdateCuratedMember")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UpdateCuratedMemberResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateCuratedMember([FromRoute] string id, [FromBody] CuratedMember CuratedMember)
    {
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<UpdateCuratedMemberRequest, UpdateCuratedMemberResponse>(
                new UpdateCuratedMemberRequest(id, CuratedMember),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpDelete("{id}", Name = "DeleteCuratedMember")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<DeleteCuratedMemberResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteCuratedMember([FromRoute] string id)
    {
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<DeleteCuratedMemberRequest, DeleteCuratedMemberResponse>(
                new DeleteCuratedMemberRequest(id),
                _ => ModelState.IsValid
            );

        if (!((DeleteCuratedMemberResponse)response).Success)
        {
            return ((DeleteCuratedMemberResponse)response).Message switch
            {
                "BadRequest" => BadRequest(new ProblemDetails { Status = 400, Title = "Invalid ID format" }),
                "NotFound" => NotFound(new ProblemDetails { Status = 404, Title = "Curated member not found" }),
                _ => StatusCode(500, new ProblemDetails { Status = 500, Title = "Internal server error" }),
            };
        }

        return Ok(response);
    }
}
