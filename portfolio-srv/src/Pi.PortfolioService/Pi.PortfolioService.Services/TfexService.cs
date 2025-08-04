using Microsoft.Extensions.Logging;
using Pi.Client.GlobalEquities.Api;
using Pi.Client.TfexService.Api;
using Pi.Common.ExtensionMethods;
using IAccountApi = Pi.Client.TfexService.Api.IAccountApi;


namespace Pi.PortfolioService.Services;

public class TfexService : ITfexService
{
    private readonly IAccountApi _accountApi;
    private readonly ILogger<TfexService> _logger;

    public TfexService(IAccountApi accountApi, ILogger<TfexService> logger)
    {
        _accountApi = accountApi;
        _logger = logger;
    }

    public async Task<List<TfexPortfolioSummary>> GetPortfolioSummary(string userId, CancellationToken ct)
    {
        try
        {
            var response = await _accountApi.SecureUserIdPortfolioSummaryGetAsync(userId, ct);

            return response.Data.Select(p => new TfexPortfolioSummary(
                p.CustCode,
                p.IsSuccess,
                p.TotalMarketValue,
                p.UpnlPercentage,
                p.TradingAccountNo,
                p.Upnl,
                p.CashBalance,
                p.TotalValue)).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "TfexService:GetPortfolioSummary: Unable to get portfolio summary with userId: {userId}", userId);
            return new List<TfexPortfolioSummary>();
        }
    }

    public async Task<IEnumerable<PortfolioAccount>> GetPortfolioAccount(string userId, CancellationToken ct)
    {
        try
        {
            var response = await _accountApi.SecureUserIdPortfolioSummaryGetAsync(userId, ct);

            return response.Data.Select(p => new PortfolioAccount(
                PortfolioAccountType.Derivative.GetEnumDescription(),
                string.Empty,
                p.TradingAccountNo,
                p.TradingAccountNo,
                p.CustCode,
                false,
                p.TotalMarketValue,
                p.CashBalance,
                p.Upnl,
                string.Empty)
            {
                TotalValue = p.TotalValue
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "TfexService:GetPortfolioAccount: Unable to get portfolio summary with userId: {userId}", userId);
            return new List<PortfolioAccount>();
        }
    }
}