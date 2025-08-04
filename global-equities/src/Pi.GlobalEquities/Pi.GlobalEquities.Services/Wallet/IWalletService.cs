using Pi.Client.WalletService.Api;
using Pi.GlobalEquities.Services.Velexa;

namespace Pi.GlobalEquities.Services.Wallet;

public interface IWalletService : IWalletApi
{
    /// <summary>
    /// Replace calling SecureWalletProductLineAvailabilityGetAsync of Wallet API by duplicate logic from Wallet API
    /// </summary>
    /// <param name="providerAccount"></param>
    /// <param name="accountSummary"></param>
    /// <param name="activeOrders"></param>
    /// <param name="currency"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<decimal> CustomWalletProductLineAvailabilityGetAsync(
        string providerAccount,
        VelexaModel.PositionResponse accountSummary,
        IEnumerable<IOrder> activeOrders,
        Currency currency,
        CancellationToken ct);

    /// <summary>
    /// This method may increase rate limit issue on Velexa API, please consider to use <see cref="CustomWalletProductLineAvailabilityGetAsync"/>
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="custCode"></param>
    /// <param name="currency"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<decimal> SecureWalletProductLineAvailabilityGetAsync(string userId, string custCode,
        Currency currency, CancellationToken ct);
    Task<ExchangeRate> GetExchangeRate(
        Currency from,
        Currency to,
        CancellationToken ct);
}
