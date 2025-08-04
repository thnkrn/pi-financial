using System.Globalization;
using Microsoft.Extensions.Logging;
using Pi.Common.CommonModels;
using Pi.GlobalEquities.Application.Services.Velexa;
using Pi.GlobalEquities.Application.Services.Wallet;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Application.Queries.Wallet;

public class WalletQueries : IWalletQueries
{
    private readonly IVelexaReadService _velexaReadService;
    private readonly IWalletService _walletService;
    public ILogger<WalletQueries> _logger;

    public WalletQueries(IVelexaReadService velexaReadService,
        IWalletService walletService, ILogger<WalletQueries> logger)
    {
        _velexaReadService = velexaReadService;
        _walletService = walletService;
        _logger = logger;
    }

    public async Task<decimal> GetLineAvailable(string userId, string custCode, Currency currency, CancellationToken ct)
    {
        //wallet API returns only USD (for now)
        var getWalletResult = _walletService.GetLineAvailable(userId, custCode, ct);

        var tasks = new List<Task> { getWalletResult };

        Task<decimal> getExchangeRateTask = Task.FromResult(0m);
        if (currency != Currency.USD)
        {
            getExchangeRateTask = _velexaReadService.GetExchangeRate(Currency.USD, currency, ct);
            tasks.Add(getExchangeRateTask);
        }

        await Task.WhenAll(tasks);

        var walletResult = await getWalletResult;
        var balance = walletResult;

        if (currency == Currency.USD)
            return balance;

        var exchangeRate = await getExchangeRateTask;
        balance *= exchangeRate;

        return balance;
    }

    public async Task<ExchangeRate> GetExchangeRate(Currency from, Currency to, CancellationToken ct)
    {
        if (from == to)
            return new() { From = from, To = to, Rate = 1 };

        var result = await _walletService.GetExchangeRate(from, to, ct);
        return result;
    }

    public decimal GetLineAvailableUsd(string accountId, AccountSummaryPosition accountSummary,
        IEnumerable<IOrder> activeOrders, decimal hkUsExRate)
    {
        return _walletService.GetLineAvailableUsd(accountId, accountSummary, activeOrders, hkUsExRate);
    }
}
