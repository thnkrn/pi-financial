using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Application.Factories;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Utils;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Commands;

public record SyncFundOrder
{
    public required DateOnly EffectiveDate { get; init; }
    public bool ForceCreateOffline { get; init; }
}

public class SyncFundOrderConsumer : IConsumer<SyncFundOrder>
{
    private readonly IFundOrderRepository _fundOrderRepository;
    private readonly IUnitHolderRepository _unitHolderRepository;
    private readonly IFundConnextService _fundConnextService;
    private readonly IOnboardService _onboardService;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<SyncFundOrderConsumer> _logger;
    private const string CacheKey = "FundOrderSync.TradingAccount";

    public SyncFundOrderConsumer(IFundOrderRepository fundOrderRepository,
        IFundConnextService fundConnextService,
        IOnboardService onboardService,
        IUnitHolderRepository unitHolderRepository,
        IMemoryCache memoryCache,
        ILogger<SyncFundOrderConsumer> logger)
    {
        _fundOrderRepository = fundOrderRepository;
        _fundConnextService = fundConnextService;
        _onboardService = onboardService;
        _unitHolderRepository = unitHolderRepository;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SyncFundOrder> context)
    {
        var filters = new FundOrderStateFilter()
        {
            Statuses = new[] { FundOrderStatus.Submitted, FundOrderStatus.Waiting, FundOrderStatus.Approved }
        };
        var effectiveDates = await _fundOrderRepository.GetEffectiveDates(filters, context.CancellationToken);
        if (!effectiveDates.Contains(context.Message.EffectiveDate)) effectiveDates.Add(context.Message.EffectiveDate);

        _logger.LogInformation("Sync fund order with effective dates {@EffectiveDates}", effectiveDates);
        foreach (var effectiveDate in effectiveDates)
        {
            var createOffline = context.Message.ForceCreateOffline || effectiveDate == context.Message.EffectiveDate;

            _logger.LogInformation("Start sync fund order of date \"{EffectiveDate}\"", effectiveDate);
            await SyncByEffectiveDateAsync(effectiveDate, context, createOffline, context.CancellationToken);
            _logger.LogInformation("Ended sync fund order of date \"{EffectiveDate}\"", effectiveDate);
        }
    }

