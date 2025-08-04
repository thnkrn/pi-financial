using Pi.Financial.FundService.Application.Models;

namespace Pi.Financial.FundService.Application.Services.ItBackofficeService;

public interface IItBackofficeService
{
    Task<List<BoFundAssetResponse>> GetAccountBalanceAsync(string accountNo, string amc, CancellationToken cancellationToken = default);
}
