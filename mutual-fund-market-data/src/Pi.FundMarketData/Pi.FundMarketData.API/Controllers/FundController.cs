using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.ExtensionMethods;
using Pi.Common.Http;
using Pi.FundMarketData.API.Attributes;
using Pi.FundMarketData.API.Factories;
using Pi.FundMarketData.API.Models;
using Pi.FundMarketData.API.Models.Filters;
using Pi.FundMarketData.API.Models.Requests;
using Pi.FundMarketData.API.Models.Responses;
using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;
using Pi.FundMarketData.Filters;
using Pi.FundMarketData.Repositories;
using Pi.FundMarketData.Utils;
using Swashbuckle.AspNetCore.Annotations;
using FundType = Pi.FundMarketData.API.Models.Filters.FundType;

namespace Pi.FundMarketData.API.Controllers;

[Route("secure/funds")]
[ApiController]
public class FundController : ControllerBase
{
    private IFundRepository _fundRepository;
    private ITradeDataRepository _tradeDataRepository;
    private IAmcRepository _amcRepository;
    private IFundHistoricalNavRepository _fundHistNavRepository;
    private ICommonDataRepository _commonDataRepository;
    private ILogger<FundController> _logger;

    public FundController(
        IFundRepository fundRepository,
        ITradeDataRepository tradeDataRepository,
        IAmcRepository amcRepository,
        IFundHistoricalNavRepository fundHistNavRepository,
        ICommonDataRepository commonDataRepository,
        ILogger<FundController> logger)
    {
        _fundRepository = fundRepository;
        _tradeDataRepository = tradeDataRepository;
        _amcRepository = amcRepository;
        _fundHistNavRepository = fundHistNavRepository;
        _commonDataRepository = commonDataRepository;
        _logger = logger;
    }

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<FundSearchResponse>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [HttpGet("search")]
    [HttpGet("/internal/funds/search")]
    public async Task<IActionResult> SearchFundAsync([FromQuery] string keyword, CancellationToken ct = default)
    {
        try
        {
            var searchRes = new List<FundSearchResponse>();

            var funds = await _fundRepository.SearchFundProfiles(keyword, ct);
            if (!funds.Any())
                return Ok(new ApiResponse<IList<FundSearchResponse>>(searchRes));

            Dictionary<string, AmcProfile> amcProfiles = await _amcRepository.GetAmcProfiles(ct);

            foreach (var fund in funds)
            {
                amcProfiles.TryGetValue(fund.AmcCode, out AmcProfile matchedAmcProfile);

                FundSearchResponse searchedFund = new(fund, matchedAmcProfile?.Logo);
                searchRes.Add(searchedFund);
            }

            ApiResponse<IEnumerable<FundSearchResponse>> response = new(searchRes);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail:
                ex.Message);
        }
    }

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<FundProfileResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [HttpGet("{symbol}/profile")]
    [HttpGet("/internal/funds/{symbol}/profile")]
    public async Task<IActionResult> GetFundProfileAsync([FromRoute, Required] string symbol, CancellationToken ct = default)
    {
        try
        {
            symbol = symbol.ToUpper();
            symbol = UtilsMethod.ReplaceEncodedSlash(symbol);

            Fund fund = await _fundRepository.GetFundProfile(symbol, ct);
            if (fund is null)
            {
                return NotFound();
            }

            var isWhiteList = await _fundRepository.WhiteListSymbolExists(symbol, ct);

            Dictionary<string, AmcProfile> amcProfiles = await _amcRepository.GetAmcProfiles(ct);
            amcProfiles.TryGetValue(fund.AmcCode, out AmcProfile matchedAmcProfile);

            FundProfileResponse profileRes = new(fund, matchedAmcProfile, isWhiteList);

            ApiResponse<FundProfileResponse> response = new(profileRes);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail:
                ex.Message);
        }
    }

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<FundProfileResponse>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [HttpPost("profiles")]
    [Obsolete]
    public async Task<IActionResult> GetFundProfilesAsync([FromBody, Required] IList<string> symbols,
        CancellationToken ct = default)
    {
        try
        {
            symbols = symbols.Select(x => x?.ToUpper()).ToList();

            if (symbols.Count > 30)
                throw new ArgumentException("The number of symbols must not exceed 30 items.");

            var profilesRes = new List<FundProfileResponse>();

            var funds = await _fundRepository.GetFundProfiles(symbols, ct);
            if (!funds.Any())
                return Ok(new ApiResponse<IList<FundProfileResponse>>(profilesRes));

            var amcProfiles = await _amcRepository.GetAmcProfiles(ct);
            foreach (var fund in funds)
            {
                amcProfiles.TryGetValue(fund.AmcCode, out AmcProfile matchedAmcProfile);

                FundProfileResponse profileRes = new(fund, matchedAmcProfile);
                profilesRes.Add(profileRes);
            }

            ApiResponse<IList<FundProfileResponse>> response = new(profilesRes);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail:
                ex.Message);
        }
    }

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<FundMarketSummaryResponse>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [HttpGet("{marketBasket}/market-summaries")]
    public async Task<IActionResult> RetrieveFundListAsync(
        [FromRoute, Required] MarketBasket marketBasket,
        [FromQuery, Required] Interval interval = Interval.Over3Months,
        CancellationToken ct = default
    )
    {
        try
        {
            IEnumerable<Fund> fundInfos = await _fundRepository.GetFundProfilesByMarketBasket(marketBasket, interval, ct);
            Dictionary<string, AmcProfile> amcProfiles = await _amcRepository.GetAmcProfiles(ct);

            if (marketBasket == MarketBasket.NewFund && interval != Interval.SinceInception)
            {
                interval = Interval.SinceInception;
            }

            List<FundMarketSummaryResponse> funds = new();
            foreach (Fund fund in fundInfos)
            {
                if (fund.AmcCode != null && amcProfiles.TryGetValue(fund.AmcCode, out AmcProfile amc))
                {
                    funds.Add(new FundMarketSummaryResponse(fund, amc.Logo, interval));
                }
            }

            ApiResponse<IEnumerable<FundMarketSummaryResponse>> response = new(funds);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail:
                ex.Message);
        }
    }

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<ApiPaginateResponse<IEnumerable<FundMarketSummaryResponse>>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [HttpGet("{marketBasket}/market-summaries/v2")]
    public async Task<IActionResult> PaginateBasket(
        [FromRoute, Required] MarketBasket marketBasket,
        [FromQuery, Required] Interval interval = Interval.Over3Months,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] BasketFiltersRequest filtersRequest = null,
        CancellationToken ct = default
    )
    {
        try
        {
            var filters = filtersRequest != null ? FilterFactory.NewFundFilter(filtersRequest) : null;
            var paginateResult = await _fundRepository.PaginateBasketFundProfiles(marketBasket, interval, page, pageSize, filters, ct);
            Dictionary<string, AmcProfile> amcProfiles = await _amcRepository.GetAmcProfiles(ct);

            if (marketBasket == MarketBasket.NewFund && interval != Interval.SinceInception)
            {
                interval = Interval.SinceInception;
            }

            List<FundMarketSummaryResponse> funds = [];
            foreach (Fund fund in paginateResult.Records)
            {
                if (fund.AmcCode != null && amcProfiles.TryGetValue(fund.AmcCode, out AmcProfile amc))
                {
                    funds.Add(new FundMarketSummaryResponse(fund, amc.Logo, interval));
                }
            }

            var resp = new ApiPaginateResponse<IEnumerable<FundMarketSummaryResponse>>(funds, paginateResult.Page,
                paginateResult.PageSize, paginateResult.Total, null, null);

            // Wrap with ApiResponse again because of app need everything under key "data"
            return Ok(new ApiResponse(resp));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail:
                ex.Message);
        }
    }

    [HttpGet("{marketBasket}/market-summaries/v2/filters")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<FilterResponse[]>))]
    public Task<IActionResult> GetFilters([FromRoute] MarketBasket marketBasket)
    {
        var result = marketBasket switch
        {
            MarketBasket.Category => FilterFactory.NewFilterResponse<BasketFiltersRequest>(),
            _ => []
        };

        return Task.FromResult<IActionResult>(Ok(new ApiResponse(result)));
    }

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<LegacyFundMarketSummaryResponse>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [HttpPost("/secure/legacy/funds/summaries")]
    [HttpPost("/internal/legacy/funds/summaries")]
    public async Task<IActionResult> LegacyRetrieveFundListBySymbolsAsync(
        [FromBody, Required] LegacySymbolsRequest req,
        CancellationToken ct = default)
    {
        try
        {
            if (req.Symbols.Count > 20)
                throw new ArgumentException("The number of symbols must not exceed 20 items.");

            var symbols = req.Symbols.Select(x => x?.ToUpper()).ToList();

            var fundInfos = await _fundRepository.GetFundProfiles(symbols, ct);
            var amcProfiles = await _amcRepository.GetAmcProfiles(ct);
            List<LegacyFundMarketSummaryResponse> funds = new();
            foreach (Fund fund in fundInfos)
            {
                if (fund.AmcCode != null && amcProfiles.TryGetValue(fund.AmcCode, out AmcProfile amc))
                {
                    funds.Add(new LegacyFundMarketSummaryResponse(fund, amc.Logo));
                }
            }

            ApiResponse<IEnumerable<LegacyFundMarketSummaryResponse>> response = new(funds);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail:
                ex.Message);
        }
    }

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<FundHistoricalNavResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [HttpGet("{symbol}/historical-nav")]
    public async Task<IActionResult> GetFundHistoricalNavAsync([FromRoute, Required] string symbol,
        [FromQuery] string interval,
        CancellationToken ct)
    {
        try
        {
            symbol = symbol.ToUpper();
            symbol = UtilsMethod.ReplaceEncodedSlash(symbol);

            Interval intervalEnum = Enum.GetValues(typeof(Interval))
                .OfType<Interval>()
                .FirstOrDefault(x =>
                    string.Equals(EnumUtils.GetEnumMemberValue(x), interval, StringComparison.OrdinalIgnoreCase));

            if (intervalEnum == default)
            {
                return BadRequest("Invalid interval value.");
            }

            HistoricalNavInfo navInfo = await _fundHistNavRepository.GetHistoricalNavInfo(symbol, intervalEnum, ct);
            var navRes = new FundHistoricalNavResponse(navInfo);

            ApiResponse<FundHistoricalNavResponse> response = new(navRes);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail:
                ex.Message);
        }
    }

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<SwitchingFundResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [HttpGet("{symbol}/switching-funds")]
    public async Task<IActionResult> RetrieveSwitchFundAsync([FromRoute, Required] string symbol,
        CancellationToken ct = default)
    {
        try
        {
            symbol = symbol.ToUpper();
            symbol = UtilsMethod.ReplaceEncodedSlash(symbol);

            ApiResponse<SwitchingFundResponse> response = new(new SwitchingFundResponse());

            IEnumerable<Switching> switchingFunds = await _tradeDataRepository.GetSwitchingFundList(symbol, ct);
            if (!switchingFunds.Any())
            {
                return Ok(response);
            }

            List<SwitchFunds> switchingFundResults = new();
            var fundCodes = switchingFunds.Select(x => x.FundCode).ToList();
            fundCodes.Add(symbol);
            var fundSwitches = await _fundRepository.GetFundProfiles(fundCodes, ct);
            string amcCode = fundSwitches.FirstOrDefault(x => x.Symbol == symbol)?.AmcCode;
            AmcProfile amcProfile = await _amcRepository.GetAmcProfile(amcCode, ct);

            foreach (Fund fundSwitch in fundSwitches)
            {
                if (fundSwitch.AmcCode == amcCode && fundSwitch.Symbol != symbol)
                {
                    switchingFundResults.Add(new SwitchFunds
                    {
                        Name = fundSwitch.Name,
                        Nav = fundSwitch.AssetValue?.Nav ?? 0,
                        Symbol = fundSwitch.Symbol,
                        ReturnPercentage = fundSwitch.AssetValue?.NavChange?.NavChangePercentage,
                        Logo = amcProfile.Logo
                    });
                }
            }

            response = new(new SwitchingFundResponse()
            {
                SwitchFunds = switchingFundResults
            });
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail:
                ex.Message);
        }
    }

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<DateTime>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [HttpGet("{symbol}/tradable-dates")]
    [HttpGet("/internal/funds/{symbol}/tradable-dates")]
    public async Task<IActionResult> GetFundTradableDatesAsync(
            [FromRoute, Required] string symbol,
            [FromQuery] FundTradableDateFilter filter,
            CancellationToken ct = default)
    {
        try
        {
            symbol = symbol.ToUpper();
            symbol = UtilsMethod.ReplaceEncodedSlash(symbol);

            List<ValidationResult> result = new();
            bool isValid = Validator.TryValidateObject(filter, new ValidationContext(filter), result, false);
            if (!isValid)
            {
                ValidationResult error = result.FirstOrDefault();
                throw new ArgumentException(error.ErrorMessage);
            }

            string[] symbols = filter.TradeType == TradeSide.Switch
                ? new[] { symbol, filter.SwitchSymbol }
                : new[] { symbol };

            var fund = await _fundRepository.GetFundProfile(symbol, ct);

            var isWhiteList = await _fundRepository.WhiteListSymbolExists(symbol, ct);
            if (!isWhiteList) return Ok(new ApiResponse<IEnumerable<DateTime>>([]));

            var closestTradeDate = fund.Purchase.GetClosestTradeDate(filter.TradeType);
            var closestTradeDateUtc = DateTime.SpecifyKind(closestTradeDate, DateTimeKind.Utc);
            var tradeCalendarType = fund.Fundamental.GetTradeCalendarType(filter.TradeType);

            var businessCalendar = await _commonDataRepository.GetBusinessCalendar(ct);
            var businessDayInRange = businessCalendar.GetBusinessDays(closestTradeDateUtc, closestTradeDateUtc.AddDays(BusinessCalendar.DateRange2WeeksIncrementor));

            var fundsTradeData = await _tradeDataRepository.GetFundTradeData(symbols, ct);
            fundsTradeData.TryGetValue(symbol, out var mainTradeData);
            mainTradeData ??= new();
            var tradableDates = mainTradeData.GetTradableDates(
                    filter.TradeType,
                    tradeCalendarType,
                    businessDayInRange);

            ApiResponse<IEnumerable<DateTime>> response = new(tradableDates);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail:
                ex.Message);
        }
    }

    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<FundTradingProfileResponse>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [HttpPost("/internal/funds/trading-profiles")]
    public async Task<IActionResult> GetFundTradingProfileAsync(
        [FromBody, Required] IList<string> symbols,
        CancellationToken ct = default)
    {
        try
        {
            symbols = symbols.Select(x => x?.ToUpper()).ToList();

            if (symbols.Count > 30)
                throw new ArgumentException("The number of symbols must not exceed 30 items.");

            var profilesRes = new List<FundTradingProfileResponse>();

            var funds = await _fundRepository.GetFundProfiles(symbols, ct);
            if (!funds.Any())
                return Ok(new ApiResponse<IList<FundTradingProfileResponse>>(profilesRes));

            var amcProfiles = await _amcRepository.GetAmcProfiles(ct);
            foreach (var fund in funds)
            {
                amcProfiles.TryGetValue(fund.AmcCode, out AmcProfile matchedAmcProfile);

                var profileRes = new FundTradingProfileResponse(fund, matchedAmcProfile);
                profilesRes.Add(profileRes);
            }

            ApiResponse<IEnumerable<FundTradingProfileResponse>> response = new(profilesRes);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail:
                ex.Message);
        }
    }
}
