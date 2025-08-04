using Pi.Common.CommonModels;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Application.Services.Velexa;

public interface IVelexaReadService
{
    Task<IEnumerable<IOrder>> GetActiveOrders(string account, string? symbolId = null,
        CancellationToken ct = default);
    Task<AccountSummaryPosition> GetAccountSummary(string account, Currency currency,
        CancellationToken ct);
    Task<decimal> GetExchangeRate(Currency from, Currency to, CancellationToken ct);
    Task<IList<IOrder>> GetOrders(string providerAccId, DateTime from, DateTime to, CancellationToken ct);
    Task<IList<TransactionItem>> GetTransactions(string accountId, DateTime from, DateTime to,
        string operationTypeGroup, bool returnExtendedInDayDuration = false, CancellationToken ct = default);
}
