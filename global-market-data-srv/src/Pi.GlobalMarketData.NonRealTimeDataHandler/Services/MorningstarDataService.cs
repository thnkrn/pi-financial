using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Request;
using Pi.GlobalMarketData.Domain.Models.Response;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.NonRealTimeDataHandler.constants;
using Pi.GlobalMarketData.NonRealTimeDataHandler.Helpers;
using Pi.GlobalMarketData.NonRealTimeDataHandler.interfaces;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.Services;

public class MorningStarDataService : BackgroundService, IMorningStarDataService
{
    private readonly string? _email;
    private readonly HttpClient _httpClient;
    private readonly ILogger<MorningStarDataService> _logger;
    private readonly IMongoService<MorningStarFlag> _morningStarFlagService;
    private readonly IMongoService<MorningStarStocks> _morningStarStocksService;
    private readonly string? _password;
    private readonly List<string> _quarterlyFocus = ["Q1", "Q2", "Q3", "Q4"];
    private readonly List<string> _semiAnnualFocus = ["S1", "S2"];
    private readonly IHostApplicationLifetime _appLifetime;

    /// <summary>
    ///
    /// </summary>
    /// <param name="httpClientFactory"></param>
    /// <param name="configuration"></param>
    /// <param name="morningStarFlagService"></param>
    /// <param name="morningStarStocksService"></param>
    /// <param name="logger"></param>
    public MorningStarDataService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IMongoService<MorningStarFlag> morningStarFlagService,
        IMongoService<MorningStarStocks> morningStarStocksService,
        ILogger<MorningStarDataService> logger,
        IHostApplicationLifetime appLifetime
    )
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientKeys.MorningStar);
        _email = configuration.GetValue<string>(ConfigurationKeys.MorningStarEmail);
        _password = configuration.GetValue<string>(ConfigurationKeys.MorningStarPassword);
        _morningStarFlagService = morningStarFlagService;
        _morningStarStocksService = morningStarStocksService;
        _appLifetime = appLifetime;

        _logger = logger;
    }

    public async Task MorningStarService(MorningStarDataHelper helper)
    {
        var requests = await _morningStarFlagService.GetAllAsync();
        var tokenEntity = await helper.Login(_email ?? "", _password ?? "");

        if (tokenEntity is { IsSuccess: true })
        {
            foreach (var request in requests)
            {
                if (request == null)
                    continue;

                if (!request.IsSubscribe)
                    continue;

                var statementTypeRequest = new MorningStarStatementTypeRequest
                {
                    ExchangeId = request.ExchangeId ?? string.Empty,
                    Identifier = request.StandardTicker ?? string.Empty,
                    StatementType = request.StatementType ?? string.Empty,
                    DataType = request.DataType ?? string.Empty,
                    StartDate = request.StartDate ?? string.Empty,
                    EndDate = request.EndDate ?? string.Empty
                };

                var periodRequest = new MorningStarPeriodRequest
                {
                    ExchangeId = request.ExchangeId ?? string.Empty,
                    Identifier = request.StandardTicker ?? string.Empty
                };

                var exchangeIdRequest = new MorningStarExchangeIdRequest
                {
                    ExchangeId = request.ExchangeId ?? string.Empty,
                    Identifier = request.StandardTicker ?? string.Empty
                };

                var excludingPeriodRequest = new MorningStarExcludingPeriodRequest
                {
                    ExchangeId = request.ExchangeId ?? string.Empty,
                    Identifier = request.StandardTicker ?? string.Empty,
                    ExcludingFrom = request.ExcludingFrom ?? string.Empty,
                    ExcludingTo = request.ExcludingTo ?? string.Empty
                };

                var morningStarStockMongo = await _morningStarStocksService.GetByFilterAsync(
                    target =>
                        target.Symbol == request.StandardTicker
                        && target.ExchangeId == request.ExchangeId
                );

                var morningStarStock =
                    morningStarStockMongo
                    ?? new MorningStarStocks
                    {
                        Symbol = request.StandardTicker ?? string.Empty,
                        ExchangeId = request.ExchangeId ?? string.Empty
                    };

                await Task.WhenAll(
                    // statementTypeRequest
                    CallIncomeStatement(helper, statementTypeRequest, morningStarStock),
                    CallBalanceSheet(helper, statementTypeRequest, morningStarStock),
                    CallProfitabilityRatios(helper, statementTypeRequest, morningStarStock),
                    // periodRequest
                    CallValuationRatios(helper, periodRequest, morningStarStock),
                    // exchangeIdRequest
                    CallCompanyFinancialAvailabilityList(
                        helper,
                        exchangeIdRequest,
                        morningStarStock
                    ),
                    CallCurrentMarketCapitalization(helper, exchangeIdRequest, morningStarStock),
                    CallSharesSnapshot(helper, exchangeIdRequest, morningStarStock),
                    CallCompanyGeneralInformation(helper, exchangeIdRequest, morningStarStock),
                    CallBusinessDescription(helper, exchangeIdRequest, morningStarStock),
                    // excludingPeriodRequest
                    CallCashDividends(helper, excludingPeriodRequest, morningStarStock)
                );

                await _morningStarStocksService.UpsertAsyncByFilterBsonDocument(
                    target =>
                        target.Symbol == request.StandardTicker
                        && target.ExchangeId == request.ExchangeId,
                    morningStarStock
                );
            }
        }
    }

    public async Task CallIncomeStatement(
        MorningStarDataHelper helper,
        MorningStarStatementTypeRequest request,
        MorningStarStocks morningStarStocks
    )
    {
        var response = await helper.GetIncomeStatement(request, null);
        _logger.LogDebug("Income Statement ------------------------> {Symbol}", request.Identifier);

        if (response == null || response.Count == 0)
            return;

        var entities = new List<IncomeStatement>();
        var statementType = MorningStarStatementType.Annual.Value;

        if (response.Any(x => _quarterlyFocus.Contains(x.Interim ?? "")))
        {
            entities = response.Where(x => _quarterlyFocus.Contains(x.Interim ?? "")).ToList();
            statementType = MorningStarStatementType.Quarterly.Value;
        }
        else if (response.Any(x => _semiAnnualFocus.Contains(x.Interim ?? "")))
        {
            entities = response.Where(x => _semiAnnualFocus.Contains(x.Interim ?? "")).ToList();
            statementType = MorningStarStatementType.SemiAnnual.Value;
        }

        if (entities.Count == 0)
            return;

        List<double> sales = [];
        List<double> operatingIncomes = [];
        List<double> netIncomes = [];
        List<double> earningsPerShares = [];
        List<double> dividendPerShares = [];
        List<double> averageShareCount = [];

        foreach (var entity in entities)
        {
            sales.Add(entity.TotalRevenue);
            operatingIncomes.Add(entity.OperatingIncome);
            netIncomes.Add(entity.NetIncome);
            earningsPerShares.Add(entity.BasicEPS);
            dividendPerShares.Add(entity.DividendPerShare);
            averageShareCount.Add(entity.BasicAverageShares);
        }

        morningStarStocks.Sales = new MorningStarStockItem
        {
            Values = sales,
            StatementType = statementType
        };
        morningStarStocks.OperatingIncomes = new MorningStarStockItem
        {
            Values = operatingIncomes,
            StatementType = statementType
        };
        morningStarStocks.NetIncomes = new MorningStarStockItem
        {
            Values = netIncomes,
            StatementType = statementType
        };
        morningStarStocks.EarningsPerShares = new MorningStarStockItem
        {
            Values = earningsPerShares,
            StatementType = statementType
        };
        morningStarStocks.DividendPerShares = new MorningStarStockItem
        {
            Values = dividendPerShares,
            StatementType = statementType
        };
        morningStarStocks.AverageShareCount = new MorningStarStockItem
        {
            Values = averageShareCount,
            StatementType = statementType
        };
    }

    public async Task CallBalanceSheet(
        MorningStarDataHelper helper,
        MorningStarStatementTypeRequest request,
        MorningStarStocks morningStarStocks
    )
    {
        var response = await helper.GetBalanceSheet(request, null);
        _logger.LogDebug("Balance Sheet ------------------------> {Symbol}", request.Identifier);

        if (response == null || response.Count == 0)
            return;

        var entities = new List<BalanceSheet>();
        var statementType = MorningStarStatementType.Annual.Value;

        if (response.Any(x => _quarterlyFocus.Contains(x.Interim ?? "")))
        {
            entities = response.Where(x => _quarterlyFocus.Contains(x.Interim ?? "")).ToList();
            statementType = MorningStarStatementType.Quarterly.Value;
        }
        else if (response.Any(x => _semiAnnualFocus.Contains(x.Interim ?? "")))
        {
            entities = response.Where(x => _semiAnnualFocus.Contains(x.Interim ?? "")).ToList();
            statementType = MorningStarStatementType.SemiAnnual.Value;
        }

        if (entities.Count == 0)
            return;

        List<double> totalAssets = [];
        List<double> totalLiabilities = [];
        List<double> liabilitiesToAssets = [];

        foreach (var entity in entities)
        {
            var entityTotalAssets = entity.TotalAssets;
            var entityTotalLiabilitiesNetMinorityInterest =
                entity.TotalLiabilitiesNetMinorityInterest;

            totalAssets.Add(entityTotalAssets);
            totalLiabilities.Add(entityTotalLiabilitiesNetMinorityInterest);
            liabilitiesToAssets.Add(entityTotalLiabilitiesNetMinorityInterest / entityTotalAssets);
        }

        morningStarStocks.TotalAssets = new MorningStarStockItem
        {
            Values = totalAssets,
            StatementType = statementType
        };
        morningStarStocks.TotalLiabilities = new MorningStarStockItem
        {
            Values = totalLiabilities,
            StatementType = statementType
        };
        morningStarStocks.LiabilitiesToAssets = new MorningStarStockItem
        {
            Values = liabilitiesToAssets,
            StatementType = statementType
        };
    }

    public async Task CallProfitabilityRatios(
        MorningStarDataHelper helper,
        MorningStarStatementTypeRequest request,
        MorningStarStocks morningStarStocks
    )
    {
        var response = await helper.GetProfitabilityRatios(request, null);
        _logger.LogDebug(
            "Profitability Ratios ------------------------> {Symbol}",
            request.Identifier
        );

        if (response == null || response.Count == 0)
            return;

        var entities = new List<ProfitabilityRatios>();
        var statementType = MorningStarStatementType.Annual.Value;

        if (response.Any(x => _quarterlyFocus.Contains(x.Interim ?? "")))
        {
            entities = response.Where(x => _quarterlyFocus.Contains(x.Interim ?? "")).ToList();
            statementType = MorningStarStatementType.Quarterly.Value;
        }
        else if (response.Any(x => _semiAnnualFocus.Contains(x.Interim ?? "")))
        {
            entities = response.Where(x => _semiAnnualFocus.Contains(x.Interim ?? "")).ToList();
            statementType = MorningStarStatementType.SemiAnnual.Value;
        }

        if (entities.Count == 0)
            return;

        List<double> operatingMargin = [];

        foreach (var entity in entities)
            operatingMargin.Add(entity.OperatingMargin);

        morningStarStocks.OperatingMargin = new MorningStarStockItem
        {
            Values = operatingMargin,
            StatementType = statementType
        };
    }

    public async Task CallValuationRatios(
        MorningStarDataHelper helper,
        MorningStarPeriodRequest request,
        MorningStarStocks morningStarStocks
    )
    {
        var response = await helper.GetValuationRatios(request, null);
        _logger.LogDebug("Valuation Ratios ------------------------> {Symbol}", request.Identifier);

        if (response == null || response.Count == 0)
            return;

        var entity = response[0];

        morningStarStocks.CashflowPerShare = entity.CFPerShare;
        morningStarStocks.PriceToEarningsRatio = entity.PriceToEPS;
        morningStarStocks.PriceToBookRatio = entity.PriceToBook;
        morningStarStocks.PriceToSalesRatio = entity.PriceToSales;
        morningStarStocks.DividendYield = entity.ForwardDividendYield;
    }

    public async Task CallCompanyFinancialAvailabilityList(
        MorningStarDataHelper helper,
        MorningStarExchangeIdRequest request,
        MorningStarStocks morningStarStocks
    )
    {
        var response = await helper.GetCompanyFinancialAvailabilityList(request, null);
        _logger.LogDebug(
            "Company Financial Availability List ------------------------> {Symbol}",
            request.Identifier
        );

        if (response == null || response.Count == 0)
            return;

        morningStarStocks.LatestFinancials = response[0].LatestQuarterlyReportDate;
    }

    public async Task CallCurrentMarketCapitalization(
        MorningStarDataHelper helper,
        MorningStarExchangeIdRequest request,
        MorningStarStocks morningStarStocks
    )
    {
        var response = await helper.GetCurrentMarketCapitalisation(request, null);
        _logger.LogDebug(
            "Current Market Capitalization ------------------------> {Symbol}",
            request.Identifier
        );

        if (response == null || response.Count == 0)
            return;

        var entity = response[0];

        morningStarStocks.MarketCapitalization = entity.MarketCap;
        morningStarStocks.Units = entity.CurrencyId ?? string.Empty;
    }

    public async Task CallSharesSnapshot(
        MorningStarDataHelper helper,
        MorningStarExchangeIdRequest request,
        MorningStarStocks morningStarStocks
    )
    {
        var response = await helper.GetSharesSnapshot(request, null);
        _logger.LogDebug("Shares Snapshot ------------------------> {Symbol}", request.Identifier);

        if (response == null)
            return;

        morningStarStocks.ShareFreeFloat = response.FloatShareToTotalSharesOutstanding;
    }

    public async Task CallCompanyGeneralInformation(
        MorningStarDataHelper helper,
        MorningStarExchangeIdRequest request,
        MorningStarStocks morningStarStocks
    )
    {
        var response = await helper.GetCompanyGeneralInformation(request, null);
        _logger.LogDebug(
            "Company General Information ------------------------> {Symbol}",
            request.Identifier
        );

        if (response == null)
            return;

        morningStarStocks.Industry = response.IndustryName ?? string.Empty;
        morningStarStocks.Sector = response.SectorName ?? string.Empty;
        morningStarStocks.Country = response.BusinessCountry ?? string.Empty;
        morningStarStocks.Website = response.WebAddress ?? string.Empty;
    }

    public async Task CallBusinessDescription(
        MorningStarDataHelper helper,
        MorningStarExchangeIdRequest request,
        MorningStarStocks morningStarStocks
    )
    {
        var response = await helper.GetBusinessDescription(request, null);
        _logger.LogDebug(
            "Business Description ------------------------> {Symbol}",
            request.Identifier
        );

        if (response == null)
            return;

        morningStarStocks.Description = response.LongDescription;
    }

    public async Task CallCashDividends(
        MorningStarDataHelper helper,
        MorningStarExcludingPeriodRequest request,
        MorningStarStocks morningStarStocks
    )
    {
        var response = await helper.GetCashDividends(request, null);
        _logger.LogDebug("Cash Dividends ------------------------> {Symbol}", request.Identifier);

        if (response == null || response.Count == 0)
            return;

        morningStarStocks.ExDividendDate = response[0].ExcludingDate;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        HttpRequestHelper<MorningStarDataService> httpRequestHelper = new(_httpClient, _logger);
        MorningStarDataHelper helper = new(httpRequestHelper);

        try
        {
            await MorningStarService(helper);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while processing MorningStar data {Message}",
                ex.Message
            );
        }
        _appLifetime.StopApplication();
    }
}
