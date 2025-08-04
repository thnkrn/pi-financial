using System.ComponentModel.DataAnnotations;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.TfexService.API.Filters;
using Pi.TfexService.API.Models.Account;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Queries.Margin;
using Pi.TfexService.Domain.Exceptions;

namespace Pi.TfexService.API.Controllers;

public class InitialMarginController(IInitialMarginQueries initialMarginQueries, IBus bus) : ControllerBase
{
    [HttpGet("secure/initial-margin/{series}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<InitialMarginDto>))]
    public async Task<IActionResult> GetInitialMargin([Required] string series, CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await initialMarginQueries.GetInitialMargin(series, cancellationToken);
            return Ok(new ApiResponse<InitialMarginDto>(resp));
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    [HttpGet("secure/initial-margin/{accountCode}/estimate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<decimal>))]
    [ServiceFilter(typeof(SecureAuthorizationFilter))]
    public async Task<IActionResult> GetEstRequiredInitialMargin(
        [FromHeader(Name = "user-id")][Required] string userId,
        [Required] string accountCode,
        [FromQuery][Required] Side side,
        [FromQuery][Required] string series,
        [FromQuery][Required] int placingUnit,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var resp = await initialMarginQueries.GetEstRequiredInitialMargin(accountCode,
                side,
                series,
                placingUnit,
                cancellationToken);
            return Ok(new ApiResponse<decimal>(resp));
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    [HttpPost("internal/initial-margin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
    public async Task<IActionResult> UpsertInitialMargin([Required][FromBody] UpsertInitialMarginRequest requestData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // send the upsert request and check if data is valid (correct type and parse-able initial value)
            var resp = await bus
                .CreateRequestClient<UpsertInitialMargin>()
                .GetResponse<UpsertInitialMarginResponse>(
                    new UpsertInitialMargin(
                        requestData.Data.Select(data =>
                        {
                            ValidateInitialMarginData(data);
                            return new InitialMarginData(
                                data.Symbol,
                                data.ProductType,
                                decimal.Parse(data.Im));
                        }).ToList(),
                        requestData.AsOfDate
                    ), cancellationToken);

            return Ok(new ApiResponse<bool>(resp.Message.IsSuccess));
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    private void ValidateInitialMarginData(UpsertInitialMarginRequestData data)
    {
        if (string.IsNullOrEmpty(data.Symbol))
        {
            throw new InvalidDataException("Invalid symbol");
        }
        if (!Enum.TryParse(typeof(ProductFamilyType), data.ProductType, out _))
        {
            throw new InvalidDataException($"Invalid product family type: {data.ProductType}");
        }
        if (!double.TryParse(data.Im, out _))
        {
            throw new InvalidDataException($"Invalid initial margin: {data.Im}");
        }
    }

    private ObjectResult HandleException(Exception e)
    {
        switch (e)
        {
            // 400
            case ArgumentException or
                InvalidDataException or
                SetTradeInvalidDataException _:
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: e.Message);
            // 401
            case SetTradeAuthException _:
                return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: e.Message);
            // 404
            case SetTradeNotFoundException _:
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: e.Message);
            // 500
            case SetTradeApiException _:
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
            default:
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
        }
    }

}