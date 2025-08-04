using Pi.Common.ExtensionMethods;
using Pi.Common.Features;
using Pi.PortfolioService.Application.Models;
using Pi.PortfolioService.Application.Services.Models.StructureNote;
using Pi.PortfolioService.DomainServices;

namespace Pi.PortfolioService.API.Services;

public class PortfolioManager : IPortfolioSummaryQueries
{
    private readonly ISiriusService _siriusService;
    private readonly IGeService _geService;
    private readonly IStructureNoteService _structureNoteService;
    private readonly IFundService _fundService;
    private readonly ISetService _setService;
    private readonly ITfexService _tfexService;
    private readonly IBondService _bondService;
    private readonly IFeatureService _featureService;
    private readonly IUserService _userService;
    private readonly ILogger<PortfolioManager> _logger;

    public PortfolioManager(
        ISiriusService siriusService,
        IGeService geService,
        IStructureNoteService structureNoteService,
        IFeatureService featureService,
        IFundService fundService,
        ISetService setService,
        ITfexService tfexService,
        IBondService bondService,
        IUserService userService,
        ILogger<PortfolioManager> logger
    )
    {
        _siriusService = siriusService;
        _geService = geService;
        _structureNoteService = structureNoteService;
        _featureService = featureService;
        _fundService = fundService;
        _setService = setService;
        _tfexService = tfexService;
        _bondService = bondService;
        _userService = userService;
        _featureService = featureService;
        _logger = logger;
    }

    public async Task<PortfolioSummary> GetPortfolioSummaryV2Async(
        string userId,
        string valueUnit,
        CancellationToken ct = default)
    {
        const string currency = "THB";
        var tasks = await FetchSummariesTasks(userId, currency, ct);

        // Await all tasks and combine results
        var results = await Task.WhenAll(tasks);

        var portfolioSummary = PortfolioSummary.Default(currency: currency);

        var snAccountType = PortfolioAccountType.Offshore.ToString();
        var snAccounts = new List<PortfolioAccount>();

        foreach (var result in results)
        {
            foreach (var account in result)
            {
                if (!string.IsNullOrWhiteSpace(account.ErrorMessage)) continue;

                if (account.AccountType == snAccountType)
                    snAccounts.Add(account);
                else
                    portfolioSummary.PortfolioAccounts.Add(account);
            }
        }

        var isRemoveDuplicateFeatureOn = _featureService.IsOn(Features.RemoveDuplicateTradingAccount);
        if (isRemoveDuplicateFeatureOn)
        {
            //remove all account that custCode equal structure notes custCode (remove custCode subtype=5, we don't use it anymore)
            portfolioSummary.PortfolioAccounts.RemoveAll(x => snAccounts.Any(y => y.CustCode == x.CustCode));
        }
        portfolioSummary.PortfolioAccounts.AddRange(snAccounts);

        portfolioSummary.PortfolioErrorAccounts.AddRange(
            results.SelectMany(x =>
            {
                return x.Where(account => !string.IsNullOrWhiteSpace(account.ErrorMessage))
                    .Select(account => new PortfolioErrorAccount(
                        account.AccountType,
                        account.AccountId,
                        account.ErrorMessage,
                        account.AccountNoForDisplay));
            })
        );

        return RecalculatePortfolio(portfolioSummary);
    }

