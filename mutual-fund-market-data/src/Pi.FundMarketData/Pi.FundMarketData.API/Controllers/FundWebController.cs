using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.FundMarketData.API.Factories;
using Pi.FundMarketData.API.Models.Requests;
using Pi.FundMarketData.API.Models.Responses;
using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;
using Pi.FundMarketData.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace Pi.FundMarketData.API.Controllers;

[Route("secure/web/funds")]
[ApiController]
public class FundWebController(
    IFundWebRepository fundWebRepository,
    IAmcRepository amcRepository,
    IFundHistoricalNavRepository fundHistoricalNavRepository,
    ILogger<FundWebController> logger)
    : ControllerBase
{
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<ApiPaginateResponse<IEnumerable<FundWebMarketSummaryResponse>>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [HttpGet("{marketBasket}/market-summaries")]
    public async Task<IActionResult> GetPaginateMarketSummaries(
        [FromRoute, Required] MarketBasket marketBasket,
        [FromQuery, Required] Interval interval = Interval.Over3Months,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] BasketFiltersRequestV2 filtersRequest = null,
        CancellationToken ct = default
    )
    {
        try
        {
            var filters = filtersRequest != null ? FilterFactory.NewFundFilter(filtersRequest) : null;
            var paginateResult = await fundWebRepository.GetPaginateFundMarketSummaries(marketBasket, interval, page, pageSize, filters, ct);
            Dictionary<string, AmcProfile> amcProfiles = await amcRepository.GetAmcProfiles(ct);

            List<FundWebMarketSummaryResponse> funds = [];
            foreach (Fund fund in paginateResult.Records)
            {
                if (fund.AmcCode != null && amcProfiles.TryGetValue(fund.AmcCode, out AmcProfile amc))
                {
                    funds.Add(new FundWebMarketSummaryResponse(fund, amc.Logo));
                }
            }

            var resp = new ApiPaginateResponse<IEnumerable<FundWebMarketSummaryResponse>>(funds, paginateResult.Page,
                paginateResult.PageSize, paginateResult.Total, null, null);

            return Ok(new ApiResponse(resp));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail:
                ex.Message);
        }
    }

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<FundWebMarketSummaryResponse>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [HttpGet("market-summaries")]
    public async Task<IActionResult> GetMarketSummaries(
        [FromQuery, Required] string[] symbols,
        CancellationToken ct = default
    )
    {
        try
        {
            if (symbols == null || symbols.Length == 0)
            {
                throw new ArgumentNullException(nameof(symbols));
            }
            var result = await fundWebRepository.GetFundMarketSummaries(symbols, ct);
            Dictionary<string, AmcProfile> amcProfiles = await amcRepository.GetAmcProfiles(ct);

            List<FundWebMarketSummaryResponse> funds = [];
            foreach (Fund fund in result)
            {
                if (fund.AmcCode != null && amcProfiles.TryGetValue(fund.AmcCode, out AmcProfile amc))
                {
                    funds.Add(new FundWebMarketSummaryResponse(fund, amc.Logo));
                }
            }

            var resp = new List<FundWebMarketSummaryResponse>(funds);

            return Ok(new ApiResponse(resp));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail:
                ex.Message);
        }
    }

    [HttpGet("market-summaries/filters")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<FilterResponse[]>))]
    public Task<IActionResult> GetFilters()
    {
        var result = FilterFactory.NewFilterResponse<BasketFiltersRequestV2>();

        return Task.FromResult<IActionResult>(Ok(new ApiResponse(result)));
    }

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<FundWebHistoricalNavResponse>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [HttpGet("historical-nav")]
    public async Task<IActionResult> GetFundHistoricalNavs(
        [FromQuery] string[] symbols,
        [FromQuery] Interval interval,
        CancellationToken ct)
    {
        try
        {
            if (symbols == null || symbols.Length == 0)
            {
                throw new ArgumentNullException(nameof(symbols));
            }

            List<HistoricalNavInfo> navInfoList = await fundHistoricalNavRepository.GetHistoricalNavInfos(symbols, interval, ct);
            var navRes = navInfoList.Select(x => new FundWebHistoricalNavResponse(x)).ToList();
            ApiResponse<List<FundWebHistoricalNavResponse>> response = new(navRes);
            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail:
                ex.Message);
        }
    }

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<FundProfileResponse>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [HttpGet("profiles")]
    public async Task<IActionResult> GetFundProfiles(
        [FromQuery, Required] string[] symbols,
        CancellationToken ct = default
    )
    {
        try
        {
            if (symbols == null || symbols.Length == 0)
            {
                throw new ArgumentNullException(nameof(symbols));
            }

            var fundMarketSummariesTask = fundWebRepository.GetFundMarketSummaries(symbols, ct);
            var amcProfilesTask = amcRepository.GetAmcProfiles(ct);
            var whiteListSymbolsExistsTask = fundWebRepository.GetWhiteListSymbolsExists(symbols, ct);
            await Task.WhenAll(fundMarketSummariesTask, amcProfilesTask, whiteListSymbolsExistsTask);

            var result = fundMarketSummariesTask.Result;
            var amcProfiles = amcProfilesTask.Result;
            var whiteListSymbolsExists = whiteListSymbolsExistsTask.Result;
            var funds = result.Select(fund => new FundProfileResponse(fund, amcProfiles.GetValueOrDefault(fund.AmcCode), whiteListSymbolsExists.GetValueOrDefault(fund.Symbol))).ToList();

            return Ok(new ApiResponse(new List<FundProfileResponse>(funds)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail:
                ex.Message);
        }
    }
}
