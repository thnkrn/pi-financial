using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pi.Common.Features;
using Pi.Common.Http;
using Pi.PortfolioService.API.Models;
using Pi.PortfolioService.Application.Models;
using Pi.PortfolioService.DomainServices;
using GeneralError = Pi.PortfolioService.API.Models.GeneralError;

namespace Pi.PortfolioService.API.Controllers;

[ApiController]
public class SummaryController : ControllerBase
{
    private readonly IPortfolioSummaryQueries _queries;
    private readonly ILogger _logger;
    private readonly IFeatureService _featureService;

    public SummaryController(IPortfolioSummaryQueries queries, ILogger<SummaryController> logger,
        IFeatureService featureService)
    {
        _logger = logger;
        _featureService = featureService;
        _queries = queries;
    }

    [HttpPost("secure/summary")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PortfolioSummaryResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> GetPortfolioSummary([FromHeader(Name = "user-id")][Required] string userId,
        [FromBody] PortfolioSummaryRequest request,
        [FromHeader(Name = "sid")] string? sid,
        [FromHeader(Name = "deviceId")] Guid? deviceId,
        CancellationToken ct = default)
    {
        try
        {
            var valueUnit = request.valueUnit;

            var portfolioSummary = _featureService.IsOn(Features.SiriusPortfolioMigration)
                ? await _queries.GetPortfolioSummaryV2Async(userId, valueUnit, ct)
                : await _queries.GetPortfolioSummaryAsync(sid!, deviceId.GetValueOrDefault(), userId, valueUnit, ct);
            var data = MapResponse(portfolioSummary);
            LogGeResponse(data);
            var response = new ApiResponse<PortfolioSummaryResponse>(data: data);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, $"Caught UnauthorizedAccessException with message: {{ExMessage}}", ex.Message);

            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: ErrorCodes.InvalidSessionId.ToUpper(),
                detail: ex.Message
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Caught Exception with message: {{ExMessage}}", ex.Message);

            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail: ex.Message
            );
        }
    }

    [HttpPost("secure/v2/summary")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PortfolioSummaryResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> GetPortfolioSummaryV2([FromHeader(Name = "user-id")][Required] string userId,
        [FromHeader(Name = "sid")] string? sid,
        [FromBody] PortfolioSummaryRequest request,
        [FromHeader(Name = "deviceId")] Guid? deviceId,
        CancellationToken ct = default)
    {
        try
        {
            var valueUnit = request.valueUnit;

            var portfolioSummary = _featureService.IsOn(Features.FetchPortfolioV2)
                ? await _queries.GetPortfolioSummaryV2Async(userId, valueUnit, ct)
                : await _queries.GetPortfolioSummaryAsync(sid!, deviceId.GetValueOrDefault(), userId, valueUnit, ct);

            // var portfolioSummary = await _queries.GetPortfolioSummaryV2Async(userId, valueUnit, ct);
            var data = MapResponse(portfolioSummary);
            var response = new ApiResponse<PortfolioSummaryResponse>(data: data);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, $"Caught UnauthorizedAccessException with message: {{ExMessage}}", ex.Message);

            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: ErrorCodes.InvalidSessionId.ToUpper(),
                detail: ex.Message
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Caught Exception with message: {{ExMessage}}", ex.Message);

            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail: ex.Message
            );
        }
    }

    private static PortfolioSummaryResponse MapResponse(PortfolioSummary portfolioSummary)
    {
        return new PortfolioSummaryResponse(
            portfolioSummary.AsOfDate.ToUnixTimeSeconds(),
            portfolioSummary.Currency,
            portfolioSummary.TotalValue.ToString("F2"),
            portfolioSummary.TotalMarketValue.ToString("F2"),
            portfolioSummary.TotalCostValue.ToString("F2"),
            portfolioSummary.CashBalance.ToString("F2"),
            portfolioSummary.Liabilities.ToString("F2"),
            portfolioSummary.Upnl.ToString("F2"),
            portfolioSummary.UpnlPercentage.ToString("F2"),
            portfolioSummary
                .PortfolioAccounts
                .Select(
                    account =>
                        new PortfolioAccountResponse(
                            account.AccountType,
                            account.AccountId,
                            account.AccountNoForDisplay.Replace("-", ""),
                            account.TradingAccountNo,
                            account.CustCode,
                            account.SblFlag,
                            account.TotalValue.ToString("F2"),
                            account.TotalMarketValue.ToString("F2"),
                            account.TotalCostValue.ToString("F2"),
                            account.CashBalance.ToString("F2"),
                            account.Upnl.ToString("F2"),
                            account.UpnlPercentage.ToString("F2"),
                            account.ErrorMessage
                        )
                ),
            portfolioSummary
                .PortfolioWalletCategorizeds
                .Select(
                    walletCategorized =>
                        new PortfolioWalletCategorizedResponse(
                            walletCategorized.AccountType,
                            walletCategorized.TotalValue.ToString("F2"),
                            walletCategorized.TotalMarketValue.ToString("F2"),
                            walletCategorized.TotalCostValue.ToString("F2"),
                            walletCategorized.CashBalance.ToString("F2"),
                            walletCategorized.Upnl.ToString("F2"),
                            walletCategorized.UpnlPercentage.ToString("F2"),
                            walletCategorized.AssetRatioInAllAsset.ToString("F2")
                        )
                ),
            portfolioSummary
                .PortfolioErrorAccounts
                .Select(
                    account =>
                        new PortfolioErrorAccountResponse(
                            account.AccountType,
                            account.AccountId,
                            account.ErrorMessage,
                            account.AccountNoForDisplay
                        )
                ),
            portfolioSummary
                .GeneralErrors
                .Select(
                    error =>
                        new GeneralError(
                            error.AccountType,
                            error.Error
                        )
                )
        );
    }

    private void LogGeResponse(PortfolioSummaryResponse data)
    {
        var geAccount = data.AccountList.Where(x =>
            x.AccountType == "Global Equity" && Guid.TryParse(x.AccountId, out var guidOutput));
        if (!geAccount.Any())
            _logger.LogInformation("PortfolioSummaryResponse: {PortfolioSummaryResponse}",
                JsonConvert.SerializeObject(data));
    }
}