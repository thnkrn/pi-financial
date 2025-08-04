using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.CommonModels;
using Pi.Common.Http;
using Pi.GlobalEquities.API.Models.Requests;
using Pi.GlobalEquities.API.Models.Responses;
using Pi.GlobalEquities.Application.Models.Dto;
using Pi.GlobalEquities.Application.Queries;
using Pi.GlobalEquities.Errors;
using Pi.GlobalEquities.Services;
using Pi.GlobalEquities.Services.Wallet;
using Pi.GlobalEquities.Utils;

namespace Pi.GlobalEquities.API.Controllers;

[Route("/secure/accounts")]
[ApiController]
public class AccountController : BaseController
{
    private readonly IAccountService _accountService;
    private readonly ITradingService _tradingService;
    private readonly IAccountQueries _accountQueries;
    private readonly IWebHostEnvironment _env;

    public AccountController(
        IAccountService accountService,
        ITradingService tradingService,
        IAccountQueries accountQueries,
        IWebHostEnvironment env,
        ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
        _accountService = accountService;
        _tradingService = tradingService;
        _accountQueries = accountQueries;
        _env = env;
    }

    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<AccountSummaryResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAccountSummary(
        [FromHeader(Name = RequestHeader.UserId), Required] string userId,
        [FromQuery] AccountSummaryRequest request,
        CancellationToken ct = default)
    {
        var accounts = await _accountQueries.GetAccountSummary(userId, request.AccountId, request.Currencies, ct);

        return Ok(MapToAccountSummaryResponse(accounts));
    }

    [HttpGet("overview")]
    [HttpGet("/internal/accounts/overview")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<MultiAccountOverviewResponse>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAccountOverview(
        [FromHeader(Name = RequestHeader.UserId), Required] string userId,
        [FromQuery] Currency currency = Currency.USD,
        CancellationToken ct = default)
    {
        var accounts = await _accountService.GetAccounts(userId, ct);
        var accOverviewRes = new MultiAccountOverviewResponse();

        if (!accounts.Any())
            return Ok(accOverviewRes);

        await ConcurrencyUtils.RunAsConcurrentAsync(accounts,
            async (account) =>
            {
                try
                {
                    var accOverview = await _tradingService.GetAccountOverview(account, currency, ct);
                    accOverviewRes.AddAccountOverview(accOverview);
                }
                catch (Exception ex)
                {
                    var accountId = account.Id;
                    var error = _env.IsProduction() ? null : $"Could not get global equities for account {accountId}, Exception: {ex.Message}";

                    accOverviewRes.AddFailedAccount(account, error);
                }
            },
            5, ct);

        return Ok(accOverviewRes);
    }

    [HttpGet("corporate-actions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CorporateActionResponse[]>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(StatusCodeResult))]
    public async Task<IActionResult> GetCorporateActions(
        [FromHeader(Name = RequestHeader.UserId), Required] string userId,
        [FromQuery, Required] string accountId,
        [FromQuery, Required] DateTime from,
        [FromQuery, Required] DateTime to,
        CancellationToken ct = default)
    {
        var account = await _accountService.GetAccount(userId, accountId, ct);
        if (account == null)
            return Forbid(AccountErrors.NotExist);

        var providerAccount = account.GetProviderAccount(Provider.Velexa);

        var corpActionTrns = await _tradingService.GetCorporateActionTransactions(providerAccount, from, to, ct);

        var corpActionTrnTrees = BuildCorporateTransactionTree(corpActionTrns, from, to);
        var results = corpActionTrnTrees.Select(x => new CorporateActionResponse(new CorporateTransaction(x)));

        return Ok(results);
    }

    private static IList<TransactionItem> BuildCorporateTransactionTree(IList<TransactionItem> transactions,
        DateTime from, DateTime to)
    {
        if (transactions == null || !transactions.Any())
            return new List<TransactionItem>();

        var fromStamp = DateTimeUtils.ConvertToTimestamp(from);
        var toStamp = DateTimeUtils.ConvertToTimestamp(to);

        var trns = transactions.ToDictionary(x => x.Id);
        var rootTrns = new List<TransactionItem>();

        foreach (var trn in trns.Values)
        {
            if (trn.ParentId == null)
            {
                if (trn.OperationType == OperationType.AutoConversion)
                    continue;

                if (trn.Timestamp < fromStamp || trn.Timestamp > toStamp)
                    continue;

                rootTrns.Add(trn);
            }
            else if (trns.TryGetValue(trn.ParentId, out TransactionItem parent))
            {
                parent.Children.Add(trn);
            }
        }

        return rootTrns;
    }

    private static AccountSummaryResponse MapToAccountSummaryResponse(AccountSummaryDto accountSummaryDto)
    {
        return new AccountSummaryResponse
        {
            TradingAccountNo = accountSummaryDto.TradingAccountNo,
            TradingAccountNoDisplay = accountSummaryDto.TradingAccountNoDisplay,
            UpnlPercentage = accountSummaryDto.UpnlPercentage,
            ExchangeRate = accountSummaryDto.ExchangeRate,
            Values = accountSummaryDto.Values.Select(x => new AccountSummaryResponse.AccountSummaryValueResponse
            {
                Currency = x.Currency,
                NetAssetValue = x.NetAssetValue,
                MarketValue = x.MarketValue,
                Cost = x.Cost,
                Upnl = x.Upnl,
                UnusedCash = x.UnusedCash,
                AccountLimit = x.AccountLimit,
                LineAvailable = x.LineAvailable
            }),
            Positions = accountSummaryDto.Positions.Select(x => new AccountSummaryResponse.PositionResponse
            {
                Symbol = x.Symbol,
                Venue = x.Venue,
                Currency = x.Currency,
                EntireQuantity = x.EntireQuantity,
                AvailableQuantity = x.AvailableQuantity,
                LastPrice = x.LastPrice,
                MarketValue = x.MarketValue,
                AveragePrice = x.AveragePrice,
                Upnl = x.Upnl,
                Cost = x.Cost,
                UpnlPercentage = x.UpnlPercentage,
                Logo = x.Logo
            })
        };
    }
}
