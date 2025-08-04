using Pi.Client.OnboardService.Model;

namespace Pi.User.Application.Services.Onboard;

public interface IOnboardTradingAccountService
{
    /// <summary>
    /// Returns list of trading accounts belonging to <paramref name="customerCode"/>.
    /// </summary>
    /// <param name="customerCode">Customer code to query trading accounts for.</param>
    /// <param name="withBankAccounts">Choose if each trading account should include bank accounts.</param>
    /// <param name="withExternalAccounts">Choose if each trading account should include external accounts.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of trading accounts.</returns>
    public Task<List<PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccount>>
        GetTradingAccountListByCustomerCodeAsync(string customerCode, bool withBankAccounts = false,
            bool withExternalAccounts = false, CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Returns list of trading accounts belonging to <paramref name="identificationNo"/>, grouped by customer code.
    /// </summary>
    /// <param name="identificationNo">Customer code to query trading accounts for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of trading accounts.</returns>
    public Task<List<PiOnboardServiceApplicationQueriesTradingAccountCustomerTradingAccountsByCustomerCode>>
        GetTradingAccountListGroupedByCustomerCodeByIdentificationNumberAsync(string identificationNo,
            CancellationToken cancellationToken = default(CancellationToken));
}