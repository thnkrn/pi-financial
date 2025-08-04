using Pi.OnePort.Db2.Models;

namespace Pi.OnePort.Db2.Repositories;

public interface ISetRepo
{
    Task<List<AccountPositionCreditBalance>> GetCreditBalanceAccountPositions(string accountNo, int page = 1, CancellationToken ct = default);
    Task<List<AccountPosition>> GetAccountPositions(string accountNo, int page = 1, CancellationToken ct = default);
    Task<List<AccountAvailable>> GetAccountsAvailable(string accountNo, int page = 1, CancellationToken ct = default);
    Task<List<AccountAvailableCreditBalance>> GetCreditBalanceAccountsAvailable(string accountNo, int page = 1, CancellationToken ct = default);
    Task<List<OfflineOrder>> GetOfflineOrders(string accountNo, int page = 1, CancellationToken ct = default);
    Task<List<Order>> GetOrders(string accountNo, int page = 1, CancellationToken ct = default);
    Task<List<AccountDeal>> GetDealsByAccountNo(string accountNo, int page = 1, CancellationToken ct = default);
    Task<List<DealOrder>> GetDealsByOrderNo(int orderNo, int page = 1, CancellationToken ct = default);
    Task<int> NewOfflineOrder(OfflineOrderRequest request, CancellationToken ct = default);
    Task<int> UpdateOfflineOrder(OfflineOrderRequest request, CancellationToken ct = default);
    Task<int> CancelOfflineOrder(CancelOfflineOrderRequest request, CancellationToken ct = default);
}
