using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Application.Models.Trading;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Application.Utils;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Domain.Events;
using OrderState = Pi.Financial.FundService.IntegrationEvents.Models.FundOrderState;

namespace Pi.Financial.FundService.Application.Commands;

public record CancelFundOrderRequest
{
    public required string BrokerOrderId { get; init; }
    public required Guid UserId { get; init; }
    public required string TradingAccountNo { get; init; }
    public required OrderSide OrderSide { get; init; }
    public required bool? Force { get; init; }
}

public class CancelFundOrderConsumer : IConsumer<CancelFundOrderRequest>
{
    private readonly IFundOrderRepository _fundOrderRepository;
    private readonly ILogger<CancelFundOrderConsumer> _logger;
    private readonly IFundConnextService _fundConnextService;
    private readonly IUserService _userService;

    public CancelFundOrderConsumer(IFundOrderRepository fundOrderRepository,
        ILogger<CancelFundOrderConsumer> logger,
        IFundConnextService fundConnextService,
        IUserService userService)
    {
        _logger = logger;
        _fundOrderRepository = fundOrderRepository;
        _fundConnextService = fundConnextService;
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<CancelFundOrderRequest> context)
    {
        try
        {
            var payload = context.Message;
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

            var fundOrderStates = await _fundOrderRepository.GetByBrokerOrderIdAndOrderSideAsync(payload.BrokerOrderId,
                payload.OrderSide, context.CancellationToken);

            var switchOrderCancelled = false;
            var correlationIdSet = new HashSet<Guid>();
            foreach (var fundOrderState in fundOrderStates)
            {
                if (fundOrderState.CurrentState != OrderState.GetName(() => OrderState.OrderPlaced)
                    && correlationIdSet.Contains(fundOrderState.CorrelationId)) continue;
                switch (fundOrderState.OrderSide)
                {
                    case OrderSide.Buy:
                        await _fundConnextService.CancelSubscriptionOrderAsync(new CancelOrderRequest
                        {
                            BrokerOrderId = payload.BrokerOrderId,
                            OrderSide = payload.OrderSide,
                            Force = payload.Force ?? false
                        }, context.CancellationToken);

                        await context.Publish(new CancelFundOrderReceived(fundOrderState.CorrelationId));
                        correlationIdSet.Add(fundOrderState.CorrelationId);
                        break;

                    case OrderSide.Sell:
                        await _fundConnextService.CancelRedemptionOrderAsync(new CancelOrderRequest
                        {
                            BrokerOrderId = payload.BrokerOrderId,
                            OrderSide = payload.OrderSide,
                            Force = payload.Force ?? false
                        }, context.CancellationToken);

                        await context.Publish(new CancelFundOrderReceived(fundOrderState.CorrelationId));
                        correlationIdSet.Add(fundOrderState.CorrelationId);
                        break;

                    case OrderSide.Switch:
                        // Handle Switch In and Switch Out Cases
                        if (!switchOrderCancelled)
                        {
                            await _fundConnextService.CancelSwitchingOrderAsync(new CancelOrderRequest
                            {
                                BrokerOrderId = payload.BrokerOrderId,
                                OrderSide = payload.OrderSide,
                                Force = payload.Force ?? false
                            }, context.CancellationToken);
                            switchOrderCancelled = true;
                        }

                        await context.Publish(new CancelFundOrderReceived(fundOrderState.CorrelationId));
                        correlationIdSet.Add(fundOrderState.CorrelationId);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            await context.RespondAsync(new CancelOrderSuccess(context.Message.BrokerOrderId));
        }
        catch (FundOrderException ex)
        {
            await context.RespondAsync(new CancelOrderFailed(context.Message.BrokerOrderId, ex.Message, ex.Code.ToString()));
        }
        catch (Exception ex)
        {
            await context.RespondAsync(new CancelOrderFailed(context.Message.BrokerOrderId, ex.Message));
        }
    }
}
