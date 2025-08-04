using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.SetService.Application.Constants;
using Pi.SetService.Application.Exceptions;
using Pi.SetService.Application.Factories;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Options;
using Pi.SetService.Application.Services.MarketService;
using Pi.SetService.Application.Services.NumberGeneratorService;
using Pi.SetService.Application.Services.OnboardService;
using Pi.SetService.Application.Services.OneportService;
using Pi.SetService.Application.Utils;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using TradingAccount = Pi.SetService.Domain.AggregatesModel.AccountAggregate.TradingAccount;

namespace Pi.SetService.Application.Commands;

public record SendOrderRequest
{
    public required Guid CorrelationId { get; init; }
    public required Guid UserId { get; init; }
    public required string TradingAccountNo { get; init; }
    public required TradingAccountType TradingAccountType { get; init; }
    public required ConditionPrice ConditionPrice { get; init; }
    public required int Volume { get; init; }
    public required int PubVolume { get; init; }
    public required OrderAction Action { get; init; }
    public required OrderType OrderType { get; init; }
    public required OrderSide OrderSide { get; init; }
    public required string SecSymbol { get; init; }
    public required Condition Condition { get; init; }
    public decimal? Price { get; init; }
    public Ttf? Ttf { get; init; }
    public bool? Lending { get; init; }
}

public record SendOrderResponse
{
    public required string OrderNo { get; init; }
    public required string BrokerOrderNo { get; init; }
    public required string EnterId { get; init; }
    public ServiceType? ServiceType { get; init; }
}

