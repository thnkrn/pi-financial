using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Application.Factories;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Metric;
using Pi.Financial.FundService.Application.Models.Trading;
using Pi.Financial.FundService.Application.Options;
using Pi.Financial.FundService.Application.Services.MarketService;
using Pi.Financial.FundService.Application.Services.Metric;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Application.Utils;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Domain.Events;
using RedemptionType = Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate.RedemptionType;

namespace Pi.Financial.FundService.Application.Commands;

public record FundOrderRequest
{
    public required Guid CorrelationId { get; init; }
    public required Guid UserId { get; init; }
    public required string FundCode { get; init; }
    public required DateOnly EffectiveDate { get; init; }
    public required string TradingAccountNo { get; init; }
    public required decimal Quantity { get; init; }
}

public record SubscriptionRequest : FundOrderRequest
{
    public required string BankAccount { get; init; }
    public required string BankCode { get; init; }
    public required string? UnitHolderId { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public bool? Recurring { get; init; }
}

public record RedemptionRequest : FundOrderRequest
{
    public required string BankAccount { get; init; }
    public required string BankCode { get; init; }
    public required string UnitHolderId { get; init; }
    public required RedemptionType RedemptionType { get; init; }
    public required bool? SellAllFlag { get; init; }
}

public record SwitchingRequest : FundOrderRequest
{
    public required string CounterFundCode { get; set; }
    public required string UnitHolderId { get; init; }
    public required RedemptionType RedemptionType { get; init; }
    public required bool? SellAllFlag { get; init; }
}

public class CreateFundOrderConsumer : IConsumer<SubscriptionRequest>, IConsumer<RedemptionRequest>, IConsumer<SwitchingRequest>
{
    private readonly IMarketService _marketservice;
    private readonly IOnboardService _onboardService;
    private readonly IOptions<FundTradingOptions> _options;
    private readonly ILogger<CreateFundOrderConsumer> _logger;
    private readonly IUserService _userService;
    private readonly IMetricService _metricService;

    public CreateFundOrderConsumer(ILogger<CreateFundOrderConsumer> logger,
        IMarketService marketService,
        IUserService userService,
        IOnboardService onboardService,
        IOptions<FundTradingOptions> options,
        IMetricService metricService)
    {
        _logger = logger;
        _marketservice = marketService;
        _userService = userService;
        _onboardService = onboardService;
        _metricService = metricService;
        _options = options;
    }

    public async Task Consume(ConsumeContext<SubscriptionRequest> context)
    {
        try
        {
            var payload = context.Message;
            ValidatePayload(payload);

            if (payload.PaymentMethod != PaymentMethod.Ats)
            {
                _logger.LogError("Unsupported payment method {PayloadPaymentMethod}", payload.PaymentMethod);
                throw new FundOrderException(FundOrderErrorCode.FOE001);
            }

            var fundInfo = await FetchFundInfo(context);

            if (fundInfo.TaxType != null && _options.Value.SubscriptionAvoidTaxTypes.Contains(fundInfo.TaxType.ToUpper()))
            {
                _logger.LogError("This fund can't buy anymore: {FundCode}", payload.FundCode);
                throw new FundOrderException(FundOrderErrorCode.FOE110);
            }

            ValidateCutOffTime(payload.EffectiveDate, fundInfo.PiBuyCutOffTime);

            if (fundInfo.FirstMinBuyAmount > _options.Value.MinBuyAmountLimit ||
                fundInfo.NextMinBuyAmount > _options.Value.MinBuyAmountLimit)
            {
                _logger.LogError("Fund min buy amount exceed limit: {ValueMinBuyAmountLimit}",
                    _options.Value.MinBuyAmountLimit);
                throw new FundOrderException(FundOrderErrorCode.FOE003);
            }

            var (custCode, customerDetail) = await FetchAccountInfo(context);

            await context.Publish(new SubscriptionFundRequestReceived
            {
                CorrelationId = payload.CorrelationId,
                TradingAccountId = customerDetail.Id,
                UnitHolderId = payload.UnitHolderId,
                FundCode = payload.FundCode,
                Channel = Channel.MOB,
                EffectiveDate = payload.EffectiveDate,
                BankAccount = payload.BankAccount,
                BankCode = payload.BankCode,
                TradingAccountNo = payload.TradingAccountNo,
                CustomerCode = custCode,
                Amount = payload.Quantity,
                PaymentType = PaymentType.AtsSa,
                Recurring = payload.Recurring ?? false
            }, publishContext =>
            {
                publishContext.RequestId = context.RequestId;
                publishContext.ResponseAddress = context.ResponseAddress;
            });
        }
        catch (FundOrderException exception)
        {
            _metricService.Send(Metrics.FundOrderError, 1, MetricFactory.NewFundOrderTag(
                GetMetricTags(OrderSide.Buy, FundOrderType.Subscription, exception.Code)));
            throw;
        }

    }