    public async Task<PortfolioSummary> GetPortfolioSummaryAsync(string sid, Guid deviceId, string userId,
        string valueUnit, CancellationToken ct = default)
    {
        var currency = "THB";

        var getFundTask = _featureService.IsOn(Features.FetchPortfolioV2)
            ? _fundService.GetPortfolioAccounts(Guid.Parse(userId), ct)
            : _fundService.GetPortfolioAccountsOld(Guid.Parse(userId), ct);

        var isGeFlagOn = _featureService.IsOn(Features.GlobalEquityPiApi);
        var getSiriusPortfolio = _siriusService.GetByToken(sid, deviceId, valueUnit, ct);
        var getMfPortfolio = _featureService.IsOn(Features.SummaryPiApi)
            ? getFundTask
            : Task.FromResult<IEnumerable<PortfolioAccount>>(new List<PortfolioAccount>());
        var getSetPortfolio = _featureService.IsOn(Features.SetPiApi)
            ? _setService.GetPortfolioAccounts(Guid.Parse(userId), sid, ct)
            : Task.FromResult<IEnumerable<PortfolioAccount>>(new List<PortfolioAccount>());
        var getGePortfolio = isGeFlagOn
            ? _geService.GetAccounts(userId, currency, ct)
            : Task.FromResult<IEnumerable<PortfolioAccount>>(new List<PortfolioAccount>());
        var getTfexPortfolio = _featureService.IsOn(Features.TfexMigration)
            ? _tfexService.GetPortfolioSummary(userId, ct)
            : Task.FromResult(new List<TfexPortfolioSummary>());
        var getSnPortfolio = _structureNoteService.GetStructureNotes(userId, currency, ct);
        var getBondPortfolio = _featureService.IsOn(Features.BondMigration)
            ? _bondService.GetAccountsOverview(userId, ct)
            : Task.FromResult(Enumerable.Empty<PortfolioAccount>());

        await Task.WhenAll(
            getSiriusPortfolio,
            getMfPortfolio,
            getSetPortfolio,
            getGePortfolio,
            getTfexPortfolio,
            getSnPortfolio,
            getBondPortfolio);

        var portfolioSummary = await getSiriusPortfolio;

        var mfPortfolios = _featureService.IsOn(Features.SummaryPiApi)
            ? await ReplacePiMutualFundAccount(portfolioSummary, getMfPortfolio)
            : new List<PortfolioAccount>();

        var setPortfolios = _featureService.IsOn(Features.SetPiApi)
            ? await ReplacePiSetAccount(portfolioSummary, getSetPortfolio)
            : new List<PortfolioAccount>();

        var piPortfolioAccounts = new List<PortfolioAccount>();
        piPortfolioAccounts.AddRange(mfPortfolios);
        piPortfolioAccounts.AddRange(setPortfolios);

        if (piPortfolioAccounts.Any())
        {
            ReplaceWithPiPortfolioAccounts(portfolioSummary, piPortfolioAccounts);
        }

        if (isGeFlagOn)
            await ReplaceGeAccount(portfolioSummary, getGePortfolio, userId);
        else
            _logger.LogInformation(
                "Feature flag 'global-equity-trading-migration'. GlobalEquityFlag: {GlobalEquityFlag}. UserId: {UserId}",
                isGeFlagOn, userId);

        if (_featureService.IsOn(Features.TfexMigration))
            await ReplaceDerivativeAccount(portfolioSummary, getTfexPortfolio);

        await AddStructureNotePortfolio(portfolioSummary, getSnPortfolio, ct);
        await AddBondPortfolio(portfolioSummary, getBondPortfolio, ct);

        return RecalculatePortfolio(portfolioSummary);
    }

    private async Task<List<Task<IEnumerable<PortfolioAccount>>>> FetchSummariesTasks(string userId, string currency,
        CancellationToken ct = default)
    {
        if (_featureService.IsOff(Features.FetchTradingAccounts))
        {
            return new List<Task<IEnumerable<PortfolioAccount>>>
            {
                _fundService.GetPortfolioAccounts(Guid.Parse(userId), ct),
                _setService.GetPortfolioAccounts(Guid.Parse(userId), ct),
                _geService.GetAccounts(userId, currency, ct),
                _tfexService.GetPortfolioAccount(userId, ct),
                _structureNoteService.GetStructureNotesPortfolioAccount(userId, currency, ct),
                _featureService.IsOn(Features.BondMigration)
                    ? _bondService.GetAccountsOverview(userId, ct)
                    : Task.FromResult(Enumerable.Empty<PortfolioAccount>())
            };
        }

        var tasks = new List<Task<IEnumerable<PortfolioAccount>>>();

        var tradingAccounts = await _userService.GetTradingAccountsAsync(Guid.Parse(userId), ct);
        tradingAccounts = tradingAccounts.ToList();
        if (!tradingAccounts.Any())
        {
            return new List<Task<IEnumerable<PortfolioAccount>>>();
        }
        var products = tradingAccounts.DistinctBy(q => q.Product.ToString()).Select(q => q.Product);

        // call by default
        tasks.Add(_structureNoteService.GetStructureNotesPortfolioAccount(userId, currency, ct));

        var executed = new HashSet<string>();
        foreach (var product in products)
        {
            if (new[] { Product.CashBalance, Product.Cash, Product.CreditBalance, Product.Dr }.Contains(product) && executed.Add("set"))
            {
                tasks.Add(_setService.GetPortfolioAccounts(Guid.Parse(userId), ct));
            }

            if (new[] { Product.CashBalance, Product.Cash, Product.Bond }.Contains(product) && executed.Add("bond"))
            {
                tasks.Add(_featureService.IsOn(Features.BondMigration)
                    ? _bondService.GetAccountsOverview(userId, ct)
                    : Task.FromResult(Enumerable.Empty<PortfolioAccount>()));
            }

            switch (product)
            {
                case Product.GlobalEquities:
                    tasks.Add(_geService.GetAccounts(userId, currency, ct));
                    break;
                case Product.Funds:
                    tasks.Add(_fundService.GetPortfolioAccounts(Guid.Parse(userId), ct));
                    break;
                case Product.Derivatives:
                    tasks.Add(_tfexService.GetPortfolioAccount(userId, ct));
                    break;
            }
        }

        return tasks;
    }

