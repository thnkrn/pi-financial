using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.SetService.API.Factories;
using Pi.SetService.API.Models;
using Pi.SetService.Application.Commands;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Models.AccountSummaries;
using Pi.SetService.Application.Queries;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.Events;
using CancelOrderRequest = Pi.SetService.Application.Commands.CancelOrderRequest;
using CancelOrderResponse = Pi.SetService.Application.Commands.CancelOrderResponse;
using ChangeOrderRequest = Pi.SetService.Application.Commands.ChangeOrderRequest;

namespace Pi.SetService.API.Controllers;

[ApiController]
public class SetTradingController(ISetQueries setQueries, IBus bus) : ControllerBase
{
    [HttpGet("secure/accounts/openOrders")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<SetOpenOrderResponse>>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetOpenOrders([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromQuery][Required] string tradingAccountNo)
    {
        var orders = await setQueries.GetOpenOrdersByTradingAccountNoAsync(userId, tradingAccountNo);
        var response = orders.Select(q =>
        {
            return q switch
            {
                SetOrderInfo setOrder => SetFactory.NewOpenOrderResponse(setOrder),
                SblOrderInfo sblOrder => SetFactory.NewOpenOrderResponse(sblOrder),
                _ => throw new ArgumentOutOfRangeException(nameof(q), q, null)
            };
        }).ToList();

        return Ok(new ApiResponse<List<SetOpenOrderResponse>>(response));
    }

    [HttpPost("secure/accounts/available/balance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<AccountInstrumentAvailableBalanceResponse>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetInstrumentAvailableBalance([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromBody][Required] AccountInstrumentAvailableBalanceRequest request)
    {
        var result =
            await setQueries.GetAccountInstrumentBalanceAsync(userId, request.TradingAccountNo, request.Symbol);

        return Ok(new ApiResponse<AccountInstrumentAvailableBalanceResponse>(
            SetFactory.NewAccountInstrumentAvailableBalanceResponse(result)));
    }

    [HttpPost("secure/accounts/sbl/available/balance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<AccountSblInstrumentAvailableBalanceResponse>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetSblInstrumentAvailableBalance([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromBody][Required] AccountInstrumentAvailableBalanceRequest request)
    {
        var result =
            await setQueries.GetAccountSblInstrumentBalanceAsync(userId, request.TradingAccountNo, request.Symbol);

        return Ok(new ApiResponse<AccountSblInstrumentAvailableBalanceResponse>(
            SetFactory.NewAccountSblInstrumentAvailableBalanceResponse(result)
            ));
    }

    [HttpGet("secure/accounts/cash/balance/summary")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SetAccountSummaryResponse>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetCashBalanceSummary([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromQuery][Required] string tradingAccountNo)
    {
        var accountBalance =
            await setQueries.GetCashAccountSummaryByTradingAccountNoAsync(userId, tradingAccountNo);

        return Ok(new ApiResponse<SetAccountSummaryResponse>(
            SetFactory.NewSetCashAccountSummaryResponse(accountBalance)));
    }

    [HttpGet("secure/accounts/credit/balance/summary")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SetCreditBalanceAccountSummaryResponse>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetCreditBalanceSummary([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromQuery][Required] string tradingAccountNo)
    {
        var accountBalance =
            await setQueries.GetCreditBalanceSummaryByTradingAccountNoAsync(userId, tradingAccountNo);

        return Ok(new ApiResponse<SetCreditBalanceAccountSummaryResponse>(
            SetFactory.NewSetCreditBalanceAccountSummaryResponse(accountBalance)));
    }

    [HttpGet("internal/accounts/balance/summary")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<SetAccountSummaryResponse>>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetInternalBalanceSummary([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromQuery] string? tradingAccountNo)
    {
        var result = new List<SetAccountSummaryResponse>();
        if (tradingAccountNo != null)
        {
            var accountBalance =
                await setQueries.GetCashAccountSummaryByTradingAccountNoAsync(userId, tradingAccountNo);
            result.Add(SetFactory.NewSetAccountSummaryResponse(accountBalance));
        }
        else
        {
            var accountBalances = await setQueries.GetAccountSummariesByUserIdAsync(userId);
            result.AddRange(accountBalances.Select(q =>
            {
                return q switch
                {
                    CashAccountSummary cashAccountSummary =>
                        SetFactory.NewSetAccountSummaryResponse(cashAccountSummary),
                    CreditBalanceAccountSummary creditBalanceAccountSummary => SetFactory.NewSetAccountSummaryResponse(
                        creditBalanceAccountSummary),
                    _ => throw new ArgumentOutOfRangeException(nameof(q), q, null)
                };
            }));
        }

        return Ok(new ApiResponse<List<SetAccountSummaryResponse>>(result));
    }

    [HttpGet("secure/accounts/cash/balance/assets")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<SetAccountAssetsResponse>>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetCashBalanceAssets([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromQuery][Required] string tradingAccountNo)
    {
        var assets = await setQueries.GetCashBalancePositionsByTradingAccountNoAsync(userId, tradingAccountNo);
        var response = assets.Select(SetFactory.NewSetEquityAssetResponse).ToList();
        return Ok(new ApiResponse<List<SetAccountAssetsResponse>>(response));
    }

    [HttpGet("secure/accounts/cash/balance/info")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SetAccountCashBalanceInfoResponse>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetCashBalanceInfo([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromQuery][Required] string tradingAccountNo)
    {
        var accountCashBalance =
            await setQueries.GetCashAccountInfoByTradingAccountNoAsync(userId, tradingAccountNo);
        return Ok(new ApiResponse<SetAccountCashBalanceInfoResponse>(
            SetFactory.NewSetAccountBalanceInfoResponse(accountCashBalance)));
    }

    [HttpGet("secure/accounts/credit/balance/assets")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<SetAccountAssetsResponse>>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetCreditBalanceAssets([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromQuery][Required] string tradingAccountNo)
    {
        var assets = await setQueries.GetCreditBalancePositionsByTradingAccountNoAsync(userId, tradingAccountNo);
        return Ok(new ApiResponse<List<SetAccountAssetsResponse>>(assets
            .Select(SetFactory.NewSetEquityAssetResponse)
            .ToList()));
    }

    [HttpGet("secure/accounts/credit/balance/info")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SetAccountCreditBalanceInfoResponse>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetCreditBalanceInfo([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromQuery][Required] string tradingAccountNo)
    {
        var accountCreditBalance =
            await setQueries.GetCreditAccountInfoByTradingAccountNoAsync(userId, tradingAccountNo);
        return Ok(new ApiResponse<SetAccountCreditBalanceInfoResponse>(
            SetFactory.NewSetAccountCreditBalanceInfoResponse(accountCreditBalance)));
    }

    [HttpGet("secure/orders/histories/open")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<SetOrderHistoryResponse>>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetOrderHistories([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromQuery][Required] string tradingAccountNo,
        [FromQuery] string? startDate, [FromQuery] string? endDate, [FromQuery] string? limit,
        [FromQuery] string? offSet)
    {
        // NOTE: defaultStart = now - 30 days and defaultEnd = now
        var effectiveDateFrom = startDate ?? DateTime.Now.AddDays(-30).ToString(SetFactory.DateFormat);
        var effectiveDateTo = endDate ?? DateTime.Now.ToString(SetFactory.DateFormat);
        var filters = new SetOrderHistoriesFilters(
            DateOnly.ParseExact(effectiveDateFrom, SetFactory.DateFormat, CultureInfo.InvariantCulture),
            DateOnly.ParseExact(effectiveDateTo, SetFactory.DateFormat, CultureInfo.InvariantCulture),
            limit != null ? int.Parse(limit) : 0,
            offSet != null ? int.Parse(offSet) : 0
        );
        var orders = await setQueries.GetOrderHistoriesByTradingAccountNoAsync(userId, tradingAccountNo, filters);
        var response = orders.Select(q =>
        {
            return q switch
            {
                SetOrderInfo setOrder => SetFactory.NewOrderHistoryResponse(setOrder),
                SblOrderInfo sblOrder => SetFactory.NewOrderHistoryResponse(sblOrder),
                _ => throw new ArgumentOutOfRangeException(nameof(q), q, null)
            };
        }).ToList();

        return Ok(new ApiResponse<List<SetOrderHistoryResponse>>(response));
    }

    [HttpGet("secure/orders/histories/confirm")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<SetTradeHistoryResponse>>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetTradeHistory([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromQuery][Required] string tradingAccountNo,
        [FromQuery] string? startDate,
        [FromQuery] string? endDate, [FromQuery] string? limit,
        [FromQuery] string? offSet)
    {
        // NOTE: defaultStart = now - 30 days and defaultEnd = now
        var effectiveDateFrom = startDate ?? DateTime.Now.AddDays(-30).ToString(SetFactory.DateFormat);
        var effectiveDateTo = endDate ?? DateTime.Now.ToString(SetFactory.DateFormat);
        var filters = new SetTradeHistoriesFilters(
            DateOnly.ParseExact(effectiveDateFrom, SetFactory.DateFormat, CultureInfo.InvariantCulture),
            DateOnly.ParseExact(effectiveDateTo, SetFactory.DateFormat, CultureInfo.InvariantCulture),
            limit != null ? int.Parse(limit) : 0,
            offSet != null ? int.Parse(offSet) : 0
        );

        var trades = await setQueries.GetTradeHistoriesByTradingAccountNoAsync(userId, tradingAccountNo, filters);
        var response = trades.Select(SetFactory.NewTradeHistoryResponse).ToList();

        return Ok(new ApiResponse<List<SetTradeHistoryResponse>>(response));
    }

    [HttpPost("trading/orders")]
    [HttpPost("secure/orders")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SetOrder>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> PlaceOrder([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromBody] PlaceOrderRequest request)
    {
        Ttf? ttf = null;
        bool? lending = null;
        if (request.Flag != null)
            foreach (var flag in request.Flag)
            {
                if (flag == OrderFlag.Nvdr) ttf = Ttf.Nvdr;
                if (flag == OrderFlag.Lending) lending = true;
            }

        var id = Guid.NewGuid();
        var client = bus.CreateRequestClient<CreateOrderRequest>();
        var response = await client.GetResponse<PlaceOrderSuccess, PlaceOrderFailed>(new CreateOrderRequest
        {
            CorrelationId = id,
            UserId = userId,
            TradingAccountNo = request.TradingAccountNo,
            ConditionPrice = request.OrderType,
            Quantity = request.Quantity,
            Price = request.Price,
            Action = request.Side,
            Symbol = request.Symbol,
            // TODO: Enable request condition value when implement Condition support from FE
            Condition = Condition.None,
            Ttf = ttf,
            Lending = lending
        });

        return response.Message switch
        {
            PlaceOrderSuccess order => Ok(new ApiResponse<SetOrder>(new SetOrder
            {
                OrderId = order.BrokerOrderId,
                OrderNo = order.OrderNo
            })),
            PlaceOrderFailed error => Problem(statusCode: StatusCodes.Status422UnprocessableEntity,
                title: error.SetErrorCode.ToString(), detail: error.ErrorMessage),
            _ => Problem($"Received unexpected response type {response.Message.GetType()}")
        };
    }

    [HttpPatch("trading/orders/change")]
    [HttpPatch("secure/orders/change")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SetOrder>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ChangeOrder([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromBody] Models.ChangeOrderRequest changeOrderRequest)
    {
        var client = bus.CreateRequestClient<ChangeOrderRequest>();
        var response = await client.GetResponse<ChangeOrderResponse>(new ChangeOrderRequest
        {
            UserId = userId,
            TradingAccountNo = changeOrderRequest.TradingAccountNo,
            BrokerOrderId = changeOrderRequest.OrderId,
            Price = changeOrderRequest.Price,
            Volume = changeOrderRequest.Volume,
            Ttf = changeOrderRequest.Flag != null && changeOrderRequest.Flag.Contains(OrderFlag.Nvdr)
                ? Ttf.Nvdr
                : null
        });

        return Ok(new ApiResponse<SetOrder>(new SetOrder
        {
            OrderId = response.Message.BrokerOrderId
        }));
    }

    [HttpPost("trading/orders/cancel")]
    [HttpPost("secure/orders/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SetOrder>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CancelOrder([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromBody] Models.CancelOrderRequest cancelOrderRequest)
    {
        var client = bus.CreateRequestClient<CancelOrderRequest>();
        var response = await client.GetResponse<CancelOrderResponse>(new CancelOrderRequest
        {
            UserId = userId,
            BrokerOrderId = cancelOrderRequest.OrderId,
            TradingAccountNo = cancelOrderRequest.TradingAccountNo
        });

        return Ok(new ApiResponse<SetOrder>(new SetOrder
        {
            OrderId = response.Message.BrokerOrderId
        }));
    }

    [HttpGet("secure/instruments/equity/margin-rate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<MarginRateResponse>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetMarginRate(
        [FromQuery][Required] string symbol)
    {
        var response = await setQueries.GetMarginRateBySymbol(symbol);
        return Ok(new ApiResponse<MarginRateResponse>(SetFactory.NewMarginRateResponse(response)));
    }
}
