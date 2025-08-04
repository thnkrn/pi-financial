using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.ExtensionMethods;
using Pi.Common.Http;
using Pi.Financial.FundService.API.Factories;
using Pi.Financial.FundService.API.Models;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Trading;
using Pi.Financial.FundService.Application.Queries;
using Pi.Financial.FundService.Application.Utils;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Domain.Events;
using RawFundOrder = Pi.Financial.Client.FundConnext.Model.FundOrder;

namespace Pi.Financial.FundService.API.Controllers;

[ApiController]
public class FundTradingController : ControllerBase
{
    private readonly IFundQueries _fundQueries;
    private readonly IBus _bus;
    private readonly ILogger<FundTradingController> _logger;

    public FundTradingController(IFundQueries fundQueries, IBus bus, ILogger<FundTradingController> logger)
    {
        _fundQueries = fundQueries;
        _bus = bus;
        _logger = logger;
    }

    [HttpGet("internal/accounts/summaries")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<AccountSummaryResponse[]>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetSummaries([FromHeader(Name = "user-id")][Required] Guid userId)
    {
        try
        {
            var summaries = await _fundQueries.GetAccountSummariesAsync(userId);
            var response = summaries.Select(summary =>
            {
                return new AccountSummaryResponse
                {
                    CustomerCode = summary.CustomerCode,
                    TradingAccountNo = summary.TradingAccountNo,
                    AsOfDate = summary.AsOfDate,
                    TotalMarketValue = summary.TotalMarketValue,
                    TotalCostValue = summary.TotalCostValue,
                    TotalUpnl = summary.TotalUpnl,
                    Assets = summary.Assets.Where(asset => asset.RemainUnit > 0).Select(FundTradingFactory.NewFundAssetResponse)
                };
            });

            return Ok(new ApiResponse<AccountSummaryResponse[]>(response.ToArray()));
        }
        catch (FundOrderException exception)
        {
            return HandleFundOrderException(exception);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong on GetSummaries endpoint");
            return Problem();
        }
    }

    [HttpGet("secure/accounts/assets")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SiriusFundAssetResponse[]>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetAssets([FromHeader(Name = "user-id")][Required] Guid userId, [FromQuery][Required] string tradingAccountNo)
    {
        try
        {
            var assets = await _fundQueries.GetAccountBalanceByTradingAccountNoAsync(userId, tradingAccountNo);
            var response = assets.Where(asset => asset.RemainUnit > 0).Select(FundTradingFactory.NewFundAssetResponse).ToArray();

            return Ok(new ApiResponse<SiriusFundAssetResponse[]>(response));
        }
        catch (FundOrderException exception)
        {
            return HandleFundOrderException(exception);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong on GetAssets endpoint");
            return Problem();
        }
    }

    [HttpGet("internal/accounts/assets")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<InternalFundAssetResponse>>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> InternalGetAssets([FromHeader(Name = "user-id")][Required] Guid userId, [FromQuery] string? tradingAccountNo)
    {
        try
        {
            var assets = await _fundQueries.GetAccountBalanceByTradingAccountNoAsync(userId, tradingAccountNo);

            return Ok(new ApiResponse<List<InternalFundAssetResponse>>(assets.Select(
                FundTradingFactory.NewInternalFundAssetResponse).ToList()));
        }
        catch (FundOrderException exception)
        {
            return HandleFundOrderException(exception);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong on InternalGetAssets endpoint");
            return Problem();
        }
    }

    [HttpDelete("internal/orders/delete")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<BrokerOrder>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> InternalDeleteOrder([FromHeader(Name = "user-id")][Required] Guid userId, DeleteOrderRequest deleteOrderRequest)
    {
        try
        {
            var client = _bus.CreateRequestClient<CancelFundOrderRequest>();
            var response = await client.GetResponse<CancelOrderSuccess, CancelOrderFailed>(
                new CancelFundOrderRequest
                {
                    UserId = userId,
                    BrokerOrderId = deleteOrderRequest.BrokerOrderId,
                    Force = deleteOrderRequest.Force,
                    OrderSide = deleteOrderRequest.OrderSide,
                    TradingAccountNo = deleteOrderRequest.TradingAccountNo
                });
            switch (response.Message)
            {
                case CancelOrderSuccess order:
                    return Ok(new ApiResponse<BrokerOrder>(new BrokerOrder()
                    {
                        BrokerOrderId = order.BrokerOrderId
                    }));
                case CancelOrderFailed error:
                    if (error.ErrorCode == null || !Enum.TryParse(error.ErrorCode, out FundOrderErrorCode errorCode))
                    {
                        return Problem();
                    }

                    return Problem(
                        statusCode: StatusCodes.Status422UnprocessableEntity,
                        title: errorCode.ToString(),
                        detail: errorCode.GetEnumDescription()
                    );
                default:
                    return Problem("Something went wrong");
            }
        }
        catch (RequestFaultException exception)
        {
            return HandleRequestFault(exception);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong on Cancel Fund Order endpoint");
            return Problem();
        }
    }

    [HttpGet("secure/accounts/openOrders")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SiriusFundOrderResponse[]>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetOpenOrders([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromQuery][Required] string tradingAccountNo,
        [FromQuery][Required] string effectiveDateFrom,
        [FromQuery][Required] string effectiveDateTo)
    {
        try
        {
            var filters = new FundOrderFilters(
                DateOnly.ParseExact(effectiveDateFrom, FundTradingFactory.DateFormat, CultureInfo.InvariantCulture),
                DateOnly.ParseExact(effectiveDateTo, FundTradingFactory.DateFormat, CultureInfo.InvariantCulture),
                new[] { FundOrderStatus.Submitted, FundOrderStatus.Approved, FundOrderStatus.Waiting });
            var orders = await _fundQueries.GetAccountFundOrdersByTradingAccountNoAsync(userId, tradingAccountNo, filters);
            var response = orders.Select(FundTradingFactory.NewFundOrderResponse).ToArray();

            return Ok(new ApiResponse<SiriusFundOrderResponse[]>(response));
        }
        catch (FundOrderException exception)
        {
            return HandleFundOrderException(exception);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong on GetOpenOrders endpoint");
            return Problem();
        }
    }

    [HttpGet("secure/orders/histories")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SiriusFundOrderHistoryResponse[]>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetOrdersHistory([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromQuery][Required] string tradingAccountNo,
        [FromQuery] string? beginEffectiveDate,
        [FromQuery] string? endEffectiveDate,
        [FromQuery][Required] string orderType)
    {
        var parsed = Enum.TryParse(orderType, out FundOrderType type);
        if (orderType == "" || (orderType.ToUpper() != "ALL" && !parsed))
        {
            _logger.LogError("Order type empty or can't parse");
            return BadRequest();
        }

        try
        {
            var effectiveDateFrom = beginEffectiveDate ?? DateTime.Now.AddDays(-7).ToString(FundTradingFactory.DateFormat);
            var effectiveDateTo = endEffectiveDate ?? DateTime.Now.ToString(FundTradingFactory.DateFormat);
            var filters = new FundOrderFilters(
                DateOnly.ParseExact(effectiveDateFrom, FundTradingFactory.DateFormat, CultureInfo.InvariantCulture),
                DateOnly.ParseExact(effectiveDateTo, FundTradingFactory.DateFormat, CultureInfo.InvariantCulture),
                new[] { FundOrderStatus.Allotted, FundOrderStatus.Rejected, FundOrderStatus.Expired, FundOrderStatus.Cancelled },
                orderType.ToUpper() != "ALL" ? type : null);
            var orders = await _fundQueries.GetAccountFundOrdersByTradingAccountNoAsync(userId, tradingAccountNo, filters);
            var response = orders.Select(FundTradingFactory.NewFundOrderHistoryResponse).OrderByDescending(ordersHistory => ordersHistory.TransactionDateTime).ToArray();

            return Ok(new ApiResponse<SiriusFundOrderHistoryResponse[]>(response));
        }
        catch (FundOrderException exception)
        {
            return HandleFundOrderException(exception);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong on GetOrdersHistory endpoint");
            return Problem();
        }
    }

    /// <summary>
    ///     Get orders history by saOrderReferenceNo
    /// </summary>
    /// <param name="orderNumbers" example="FOSW202411260004,FOSUB202403230022">saOrderReferenceNo</param>
    [HttpGet("internal/orders/histories/orderNo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SiriusFundOrderHistoryResponse[]>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetOrdersHistoryByOrderNo([FromQuery][Required] string orderNumbers)
    {
        try
        {
            // EffectiveDateFrom and EffectiveDateTo are not used in this endpoint
            var filters = new FundOrderFilters(
                DateOnly.MinValue,
                DateOnly.MinValue,
                [FundOrderStatus.Submitted, FundOrderStatus.Approved, FundOrderStatus.Waiting, FundOrderStatus.Allotted, FundOrderStatus.Rejected, FundOrderStatus.Expired, FundOrderStatus.Cancelled]);

            var orderNoList = orderNumbers
                .Split(',')
                .Select(orderNo => orderNo.Trim())
                .Where(orderNo => !string.IsNullOrWhiteSpace(orderNo))
                .Distinct()
                .ToList();

            var orders = await _fundQueries.GetFundOrdersByOrderNoAsync(orderNoList, filters);
            var response = orders
                .Select(FundTradingFactory.NewFundOrderHistoryResponse)
                .OrderByDescending(ordersHistory => ordersHistory.TransactionDateTime)
                .ToArray();

            return Ok(new ApiResponse<SiriusFundOrderHistoryResponse[]>(response));
        }
        catch (FundOrderException exception)
        {
            return HandleFundOrderException(exception);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong on GetOrdersHistory endpoint");
            return Problem();
        }
    }

    [HttpGet("secure/orders/switch/info")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SwitchInfo>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetSwitchInfo([FromHeader(Name = "user-id")][Required] Guid userId, [FromQuery] SwitchInfoRequest switchInfoRequest)
    {
        try
        {
            var result = await _fundQueries.GetSwitchInfoByTradingAccountNoAsync(userId, switchInfoRequest.TradingAccountNo, switchInfoRequest.Symbol, switchInfoRequest.TargetSymbol);

            return Ok(new ApiResponse<SwitchInfo>(new SwitchInfo()
            {
                MinSwitchUnit = decimal.Round(result.MinSwitchUnit, 4),
                MinSwitchAmount = decimal.Round(result.MinSwitchAmount, 4)
            }));
        }
        catch (FundOrderException exception)
        {
            return HandleFundOrderException(exception);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong on GetSwitchInfo endpoint");
            return Problem();
        }
    }

    [HttpPost("trading/orders/buy")]
    [HttpPost("secure/orders/buy")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<FundOrderPlaced>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Buy([FromHeader(Name = "user-id")][Required] Guid userId,
       [FromBody] SiriusPlaceBuyOrderRequest siriusPlaceOrderRequest)
    {
        return await ProcessBuyOrder(userId, siriusPlaceOrderRequest, HandlePlaceOrderFailed);
    }

    /// <summary>
    /// Subscribes to a fund order from internal DCA, which will contain the full error message.
    /// </summary>
    [HttpPost("internal/orders/buy")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<FundOrderPlaced>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> InternalOrderBuy
    (
        [FromHeader(Name = "user-id")][Required] Guid userId,
        [FromBody] SiriusPlaceBuyOrderRequest siriusPlaceOrderRequest)
    {
        return await ProcessBuyOrder(userId, siriusPlaceOrderRequest, HandleInternalPlaceOrderFailed);
    }

    [HttpPost("trading/orders/sell")]
    [HttpPost("secure/orders/sell")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<FundOrderPlaced>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Sell([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromBody] SiriusPlaceSellOrderRequest siriusPlaceOrderRequest)
    {
        try
        {
            var client = _bus.CreateRequestClient<RedemptionRequest>();
            var response = await client.GetResponse<RedemptionFundOrder, PlaceOrderFailed>(new RedemptionRequest
            {
                CorrelationId = Guid.NewGuid(),
                UserId = userId,
                UnitHolderId = siriusPlaceOrderRequest.UnitHolderId,
                FundCode = siriusPlaceOrderRequest.Symbol,
                EffectiveDate = DateOnly.ParseExact(siriusPlaceOrderRequest.EffectiveDate,
                    FundTradingFactory.DateFormat,
                    CultureInfo.InvariantCulture),
                BankAccount = siriusPlaceOrderRequest.BankAccount,
                TradingAccountNo = siriusPlaceOrderRequest.TradingAccountNo,
                BankCode = siriusPlaceOrderRequest.BankCode,
                Quantity = siriusPlaceOrderRequest.Quantity,
                SellAllFlag = siriusPlaceOrderRequest.SellAllFlag,
                RedemptionType = siriusPlaceOrderRequest.UnitType
            });

            switch (response.Message)
            {
                case RedemptionFundOrder order:
                    return Ok(new ApiResponse<FundOrderPlaced>(new FundOrderPlaced
                    {
                        OrderNo = order.SaOrderReferenceNo,
                        UnitHolderId = order.UnitHolderId,
                        BrokerOrderId = order.TransactionId,
                        SettlementDate = order.SettlementDate.ToString(FundTradingFactory.DateFormat, CultureInfo.InvariantCulture)
                    }));
                case PlaceOrderFailed error:
                    return HandlePlaceOrderFailed(error, OrderSide.Sell);
                default:
                    return Problem("Something went wrong");
            }
        }
        catch (RequestFaultException exception)
        {
            return HandleRequestFault(exception, OrderSide.Sell);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong on Sell endpoint");
            return Problem();
        }
    }

    [HttpPost("trading/orders/switch")]
    [HttpPost("secure/orders/switch")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<FundOrderPlaced>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Switch([FromHeader(Name = "user-id")][Required] Guid userId,
        [FromBody] SiriusPlaceSwitchOrderRequest siriusPlaceOrderRequest)
    {
        if (siriusPlaceOrderRequest.TargetSymbol.All(char.IsWhiteSpace))
        {
            _logger.LogError("Target fund code can't be space");
            return BadRequest();
        }

        try
        {
            var client = _bus.CreateRequestClient<SwitchingRequest>();
            var response = await client.GetResponse<SwitchingFundOrder, PlaceOrderFailed>(new SwitchingRequest
            {
                CorrelationId = Guid.NewGuid(),
                UserId = userId,
                UnitHolderId = siriusPlaceOrderRequest.UnitHolderId,
                FundCode = siriusPlaceOrderRequest.Symbol,
                EffectiveDate = DateOnly.ParseExact(siriusPlaceOrderRequest.EffectiveDate,
                    FundTradingFactory.DateFormat,
                    CultureInfo.InvariantCulture),
                TradingAccountNo = siriusPlaceOrderRequest.TradingAccountNo,
                CounterFundCode = siriusPlaceOrderRequest.TargetSymbol,
                Quantity = siriusPlaceOrderRequest.Quantity,
                SellAllFlag = siriusPlaceOrderRequest.SellAllFlag,
                RedemptionType = siriusPlaceOrderRequest.UnitType
            });

            switch (response.Message)
            {
                case SwitchingFundOrder order:
                    return Ok(new ApiResponse<FundOrderPlaced>(new FundOrderPlaced
                    {
                        OrderNo = order.SaOrderReferenceNo,
                        UnitHolderId = order.UnitHolderId,
                        BrokerOrderId = order.TransactionId
                    }));
                case PlaceOrderFailed error:
                    return HandlePlaceOrderFailed(error, OrderSide.Switch);
                default:
                    return Problem("Something went wrong");
            }
        }
        catch (RequestFaultException exception)
        {
            return HandleRequestFault(exception, OrderSide.Switch);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong on SwitchOrder endpoint");
            return Problem();
        }
    }

    [HttpGet("internal/orders/raw-orders")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<RawFundOrder[]>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetRawFundOrders([FromQuery][Required] DateOnly effectiveDate)
    {
        try
        {
            var fundOrders = await _fundQueries.GetRawOrdersAsync(effectiveDate);
            return Ok(new ApiResponse<RawFundOrder[]>(fundOrders.ToArray()));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong on GetRawFundOrders endpoint");
            return Problem();
        }
    }

    private ActionResult HandleFundOrderException(FundOrderException exception)
    {
        _logger.LogError(exception, "Received FundOrderException with code {Code}", exception.Code.ToString());
        return Problem(
            statusCode: StatusCodes.Status422UnprocessableEntity,
            title: exception.Code.ToString(),
            detail: exception.Code.GetEnumDescription()
        );
    }

    private ActionResult HandlePlaceOrderFailed(PlaceOrderFailed error, OrderSide side)
    {
        _logger.LogError("Place order {Side} failed ({Code})", side, error.ErrorCode.ToString());

        return Problem(
            statusCode: StatusCodes.Status422UnprocessableEntity,
            title: error.ErrorCode.ToString(),
            detail: error.ErrorCode.GetEnumDescription()
        );
    }

    private ActionResult HandleInternalPlaceOrderFailed(PlaceOrderFailed error, OrderSide side)
    {
        _logger.LogError("Place order {Side} failed ({Code}) orderNo ({OrderNo})", side, error.ErrorCode.ToString(), error.OrderNo);

        return Problem(
            statusCode: StatusCodes.Status422UnprocessableEntity,
            title: $"{error.ErrorCode.ToString()}-{error.ErrorCode.ToDescriptionString()}",
            detail: error.ExceptionInfo?.Message,
            instance: error.OrderNo
        );
    }

    private ActionResult HandleRequestFault(RequestFaultException exception, OrderSide? side = null)
    {
        var errorCode = ErrorFactory.NewErrorCode(exception);
        _logger.LogError(exception, "{Side} fund order failed ({Code})", side.ToString(), errorCode.ToString());

        return errorCode != null ? Problem(
            statusCode: StatusCodes.Status422UnprocessableEntity,
            title: errorCode.ToString(),
            detail: errorCode.GetEnumDescription()
        ) : Problem(title: exception.ToString(), detail: exception.Message);
    }

    private async Task<ActionResult> ProcessBuyOrder(
        Guid userId,
        SiriusPlaceBuyOrderRequest siriusPlaceOrderRequest,
        Func<PlaceOrderFailed, OrderSide, ActionResult> handlePlaceOrderFailed)
    {
        if (siriusPlaceOrderRequest.UnitHolderId != null && siriusPlaceOrderRequest.UnitHolderId.All(char.IsWhiteSpace))
        {
            _logger.LogError("Unit holder id can't be space");
            return BadRequest();
        }

        try
        {
            var id = Guid.NewGuid();
            var client = _bus.CreateRequestClient<SubscriptionRequest>();
            var response = await client.GetResponse<SubscriptionFundOrder, PlaceOrderFailed>(new SubscriptionRequest
            {
                CorrelationId = id,
                UserId = userId,
                UnitHolderId = siriusPlaceOrderRequest.UnitHolderId?.Trim(),
                FundCode = siriusPlaceOrderRequest.Symbol,
                EffectiveDate = DateOnly.ParseExact(siriusPlaceOrderRequest.EffectiveDate,
                    FundTradingFactory.DateFormat,
                    CultureInfo.InvariantCulture),
                BankAccount = siriusPlaceOrderRequest.BankAccount,
                BankCode = siriusPlaceOrderRequest.BankCode,
                TradingAccountNo = siriusPlaceOrderRequest.TradingAccountNo,
                Quantity = siriusPlaceOrderRequest.Quantity,
                PaymentMethod = siriusPlaceOrderRequest.PaymentMethod,
                Recurring = siriusPlaceOrderRequest.Recurring ?? false
            });

            switch (response.Message)
            {
                case SubscriptionFundOrder order:
                    return Ok(new ApiResponse<FundOrderPlaced>(new FundOrderPlaced
                    {
                        OrderNo = order.SaOrderReferenceNo,
                        UnitHolderId = order.UnitHolderId,
                        BrokerOrderId = order.TransactionId,
                    }));
                case PlaceOrderFailed error:
                    // Failed from FundConnext
                    return handlePlaceOrderFailed(error, OrderSide.Buy);
                default:
                    // Should never happen
                    return Problem("Something went wrong");
            }
        }
        catch (RequestFaultException exception)
        {
            // Failed before sending to FundConnext
            return HandleRequestFault(exception, OrderSide.Buy);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong on Buy endpoint");
            return Problem();
        }
    }
}
