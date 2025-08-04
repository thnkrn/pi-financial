using Pi.Common.Domain.AggregatesModel.BankAggregate;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Application.Factories;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Models.Trading;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.ItBackofficeService;
using Pi.Financial.FundService.Application.Services.MarketService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Application.Utils;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using RawFundOrder = Pi.Financial.Client.FundConnext.Model.FundOrder;

namespace Pi.Financial.FundService.Application.Queries;

public class FundQueries : IFundQueries
{
    private readonly IFundConnextService _fundConnextService;
    private readonly IOnboardService _onboardService;
    private readonly IMarketService _marketService;
    private readonly IBankInfoRepository _bankInfoRepository;
    private readonly IItBackofficeService _itBackofficeService;
    private readonly IUserService _userService;
    private readonly IFundOrderRepository _fundOrderRepository;
    private const string FallbackAmc = "ASSETFUND";

    public FundQueries(IFundConnextService fundConnextService,
        IOnboardService onboardService,
        IMarketService marketService,
        IBankInfoRepository bankInfoRepository,
        IItBackofficeService itBackofficeService,
        IUserService userService,
        IFundOrderRepository fundOrderRepository)
    {
        _fundConnextService = fundConnextService;
        _onboardService = onboardService;
        _marketService = marketService;
        _bankInfoRepository = bankInfoRepository;
        _itBackofficeService = itBackofficeService;
        _userService = userService;
        _fundOrderRepository = fundOrderRepository;
    }

    public async Task<List<AccountSummary>> GetAccountSummariesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var custCodes = await _userService.GetFundCustomerCodesByUserId(userId, cancellationToken);
        custCodes = custCodes.ToArray();

        if (!custCodes.Any())
        {
            throw new FundOrderException(FundOrderErrorCode.FOE102);
        }

        var func = new Func<string, string, Task<AccountSummary?>>(async (custCode, accountNo) =>
        {
            try
            {
                bool accountExist = await _fundConnextService.CheckAccountExist(accountNo, cancellationToken);
                if (!accountExist)
                {
                    return null;
                }

                var assets = await GetAccountBalanceByTradingAccountNoAsync(custCode, accountNo, cancellationToken);

                return new AccountSummary(custCode, accountNo, DateTime.UtcNow, assets);
            }
            catch (Exception)
            {
                return null;
            }
        });

        var results = await Task.WhenAll(custCodes.Select(async custCode =>
        {
            var summaries = await Task.WhenAll(
                func(custCode, $"{custCode}-M"),
                func(custCode, $"{custCode}-1")
            );

            return summaries.Where(q => q != null).Select(q => q!);
        }));