    private async Task AddStructureNotePortfolio(
        PortfolioSummary portfolioSummary,
        Task<IEnumerable<StructureNoteAccountSummary>?> getStructureNotes,
        CancellationToken ct = default)
    {
        try
        {
            var structureNotes = await getStructureNotes;

            var offshoreAccType = PortfolioAccountType.Offshore.GetEnumDescription();

            if (structureNotes == null)
                portfolioSummary.GeneralErrors.Add(new GeneralError(offshoreAccType,
                    "Failed to get account overview from Structure Notes API."));

            else
            {
                structureNotes = structureNotes.ToArray();
                if (!structureNotes.Any())
                    return;

                var accountPortfolios = structureNotes.Select(sn =>
                    new PortfolioAccount(sn.AccountType, sn.AccountId, sn.AccountNoForDisplay,
                        sn.AccountNoForDisplay, sn.CustCode, false, sn.TotalMarketValue, 0,
                        sn.Upnl, sn.ErrorMessage)).ToArray();

                if (_featureService.IsOn(Features.RemoveDuplicateTradingAccount))
                {
                    portfolioSummary.PortfolioAccounts.RemoveAll(account =>
                        accountPortfolios.Any(snAccount => snAccount.CustCode == account.CustCode));
                }

                portfolioSummary.PortfolioAccounts.AddRange(accountPortfolios);

                var snPortfolioWalletCategoried = new PortfolioWalletCategorized(
                    PortfolioAccountType.Offshore.ToString(),
                    accountPortfolios.Sum(sn => sn.TotalValue),
                    accountPortfolios.Sum(sn => sn.CashBalance),
                    accountPortfolios.Sum(sn => sn.Upnl),
                    0
                );
                portfolioSummary.PortfolioWalletCategorizeds.Add(snPortfolioWalletCategoried);

                var errorAccounts = accountPortfolios.Where(sn => !string.IsNullOrEmpty(sn.ErrorMessage));

                var portfolioErrorAccounts = errorAccounts.Select(errorAccount =>
                    new PortfolioErrorAccount(
                        errorAccount.AccountType,
                        errorAccount.AccountId,
                        errorAccount.ErrorMessage,
                        errorAccount.AccountNoForDisplay));

                portfolioSummary.PortfolioErrorAccounts.AddRange(portfolioErrorAccounts);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(AddStructureNotePortfolio)} failed with error: {{ExMessage}}", ex.Message);
        }
    }

    private async Task AddBondPortfolio(
        PortfolioSummary portfolioSummary,
        Task<IEnumerable<PortfolioAccount>> getBondPortfolio,
        CancellationToken ct = default)
    {
        try
        {
            var bondPorts = await getBondPortfolio;

            var bondAccType = PortfolioAccountType.Bond.GetEnumDescription();

            bondPorts = bondPorts.ToArray();
            if (bondPorts == null || !bondPorts.Any())
            {
                portfolioSummary.GeneralErrors.Add(new GeneralError(bondAccType,
                    "Failed to get account overview from Bond API."));

                return;
            }

            ReplaceAccounts(bondAccType, portfolioSummary, bondPorts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"AddBondPortfolio failed with error: {{ExMessage}}", ex.Message);
        }
    }

