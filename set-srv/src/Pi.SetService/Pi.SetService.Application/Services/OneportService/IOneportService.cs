using Pi.SetService.Application.Models;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Services.OneportService;

public interface IOnePortService
{
    Task<List<OnlineOrder>> GetOrdersByAccountNo(string tradingAccountNo, CancellationToken cancellationToken = default);
    Task<OnlineOrder?> GetOrderByAccountNoAndOrderNo(string tradingAccountNo, string orderNo, CancellationToken cancellationToken = default);
    Task<List<OfflineOrder>> GetOfflineOrdersByAccountNo(string tradingAccountNo,
        CancellationToken cancellationToken = default);
    Task<OfflineOrder?> GetOfflineOrderByAccountNoAndOrderNo(string tradingAccountNo, string orderNo,
        CancellationToken cancellationToken = default);
    Task<List<Deal>> GetDealsByAccountNo(string accountNo, CancellationToken cancellationToken = default);
    Task<BrokerOrderResponse> CreateNewOrder(NewOrder requestedOrder);
    Task<NewOfflineOrderResponse> CreateNewOfflineOrder(NewOfflineOrder requestedOrder);
    Task<BrokerOrderResponse> CancelOrder(CancelOrder requestedOrder, CancellationToken cancellationToken = default);
    Task CancelOfflineOrder(CancelOfflineOrder requestedOrder, CancellationToken cancellationToken = default);
    Task<BrokerOrderResponse> ChangeOrder(ChangeOrder requestedOrder, CancellationToken cancellationToken = default);
    Task ChangeOfflineOrder(ChangeOfflineOrder requestedOrder, CancellationToken cancellationToken = default);
    Task<List<AccountPosition>> GetPositions(string tradingAccountNo, CancellationToken cancellationToken = default);
    Task<List<AccountPositionCreditBalance>> GetPositionsCreditBalance(string tradingAccountNo, CancellationToken cancellationToken = default);
    Task<List<AvailableCashBalance>> GetAvailableCashBalances(string tradingAccountNo, CancellationToken cancellationToken = default);
    Task<List<AvailableCreditBalance>> GetAvailableCreditBalances(string tradingAccountNo, CancellationToken cancellationToken = default);
}
