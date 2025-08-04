using System.Security.Claims;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi.BackofficeService.API.Models;
using Pi.BackofficeService.API.Startup;
using Pi.BackofficeService.Application.Commands.User;
using Pi.BackofficeService.Application.Models.Sbl;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Application.Services.SblService;
using Pi.Common.Http;

namespace Pi.BackofficeService.API.Controllers;

[Authorize]
[ApiController]
[Route("sbl")]
public class SblController(ISblQueries sblQueries, ISblService sblService, IBus bus) : ControllerBase
{
    private const int MaxSize = 10 * 1024 * 1024; // 10MB

    [Authorize(Policy = "SblRead")]
    [HttpGet("orders/paginate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiPaginateResponse<List<SblOrder>>))]
    public async Task<IActionResult> GetSblOrders([FromQuery] PaginateQuery paginate, [FromQuery] SblOrderFilters filters)
    {
        var result = await sblQueries.GetSblOrderPaginate(
            filters,
            paginate.Page,
            paginate.PageSize,
            paginate.OrderBy,
            paginate.OrderDir?.ToLower() ?? "desc");

        return Ok(new ApiPaginateResponse<List<SblOrder>>(result.Records,
            result.Page,
            result.PageSize,
            result.Total,
            result.OrderBy,
            result.OrderDir));
    }

    [Authorize(Policy = "SblRead")]
    [HttpGet("instruments/paginate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiPaginateResponse<List<SblInstrument>>))]
    public async Task<IActionResult> GetSblInstruments([FromQuery] PaginateQuery paginate,
        [FromQuery] SblInstrumentFilters filters)
    {
        var result = await sblQueries.GetSblInstrumentsPaginate(
            filters,
            paginate.Page,
            paginate.PageSize,
            paginate.OrderBy,
            paginate.OrderDir?.ToLower() ?? "desc");

        return Ok(new ApiPaginateResponse<List<SblInstrument>>(result.Records,
            result.Page,
            result.PageSize,
            result.Total,
            result.OrderBy,
            result.OrderDir));
    }

    [Authorize(Policy = "SblEdit")]
    [HttpPatch("orders/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiPaginateResponse<SblOrder>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitReview(Guid id, [FromBody] SubmitReviewSblOrderRequest request)
    {
        var updated = await sblService.SubmitReviewSblOrderAsync(
            new SubmitReview
            {
                Id = id,
                SblOrderStatus = request.Status switch
                {
                    SblReviewOrderStatus.Approved => SblOrderStatus.Approved,
                    SblReviewOrderStatus.Rejected => SblOrderStatus.Rejected,
                    _ => throw new ArgumentOutOfRangeException(nameof(request.Status), request.Status, string.Empty)
                },
                ReviewerId = await GetUserId(),
                RejectedReason = request.RejectedReason
            });

        return Ok(new ApiResponse<SblOrder>(updated));
    }

    [Authorize(Policy = "SblEdit")]
    [HttpPost("instruments/upload")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiPaginateResponse<UploadSblInstrumentResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        switch (file.Length)
        {
            case 0:
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "File content can not be empty");
            case > MaxSize:
                return Problem(statusCode: StatusCodes.Status400BadRequest,
                    detail: $"File size too large (should below or equal {MaxSize})");
        }

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (fileExtension != ".csv") return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Invalid file type");

        var created = await sblService.UploadSblInstruments(file.FileName, file.OpenReadStream());

        return Ok(new ApiResponse<UploadSblInstrumentResponse>(new UploadSblInstrumentResponse(created)));
    }

    private async Task<Guid> GetUserId()
    {
        var response = await bus.CreateRequestClient<UserUpdateOrCreateRequest>()
            .GetResponse<UserIdResponse>(new UserUpdateOrCreateRequest(
                (Guid)User.GetIamId()!,
                User.FindFirstValue("first_name")!,
                User.FindFirstValue("last_name")!,
                User.FindFirstValue(ClaimTypes.Email)!
            ));

        return response.Message.Id;
    }
}