    private async Task<List<PortfolioAccount>> ReplacePiSetAccount(PortfolioSummary portfolioSummary,
        Task<IEnumerable<PortfolioAccount>> getSetPortfolio)
    {
        var validAccountTypes = new HashSet<string>
        {
            PortfolioAccountType.Cash.GetEnumDescription(),
            PortfolioAccountType.CashBalance.GetEnumDescription(),
            PortfolioAccountType.CreditBalance.GetEnumDescription(),
        };

        var includeMargin = _featureService.IsOn(Features.SetMarginSummary);
        var balances = await getSetPortfolio;
        var balancesMap = balances.Where(q => q.AccountType != PortfolioAccountType.CreditBalance.GetEnumDescription() || includeMargin)
            .ToDictionary(GeneratePortfolioMapKey, v => v);

        var replace = new List<PortfolioAccount>();
        portfolioSummary.PortfolioAccounts.ForEach(sirius =>
        {
            if (!validAccountTypes.Contains(sirius.AccountType))
            {
                return;
            }

            if (balancesMap.TryGetValue(GeneratePortfolioMapKey(sirius), out var pi))
            {
                replace.Add(pi with { AccountId = sirius.AccountId });
            }
            else
            {
                replace.Add(sirius with
                {
                    TradingAccountNo = $"{sirius.AccountNoForDisplay[..^1]}-{sirius.AccountNoForDisplay[^1..]}"
                });
            }
        });

        return replace;
    }

    async Task ReplaceGeAccount(
        PortfolioSummary summary,
        Task<IEnumerable<PortfolioAccount>> getGePortfolio,
        string userId)
    {
        var geSummaries = await getGePortfolio;

        var geAccType = PortfolioAccountType.GlobalEquities.GetEnumDescription();

        var portfolioAccounts = geSummaries.ToList();
        if (!portfolioAccounts.Any())
        {
            summary.GeneralErrors.Add(new GeneralError(geAccType,
                "Failed to get account overview from Global Equities API."));
            _logger.LogInformation("`geSummaries` is null. UserId: {UserId}", userId);
        }
        else
            ReplaceAccounts(geAccType, summary, portfolioAccounts);
    }

    private static void ReplaceAccounts(string acctype, PortfolioSummary summary,
        IEnumerable<PortfolioAccount> portfolioAcc)
    {
        var accounts = portfolioAcc.ToArray();

        summary.PortfolioAccounts.RemoveAll(x => x.AccountType == acctype);
        summary.PortfolioAccounts.AddRange(accounts);

        summary.PortfolioErrorAccounts.RemoveAll(x => x.AccountType == acctype);
        var errorAccounts = accounts.Where(sn => !string.IsNullOrEmpty(sn.ErrorMessage));
        foreach (var account in errorAccounts)
        {
            summary.PortfolioErrorAccounts.Add(new PortfolioErrorAccount(
                account.AccountType,
                account.AccountId,
                account.ErrorMessage,
                account.AccountNoForDisplay));
        }
    }

