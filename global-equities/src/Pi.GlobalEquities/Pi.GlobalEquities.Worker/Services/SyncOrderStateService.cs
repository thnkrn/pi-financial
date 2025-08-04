using Microsoft.Extensions.Options;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;
using Pi.GlobalEquities.Repositories;
using Pi.GlobalEquities.Services;
using Pi.GlobalEquities.Worker.ExternalServices;
using Pi.GlobalEquities.Worker.ExternalServices.Notification;
using IFeatureService = Pi.GlobalEquities.Worker.ExternalServices.FeatureFlags.IFeatureService;

namespace Pi.GlobalEquities.Worker.Services;

public class SyncOrderStateService : BackgroundService
{
    private readonly IWorkerOrderRepository _orderRepo;
    private readonly IWorkerAccountRepository _accountRepo;
    private readonly IWorkerJobRepository _jobRepo;
    private readonly INotificationService _notificationService;
    private readonly IOptions<NotificationApiConfig> _notificationOptions;
    private NotificationApiConfig _notificationApiConfig => _notificationOptions.Value;
    private readonly ITradingReadService _velexaServiceBase;
    private readonly IFeatureService _featureService;
    private const string NotificationFeatureFlag = "global-equity-push-notification";
    private const string SyncOrderJob = "SyncOrderJob";

    readonly ILogger _logger;

    public SyncOrderStateService(
        IWorkerOrderRepository orderRepo,
        IWorkerAccountRepository accountRepo,
        IWorkerJobRepository jobRepo,
        INotificationService notificationService,
        IOptions<NotificationApiConfig> notificationOptions,
        ITradingReadService velexaServiceBase,
        IFeatureService featureService,
        ILogger<SyncOrderStateService> logger)
    {
        _orderRepo = orderRepo;
        _notificationService = notificationService;
        _notificationOptions = notificationOptions;
        _velexaServiceBase = velexaServiceBase;
        _accountRepo = accountRepo;
        _jobRepo = jobRepo;
        _featureService = featureService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var oneMinTimer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        do
        {
            _logger.LogInformation("Start SyncOrderState.");

            try
            {
                await SyncOrderState(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on SyncFinalOrderState.");
            }
        } while (await oneMinTimer.WaitForNextTickAsync(ct));
    }

    private async Task SyncOrderState(CancellationToken ct)
    {
        var currentTime = DateTime.UtcNow;
        var (startTime, endTime) = await GetStartAndEndTime(currentTime, ct);

        var extOrders = await _velexaServiceBase.GetOrders(startTime, endTime, ct);

        var isWhiteList = string.Equals(_notificationApiConfig.Mode, NotificationFlags.WhiteList.ToString(),
            StringComparison.OrdinalIgnoreCase);
        var isAllModeFeatureFlagOn = !isWhiteList && _featureService.IsOn(NotificationFeatureFlag);

        extOrders = extOrders.OrderBy(x => x.ProviderInfo.ModifiedAt);
        bool isErrorExist = false;
        foreach (var extOrder in extOrders)
        {
            OrderUpsertResult orderUpsertResult = null;
            try
            {
                var existingOrder = await _orderRepo.GetOrder(extOrder.Id, ct);
                orderUpsertResult = await UpsertOrderState(existingOrder, extOrder, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error on UpsertOrderState, OrderId {OrderId}, ExtOrderId {ExtOrderId}, AccountId {AccountId}, SymbolId {SymbolId}",
                    extOrder?.Id, extOrder?.ProviderInfo.OrderId,
                    extOrder?.AccountId, extOrder?.SymbolId);
                isErrorExist = true;
            }

            await SendNotification(orderUpsertResult, extOrder, isWhiteList, isAllModeFeatureFlagOn, ct);

            if (!isErrorExist)
                await SetLatestSyncTime(extOrder.ProviderInfo.ModifiedAt, ct);
        }
    }

    private async Task<(DateTime, DateTime)> GetStartAndEndTime(DateTime currentTime, CancellationToken ct)
    {
        var result = await _jobRepo.GetJobDetails<SyncOrderJob>(SyncOrderJob, ct);
        var startTime = result?.Data?.LastSyncTime;

        var fromTime = startTime?.AddSeconds(-5) ?? currentTime.AddMinutes(-1);

        return (fromTime, currentTime);
    }

    private async Task SetLatestSyncTime(DateTime currentTime, CancellationToken ct)
    {
        var lastSyncTime = new SyncOrderJob { LastSyncTime = currentTime };
        var jobDetail = new WorkerJob<SyncOrderJob> { Name = SyncOrderJob, Data = lastSyncTime };
        await _jobRepo.ReplaceJobDetails(jobDetail, ct);
    }

    private async Task<OrderUpsertResult> UpsertOrderState(IOrder order, IOrder extOrder, CancellationToken ct)
    {
        OrderUpsertResult result = null;
        if (order is null)
        {
            await SetOrderOwner(extOrder, ct);
            await _orderRepo.CreateOrder(extOrder, ct);
            result = new() { IsNewOrder = true, Order = extOrder };
        }
        else
        {
            await SetOrderOwner(order, ct);
            var orderUpdate = new OrderUpdates
            {
                ProviderId = extOrder.Id,
                LimitPrice = extOrder.LimitPrice,
                StopPrice = extOrder.StopPrice,
                Quantity = extOrder.Quantity,
                Fills = extOrder.Fills,
                Status = extOrder.Status,
                ProviderInfo = extOrder.ProviderInfo
            };

            if (order.Update(orderUpdate, out _))
            {
                await _orderRepo.UpdateOrder(order.Id, order, ct);
                result = new() { IsNewOrder = false, Order = order };
            }
        }

        return result;
    }

    private async Task SetOrderOwner(IOrder order, CancellationToken ct)
    {
        if (order.UserId == null || order.AccountId == null)
        {
            var providerAccountId = order.ProviderInfo.AccountId;
            var account = await _accountRepo.GetAccountByProviderAccount(Provider.Velexa, providerAccountId, ct);
            if (account == null)
                return;

            order.SetOwner(account.UserId, account.Id);
        }
    }

    private async Task SendNotification(OrderUpsertResult orderUpsertResult, IOrder extOrder, bool isWhiteList,
        bool isAllModeFeatureFlagOn, CancellationToken ct)
    {
        var order = orderUpsertResult?.Order;
        if (order == null || (!order.HasBeenModified && !orderUpsertResult.IsNewOrder))
            return;

        var cmsTemplateId = _notificationService.GetCmsTemplateId(order.Status, order.Side);
        if (cmsTemplateId == null)
            return;

        var isOn = isWhiteList
            ? _featureService.IsOn(order.UserId, NotificationFeatureFlag)
            : isAllModeFeatureFlagOn;
        if (isOn)
            await _notificationService.SendNotification(cmsTemplateId.Value, order, ct);
    }
}

class OrderUpsertResult
{
    public bool IsNewOrder { get; init; }
    public IOrder Order { get; init; }
}

