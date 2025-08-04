using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.SetService.Application.Exceptions;
using Pi.SetService.Application.Factories;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Options;
using Pi.SetService.Application.Services.MarketService;
using Pi.SetService.Application.Services.OnboardService;
using Pi.SetService.Application.Services.OneportService;
using Pi.SetService.Application.Services.UserService;
using Pi.SetService.Application.Utils;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Application.Commands;

public record CancelOrderRequest
{
    public required Guid UserId { get; init; }
    public required string BrokerOrderId { get; init; }
    public required string TradingAccountNo { get; init; }
}

public record CancelOrderResponse(string BrokerOrderId);

public class CancelOrderConsumer(
    IMarketService marketService,
    IOnePortService onePortService,
    IOnboardService onboardService,
    IUserService userService,
    IEquityOrderStateRepository equityOrderStateRepository,
    IOptions<SetTradingOptions> options,
    ILogger<CancelOrderConsumer> logger)
    : IConsumer<CancelOrderRequest>
{
    private readonly SetTradingOptions _options = options.Value;

    public async Task Consume(ConsumeContext<CancelOrderRequest> context)
    {
        try
        {
            var tradingAccount = await ValidateAccount(context);
            var marketStatus = await marketService.GetCurrentMarketStatus();
            if (marketStatus == MarketStatus.Maintenance)
                throw new SetException(SetErrorCode.SE107,
                    $"System maintenance time between {_options.MaintenanceStartTime} to {_options.MaintenanceEndTime}");

            var onlineOrdersTask = onePortService.GetOrderByAccountNoAndOrderNo(context.Message.TradingAccountNo,
                context.Message.BrokerOrderId, context.CancellationToken);
            var offlineOrdersTask = onePortService.GetOfflineOrderByAccountNoAndOrderNo(
                context.Message.TradingAccountNo, context.Message.BrokerOrderId, context.CancellationToken);

            await Task.WhenAll(onlineOrdersTask, offlineOrdersTask);

            var onlineOrder = await onlineOrdersTask;
            var offlineOrder = await offlineOrdersTask;

            BaseOrder targetOrder;
            if (onlineOrder != null && onlineOrder.IsOpenOrder())
            {
                await CancelOnlineOrder(context, onlineOrder);
                targetOrder = onlineOrder;
            }
            else if (offlineOrder != null)
            {
                await CancelOfflineOrder(context, offlineOrder);
                targetOrder = offlineOrder;
            }
            else
            {
                throw new SetException(SetErrorCode.SE108);
            }

            await PublishDomainEvent(context, targetOrder, tradingAccount);
            await context.RespondAsync(new CancelOrderResponse(targetOrder.OrderNo.ToString()));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Cancel order failed: {@Message}", context.Message);
            throw;
        }
    }

    private async Task<TradingAccount> ValidateAccount(ConsumeContext<CancelOrderRequest> context)
    {
        var custCode = TradingAccountHelper.GetCustCodeBySetTradingAccountNo(context.Message.TradingAccountNo);
        if (custCode == null) throw new SetException(SetErrorCode.SE001);

        var custCodes = await userService.GetCustomerCodesByUserId(context.Message.UserId);

        // Validate App User
        if (!custCodes.Contains(custCode)) throw new SetException(SetErrorCode.SE101);

        var tradingAccounts =
            await onboardService.GetSetTradingAccountsByUserIdAsync(context.Message.UserId, custCode, context.CancellationToken);
        var tradingAccount = tradingAccounts.Find(q => q.TradingAccountNo == context.Message.TradingAccountNo);

        // Validate Trading Account
        if (tradingAccount == null) throw new SetException(SetErrorCode.SE102);

        return tradingAccount;
    }

    private async Task CancelOnlineOrder(ConsumeContext<CancelOrderRequest> context, OnlineOrder onlineOrder)
    {
        if (onlineOrder.OrderDateTime == null || !onlineOrder.IsOpenOrder()) throw new SetException(SetErrorCode.SE108);

        var response = await onePortService.CancelOrder(new CancelOrder
        {
            OrderNo = $"SOCA{context.Message.BrokerOrderId}", // TODO: Might move this into oneport service instead
            BrokerOrderId = context.Message.BrokerOrderId,
            EnterId = _options.EnterId
        }, context.CancellationToken);

        if (response.ExecutionTransRejectType != null || response.Status == BrokerOrderStatus.Rejected)
        {
            logger.LogError("Cancel Order got rejected from {Source} with reason {ResponseReason}",
                response.ExecutionTransRejectType.ToString(),
                response.Reason);
            throw ErrorFactory.NewSetException(response);
        }
    }

    private async Task CancelOfflineOrder(ConsumeContext<CancelOrderRequest> context, OfflineOrder offlineOrder)
    {
        if (offlineOrder.OrderDateTime == null || !offlineOrder.IsOpenOrder())
            throw new SetException(SetErrorCode.SE108);

        await onePortService.CancelOfflineOrder(new CancelOfflineOrder
        {
            OrderDateTime = (DateTime)offlineOrder.OrderDateTime!,
            BrokerOrderId = offlineOrder.OrderNo,
            CancelId = "",
            CancelDateTime = DateTime.UtcNow,
            DelFlag = true
        }, context.CancellationToken);
    }

    private async Task PublishDomainEvent(ConsumeContext<CancelOrderRequest> context, BaseOrder targetOrder,
        TradingAccount tradingAccount)
    {
        if (targetOrder.OrderDateTime == null) return;

        var filters = new EquityOrderStateFilters
        {
            BrokerOrderId = targetOrder.OrderNo.ToString(),
            CreatedDate = DateOnly.FromDateTime((DateTime)targetOrder.OrderDateTime)
        };
        var orderStates = await equityOrderStateRepository.GetEquityOrderStates(filters);
        var orderState = orderStates.FirstOrDefault(q => q.BrokerOrderId == context.Message.BrokerOrderId);
        if (orderState == null)
            await context.Publish(DomainFactory.NewSyncCreateOrderReceived(targetOrder, tradingAccount));
    }
}