    private async Task SyncByEffectiveDateAsync(DateOnly effectiveDate, IPublishEndpoint context, bool addOffline = false, CancellationToken cancellationToken = default)
    {
        var fundOrders = await _fundConnextService.GetFundOrdersAsync(effectiveDate, null, cancellationToken);

        if (fundOrders.Count == 0)
        {
            _logger.LogInformation("Fund Order not found on the effective date: {EffectiveDate}", effectiveDate);
            return;
        }

        var brokerOrderIds = fundOrders.Select(q => q.BrokerOrderId).ToArray();
        var fundOrderStates = await _fundOrderRepository.GetByBrokerOrderIds(brokerOrderIds, cancellationToken);
        _logger.LogInformation("Total fund order at FundConnext: {Count}", fundOrders.Count);

        var unitHolderIdSet = new HashSet<string>();
        foreach (var fundOrder in fundOrders)
        {
            if (IsValidQuantity(fundOrder))
            {
                _logger.LogError("Fund amount or unit is <= 0 for Broker Order Id: {BrokerOrderId}", fundOrder.BrokerOrderId);
                continue;
            }

            var orderState = fundOrderStates.Find(q => q.BrokerOrderId == fundOrder.BrokerOrderId && q.OrderType == fundOrder.OrderType);

            if (orderState != null)
            {
                _logger.LogTrace("Updating order of Broker Order Id: {BrokerOrderId}", orderState.BrokerOrderId);
                EntityFactory.SyncFundOrderState(orderState, fundOrder);
                await _fundOrderRepository.Update(orderState);
            }
            else if (addOffline)
            {
                var custCode = TradingAccountHelper.GetCustCodeByFundTradingAccountNo(fundOrder.AccountId);
                Guid? tradingAccountId = null;

                if (custCode != null)
                {
                    fundOrder.SetCustcode(custCode);

                    var cache = _memoryCache.Get<(string, Guid?)>(CacheKey + "." + custCode);
                    if (cache.Item1 == custCode)
                    {
                        tradingAccountId = cache.Item2;
                    }
                    else
                    {
                        var tradingAccount = await _onboardService.GetMutualFundTradingAccountByCustCodeAsync(custCode, cancellationToken);
                        tradingAccountId = tradingAccount?.Id;
                        _memoryCache.Set(CacheKey + "." + custCode, (custCode, tradingAccountId), new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1)));
                    }
                }

                await CreateOfflineOrder(fundOrder, tradingAccountId, cancellationToken);

                if (unitHolderIdSet.Contains(fundOrder.UnitHolderId)) continue;
                await SyncUnitHolder(fundOrder, context, cancellationToken);
                unitHolderIdSet.Add(fundOrder.UnitHolderId);
            }
        }
    }

    private async Task CreateOfflineOrder(FundOrder fundOrder, Guid? tradingAccountId,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("Add offline orders with Broker Order Id: {BrokerOrderId}", fundOrder.BrokerOrderId);
        try
        {
            var fundOrderState = await _fundOrderRepository.CreateAsync(EntityFactory.NewFundOrderState(fundOrder, tradingAccountId), cancellationToken);
            if (fundOrderState == null)
            {
                _logger.LogError("Sync failed for Offline fund order with Broker Order Id \"{BrokerOrderId}\"",
                    fundOrder.BrokerOrderId);
                throw new SyncException("Sync failed for Offline Fund Order");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create Offline fund order with Broker Order Id \"{BrokerOrderId}\" on effective date {EffectiveDate}",
                fundOrder.BrokerOrderId, fundOrder.EffectiveDate);
        }
    }

    private async Task SyncUnitHolder(FundOrder fundOrder, IPublishEndpoint context, CancellationToken cancellationToken = default)
    {
        var unitHoldersCount = await _unitHolderRepository.CountUnitHolderAsync(fundOrder.UnitHolderId, cancellationToken);
        if (unitHoldersCount == 0)
        {
            _logger.LogError("Event Published for Adding New UnitHolderID \"{UnitHolderId}\"", fundOrder.UnitHolderId);
            var unitHolderType = GetUnitHolderType(fundOrder.AccountType);
            if (unitHolderType == null)
            {

                _logger.LogError(
                    "Sync failed for new unit holder Id: \"{UnitHolderId}\"for Offline fund order with Broker Order Id: \"{BrokerOrderId}\"",
                    fundOrder.UnitHolderId, fundOrder.BrokerOrderId);
                throw new SyncException("Sync failed for new unit holder for offline fund order");
            }

            await context.Publish(new CreateUnitHolder
            {
                CustomerCode = fundOrder.CustCode,
                TradingAccountNo = fundOrder.AccountId,
                UnitHolderId = fundOrder.UnitHolderId,
                FundCode = fundOrder.FundCode,
                UnitHolderType = (UnitHolderType)unitHolderType
            }, cancellationToken);
        }
    }

    private static UnitHolderType? GetUnitHolderType(FundAccountType? accountType)
    {
        return accountType switch
        {
            FundAccountType.SEG => UnitHolderType.SEG,
            FundAccountType.OMN => UnitHolderType.OMN,
            FundAccountType.SEG_NT => UnitHolderType.SEG,
            FundAccountType.SEG_T => UnitHolderType.SEG,
            FundAccountType.OMN_NT => UnitHolderType.OMN,
            FundAccountType.OMN_T => UnitHolderType.OMN,
            _ => null
        };
    }

    private static bool IsValidQuantity(FundOrder fundOrder)
    {
        return (fundOrder.Unit != null && fundOrder.Unit <= 0) || (fundOrder.Amount != null && fundOrder.Amount <= 0);
    }
}
