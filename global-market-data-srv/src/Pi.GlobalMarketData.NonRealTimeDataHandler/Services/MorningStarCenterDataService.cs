using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Request;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.NonRealTimeDataHandler.constants;
using Pi.GlobalMarketData.NonRealTimeDataHandler.Helpers;
using Pi.GlobalMarketData.NonRealTimeDataHandler.interfaces;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.Services;

public class MorningStarCenterDataService : BackgroundService, IMorningStarCenterDataService
{
    private readonly string _accountCode;
    private readonly string _password;
    private readonly HttpClient _httpClient;
    private readonly ILogger<MorningStarCenterDataService> _logger;
    private readonly IMongoService<MorningStarFlag> _morningStarFlagService;
    private readonly IMongoService<MorningStarEtfs> _morningStarETFsService;
    private readonly IHostApplicationLifetime _appLifetime;

    public MorningStarCenterDataService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IMongoService<MorningStarFlag> morningStarFlagService,
        IMongoService<MorningStarEtfs> morningStarETFsService,
        ILogger<MorningStarCenterDataService> logger,
        IHostApplicationLifetime appLifetime
    )
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientKeys.MorningStarCenter);
        _accountCode =
            configuration.GetValue<string>(ConfigurationKeys.MorningStarCenterAccountCode) ?? "";
        _password =
            configuration.GetValue<string>(ConfigurationKeys.MorningStarCenterPassword) ?? "";
        _morningStarFlagService = morningStarFlagService;
        _morningStarETFsService = morningStarETFsService;
        _logger = logger;
        _appLifetime = appLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        HttpRequestHelper<MorningStarCenterDataService> httpRequestHelper =
            new(_httpClient, _logger);
        MorningStarCenterDataHelper helper = new(httpRequestHelper);

        
        try
        {
            await MorningStarService(helper);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving MorningStar data  {Message}", ex.Message);
        }
        _appLifetime.StopApplication();

    }

    public async Task MorningStarService(MorningStarCenterDataHelper helper)
    {
        var createAccessCodeResponse = await helper.CreateAccessCode(_accountCode, _password);

        var accessCode = createAccessCodeResponse?.Data?.Api?.Code;
        _logger.LogDebug("accessCode ------------------------> {AccessCode}", accessCode);

        if (accessCode == null)
        {
            _logger.LogDebug("accessCode is null");
            return;
        }

        var queryAccessCodeResponse = await helper.QueryAccessCode(
            _accountCode,
            _password,
            accessCode
        );

        if (queryAccessCodeResponse?.Status?.Message != "OK")
        {
            _logger.LogDebug("accessCode verification failed");
            return;
        }

        _logger.LogDebug("accessCode verification success");

        if (createAccessCodeResponse is { Data.Api.Expired: false })
        {
            var requests = await _morningStarFlagService.GetAllAsync();

            foreach (var request in requests)
            {
                if (!request.IsSubscribe || request.InstrumentCategory == StockDetail.Stock.Value)
                    continue;

                var identifier = request.Isin;

                MorningStarCenterApiRequest apiRequest =
                    new() { Identifier = identifier, AccessCode = accessCode };

                var morningStarETFs = await _morningStarETFsService.GetByFilterAsync(target =>
                    target.Isin == identifier
                );

                morningStarETFs ??= new MorningStarEtfs
                {
                    Symbol = request.StandardTicker,
                    ExchangeId = request.ExchangeId,
                    Isin = identifier
                };

                await Task.WhenAll(
                    CallNetAssets(helper, apiRequest, morningStarETFs),
                    CallFundShareClassBasicInfo(helper, apiRequest, morningStarETFs),
                    CallCurrentPrice(helper, apiRequest, morningStarETFs),
                    CallDailyPerformance(helper, apiRequest, morningStarETFs),
                    CallYields(helper, apiRequest, morningStarETFs),
                    CallProspectusFees(helper, apiRequest, morningStarETFs),
                    CallInvestmentCriteria(helper, apiRequest, morningStarETFs)
                );

                if (string.IsNullOrEmpty(morningStarETFs.MarketCap))
                    continue;

                await _morningStarETFsService.UpsertAsyncByFilter(
                    target => target.Isin == identifier && target.Symbol == morningStarETFs.Symbol,
                    morningStarETFs
                );
            }
        }

        var deleteAccessCodeResponse = await helper.DeleteAccessCode(
            _accountCode,
            _password,
            accessCode
        );
        if (deleteAccessCodeResponse?.Status?.Message != "OK")
            _logger.LogDebug("Delete accessCode failed");
        else
            _logger.LogDebug("Delete accessCode success");
    }

    private async Task CallNetAssets(
        MorningStarCenterDataHelper helper,
        MorningStarCenterApiRequest request,
        MorningStarEtfs morningStarETFs
    )
    {
        var response = await helper.GetNetAssets(request);
        if (response == null)
            return;

        _logger.LogDebug("Net Assets ------------------------> {Identifier}", request.Identifier);

        morningStarETFs.MarketCap = response.ShareClassNetAssets ?? string.Empty;
    }

    private async Task CallFundShareClassBasicInfo(
        MorningStarCenterDataHelper helper,
        MorningStarCenterApiRequest request,
        MorningStarEtfs morningStarETFs
    )
    {
        var response = await helper.GetFundShareClassBasicInfo(request);
        if (response == null)
            return;

        _logger.LogDebug(
            "Fund Share Classic Basic Information ------------------------> {Identifier}",
            request.Identifier
        );

        morningStarETFs.Website = response.ProviderCompanyWebsite ?? string.Empty;
        morningStarETFs.AssetClassFocus = response.BroadCategoryGroup ?? string.Empty;
        morningStarETFs.Category = response.AggregatedCategoryName ?? string.Empty;
        morningStarETFs.Manager = response.ProviderCompanyName ?? string.Empty;
    }

    private async Task CallCurrentPrice(
        MorningStarCenterDataHelper helper,
        MorningStarCenterApiRequest request,
        MorningStarEtfs morningStarETFs
    )
    {
        var response = await helper.GetCurrentPrice(request);
        if (response == null)
            return;

        _logger.LogDebug(
            "Current Price ------------------------> {Identifier}",
            request.Identifier
        );

        morningStarETFs.LatestNAV = response.DayEndNAV ?? string.Empty;
        morningStarETFs.Currency = response.CurrencyISO3 ?? string.Empty;
    }

    private async Task CallDailyPerformance(
        MorningStarCenterDataHelper helper,
        MorningStarCenterApiRequest request,
        MorningStarEtfs morningStarETFs
    )
    {
        var response = await helper.GetDailyPerformance(request);
        if (response == null)
            return;

        _logger.LogDebug(
            "Daily Performance ------------------------> {Identifier}",
            request.Identifier
        );

        morningStarETFs.ExDividendDate = response.DividendDate ?? string.Empty;
    }

    private async Task CallYields(
        MorningStarCenterDataHelper helper,
        MorningStarCenterApiRequest request,
        MorningStarEtfs morningStarETFs
    )
    {
        var response = await helper.GetYields(request);
        if (response == null)
            return;

        _logger.LogDebug("Yields ------------------------> {Identifier}", request.Identifier);

        morningStarETFs.Dividend = response.Yield1Yr ?? string.Empty;
    }

    private async Task CallProspectusFees(
        MorningStarCenterDataHelper helper,
        MorningStarCenterApiRequest request,
        MorningStarEtfs morningStarETFs
    )
    {
        var response = await helper.GetProspectusFees(request);
        if (response == null)
        {
            _logger.LogDebug("Null Response");
            return;
        }

        _logger.LogDebug(
            "Prospectus Fees ------------------------> {Identifier}",
            request.Identifier
        );

        morningStarETFs.ExpenseRatio = response.NetExpenseRatio ?? string.Empty;
    }

    private async Task CallInvestmentCriteria(
        MorningStarCenterDataHelper helper,
        MorningStarCenterApiRequest request,
        MorningStarEtfs morningStarETFs
    )
    {
        var response = await helper.GetInvestmentCriteria(request);
        if (response == null)
            return;

        _logger.LogDebug(
            "Investment Criteria ------------------------> {Identifier}",
            request.Identifier
        );

        morningStarETFs.Description = response.InvestmentStrategy ?? string.Empty;
    }
}