    public async Task Consume(ConsumeContext<RedemptionRequest> context)
    {
        var payload = context.Message;
        try
        {
            ValidatePayload(payload);
            var fundInfo = await FetchFundInfo(context);

            if (fundInfo.TaxType != null && _options.Value.SegTaxTypes.Contains(fundInfo.TaxType))
            {
                _logger.LogError(
                    "This fund has tax benefits: {FundCode}, Please contact your Marketing to check the term of redemption",
                    payload.FundCode);
                throw new FundOrderException(FundOrderErrorCode.FOE105);
            }

            ValidateCutOffTime(payload.EffectiveDate, fundInfo.PiSellCutOffTime);

            var (custCode, customerDetail) = await FetchAccountInfo(context);

            await context.Publish(new RedemptionFundRequestReceived
            {
                CorrelationId = payload.CorrelationId,
                TradingAccountId = customerDetail.Id,
                UnitHolderId = payload.UnitHolderId,
                FundCode = payload.FundCode,
                Channel = Channel.MOB,
                EffectiveDate = payload.EffectiveDate,
                BankAccount = payload.BankAccount,
                BankCode = payload.BankCode,
                TradingAccountNo = payload.TradingAccountNo,
                CustomerCode = custCode,
                Unit = payload.RedemptionType == RedemptionType.Unit ? payload.Quantity : null,
                Amount = payload.RedemptionType == RedemptionType.Amount ? payload.Quantity : null,
                RedemptionType = payload.RedemptionType,
                SellAllFlag = payload.SellAllFlag
            }, publishContext =>
            {
                publishContext.RequestId = context.RequestId;
                publishContext.ResponseAddress = context.ResponseAddress;
            });
        }
        catch (FundOrderException exception)
        {
            _metricService.Send(Metrics.FundOrderError, 1, MetricFactory.NewFundOrderTag(
                GetMetricTags(OrderSide.Sell, FundOrderType.Redemption, exception.Code, payload.RedemptionType)));
            throw;
        }
    }

    public async Task Consume(ConsumeContext<SwitchingRequest> context)
    {
        var payload = context.Message;
        try
        {
            ValidatePayload(payload);
            var fundInfo = await FetchFundInfo(context);

            ValidateCutOffTime(payload.EffectiveDate, fundInfo.PiSellCutOffTime);

            var (custCode, customerDetail) = await FetchAccountInfo(context);

            await context.Publish(new SwitchingFundRequestReceived
            {
                TradingAccountId = customerDetail.Id,
                UnitHolderId = payload.UnitHolderId,
                CorrelationId = payload.CorrelationId,
                FundCode = payload.FundCode,
                EffectiveDate = payload.EffectiveDate,
                TradingAccountNo = payload.TradingAccountNo,
                CustomerCode = custCode,
                Unit = payload.RedemptionType == RedemptionType.Unit
                    ? payload.Quantity
                    : null,
                Amount = payload.RedemptionType == RedemptionType.Amount
                    ? payload.Quantity
                    : null,
                RedemptionType = payload.RedemptionType,
                SellAllFlag = payload.SellAllFlag,
                CounterFundCode = payload.CounterFundCode,
                Channel = Channel.MOB
            }, publishContext =>
            {
                publishContext.RequestId = context.RequestId;
                publishContext.ResponseAddress = context.ResponseAddress;
            });
        }
        catch (FundOrderException exception)
        {
            _metricService.Send(Metrics.FundOrderError, 1, MetricFactory.NewFundOrderTag(
                GetMetricTags(OrderSide.Switch, FundOrderType.SwitchOut, exception.Code, payload.RedemptionType)));
            throw;
        }
    }

    private void ValidateCutOffTime(DateOnly effectiveDate, DateTime cutOffTime)
    {
        if (DateTime.UtcNow < cutOffTime || DateUtils.GetThToday() != effectiveDate) return;

        _logger.LogError("Too close cut off time (cut off time={CutOffTime})", cutOffTime);
        throw new FundOrderException(FundOrderErrorCode.FOE112);
    }

    private async Task<(string custCode, TradingAccountInfo customerDetail)> FetchAccountInfo(ConsumeContext<FundOrderRequest> context)
    {
        var custCode = TradingAccountHelper.GetCustCodeByFundTradingAccountNo(context.Message.TradingAccountNo);

        if (custCode == null)
        {
            throw new FundOrderException(FundOrderErrorCode.FOE102);
        }

        var userCustCodes = await _userService.GetCustomerCodesByUserId(context.Message.UserId);
        if (!userCustCodes.Contains(custCode))
        {
            _logger.LogError("Invalid user id");
            throw new FundOrderException(FundOrderErrorCode.FOE102);
        }

        var customerDetail = await _onboardService.GetMutualFundTradingAccountByCustCodeAsync(custCode, context.CancellationToken);

        if (customerDetail == null)
        {
            _logger.LogError("Trading account cannot be found");
            throw new FundOrderException(FundOrderErrorCode.FOE102);
        }

        return (custCode, customerDetail);
    }

    private async Task<FundInfo> FetchFundInfo(ConsumeContext<FundOrderRequest> context)
    {
        var fundInfo = await _marketservice.GetFundInfoByFundCodeAsync(context.Message.FundCode, context.CancellationToken);

        if (fundInfo == null)
        {
            _logger.LogError("Fund info cannot be found: {FundCode}", context.Message.FundCode);
            throw new FundOrderException(FundOrderErrorCode.FOE101);
        }

        return fundInfo;
    }

    private void ValidatePayload(FundOrderRequest payload)
    {
        if (payload.Quantity <= 0)
        {
            _logger.LogError("Invalid quantity");
            throw new FundOrderException(FundOrderErrorCode.FOE002);
        }
    }

    private MetricTags GetMetricTags(OrderSide orderSide, FundOrderType orderType, FundOrderErrorCode? errorCode = null,
        RedemptionType? redemptionType = null)
    {
        return new MetricTags
        {
            OrderSide = orderSide,
            RedemptionType = redemptionType,
            OrderType = orderType,
            ErrorCode = errorCode
        };
    }
}
