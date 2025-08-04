using Pi.Common.CommonModels;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Application.Services.Wallet;

public interface IWalletService
{
    Task<decimal> GetLineAvailable(string userId, string custCode, CancellationToken ct);
    Task<ExchangeRate> GetExchangeRate(Currency from, Currency to, CancellationToken ct);
    decimal GetLineAvailableUsd(string accountId, AccountSummaryPosition accountSummary,
        IEnumerable<IOrder> activeOrders, decimal hkUsExRate);
}
