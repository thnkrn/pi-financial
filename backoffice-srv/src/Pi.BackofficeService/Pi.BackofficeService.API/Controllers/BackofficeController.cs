using Microsoft.AspNetCore.Mvc;
using Pi.BackofficeService.API.Factories;
using Pi.BackofficeService.API.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.Common.Http;

namespace Pi.BackofficeService.API.Controllers;

[ApiController]
public class BackofficeController : ControllerBase
{
    private readonly IBackofficeQueries _backofficeQueries;

    public BackofficeController(IBackofficeQueries backofficeQueries)
    {
        _backofficeQueries = backofficeQueries;
    }

    [HttpGet("account_types")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<NameAliasResponse>>))]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> Product([FromQuery] ProductType productType = ProductType.ThaiEquity)
    {
        var records = await _backofficeQueries.GetProducts(productType);
        var response = records.Select(q => DtoFactory.NewGenericNameAliasResponse((ProductResponse)q))
            .ToList();

        return Ok(new ApiResponse<List<NameAliasResponse<ProductResponse>>>(response));
    }

    [HttpGet("banks")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<Bank>>))]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> Banks([FromQuery] string? channel)
    {
        var records = await _backofficeQueries.GetBanks(channel);

        return Ok(new ApiResponse<List<Bank>>(records));
    }
}
