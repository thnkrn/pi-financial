using System.ComponentModel.DataAnnotations;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.User.Application.Commands;
using Pi.User.Application.Models.ErrorCode;
using Pi.User.Application.Models.Examination;
using Pi.User.Application.Queries.Examination;

namespace Pi.User.API.Controllers;

[ApiController]
public class ExaminationController : ControllerBase
{
    private readonly IBus _bus;
    private readonly IExaminationQueries _examinationQueries;

    public ExaminationController(
        IBus bus,
        IExaminationQueries examinationQueries)
    {
        _bus = bus;
        _examinationQueries = examinationQueries;
    }

    [HttpGet("internal/examination/{userId}", Name = "GetExaminationsByUserId")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<ExaminationDto>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetExaminationsByUserId(
        [FromRoute] string userId,
        [FromQuery]
        string? examName
    )
    {
        try
        {
            var result = await _examinationQueries.GetByUserIdAsync(Guid.Parse(userId), examName);

            return Ok(new ApiResponse<List<ExaminationDto>>(result));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [HttpPost("internal/examination", Name = "CreateOrUpdateExamination")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SubmitExaminationResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrUpdateExamination(
        [FromBody]
        SubmitExaminationRequest payload
    )
    {
        try
        {
            var client = _bus.CreateRequestClient<SubmitExaminationRequest>();
            var (status, error) = await client.GetResponse<SubmitExaminationResponse, ErrorCodeResponse>(payload);

            if (status.IsCompletedSuccessfully)
            {
                var okResp = await status;
                return Ok(new ApiResponse<SubmitExaminationResponse>(okResp.Message));
            }

            var errResp = await error;
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: errResp.Message.ErrorCode);

        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }
}