    private async Task<List<PortfolioAccount>> ReplacePiMutualFundAccount(PortfolioSummary portfolioSummary,
        Task<IEnumerable<PortfolioAccount>> getMfPortfolio)
    {
        var fundPortfolioAccounts = await getMfPortfolio;

        var fundPortfolioAccountsMap = fundPortfolioAccounts.ToDictionary(
            GeneratePortfolioMapKey,
            v => v);

        var replace = new List<PortfolioAccount>();
        portfolioSummary.PortfolioAccounts.ForEach(sirius =>
        {
            if (!string.Equals(
                    sirius.AccountType.Replace(" ", ""),
                    PortfolioAccountType.MutualFund.ToString(),
                    StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            if (fundPortfolioAccountsMap.TryGetValue(GeneratePortfolioMapKey(sirius), out var pi))
                replace.Add(pi with { AccountId = sirius.AccountId });
        });

        return replace;
    }

    private async Task ReplaceDerivativeAccount(PortfolioSummary portfolioSummary,
        Task<List<TfexPortfolioSummary>> getTfexPortfolio)
    {
        const string derivative = "Derivative";
        var response = await getTfexPortfolio;

        if (!response.Any())
        {
            var accountType = PortfolioAccountType.Derivative.GetEnumDescription();
            portfolioSummary.GeneralErrors.Add(new GeneralError(accountType,
                "Failed to get account overview from TFEX API."));
            return;
        }

        // backward compatible
        var errorMessage = portfolioSummary.PortfolioErrorAccounts
            .Find(p => p.AccountType == derivative && !string.IsNullOrEmpty(p.ErrorMessage))?
            .ErrorMessage ?? string.Empty;
        var accountIdDictionary = portfolioSummary.PortfolioAccounts
            .Where(p => p.AccountType == derivative && !string.IsNullOrEmpty(p.AccountId))
            .GroupBy(p => p.CustCode)
            .ToDictionary(g => g.Key, g => g.First().AccountId);

        // pi tfex account list
        var portfolioAccountList = response.Where(t => t.IsSuccess).Select(p =>
            {
                var accountId = accountIdDictionary.GetValueOrDefault(p.CustCode) ?? string.Empty;
                return new PortfolioAccount(derivative, accountId,
                        p.TradingAccountNo, p.TradingAccountNo, p.CustCode, false,
                        p.TotalMarketValue, p.CashBalance, p.Upnl, errorMessage)
                { TotalValue = p.TotalValue };
            }
        ).ToList();
        var portfolioWalletCategorizedList = response.Where(t => t.IsSuccess).Select(p =>
            new PortfolioWalletCategorized(derivative, p.TotalMarketValue, p.CashBalance, p.Upnl, 0)
            { TotalValue = p.TotalValue });

        // override sirius
        var replacingCustCodes = portfolioAccountList.Select(p => p.CustCode).ToList();

        // Remove all entries when pi tfex available
        portfolioSummary.PortfolioWalletCategorizeds.RemoveAll(p => p.AccountType == derivative);
        portfolioSummary.PortfolioAccounts.RemoveAll(p =>
            p.AccountType == derivative && replacingCustCodes.Contains(p.CustCode));

        // Add the updated lists.
        portfolioSummary.PortfolioWalletCategorizeds.AddRange(portfolioWalletCategorizedList);
        portfolioSummary.PortfolioAccounts.AddRange(portfolioAccountList);
    }

    private static string GeneratePortfolioMapKey(PortfolioAccount account)
    {
        return $"{account.AccountType}-{account.AccountNoForDisplay.Replace("-", "")}";
    }

    private static void ReplaceWithPiPortfolioAccounts(PortfolioSummary portfolioSummary,
        List<PortfolioAccount> piPortfolioAccounts)
    {
        piPortfolioAccounts.ForEach(pi =>
        {
            portfolioSummary.PortfolioAccounts.RemoveAll(sirius =>
            {
                if (pi.AccountType != sirius.AccountType)
                {
                    return false;
                }

                return sirius.AccountId == pi.AccountId;
            });
            portfolioSummary.PortfolioAccounts.Add(pi);
        });
    }

    private static PortfolioSummary RecalculatePortfolio(PortfolioSummary portfolioSummary)
    {
        return portfolioSummary with
        {
            AsOfDate = DateTime.UtcNow,
            PortfolioWalletCategorizeds = portfolioSummary.PortfolioAccounts.GroupBy(q => q.AccountType,
                (accountType, accounts) =>
                {
                    accounts = accounts.ToArray();
                    var wl = new PortfolioWalletCategorized(accountType,
                        accounts.Sum(a => a.TotalMarketValue),
                        accounts.Sum(a => a.CashBalance),
                        accounts.Sum(a => a.Upnl),
                        accounts.Sum(a => a.TotalMarketValue)
                    );

                    return wl with
                    {
                        AssetRatioInAllAsset = portfolioSummary.TotalValue != 0
                            ? wl.TotalValue / portfolioSummary.TotalValue * 100
                            : 0
                    };
                }).ToList()
        };
    }
}
