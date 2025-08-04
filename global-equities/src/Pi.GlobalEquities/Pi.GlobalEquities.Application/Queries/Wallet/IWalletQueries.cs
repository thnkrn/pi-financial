using Pi.Common.CommonModels;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Application.Queries.Wallet;

public interface IWalletQueries
{
    /// <summary>
    /// This method may increase rate limit issue on Velexa API, please consider to use <see cref="CustomWalletProductLineAvailabilityGetAsync"/>
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="custCode"></param>
    /// <param name="currency"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<decimal> GetLineAvailable(string userId, string custCode,
        Currency currency, CancellationToken ct);
    Task<ExchangeRate> GetExchangeRate(
        Currency from,
        Currency to,
        CancellationToken ct);
    decimal GetLineAvailableUsd(string accountId, AccountSummaryPosition accountSummary,
        IEnumerable<IOrder> activeOrders, decimal hkUsExRate);
}
