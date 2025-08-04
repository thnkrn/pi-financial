using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Models.Trading;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using RawFundOrder = Pi.Financial.Client.FundConnext.Model.FundOrder;

namespace Pi.Financial.FundService.Application.Queries;

public interface IFundQueries
{
    Task<List<AccountSummary>> GetAccountSummariesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<FundAsset>> GetAccountBalanceByTradingAccountNoAsync(Guid userId, string? tradingAccountNo = null,
        CancellationToken cancellationToken = default);

    Task<List<FundOrder>> GetAccountFundOrdersByTradingAccountNoAsync(Guid userId, string tradingAccountNo, FundOrderFilters filters,
        CancellationToken cancellationToken = default);
    Task<List<FundOrder>> GetFundOrdersByOrderNoAsync(List<string> orderNoList, FundOrderFilters filters, CancellationToken cancellationToken = default);
    Task<SwitchInfo> GetSwitchInfoByTradingAccountNoAsync(Guid userId, string tradingAccountNo, string fundCode, string counterFundCode, CancellationToken cancellationToken = default);
    Task<bool> IsFundAccountExistAsync(string identificationCardNo, string? passportCountry = null, CancellationToken cancellationToken = default);
    Task<List<RawFundOrder>> GetRawOrdersAsync(DateOnly effectiveDate, CancellationToken cancellationToken = default);
}

public class FundAccountNotFoundException : Exception
{
    public FundAccountNotFoundException(string? message) : base(message)
    {
    }

    public FundAccountNotFoundException()
    {
    }
}
