namespace Pi.GlobalEquities.Services;

public interface ITradingReadService
{
    Task<IOrder> GetOrder(string orderId, CancellationToken ct);
    Task<IEnumerable<Order>> GetOrders(DateTime from, DateTime to, CancellationToken ct);
    Task<IEnumerable<Order>> GetOrders(string accountId,
        DateTime from, DateTime to,
        OrderSide? side, bool? hasFilled,
        OrderStatus[] excludeStatuses,
        CancellationToken ct);
    Task<AccountOverview> GetAccountOverview(IAccount account, Currency currency, CancellationToken ct);

    Task<IList<TransactionItem>>
        GetTransactions(string accountId, DateTime from, DateTime to, CancellationToken ct);

    Task<IList<TransactionItem>> GetCorporateActionTransactions(string accountId, DateTime from, DateTime to,
        CancellationToken ct);
}
