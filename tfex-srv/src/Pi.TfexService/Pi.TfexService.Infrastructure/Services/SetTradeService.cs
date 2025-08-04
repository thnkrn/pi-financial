using System.Net;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Financial.Client.Settrade;
using Pi.Financial.Client.SetTradeOms.Api;
using Pi.Financial.Client.SetTradeOms.Model;
using Pi.TfexService.Application.Commands;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Application.Utils;
using Pi.TfexService.Domain.Exceptions;
using Pi.TfexService.Domain.Models;
using Pi.TfexService.Infrastructure.Models;
using Pi.TfexService.Infrastructure.Options;
using Polly;
using ApiException = Pi.Financial.Client.SetTradeOms.Client.ApiException;
using Currency = Pi.TfexService.Application.Services.SetTrade.Currency;
using PlaceOrderRequest = Pi.Financial.Client.SetTradeOms.Model.PlaceOrderRequest;
using Portfolio = Pi.TfexService.Application.Services.SetTrade.Portfolio;
using PortfolioResponse = Pi.TfexService.Application.Services.SetTrade.PortfolioResponse;
using Position = Pi.TfexService.Application.Models.Position;
using PriceType = Pi.TfexService.Application.Models.PriceType;
using SecurityType = Pi.TfexService.Application.Services.SetTrade.SecurityType;
using Side = Pi.TfexService.Application.Models.Side;
using TotalPortfolio = Pi.TfexService.Application.Services.SetTrade.TotalPortfolio;
using TriggerCondition = Pi.TfexService.Application.Models.TriggerCondition;
using TriggerSession = Pi.TfexService.Application.Models.TriggerSession;

namespace Pi.TfexService.Infrastructure.Services;

public class SetTradeService : ISetTradeService
{
    private readonly SetTradeOptions _options;
    private readonly IDistributedCache _cache;
    private readonly ISetTradeOmsApi _setTradeOmsApi;
    private readonly IBus _bus;
    private readonly string _brokerId;
    private readonly ILogger<SetTradeService> _logger;
    private static readonly List<string> EntryTimeStatus = ["S", "SX", "SXCH", "MP", "M", "OF", "W"];
    private static readonly List<string> ActiveOrderStatus = ["S", "SX", "SXCH", "MP", "OF", "W"];

    public SetTradeService(
        ILogger<SetTradeService> logger,
        IOptionsSnapshot<SetTradeOptions> options,
        IDistributedCache cache,
        ISetTradeOmsApi setTradeOmsApi,
        IBus bus)
    {
        _logger = logger;
        _options = options.Value;
        _cache = cache;
        _setTradeOmsApi = setTradeOmsApi;
        _bus = bus;

        _brokerId = _options.BrokerId;
    }

    public async Task<AccountInfo> GetAccountInfo(string accountCode, CancellationToken cancellationToken)
    {
        try
        {
            return await WithRetryAuthToken(async () =>
            {
                var brokerId = _options.BrokerId;
                var accessToken = await GetAccessToken();
                var authorization = $"Bearer {accessToken}";

                var resp = await _setTradeOmsApi.GetAccountInfoAsync(brokerId, accountCode, authorization,
                    cancellationToken);

                return new AccountInfo(
                    resp.CreditLine,
                    resp.ExcessEquity,
                    resp.CashBalance,
                    resp.Equity,
                    resp.TotalMR,
                    resp.TotalMM,
                    resp.TotalFM,
                    resp.CallForceFlag,
                    resp.CallForceMargin,
                    resp.LiquidationValue,
                    resp.DepositWithdrawal,
                    resp.CallForceMarginMM,
                    resp.InitialMargin,
                    resp.ClosingMethod
                );
            });
        }
        catch (Exception e)
        {
            throw HandleException(e, "GetAccountInfo", accountCode);
        }
    }

