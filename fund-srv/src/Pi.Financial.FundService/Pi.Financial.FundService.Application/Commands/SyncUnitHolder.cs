using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Commands;

public record SyncUnitHolder();

public class SyncUnitHolderConsumer : IConsumer<SyncUnitHolder>
{
    private readonly IFundOrderRepository _fundOrderRepository;
    private readonly IFundConnextService _fundConnextService;
    private readonly IUnitHolderRepository _unitHolderRepository;
    private readonly ILogger<SyncUnitHolderConsumer> _logger;

    public SyncUnitHolderConsumer(IFundOrderRepository fundOrderRepository, IFundConnextService fundConnextService,
        IUnitHolderRepository unitHolderRepository, ILogger<SyncUnitHolderConsumer> logger)
    {
        _fundOrderRepository = fundOrderRepository;
        _fundConnextService = fundConnextService;
        _unitHolderRepository = unitHolderRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SyncUnitHolder> context)
    {
        var filters = new FundOrderStateFilter()
        {
            DummyUnitHolder = true
        };
        var fundOrderStates = await _fundOrderRepository.GetAsync(filters, context.CancellationToken);

        if (fundOrderStates.Count == 0)
        {
            _logger.LogInformation("Fund order state with dummy unit holder not found");
            return;
        }

        var effectiveDates = fundOrderStates.Where(q => q.EffectiveDate != null)
            .Select(q => (DateOnly)q.EffectiveDate!)
            .Distinct()
            .ToList();

        var brokerFundOrders = await FetchFundOrders(effectiveDates, context.CancellationToken);

        if (brokerFundOrders.Count == 0)
        {
            _logger.LogInformation("Broker fund order not found within effective date range: {EffectiveDates}", string.Join(",", effectiveDates));
            return;
        }

        var orderStates = fundOrderStates.Where(q => q.BrokerOrderId != null)
            .ToDictionary(key => key.BrokerOrderId!, element => element);

        var mapped = new HashSet<string>();
        foreach (var brokerFundOrder in brokerFundOrders)
        {
            orderStates.TryGetValue(brokerFundOrder.BrokerOrderId, out var fundOrderState);

            if (fundOrderState?.UnitHolderId == null ||
                mapped.Contains(fundOrderState.UnitHolderId!) ||
                brokerFundOrder.UnitHolderId.ToUpper().StartsWith("DM"))
            {
                continue;
            }

            mapped.Add(fundOrderState.UnitHolderId);
            var oldUnitHolderId = fundOrderState.UnitHolderId;
            _logger.LogInformation("Update \"{OldUnitHolderId}\" to \"{NewUnitHolderId}\"", oldUnitHolderId, brokerFundOrder.UnitHolderId);
            await _fundOrderRepository.UpdateUnitHolderIdAsync(oldUnitHolderId,
                brokerFundOrder.UnitHolderId, context.CancellationToken);
            await _unitHolderRepository.UpdateUnitHolderIdAsync(oldUnitHolderId,
                brokerFundOrder.UnitHolderId, context.CancellationToken);
        }
    }

    private async Task<List<FundOrder>> FetchFundOrders(List<DateOnly> effectiveDates, CancellationToken cancellationToken)
    {
        var brokerFundOrders = await Task.WhenAll(effectiveDates.Select(async date => await FetchFundOrdersWithRetry(date, cancellationToken)));

        return brokerFundOrders.SelectMany(orderList => orderList).ToList();
    }

    private async Task<List<FundOrder>> FetchFundOrdersWithRetry(DateOnly date, CancellationToken cancellationToken, int maxRetries = 3)
    {
        var retryCount = 0;

        while (retryCount < maxRetries)
        {
            try
            {
                return await _fundConnextService.GetFundOrdersAsync(date, FundOrderStatus.Allotted, cancellationToken);
            }
            catch (Exception e)
            {
                retryCount++;
                _logger.LogError(e, "Fetch fund order \"{Date}\" retried #{RetryCount}", date, retryCount);
                if (retryCount == maxRetries)
                {
                    _logger.LogError(e, "Fetch fund order \"{Date}\" failed", date);
                    throw;
                }
            }
        }

        return new List<FundOrder>();
    }
}