        return results.SelectMany(q => q).ToList();
    }

    public async Task<List<FundAsset>> GetAccountBalanceByTradingAccountNoAsync(Guid userId, string? tradingAccountNo = null,
        CancellationToken cancellationToken = default)
    {
        var custCodes = await _userService.GetFundCustomerCodesByUserId(userId, cancellationToken);
        custCodes = custCodes.ToArray();

        if (!custCodes.Any())
        {
            throw new FundOrderException(FundOrderErrorCode.FOE102);
        }

        if (tradingAccountNo != null)
        {
            var custCode = await ValidateToGetCustCode(tradingAccountNo, cancellationToken);
            if (!custCodes.Contains(custCode))
            {
                throw new FundOrderException(FundOrderErrorCode.FOE102);
            }

            return await GetAccountBalanceByTradingAccountNoAsync(custCode, tradingAccountNo, cancellationToken);
        }

        var results = await Task.WhenAll(custCodes.Select(async custCode =>
        {
            var func = new Func<string, string, Task<List<FundAsset>>>(async (custCode, accountNo) =>
            {
                try
                {
                    return await GetAccountBalanceByTradingAccountNoAsync(custCode, accountNo, cancellationToken);
                }
                catch (FundOrderException)
                {
                    return new List<FundAsset>();
                }
            });

            var assets = await Task.WhenAll(
                func(custCode, $"{custCode}-M"),
                func(custCode, $"{custCode}-1")
            );

            return assets.SelectMany(q => q);
        }));
        return results.SelectMany(q => q).ToList();
    }

    private async Task<List<FundAsset>> GetAccountBalanceByTradingAccountNoAsync(string custCode, string tradingAccountNo, CancellationToken cancellationToken = default)
    {
        var assets = await _fundConnextService.GetAccountBalanceAsync(tradingAccountNo, cancellationToken);

        if (assets.Count == 0)
        {
            return new List<FundAsset>();
        }

        Dictionary<string, BoFundAssetResponse>? boFundAssetResponses = null;
        if (assets.Exists(q => q is { Unit: 0, Amount: 0, RemainAmount: 0, RemainUnit: 0 }))
        {
            var backofficeAssets = await _itBackofficeService.GetAccountBalanceAsync(tradingAccountNo, FallbackAmc, cancellationToken);
            boFundAssetResponses = backofficeAssets.ToDictionary(q => $"{q.FundCode}-{q.UnitholderId}", a => a);
        }

        var fundInfos = await _marketService.GetFundInfosAsync(
            assets.Select(q => q.FundCode).Distinct().ToArray(),
            cancellationToken
        );

        return assets.Select(q =>
        {
            if (q is { Unit: 0, Amount: 0, RemainAmount: 0, RemainUnit: 0 }
                && boFundAssetResponses != null
                && boFundAssetResponses.TryGetValue($"{q.FundCode}-{q.UnitholderId}", out var boAsset))
            {
                q = q with
                {
                    Unit = boAsset.Unit,
                    Amount = boAsset.Amount,
                    RemainUnit = boAsset.RemainUnit,
                    RemainAmount = boAsset.RemainAmount,
                    PendingUnit = boAsset.PendingUnit,
                    PendingAmount = boAsset.PendingAmount,
                };
            }

            var fundAsset = new FundAsset(q.FundCode, custCode, tradingAccountNo, q.UnitholderId)
            {
                Unit = q.Unit,
                AsOfDate = q.NavDate,
                MarketPrice = q.Nav,
                AvgCostPrice = q.AvgCost,
                RemainUnit = q.RemainUnit,
                RemainAmount = q.RemainAmount,
                PendingAmount = q.PendingAmount,
                PendingUnit = q.PendingUnit,
            };

            var fundInfo = fundInfos.FirstOrDefault(f => string.Equals(f.FundCode, fundAsset.FundCode, StringComparison.CurrentCultureIgnoreCase));
            if (fundInfo != null) fundAsset.SetInfo(fundInfo);

            return fundAsset;
        }).ToList();
    }

    public async Task<List<FundOrder>> GetAccountFundOrdersByTradingAccountNoAsync(Guid userId, string tradingAccountNo, FundOrderFilters filters, CancellationToken cancellationToken = default)
    {
        var custCode = await ValidateToGetCustCode(tradingAccountNo, cancellationToken);
        await ValidateUserCustCode(userId, custCode);

        var orders = await _fundConnextService.GetAccountFundOrdersAsync(tradingAccountNo,
            filters.EffectiveDateFrom,
            filters.EffectiveDateTo,
            cancellationToken);

        if (orders.Count == 0)
        {
            return new List<FundOrder>();
        }

        return await GetFundOrders(orders, filters, custCode, cancellationToken);
    }

    public async Task<List<FundOrder>> GetFundOrdersByOrderNoAsync(List<string> orderNoList, FundOrderFilters filters, CancellationToken cancellationToken = default)
    {
        // Get FundOrder from FundConnext API.
        var tasks = orderNoList.Select(
                orderNo => _fundConnextService.GetFundOrdersByOrderNoAsync(orderNo, cancellationToken)
            );
        var orders = (await Task.WhenAll(tasks))
            .SelectMany(x => x)
            .ToList();

        // Some orders weren't retrieved from the API, so we fetch those orders from the fund_order_state database instead.
        var notExistOrderNo = orderNoList.Except(orders.Select(x => x.OrderNo)).ToList();
        if (notExistOrderNo.Count != 0)
        {
            var fundOrderStates = await GetFundOrderStateByOrderNo(notExistOrderNo, cancellationToken);
            orders.AddRange(fundOrderStates);
        }

        // Fill BankInfo & FundInfo
        var result = await GetFundOrders(orders, filters, string.Empty, cancellationToken);
        result.ForEach(order =>
        {
            var custCode = TradingAccountHelper.GetCustCodeByFundTradingAccountNo(order.AccountId);
            if (!string.IsNullOrEmpty(custCode)) order.SetCustcode(custCode);
        });
        return result;
    }

    private async Task<List<FundOrder>> GetFundOrderStateByOrderNo(List<string> orderNoList, CancellationToken cancellationToken = default)
    {
        var fundOrderStates = await _fundOrderRepository.GetByOrderNoAsync(orderNoList.ToArray(), cancellationToken);
        return fundOrderStates.Select(FundTradingFactory.NewFundOrder).ToList();
    }

    public async Task<SwitchInfo> GetSwitchInfoByTradingAccountNoAsync(Guid userId, string tradingAccountNo, string fundCode, string counterFundCode,
        CancellationToken cancellationToken = default)
    {
        var custCode = await ValidateToGetCustCode(tradingAccountNo, cancellationToken);
        await ValidateUserCustCode(userId, custCode);
        var assets = await _fundConnextService.GetAccountBalanceAsync(tradingAccountNo, cancellationToken);

        var fundAsset = assets.Find(q => q.FundCode == fundCode);
        if (fundAsset == null)
        {
            return new SwitchInfo
            {
                MinSwitchUnit = 0m,
                MinSwitchAmount = 0m
            };
        }

        if (fundAsset.Nav == 0)
        {
            throw new FundOrderException(FundOrderErrorCode.FOE109);
        }

        var fundInfos = await _marketService.GetFundInfosAsync(
            new[] { fundCode, counterFundCode },
            cancellationToken
        );

        var infos = fundInfos.ToArray();
        var fund = Array.Find(infos, q => string.Equals(q.FundCode, fundCode, StringComparison.CurrentCultureIgnoreCase));
        var counterFund = Array.Find(infos, q => string.Equals(q.FundCode, counterFundCode, StringComparison.CurrentCultureIgnoreCase));

        if (fund == null || counterFund == null)
        {
            throw new FundOrderException(FundOrderErrorCode.FOE101);
        }

        var minSwitchOutUnit = fund.MinSellUnit;
        var minSwitchOutAmount = fund.MinSellAmount;

        if (decimal.Subtract(fundAsset.RemainUnit, fund.MinSellUnit) > 0 && fund.MinBalanceUnit > decimal.Subtract(fundAsset.RemainUnit, fund.MinSellUnit))
        {
            minSwitchOutUnit = fundAsset.RemainUnit;
        }

        if (decimal.Subtract(fundAsset.RemainAmount, fund.MinSellAmount) > 0 && fund.MinBalanceAmount > decimal.Subtract(fundAsset.RemainAmount, fund.MinSellAmount))
        {
            minSwitchOutAmount = fundAsset.RemainAmount;
        }

        var minSwitchInAmount = counterFund.FirstMinBuyAmount;
        var counterFundNav = counterFund.Nav;
        var counterFundAsset = assets.Find(q => q.FundCode == counterFundCode);
        if (counterFundAsset != null)
        {
            minSwitchInAmount = counterFund.NextMinBuyAmount;
            counterFundNav = counterFundAsset.Nav;
        }

        var minSwitchInUnit = counterFundNav != null && counterFundNav != 0 ? decimal.Divide(minSwitchInAmount, (decimal)counterFundNav) : 0; // This is estimate min buy unit

        return new SwitchInfo
        {
            MinSwitchUnit = decimal.Max(minSwitchOutUnit, minSwitchInUnit),
            MinSwitchAmount = decimal.Max(minSwitchOutAmount, minSwitchInAmount)
        };
    }

    public async Task<bool> IsFundAccountExistAsync(string identificationCardNo, string? passportCountry = null, CancellationToken cancellationToken = default)
    {
        var investorProfile = await _fundConnextService.GetCustomerProfileAndAccount(identificationCardNo, passportCountry, cancellationToken);
        return investorProfile?.Accounts?.Count > 0;
    }

    private async Task ValidateUserCustCode(Guid userId, string custCode)
    {
        var custCodes = await _userService.GetCustomerCodesByUserId(userId);
        if (!custCodes.Contains(custCode))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE102);
        }
    }

    private async Task<string> ValidateToGetCustCode(string tradingAccountNo, CancellationToken cancellationToken = default)
    {
        var custCode = TradingAccountHelper.GetCustCodeByFundTradingAccountNo(tradingAccountNo);

        if (custCode == null)
        {
            throw new FundOrderException(FundOrderErrorCode.FOE102);
        }

        var fundTradingAccount = await _onboardService.GetMutualFundTradingAccountByCustCodeAsync(custCode, cancellationToken);

        if (fundTradingAccount == null)
        {
            throw new FundOrderException(FundOrderErrorCode.FOE102);
        }

        return custCode;
    }

    /// <summary>
    /// Using by Incentive System
    /// </summary>
    public async Task<List<RawFundOrder>> GetRawOrdersAsync(DateOnly effectiveDate, CancellationToken cancellationToken = default)
    {
        var fundOrders = await _fundConnextService.GetRawFundOrdersAsync(effectiveDate, cancellationToken);
        return fundOrders;
    }

    private async Task<List<FundOrder>> GetFundOrders(List<FundOrder> orders, FundOrderFilters filters, string custCode, CancellationToken cancellationToken = default)
    {
        var bankInfos = await _bankInfoRepository.GetByBankCodesAsync(
            orders.Select(q => new string(q.BankCode?.Trim() != "" ? q.BankCode?.Trim() : null)).Distinct().ToArray(), cancellationToken);

        var fundInfos = await _marketService.GetFundInfosAsync(
            orders.Select(q => q.FundCode).Distinct().ToArray(),
            cancellationToken
        );

        var result = new List<FundOrder>();
        orders.ForEach(q =>
        {
            if (!filters.FundStatus.Contains(q.Status) ||
                (filters.OrderType != null && filters.OrderType != q.OrderType))
            {
                return;
            }

            q.SetCustcode(custCode);

            var bankInfo = bankInfos.FirstOrDefault(f => f.Code == q.BankCode);
            if (bankInfo != null) q.SetBankInfo(bankInfo);

            var fundInfo = fundInfos.FirstOrDefault(f => string.Equals(f.FundCode, q.FundCode, StringComparison.CurrentCultureIgnoreCase));
            if (fundInfo != null) q.SetFundInfo(fundInfo);

            result.Add(q);
        });

        return result;
    }
}
