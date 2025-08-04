using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Financial.FundService.Application.Factories;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Domain.Events;

namespace Pi.Financial.FundService.Application.Commands;

public class SyncSwitchFundOrderConsumer : IConsumer<SwitchOrderPlaced>
{
    private readonly IFundOrderRepository _fundOrderRepository;
    private readonly IFundConnextService _fundConnextService;
    private readonly ILogger<SyncSwitchFundOrderConsumer> _logger;

    public SyncSwitchFundOrderConsumer(IFundOrderRepository fundOrderRepository, IFundConnextService fundConnextService,
        ILogger<SyncSwitchFundOrderConsumer> logger)
    {
        _fundOrderRepository = fundOrderRepository;
        _fundConnextService = fundConnextService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SwitchOrderPlaced> context)
    {
        var fundOrderState = await _fundOrderRepository.GetAsync(context.Message.CorrelationId, context.CancellationToken);

        if (fundOrderState?.OrderType != FundOrderType.SwitchOut)
        {
            _logger.LogError("Fund order state invalid {CorrelationId}", fundOrderState?.CorrelationId);
            return;
        }

        var brokerFundOrders = await _fundConnextService.GetFundOrdersByOrderNoAsync(context.Message.OrderNo, context.CancellationToken);

        var switchInOrder = brokerFundOrders.Find(q => q.OrderType == FundOrderType.SwitchIn);

        if (switchInOrder == null)
        {
            _logger.LogError("SwitchIn fund order not found ({OrderNo})", context.Message.OrderNo);
            return;
        }

        if (fundOrderState.CustomerCode != null)
        {
            switchInOrder.SetCustcode(fundOrderState.CustomerCode);
        }
        var switchInOrderState = EntityFactory.NewFundOrderState(switchInOrder, fundOrderState.TradingAccountId);
        switchInOrderState.CurrentState = null;
        switchInOrderState.OrderType = FundOrderType.SwitchIn;
        switchInOrderState.SellAllUnit = null;
        switchInOrderState.RedemptionType = null;
        switchInOrderState.Amount = switchInOrder.Amount;
        switchInOrderState.Unit = switchInOrder.Unit;
        switchInOrderState.FundCode = switchInOrder.FundCode;
        switchInOrderState.CounterFundCode = null;

        await context.Publish(new CreateSwitchInFundOrderReceived(switchInOrderState));
    }
}