    public async Task<PortfolioResponse> GetPortfolio(string accountCode, CancellationToken cancellationToken)
    {
        try
        {
            return await WithRetryAuthToken(async () =>
            {
                var accessToken = await GetAccessToken();
                var authorization = $"Bearer {accessToken}";

                var resp = await _setTradeOmsApi.GetPortfolioInvestorAsync(_brokerId, accountCode, authorization, null,
                    cancellationToken);

                return new PortfolioResponse(
                    resp.PortfolioList.Select(p => new Portfolio(
                        p.BrokerId,
                        p.AccountNo,
                        p.Symbol,
                        p.Underlying,
                        (SecurityType)p.SecurityType!.Value,
                        p.LastTradingDate,
                        p.Multiplier,
                        (Currency)p.Currency!.Value,
                        p.CurrentXRT,
                        p.AsOfDateXRT,
                        p.HasLongPosition,
                        p.StartLongPosition,
                        p.ActualLongPosition,
                        p.AvailableLongPosition,
                        p.StartLongPrice,
                        p.StartLongCost,
                        p.LongAvgPrice,
                        p.LongAvgCost,
                        p.ShortAvgCostTHB,
                        p.LongAvgCostTHB,
                        p.OpenLongPosition,
                        p.CloseLongPosition,
                        p.StartXRTLong,
                        p.StartXRTLongCost,
                        p.AvgXRTLong,
                        p.AvgXRTLongCost,
                        p.HasShortPosition,
                        p.StartShortPosition,
                        p.ActualShortPosition,
                        p.AvailableShortPosition,
                        p.StartShortPrice,
                        p.StartShortCost,
                        p.ShortAvgPrice,
                        p.ShortAvgCost,
                        p.OpenShortPosition,
                        p.CloseShortPosition,
                        p.StartXRTShort,
                        p.StartXRTShortCost,
                        p.AvgXRTShort,
                        p.AvgXRTShortCost,
                        p.MarketPrice,
                        p.RealizedPL,
                        p.RealizedPLByCost,
                        p.RealizedPLCurrency,
                        p.RealizedPLByCostCurrency,
                        p.ShortAmount,
                        p.LongAmount,
                        p.ShortAmountByCost,
                        p.LongAmountByCost,
                        p.PriceDigit,
                        p.SettleDigit,
                        p.LongUnrealizePL,
                        p.LongUnrealizePLByCost,
                        p.LongPercentUnrealizePL,
                        p.LongPercentUnrealizePLByCost,
                        p.LongOptionsValue,
                        p.LongMarketValue,
                        p.ShortUnrealizePL,
                        p.ShortPercentUnrealizePL,
                        p.ShortUnrealizePLByCost,
                        p.ShortPercentUnrealizePLByCost,
                        p.ShortOptionsValue,
                        p.ShortMarketValue,
                        p.LongAvgPriceTHB,
                        p.ShortAvgPriceTHB,
                        p.ShortAmountCurrency,
                        p.LongAmountCurrency,
                        p.LongMarketValueCurrency,
                        p.ShortMarketValueCurrency,
                        p.LongUnrealizePLCurrency,
                        p.ShortUnrealizePLCurrency,
                        p.LongUnrealizedPLByCostCurrency,
                        p.ShortUnrealizedPLByCostCurrency,
                        p.LongAmountByCostCurrency,
                        p.ShortAmountByCostCurrency)).ToList(),
                    new TotalPortfolio(
                        resp.TotalPortfolio.Amount,
                        resp.TotalPortfolio.MarketValue,
                        resp.TotalPortfolio.AmountByCost,
                        resp.TotalPortfolio.UnrealizePL,
                        resp.TotalPortfolio.UnrealizePLByCost,
                        resp.TotalPortfolio.RealizePL,
                        resp.TotalPortfolio.RealizePLByCost,
                        resp.TotalPortfolio.PercentUnrealizePL,
                        resp.TotalPortfolio.PercentUnrealizePLByCost,
                        resp.TotalPortfolio.OptionsValue
                    ));
            });
        }
        catch (Exception e)
        {
            throw HandleException(e, "GetPortfolio", accountCode);
        }
    }