public class SendOrderConsumer(
    IMarketService marketService,
    IOnePortService onePortService,
    IOnboardService onboardService,
    IEquityNumberGeneratorService numberGeneratorService,
    IOptions<SetTradingOptions> options,
    ILogger<SendOrderConsumer> logger)
    : IConsumer<SendOrderRequest>
{
    private readonly SetTradingOptions _options = options.Value;

    public async Task Consume(ConsumeContext<SendOrderRequest> context)
    {
        try
        {
            ValidateAction(context);
            await ValidatePrice(context);

            var instrumentProfile = await marketService.GetInstrumentProfile(context.Message.SecSymbol, context.CancellationToken);
            if (instrumentProfile == null) throw new SetException(SetErrorCode.SE104);

            switch (context.Message.Action)
            {
                case OrderAction.Sell:
                    {
                        var tradingAccount = await GetTradingAccount(context);
                        var positions =
                            await onePortService.GetPositions(context.Message.TradingAccountNo, context.CancellationToken);
                        tradingAccount.SetPositions(positions);
                        ValidateSell(context, tradingAccount);
                        break;
                    }
                case OrderAction.Short:
                    {
                        await ValidatePosition(context, StockType.Borrow);
                        break;
                    }
                case OrderAction.Cover:
                    {
                        await ValidatePosition(context, StockType.Short);
                        break;
                    }
            }

            var (orderNo, brokerOrderId, serviceType) = await SendOrder(context, instrumentProfile,
                context.Message.OrderSide, context.Message.OrderType);

            await context.RespondAsync(new SendOrderResponse
            {
                OrderNo = orderNo,
                BrokerOrderNo = brokerOrderId,
                ServiceType = serviceType,
                EnterId = _options.EnterId
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Send order failed: {@Message}", context.Message);
            throw;
        }
    }

    private async Task ValidatePosition(ConsumeContext<SendOrderRequest> context, StockType stockType)
    {
        var positions = await onePortService.GetPositions(context.Message.TradingAccountNo, context.CancellationToken);
        var position = positions.Find(q => q.StockType == stockType && q.SecSymbol == context.Message.SecSymbol);

        if (position == null) throw new SetException(SetErrorCode.SE114);

        if (position.AvailableVolume < context.Message.Volume) throw new SetException(SetErrorCode.SE106);
    }

    private static void ValidateAction(ConsumeContext<SendOrderRequest> context)
    {
        if (new[] { OrderAction.Borrow, OrderAction.Return }.Contains(context.Message.Action))
            throw new SetException(SetErrorCode.SE003);

        try
        {
            var (orderSide, orderType) =
                DomainFactory.NewOrderSideAndOrderType(context.Message.Action, context.Message.Lending);
            if (orderSide != context.Message.OrderSide || orderType != context.Message.OrderType)
                throw new SetException(SetErrorCode.SE002);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new SetException(SetErrorCode.SE002);
        }
    }

    private async Task ValidatePrice(ConsumeContext<SendOrderRequest> context)
    {
        if (context.Message.ConditionPrice != ConditionPrice.Limit ||
            context.Message.Action == OrderAction.Borrow) return;

        var price = context.Message.Price;
        if (price == null) throw new SetException(SetErrorCode.SE004, "Limit order price should not be null");

        var ceilingFloor = await marketService.GetCeilingFloor(context.Message.SecSymbol, context.CancellationToken);
        if (ceilingFloor == null) throw new SetException(SetErrorCode.SE104);

        if (price > ceilingFloor.Ceiling || price < ceilingFloor.Floor)
            throw new SetException(SetErrorCode.SE105, "Price should between ceiling and floor");
    }

    private async Task<TradingAccount> GetTradingAccount(ConsumeContext<SendOrderRequest> context)
    {
        var custCode = TradingAccountHelper.GetCustCodeBySetTradingAccountNo(context.Message.TradingAccountNo);
        if (custCode == null) throw new SetException(SetErrorCode.SE001);

        var tradingAccounts =
            await onboardService.GetSetTradingAccountsByUserIdAsync(context.Message.UserId, custCode, context.CancellationToken);
        var tradingAccount = tradingAccounts.Find(q => q.TradingAccountNo == context.Message.TradingAccountNo);

        // Validate Trading Account
        if (tradingAccount == null) throw new SetException(SetErrorCode.SE102);

        return tradingAccount;
    }

    private static void ValidateSell(ConsumeContext<SendOrderRequest> context, TradingAccount tradingAccount)
    {
        var totalAvailableVolume = context.Message.Ttf switch
        {
            Ttf.Nvdr => tradingAccount.GetTotalVolumeNvdrStock(context.Message.SecSymbol),
            _ when context.Message.Lending == true => tradingAccount.GetTotalVolumeNoneNvdrStock(
                context.Message.SecSymbol, StockType.Lending),
            _ => tradingAccount.GetTotalVolumeNoneNvdrStock(context.Message.SecSymbol, StockType.Normal)
        };

        if (totalAvailableVolume < context.Message.Volume) throw new SetException(SetErrorCode.SE106);
    }

    private async Task<(string orderNo, string brokerOrderId, ServiceType? serviceType)> SendOrder(
        ConsumeContext<SendOrderRequest> context,
        InstrumentProfile instrumentProfile,
        OrderSide side,
        OrderType orderType)
    {
        ServiceType? serviceType = null;
        string orderNo, brokerOrderId;

        var marketStatus = await marketService.GetCurrentMarketStatus();
        switch (marketStatus)
        {
            case MarketStatus.Open:
            case MarketStatus.Closed when IsSendOrderConnectionStarted():
                (orderNo, brokerOrderId) = await SendMarketOrder(context, instrumentProfile, side, orderType);
                break;
            case MarketStatus.OffHour:
            case MarketStatus.Closed:
                (orderNo, serviceType, brokerOrderId) = await SendOfflineOrder(context, side, orderType);
                break;
            case MarketStatus.Maintenance:
                throw new SetException(SetErrorCode.SE107,
                    $"System maintenance time between {_options.MaintenanceStartTime} to {_options.MaintenanceEndTime}");
            default:
                throw new ArgumentOutOfRangeException(nameof(marketStatus), marketStatus, null);
        }

        return (orderNo, brokerOrderId, serviceType);
    }

    private async Task<(string orderNo, string brokerOrderId)> SendMarketOrder(ConsumeContext<SendOrderRequest> context,
        InstrumentProfile instrumentProfile, OrderSide side,
        OrderType orderType)
    {
        var orderNo =
            await numberGeneratorService.GenerateAndUpdateOnlineOrderNoAsync(context.Message.CorrelationId,
                context.CancellationToken);
        var response = await onePortService.CreateNewOrder(new NewOrder
        {
            AccountNo = context.Message.TradingAccountNo,
            OrderNo = orderNo,
            EnterId = _options.EnterId,
            SecSymbol = instrumentProfile.Symbol,
            Side = side,
            Price = context.Message.Price,
            ConPrice = context.Message.ConditionPrice,
            Volume = context.Message.Volume,
            PublishVol = context.Message.PubVolume,
            Condition = context.Message.Condition,
            Ttf = context.Message.Ttf ?? Ttf.None,
            OrderType = orderType
        });

        if (response.ExecutionTransRejectType != null || response.Status == BrokerOrderStatus.Rejected)
        {
            logger.LogError("Send Order got rejected from {Source} with reason {ResponseReason}",
                response.ExecutionTransRejectType.ToString(),
                response.Reason);
            throw ErrorFactory.NewSetException(response);
        }

        var brokerOrderId = response.BrokerOrderId;

        return (orderNo, brokerOrderId);
    }

    private async Task<(string orderNo, ServiceType serviceType, string brokerOrderId)> SendOfflineOrder(
        ConsumeContext<SendOrderRequest> context, OrderSide side, OrderType orderType)
    {
        ValidateOfflinePayload(context);

        var offlineOrderNo =
            await numberGeneratorService.GenerateAndUpdateOfflineOrderNoAsync(context.Message.CorrelationId,
                context.CancellationToken);
        var orderNo = offlineOrderNo.ToString();
        var serviceType = ServiceType.Vip;
        await onePortService.CreateNewOfflineOrder(new NewOfflineOrder
        {
            AccountNo = context.Message.TradingAccountNo,
            OrderNo = offlineOrderNo,
            OrderType = orderType,
            OrderDateTime = DateTime.UtcNow,
            EnterId = _options.EnterId,
            SecSymbol = context.Message.SecSymbol,
            Side = side,
            Price = context.Message.Price ?? 0m,
            Volume = context.Message.Volume,
            PubVolume = context.Message.PubVolume,
            ConditionPrice = context.Message.ConditionPrice,
            Condition = context.Message.Condition,
            TrusteeId = context.Message.Ttf ?? Ttf.None,
            BrokerNo = Broker.BrokerNo,
            ServiceType = serviceType
        });
        var brokerOrderId = offlineOrderNo.ToString();

        return (orderNo, serviceType, brokerOrderId);
    }

    private bool IsSendOrderConnectionStarted()
    {
        var thNow = DateTimeHelper.ThNow();
        var todayStr = thNow.ToString(DateTimeHelper.DateFormat);
        var normalStart = DateTime.ParseExact(todayStr + " " + _options.NormalStartTime,
            DateTimeHelper.DateTimeFormat, CultureInfo.InvariantCulture);
        var normalEnd = DateTime.ParseExact(todayStr + " " + _options.NormalEndTime, DateTimeHelper.DateTimeFormat,
            CultureInfo.InvariantCulture);

        return thNow >= normalStart && thNow < normalEnd;
    }

    private static void ValidateOfflinePayload(ConsumeContext<SendOrderRequest> context)
    {
        if (new[] { ConditionPrice.Mtl, ConditionPrice.Mkt }.Contains(context.Message.ConditionPrice))
            throw new SetException(SetErrorCode.SE010);

        if (new[] { Condition.Gtc, Condition.Gtd }.Contains(context.Message.Condition))
            throw new SetException(SetErrorCode.SE011);
    }
}