    public async Task<PaginatedSetTradeOrder> GetOrders(
        string accountCode,
        int page,
        int pageSize,
        string? sort,
        Side? side = null,
        DateOnly? dateFrom = null,
        DateOnly? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await WithRetryAuthToken(async () =>
            {
                var accessToken = await GetAccessToken();
                var authorization = $"Bearer {accessToken}";

                var orderResponseTask = _setTradeOmsApi.ListOrderByAccountNoInvestorAsync(_brokerId, accountCode,
                    authorization, sort, cancellationToken);
                var tradeResponseTask = _setTradeOmsApi.ListTradeInvestorAsync(_brokerId, accountCode, authorization,
                    sort, cancellationToken);

                await Task.WhenAll(orderResponseTask, tradeResponseTask).WithAggregatedExceptions();

                var orderResponse = await orderResponseTask;
                var tradeResponse = await tradeResponseTask;

                orderResponse.RemoveAll(order =>
                {
                    if (side.HasValue && order.Side?.ToSide() != side)
                    {
                        return true;
                    }

                    if (EntryTimeStatus.Contains(order.Status))
                    {
                        if (dateFrom.HasValue && order.EntryTime.HasValue && DateOnly.FromDateTime(order.EntryTime.Value) < dateFrom)
                        {
                            return true;
                        }
                        if (dateTo.HasValue && order.EntryTime.HasValue && DateOnly.FromDateTime(order.EntryTime.Value) > dateTo)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (dateFrom.HasValue && order.CancelTime.HasValue && DateOnly.FromDateTime(order.CancelTime.Value) < dateFrom)
                        {
                            return true;
                        }
                        if (dateTo.HasValue && order.CancelTime.HasValue && DateOnly.FromDateTime(order.CancelTime.Value) > dateTo)
                        {
                            return true;
                        }
                    }

                    return false;
                });

                var (orders, hasNextPage) = WithPagination(orderResponse, page, pageSize);

                var orderList = orders
                    .Select(order =>
                    {
                        var tradeData = tradeResponse.Find(t => t.OrderNo == order.OrderNo);
                        return MapSetTradeOrder(order, tradeData?.Px ?? 0);
                    })
                    .ToList();

                return new PaginatedSetTradeOrder(orderList, hasNextPage);
            });
        }
        catch (Exception e)
        {
            throw HandleException(e, "GetOrders", accountCode);
        }
    }

    public async Task<List<SetTradeOrder>> GetActiveOrders(
        string accountCode,
        string? sort,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await WithRetryAuthToken(async () =>
            {
                var accessToken = await GetAccessToken();
                var authorization = $"Bearer {accessToken}";

                var orderResponseTask = _setTradeOmsApi.ListOrderByAccountNoInvestorAsync(_brokerId, accountCode,
                    authorization, sort, cancellationToken);
                var tradeResponseTask = _setTradeOmsApi.ListTradeInvestorAsync(_brokerId, accountCode, authorization,
                    sort, cancellationToken);

                await Task.WhenAll(orderResponseTask, tradeResponseTask).WithAggregatedExceptions();

                var orderResponse = await orderResponseTask;
                var tradeResponse = await tradeResponseTask;

                orderResponse.RemoveAll(order => !ActiveOrderStatus.Contains(order.Status));

                var response = orderResponse
                    .Select(order =>
                    {
                        var tradeData = tradeResponse.Find(t => t.OrderNo == order.OrderNo);
                        return MapSetTradeOrder(order, tradeData?.Px ?? 0);
                    })
                    .ToList();

                return response;
            });
        }
        catch (Exception e)
        {
            throw HandleException(e, "GetActiveOrders", accountCode);
        }
    }

    public async Task<SetTradeOrder> GetOrderByNo(string accountCode, long orderNo, CancellationToken cancellationToken)
    {
        try
        {
            return await WithRetryAuthToken(async () =>
            {
                var authorization = await GetAuthorizationHeader();

                var orderResponseTask = _setTradeOmsApi.GetOrderInvestorAsync(_brokerId, accountCode, orderNo,
                    authorization,
                    cancellationToken);
                var tradeResponseTask = _setTradeOmsApi.ListTradeInvestorAsync(_brokerId, accountCode, authorization,
                    null, cancellationToken);

                await Task.WhenAll(orderResponseTask, tradeResponseTask).WithAggregatedExceptions();

                var orderResponse = await orderResponseTask;
                var tradeResponse = await tradeResponseTask;

                var tradeData = tradeResponse.Find(t => t.OrderNo == orderResponse.OrderNo);
                var order = MapSetTradeOrder(orderResponse, tradeData?.Px ?? 0);

                return order;
            });
        }
        catch (Exception e)
        {
            throw HandleException(e, "GetOrder", accountCode);
        }
    }

    /// <summary>
    /// GetTrades - For Debug Only
    /// </summary>
    public async Task<List<TradeResponse>> GetTrades(string accountCode, string? sort,
        CancellationToken cancellationToken)
    {
        try
        {
            return await WithRetryAuthToken(async () =>
            {
                var authorization = await GetAuthorizationHeader();

                var resp = await _setTradeOmsApi.ListTradeInvestorAsync(_brokerId, accountCode, authorization,
                    sort, cancellationToken);

                return resp;
            });
        }
        catch (Exception e)
        {
            throw HandleException(e, "GetTrades", accountCode);
        }
    }

    public async Task<bool> CancelOrder(string accountCode, long orderNo, CancellationToken cancellationToken = default)
    {
        try
        {
            return await WithRetryAuthToken(async () =>
            {
                var authorization = await GetAuthorizationHeader();

                await _setTradeOmsApi.CancelOrderInvestorAsync(_brokerId, accountCode, orderNo, authorization,
                    cancellationToken);

                return true;
            });
        }
        catch (Exception e)
        {
            throw HandleException(e, "CancelOrder", accountCode);
        }
    }

    public async Task<bool> UpdateOrder(string accountCode, long orderNo, decimal? price, int? volume,
        bool bypassWarning = true, CancellationToken cancellationToken = default)
    {
        try
        {
            return await WithRetryAuthToken(async () =>
            {
                var authorization = await GetAuthorizationHeader();
                var changeOrderRequest = new ChangeOrderRequestV3(price, volume, bypassWarning);

                await _setTradeOmsApi.ChangeOrderInvestorAsync(_brokerId, accountCode, orderNo, authorization,
                    changeOrderRequest, cancellationToken);

                return true;
            });
        }
        catch (Exception e)
        {
            throw HandleException(e, "UpdateOrder", accountCode);
        }
    }

    public async Task<SetTradeOrder> PlaceOrder(string userId, string customerCode, string accountCode,
        SetTradePlaceOrderRequest.PlaceOrderInfo placeOrderInfo,
        CancellationToken cancellationToken)
    {
        // for logging purpose
        string? requestJson = null;
        string? responseJson = null;
        PlaceOrderRequest? orderRequest = null;
        SetTradeOrder? orderResponse = null;
        string? orderNo = null;
        DateTime? requestedAt = null;
        DateTime? completedAt = null;
        bool isSuccess = false;
        string? failedReason = null;
        try
        {
            return await WithRetryAuthToken(async () =>
            {
                var authorization = await GetAuthorizationHeader();

                orderRequest = new PlaceOrderRequest(
                    placeOrderInfo.Series,
                    placeOrderInfo.Side.ToSide(),
                    placeOrderInfo.Position.ToPosition(),
                    placeOrderInfo.PriceType.ToPriceType(),
                    placeOrderInfo.Price,
                    placeOrderInfo.Volume,
                    placeOrderInfo.IcebergVol,
                    placeOrderInfo.ValidityType?.ToValidityType() ?? 0,
                    placeOrderInfo.ValidityDateCondition ?? string.Empty,
                    placeOrderInfo.StopCondition?.ToTriggerCondition(),
                    (placeOrderInfo.StopCondition.HasValue ? placeOrderInfo.StopSymbol : null)!,
                    placeOrderInfo.StopPrice,
                    placeOrderInfo.TriggerSession?.ToTriggerSession(),
                    placeOrderInfo.BypassWarning ?? true
                );

                // stamp request time for logging purpose
                requestedAt = DateTime.UtcNow;
                requestJson = orderRequest.ToJson();

                var response =
                    await _setTradeOmsApi.PlaceOrderInvestorAsync(_brokerId, accountCode, authorization, orderRequest,
                        cancellationToken);

                // map request and response to json and stamp complete time for logging purpose
                completedAt = DateTime.UtcNow;
                orderNo = response.OrderNo.ToString();
                responseJson = response.ToJson();
                isSuccess = true;

                orderResponse = MapSetTradeOrder(response, 0);
                return orderResponse;
            });
        }
        catch (Exception e)
        {
            failedReason = e.Message;
            requestJson = orderRequest?.ToJson();
            throw HandleException(e, "PlaceOrder", accountCode);
        }
        finally
        {
            await Log(new LogActivitiesRequest(
                userId,
                customerCode,
                accountCode,
                RequestType.PlaceOrder,
                requestJson,
                orderNo,
                responseJson,
                requestedAt,
                completedAt,
                isSuccess,
                failedReason,
                orderResponse
            ), cancellationToken);
        }
    }

    public async Task<SettradeStreamResponse> GetSetTradeStreamInfo(CancellationToken cancellationToken)
    {
        try
        {
            return await WithRetryAuthToken(async () =>
            {
                var authorization = await GetAuthorizationHeader();
                return await _setTradeOmsApi.GetSetTradeStreamUrlAndTokenAsync(_brokerId, authorization,
                    cancellationToken);
            });
        }
        catch (Exception e)
        {
            throw HandleException(e, "GetSetTradeStreamInfo");
        }
    }

    public async Task<string> GetAccessToken(bool forceGenerateNewToken = false)
    {
        if (!forceGenerateNewToken)
        {
            var accessToken = await _cache.GetStringAsync(CacheKeys.SetTradeAccessToken);
            if (accessToken != null)
            {
                return accessToken;
            }
        }

        return await GenerateAccessToken();
    }

    private async Task<string> GetAuthorizationHeader()
    {
        var accessToken = await GetAccessToken();
        return $"Bearer {accessToken}";
    }

    private async Task<string> GenerateAccessToken()
    {
        var authTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var authResponse = await Auth("", authTime);
        var expireIn = authResponse.ExpiresIn;
        var accessToken = authResponse.AccessToken;

        var cacheEntryOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(expireIn));

        await _cache.SetStringAsync(CacheKeys.SetTradeAccessToken, accessToken, cacheEntryOptions);

        return accessToken;
    }

    private async Task<SetTradeAuthResponse> Auth(string authParam, long timestamp)
    {
        try
        {
            var appCode = _options.Application;
            var apiKey = _options.ApiKey;
            var appSecret = _options.AppSecret;
            var signature = SetTradeSignature.CreateSignature(appSecret, apiKey, authParam, timestamp);

            var authRequest = new SettradeAuthRequest(apiKey, authParam, signature, timestamp.ToString());
            var authResponse = await _setTradeOmsApi.AuthLoginAsync(_brokerId, appCode, authRequest);

            return new SetTradeAuthResponse(true, authResponse.TokenType, authResponse.AccessToken,
                authResponse.RefreshToken, authResponse.ExpiresIn);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "SetTrade Authentication: Failed to log into SetTrade. Error: {Message}", e.Message);
            throw new SetTradeAuthException("Internal Errors.");
        }
    }

    private async Task<T> WithRetryAuthToken<T>(Func<Task<T>> function)
    {
        // migrate to v8 syntax
        // var context = ResilienceContextPool.Shared.Get();
        // var pipeline = new ResiliencePipelineBuilder()
        //     .AddRetry(new RetryStrategyOptions
        //     {
        //         ShouldHandle = new PredicateBuilder()
        //             .Handle<ApiException>(response =>
        //                 response.ErrorCode is (int)HttpStatusCode.Unauthorized or (int)HttpStatusCode.Forbidden
        //                 || HandleApiException(response, "") is SetTradeAuthException)
        //             .HandleInner<ApiException>(response =>
        //                 response.ErrorCode is (int)HttpStatusCode.Unauthorized or (int)HttpStatusCode.Forbidden
        //                 || HandleApiException(response, "") is SetTradeAuthException),
        //         MaxRetryAttempts = _options.AuthMaxRetry,
        //         OnRetry = async _ => { await GenerateAccessToken(); }
        //     })
        //     .Build();

        // still cannot figure out how to execute async using function as a parameter instead of invoke that function here

        var policy = await Policy
            .Handle<ApiException>(response =>
                response.ErrorCode is (int)HttpStatusCode.Unauthorized or (int)HttpStatusCode.Forbidden
                || HandleApiException(response, "ApiCall") is SetTradeAuthException)
            .OrInner<ApiException>(response =>
                response.ErrorCode is (int)HttpStatusCode.Unauthorized or (int)HttpStatusCode.Forbidden
                || HandleApiException(response, "ApiCall") is SetTradeAuthException)
            .RetryAsync(_options.AuthMaxRetry, async (_, _, _) => { await GenerateAccessToken(); })
            .ExecuteAndCaptureAsync(function);

        if (policy.Outcome == OutcomeType.Successful)
        {
            return policy.Result;
        }

        throw policy.FinalException;
    }

    private (List<T> Items, bool HasNextPage) WithPagination<T>(List<T> source, int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        if (skip >= source.Count)
        {
            return (new List<T>(), false);
        }

        var take = Math.Min(pageSize, source.Count - skip);
        var paginatedList = source.Skip(skip).Take(take).ToList();
        var hasNextPage = skip + take < source.Count;

        return (paginatedList, hasNextPage);
    }

    private static SetTradeOrder MapSetTradeOrder(OrderResponse orderResponse, decimal matchedPrice)
    {
        return new SetTradeOrder(
            orderResponse.OrderNo,
            orderResponse.TfxOrderNo,
            orderResponse.AccountNo,
            orderResponse.EntryId,
            orderResponse.EntryTime,
            orderResponse.TradeDate,
            orderResponse.TransactionTime,
            orderResponse.CancelId,
            orderResponse.CancelTime,
            orderResponse.Symbol,
            orderResponse.Side.HasValue ? (Side)orderResponse.Side : null,
            orderResponse.Position.HasValue ? (Position)orderResponse.Position : null,
            orderResponse.PriceType.HasValue ? (PriceType)orderResponse.PriceType : null,
            orderResponse.Price,
            matchedPrice,
            orderResponse.Qty,
            orderResponse.IcebergVol,
            orderResponse.BalanceQty,
            orderResponse.MatchQty,
            orderResponse.CancelQty,
            orderResponse.Validity.HasValue ? (Validity)orderResponse.Validity : null,
            orderResponse.ValidToDate,
            orderResponse.IsStopOrderNotActivate,
            orderResponse.TriggerCondition.HasValue ? (TriggerCondition)orderResponse.TriggerCondition : null,
            orderResponse.TriggerSymbol,
            orderResponse.TriggerPrice,
            orderResponse.TriggerSession.HasValue ? (TriggerSession)orderResponse.TriggerSession : null,
            orderResponse.Status,
            orderResponse.ShowStatus,
            orderResponse.StatusMeaning,
            orderResponse.RejectCode,
            orderResponse.RejectReason,
            orderResponse.Cpm,
            orderResponse.TrType,
            orderResponse.TerminalType,
            orderResponse.VarVersion,
            orderResponse.CanCancel,
            orderResponse.CanChange,
            orderResponse.PriceDigit,
            EntryTimeStatus.Contains(orderResponse.Status) ? orderResponse.EntryTime : orderResponse.CancelTime
        );
    }

    private Exception HandleException(Exception e, string methodName, string? accountNo = null)
    {
        if (e is ApiException apiException)
        {
            return HandleApiException(apiException, methodName, accountNo);
        }

        return new SetTradeApiException(
            $"SetTradeService {methodName}: Error while calling SetTrade for AccountNo: {accountNo}", string.Empty, e);
    }

    private Exception HandleApiException(ApiException e, string methodName, string? accountNo = null)
    {
        _logger.LogError(e, "SetTradeService Exception");

        var apiException = ApiExceptionHelper.DeserializeApiException(e.ErrorContent);

        _logger.LogError("SetTradeService {MethodName}: Account No: {AccountNo} Error while calling SetTrade: {ErrorMessage}", methodName, accountNo, apiException.ErrorMessage);

        return apiException.ErrorCode switch
        {
            "API-401" or "GWD-03" => CreateSetTradeException<SetTradeAuthException>("Access token is invalid or has expired"),
            "GWD-00" => CreateSetTradeException<SetTradeInvalidDataException>(apiException.ErrorMessage ?? "Invalid data"),
            "GWD-01" => HandleSetTradeErrorMessage(apiException.ErrorMessage),
            _ => CreateSetTradeException<SetTradeApiException>(apiException.ErrorMessage ?? "An unknown error occurred")
        };

        Exception HandleSetTradeErrorMessage(string? errorMessage)
        {
            if (errorMessage is null)
            {
                return CreateSetTradeException<SetTradeApiException>("An unknown error occurred");
            }

            return errorMessage switch
            {
                _ when errorMessage.Contains("G001: Account not found") =>
                    CreateSetTradeException<SetTradeNotFoundException>(errorMessage),
                _ when errorMessage.Contains("G001: Series not found") =>
                    CreateSetTradeException<SetTradeSeriesNotFoundException>(errorMessage),
                _ when errorMessage.Contains("G001: Order not found") =>
                    CreateSetTradeException<SetTradeNotFoundException>(errorMessage),
                _ when errorMessage.Contains("G003: Invalid price") =>
                    CreateSetTradeException<SetTradePriceOutOfRangeException>(errorMessage),
                _ when errorMessage.Contains("O001: Cannot open both long and short position in same series") =>
                    CreateSetTradeException<SetTradePlaceOrderBothSideException>(errorMessage),
                _ when errorMessage.Contains("O001: Not enough position to close") =>
                    CreateSetTradeException<SetTradeNotEnoughPositionException>(errorMessage),
                _ when errorMessage.Contains("O001: Not enough excess equity") =>
                    CreateSetTradeException<SetTradeNotEnoughExcessEquityException>(errorMessage),
                _ when errorMessage.Contains("O001: No value changed") =>
                    CreateSetTradeException<SetTradeUpdateOrderNoValueChangedException>(errorMessage),
                _ when errorMessage.Contains("O001: Not allow to trade during this time") || errorMessage.Contains("G003: Not allowed to trade") =>
                    CreateSetTradeException<SetTradeOutsideTradingHoursException>(errorMessage),
                _ when errorMessage.Contains("O001: Exceed line available") =>
                    CreateSetTradeException<SetTradeNotEnoughLineAvailableException>(errorMessage),
                _ when errorMessage.Contains("O002") && errorMessage.Contains("Price is out of range from last done") =>
                    CreateSetTradeException<SetTradePriceOutOfRangeFromLastDoneException>(errorMessage),
                _ =>
                    CreateSetTradeException<SetTradeApiException>(errorMessage)
            };
        }

        Exception CreateSetTradeException<T>(string errorMessage) where T : Exception
        {
            // Extract only the message part after the colon
            var extractedErrorMessage = errorMessage.Contains(':')
                ? errorMessage[(errorMessage.IndexOf(':') + 1)..].Trim()
                : errorMessage;

            // Create the exception with the extracted message
            return (T)Activator.CreateInstance(typeof(T), $"{extractedErrorMessage}", apiException.ErrorCode, e)!;
        }
    }

    private async Task Log(LogActivitiesRequest logActivitiesRequest, CancellationToken cancellationToken = default)
    {
        await _bus.Publish(logActivitiesRequest, cancellationToken);
    }
}
