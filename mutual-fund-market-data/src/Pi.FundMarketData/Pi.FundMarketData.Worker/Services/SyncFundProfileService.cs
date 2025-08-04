using MongoDB.Driver;
using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;
using Pi.FundMarketData.Repositories;
using Pi.FundMarketData.Repositories.SqlDatabase;
using Pi.FundMarketData.Utils;
using Pi.FundMarketData.Worker.Services.FundMarket.FundConnext;
using Pi.FundMarketData.Worker.Services.FundMarket.FundConnext.Models;
using Pi.FundMarketData.Worker.Services.FundMarket.Morningstar;
using Pi.FundMarketData.Worker.Services.FundMarket.Sirius;
using Fee = Pi.FundMarketData.DomainModels.Fee;
using FeeMapping = Pi.FundMarketData.Worker.Services.FundMarket.FundConnext.Models.Fee;
using HistoricalNav = Pi.FundMarketData.DomainModels.HistoricalNav;
using TradeCalendar = Pi.FundMarketData.DomainModels.TradeCalendar;

namespace Pi.FundMarketData.Worker.Services;

public class SyncFundProfileService : BackgroundService
{
    private readonly IFundRepository _fundRepository;
    private readonly ITradeDataRepository _tradeDataRepository;
    private readonly IFundHistoricalNavRepository _fundHistNavRepository;
    private readonly IHolidayRepository _holidayRepository;
    private readonly ICommonDataRepository _commonDataRepository;
    private readonly IFundConnextService _fundConnextService;
    private readonly IMorningstarService _morningstarService;
    private readonly ILogger _logger;
    private const string acceptCurrency = "THB";

    private Dictionary<string, string> _extFundCategoryIdToFundCategories;

    public SyncFundProfileService(
        IFundRepository fundRepository,
        ITradeDataRepository tradeDataRepository,
        IFundHistoricalNavRepository fundHistNavRepository,
        ICommonDataRepository commonDataRepository,
        IHolidayRepository holidayRepository,
        IFundConnextService fundConnextService,
        IMorningstarService morningstarService,
        ILogger<SyncFundProfileService> logger)
    {
        _fundRepository = fundRepository;
        _tradeDataRepository = tradeDataRepository;
        _fundHistNavRepository = fundHistNavRepository;
        _commonDataRepository = commonDataRepository;
        _holidayRepository = holidayRepository;
        _fundConnextService = fundConnextService;
        _morningstarService = morningstarService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        DateTime startTimeTh = default;
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var utcNow = DateTime.UtcNow;
                var plus7Tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                startTimeTh = TimeZoneInfo.ConvertTimeFromUtc(utcNow, plus7Tz);

                // if (utcNow.Hour >= 17 && utcNow.Hour <= 19) //update only th time = 0:00 am - 2:59 am
                await SyncPiHoliday(utcNow, ct);

                var authorizedSymbols = await GetFundConnextAuthorizedFunds(startTimeTh, ct);
                await SyncFundConnextFundProfile(authorizedSymbols, startTimeTh, ct);
                await SyncFundConnextNav(authorizedSymbols, startTimeTh, ct);

                await _morningstarService.Authenticate(ct);
                var fundKeys = await SyncMorningstarFundBasicInfos(authorizedSymbols, ct);
                await SyncMorningstarFundNavsAndPerformances(authorizedSymbols, ct);

                await SyncFundConnextSwitching(authorizedSymbols, startTimeTh, ct);
                await SyncFundConnextTradeCalendar(authorizedSymbols, startTimeTh, ct);
                await SyncFundConnextHoliday(authorizedSymbols, startTimeTh, ct);
                await SyncFundConnextSwitchingFee(authorizedSymbols, startTimeTh, ct);

                await SyncMorningstarStockSectorAllocations(authorizedSymbols, ct);
                await SyncMorningstarAssetClassAllocations(authorizedSymbols, ct);
                await SyncMorningstarFeesAndExpenses(authorizedSymbols, ct);
                await SyncMorningstarRegionalAllocations(authorizedSymbols, ct);

                var mstarIds = fundKeys.Select(x => x.mstarId);
                await SyncMorningstarTop25UnderlyingHoldings(mstarIds, ct);
                await SyncMorningstarHistoricalDistributions(utcNow, mstarIds, ct);
                await SyncMorningstarHistoricalNavsAndReturnPercentages(utcNow, fundKeys, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not sync.");
            }
            finally
            {
                var timeUntilNextRun = GetTimeUntilNextRun(startTimeTh);

                _logger.LogInformation($"Delay for {timeUntilNextRun.TotalMinutes} minutes.");

                await Task.Delay(timeUntilNextRun, ct);
            }
        }
    }

    private TimeSpan GetTimeUntilNextRun(DateTime startTimeTh)
    {
        var utcNow = DateTime.UtcNow;
        var plus7Tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var currentTimeTh = TimeZoneInfo.ConvertTimeFromUtc(utcNow, plus7Tz);

        var nextHour = startTimeTh.AddHours(1);
        var nextHourAndHalf = new DateTime(
            nextHour.Year,
            nextHour.Month,
            nextHour.Day,
            nextHour.Hour,
            30,
            0);

        var timeUntilNextRun = nextHourAndHalf - currentTimeTh;

        return timeUntilNextRun.TotalMilliseconds > 0 ? timeUntilNextRun : TimeSpan.Zero;
    }

    private async Task<string> GetFundCategoryByExtCategory(string extCategory, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(extCategory))
            return null;
        if (_extFundCategoryIdToFundCategories is null)
        {
            var extFundCategories = (await _commonDataRepository.GetExtFundCategories(ct))
                .ToDictionary(x => x.Id);
            var fundCategories = await _commonDataRepository.GetFundCategories(ct);
            var flattenedByExtIds = fundCategories.SelectMany(x =>
                x.ExtFundCategoryIds.Select(id => new { x.Name, extFcId = id }));

            _extFundCategoryIdToFundCategories = flattenedByExtIds
                .ToDictionary(x => extFundCategories[x.extFcId].ExternalId, x => x.Name);
        }

        return _extFundCategoryIdToFundCategories.TryGetValue(extCategory, out string fundcategory)
            ? fundcategory
            : null;
    }

    private async Task<HashSet<string>> GetFundConnextAuthorizedFunds(DateTime dateTimeNow, CancellationToken ct)
    {
        _logger.LogInformation("Start GetFundConnextAuthorizedFunds.");

        try
        {
            await _fundConnextService.Authenticate(ct);
            var fundMappings = (await _fundConnextService.GetFundMappings(dateTimeNow, ct))?.ToList();

            _logger.LogInformation("Total Pi authorized funds: {fundCount}.", fundMappings?.Count);
            if (fundMappings.Any())
            {
                var symbols = fundMappings.Select(x => x.FundCode).ToList();
                await _fundRepository.ReplaceWhitelistSymbols(symbols, ct);
                return new HashSet<string>(symbols);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on SyncFundConnextFundMapping.");
        }

        var existingWhiteLists = await _fundRepository.GetWhitelistSymbols(ct);

        _logger.LogInformation("Stop GetFundConnextAuthorizedFunds.");
        return new HashSet<string>(existingWhiteLists);
    }

    private async Task SyncPiHoliday(DateTime utcNow, CancellationToken ct)
    {
        _logger.LogInformation("Start SyncPiHoliday.");

        var endSyncDate =
            utcNow.AddDays(BusinessCalendar.DateRange2WeeksIncrementor)
                .AddMonths(1); //add 1 month to handle just in case that PiSql could not be synced
        try
        {
            var holidays = await _holidayRepository.GetBusinessHolidays(ct);
            var businessHolidays = holidays.Select(x => x.HolidayDate)
                .Where(x => x.Date >= utcNow && x.Date <= endSyncDate);

            await _commonDataRepository.UpsertBusinessCalendar(businessHolidays, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on SyncPiHoliday from Pi SQL from: {from}, to: {to}", utcNow, endSyncDate);
        }
        _logger.LogInformation("Stop SyncPiHoliday.");
    }

    private async Task SyncFundConnextFundProfile(HashSet<string> authSymbols, DateTime dateTimeNow, CancellationToken ct)
    {
        _logger.LogInformation("Start SyncFundConnextFundProfile.");

        try
        {
            await _fundConnextService.Authenticate(ct);
            var fundProfiles = await _fundConnextService.GetFundProfiles(dateTimeNow, ct);

            _logger.LogInformation("Total fundconnext funds: {fundCount}.", fundProfiles?.Count());
            if (fundProfiles != null)
            {
                foreach (var fund in fundProfiles)
                {
                    if (!authSymbols.Contains(fund.FundCode))
                        continue;

                    if (fund.Currency == acceptCurrency)
                    {
                        try
                        {
                            Purchase purchase = new()
                            {
                                MinimumInitialBuy = fund.FstLowBuyVal,
                                MinimumSubsequentBuy = fund.NxtLowBuyVal,
                                MinimumSellUnit = fund.LowSellUnit,
                                MinimumSellAmount = fund.LowSellVal,
                                MinHoldUnit = fund.LowBalUnit,
                                MinHoldAmount = fund.LowBalVal,
                                SellSettlementDay = fund.SellSettlementDay,
                                BuyCutOffTime = fund.BuyCutOffTime,
                                SellCutOffTime = fund.SellCutOffTime,
                                AsOfDate = fund.AsOfDate
                            };

                            var patch = Builders<Fund>.Update
                                .Set(x => x.Isin, fund.Isin)
                                .Set(x => x.Symbol, fund.FundCode)
                                .Set(x => x.PreviousSymbol, fund.PreviousFundCode)
                                .Set(x => x.Name, fund.Name)
                                .Set(x => x.AmcCode, fund.AmcCode)
                                .Set(x => x.FundClassSymbol, fund.FundClassCode)
                                .Set(x => x.Fundamental.RiskLevel, fund.FundRiskLevel)
                                .Set(x => x.Fundamental.IsForeignInvestment, fund.FifFlag)
                                .Set(x => x.Fundamental.HasCurrencyRisk, fund.FxRiskFlag)
                                .Set(x => x.Fundamental.AssetClassFocus, fund.FundPolicy)
                                .Set(x => x.Fundamental.AllowSwitchOut, fund.SwitchOutFlag)
                                .Set(x => x.Fundamental.BuyCalendarType, fund.BuyPeriodFlag.ToTradeCalendarType())
                                .Set(x => x.Fundamental.SellCalendarType, fund.SellPeriodFlag.ToTradeCalendarType())
                                .Set(x => x.Fundamental.SwitchOutCalendarType, fund.SwitchOutPeriodFlag.ToTradeCalendarType())
                                .Set(x => x.Fundamental.RegistrationDate, fund.RegistrationDate)
                                .Set(x => x.Fundamental.IsDividend, fund.DividendFlag)
                                .Set(x => x.Fundamental.TaxType, fund.TaxType)
                                .Set(x => x.Fundamental.Currency, fund.Currency)
                                .Set(x => x.Fundamental.FundType, FundTypeExtension.ParseFundType(fund.FundType))
                                .Set(x => x.Fundamental.ProjectType, ProjectTypeExtension.ParseProjectType(fund.ProjectRetailType))
                                .Set(x => x.Fundamental.IsFatcaAllow, fund.FatcaAllowFlag)
                                .Set(x => x.Fundamental.IsDerivative, fund.DerivativeFlag)
                                .Set(x => x.Fundamental.HasHealthInsuranceBenefit, fund.HealthInsuranceBenefit)
                                .Set(x => x.Fundamental.InvestorAlerts, InvestorAlertExtension.ParseInvestorAlerts(fund.InvestorAlerts?.Split(",")))
                                .Set(x => x.Fundamental.ComplexFundUrl, fund.ComplexFundUrl)
                                .Set(x => x.Fundamental.ComplexFundRiskAckUrl, fund.ComplexFundRiskAckUrl)
                                .Set(x => x.Fundamental.RedemptionType, fund.RedemptionType)
                                .Set(x => x.IsInLegacyMarket, LegacyMarket.FundSymbols.Contains(fund.FundCode))
                                .Set(x => x.Fundamental.AsOfDate, fund.AsOfDate)
                                .Set(x => x.Purchase, purchase)
                                .CurrentDate(x => x.LastModified);
                            await _fundRepository.UpsertFundWithPreviousSymbol(fund.FundCode, fund.PreviousFundCode, patch, ct);

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error on SyncFundConnextFundProfile, Symbol {Symbol}.",
                                fund.FundCode);
                        }
                    }

                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on {nameof(SyncFundConnextFundProfile)}.");
        }

        _logger.LogInformation("Stop SyncFundConnextFundProfile.");
    }

    private async Task SyncFundConnextNav(HashSet<string> authSymbols, DateTime dateTimeNow, CancellationToken ct)
    {
        _logger.LogInformation("Start SyncFundConnextNav.");

        try
        {
            await _fundConnextService.Authenticate(ct);
            var dayT = 0;
            DateTime? dateTimeNav = null;
            IEnumerable<Nav> navs = Enumerable.Empty<Nav>();
            do
            {
                try
                {
                    dateTimeNav = dateTimeNow.AddDays(dayT);
                    navs = await _fundConnextService.GetFundNavInfos(dateTimeNav.Value, ct);
                    dayT -= 1;
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex, "Cannot get FundConnext Nav file of day {dateTimeNav}.", dateTimeNav?.Date);
                    dayT -= 1;
                }
            } while (!navs.Any() && dayT >= -7);

            foreach (var nav in navs)
            {
                if (!authSymbols.Contains(nav.FundCode))
                    continue;

                try
                {
                    var patch = Builders<Fund>.Update
                        .Set(x => x.AssetValue.Aum, nav.Aum)
                        .Set(x => x.AssetValue.Nav, nav.NavVal)
                        .Set(x => x.AssetValue.TotalUnit, nav.TotalUnit)
                        .Set(x => x.AssetValue.AsOfDate, nav.NavDate)
                        .CurrentDate(x => x.LastModified);

                    await _fundRepository.UpdateFund(nav.FundCode, patch, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error on SyncFundConnextNav, Symbol {Symbol}.",
                        nav.FundCode);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on {nameof(SyncFundConnextNav)}.");
        }

        _logger.LogInformation("Stop SyncFundConnextNav.");
    }

    private async Task SyncFundConnextSwitching(HashSet<string> authSymbols, DateTime dateTimeNow, CancellationToken ct)
    {
        _logger.LogInformation("Start SyncFundConnextSwitching.");
        try
        {
            await _fundConnextService.Authenticate(ct);
            var switchingData = await _fundConnextService.GetSwitchingInfos(dateTimeNow, ct);

            var fundOutList = switchingData.ToLookup(x => x.FundCodeOut);
            foreach (var fundOut in fundOutList)
            {
                if (!authSymbols.Contains(fundOut.Key))
                    continue;

                try
                {
                    var switchingList = fundOut.Select(x => new Switching()
                    {
                        FundCode = x.FundCodeIn,
                        SwitchingType = x.SwitchingType,
                        SwitchSettlementDay = x.SwitchSettlementDay
                    }).ToList();

                    if (switchingList.Any())
                    {
                        var patch = Builders<FundTradeData>.Update
                            .Set(x => x.Switchings, switchingList)
                            .CurrentDate(x => x.LastModified);

                        await _tradeDataRepository.UpsertFundTradeData(fundOut.Key, patch, ct);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error on SyncFundConnextSwitching, Symbol {Symbol}.",
                        fundOut.Key);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on {nameof(SyncFundConnextSwitching)}.");
        }

        _logger.LogInformation("Stop SyncFundConnextSwitching.");
    }

    private async Task SyncFundConnextSwitchingFee(HashSet<string> authSymbols, DateTime dateTimeNow, CancellationToken ct)
    {
        _logger.LogInformation("Start SyncFundSwitchingFee.");
        try
        {
            await _fundConnextService.Authenticate(ct);
            var feeInfos = await _fundConnextService.GetFeeInfos(dateTimeNow, ct);

            var fees = feeInfos
                .Where(x => x.EffectiveDate <= dateTimeNow && x.FeeType == FeeMapping.SwitchingInFeeType)
                .OrderByDescending(x => x.EffectiveDate)
                .GroupBy(x => x.FundCode)
                .Select(x => x.First());

            foreach (var fee in fees)
            {
                if (!authSymbols.Contains(fee.FundCode))
                    continue;

                try
                {
                    var patch = Builders<Fund>.Update
                        .Set(x => x.Fee.SwitchInFee, fee.ActualFee)
                        .Set(x => x.Fee.SwitchInFeeUnit, int.TryParse(fee.FeeUnit, out int feeUnit) ? (FeeUnit)feeUnit : null)
                        .Set(x => x.Fee.SwitchInEffectiveDate, fee.EffectiveDate)
                        .CurrentDate(x => x.LastModified);
                    await _fundRepository.UpdateFund(fee.FundCode, patch, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error on SyncFundSwitchingFee, Symbol {Symbol}.", fee.FundCode);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on {nameof(SyncFundConnextSwitchingFee)}.");
        }

        _logger.LogInformation("Stop SyncFundConnextSwitchingFee.");
    }

    private async Task SyncFundConnextTradeCalendar(HashSet<string> authSymbols, DateTime dateTimeNow, CancellationToken ct)
    {
        _logger.LogInformation("Start SyncFundConnextTradeCalendar.");
        try
        {
            await _fundConnextService.Authenticate(ct);
            var tradeDataList = await _fundConnextService.GetTradeCalendars(dateTimeNow, ct);

            var fundCodeList = tradeDataList.ToLookup(x => x.FundCode);
            foreach (var fundCode in fundCodeList)
            {
                if (!authSymbols.Contains(fundCode.Key))
                    continue;

                try
                {
                    var forward15Days = dateTimeNow.AddDays(15);
                    var tradeCalendars = fundCode
                        .Where(x => x.TradeDate <= forward15Days)
                        .Select(x =>
                            new TradeCalendar()
                            {
                                TransactionCode = x.TransactionCode,
                                TradePermission = x.TradeType,
                                TradeDate = x.TradeDate
                            })
                        .ToList();

                    if (tradeCalendars.Any())
                    {
                        var patch = Builders<FundTradeData>.Update
                            .Set(x => x.TradeCalendars, tradeCalendars)
                            .CurrentDate(x => x.LastModified);
                        await _tradeDataRepository.UpsertFundTradeData(fundCode.Key, patch, ct);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error on SyncFundConnextTradeCalendar, Symbol {Symbol}.",
                        fundCode.Key);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on SyncFundConnextTradeCalendar.");
        }

        _logger.LogInformation("Stop SyncFundConnextTradeCalendar.");
    }

    private async Task SyncFundConnextHoliday(HashSet<string> authSymbols, DateTime dateTimeNow, CancellationToken ct)
    {
        _logger.LogInformation("Start SyncFundConnextHoliday.");
        try
        {
            await _fundConnextService.Authenticate(ct);
            var fundHolidays = await _fundConnextService.GetFundHolidays(dateTimeNow, ct);

            var fundHolidayGroups = fundHolidays.ToLookup(x => x.FundCode);
            foreach (var holidayGroup in fundHolidayGroups)
            {
                if (!authSymbols.Contains(holidayGroup.Key))
                    continue;

                try
                {
                    var holidays = holidayGroup
                        .Select(x => x.HolidayDate)
                        .ToList();

                    if (holidays.Any())
                    {
                        var patch = Builders<FundTradeData>.Update
                            .Set(x => x.Holidays, holidays)
                            .CurrentDate(x => x.LastModified);
                        await _tradeDataRepository.UpsertFundTradeData(holidayGroup.Key, patch, ct);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error on SyncFundConnextHoliday, Symbol {Symbol}.",
                        holidayGroup.Key);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on SyncFundConnextHoliday.");
        }

        _logger.LogInformation("Stop SyncFundConnextHoliday.");
    }

    private async Task<IEnumerable<(string mstarId, string authSymbol, DateTime? inceptionDate)>> SyncMorningstarFundBasicInfos(HashSet<string> authSymbols,
        CancellationToken ct)
    {
        _logger.LogInformation("Start SyncMorningstarFundBasicInfos.");
        try
        {
            var fundBasicInfos = await _morningstarService.GetFundBasicInfos(ct);

            _logger.LogInformation("Total mstar funds: {fundCount}.", fundBasicInfos?.Count());

            List<(string mstarId, string symbol, DateTime? inceptionDate)> results = new();

            if (fundBasicInfos != null)
            {
                foreach (var fund in fundBasicInfos)
                {
                    if (!authSymbols.Contains(fund.ThailandFundCode))
                        continue;

                    results.Add((fund.MStarID, fund.ThailandFundCode, fund.InceptionDate));

                    try
                    {
                        var patch = Builders<Fund>.Update
                            .Set(x => x.MorningstarId, fund.MStarID)
                            .Set(x => x.Category, await GetFundCategoryByExtCategory(fund.CategoryName, ct))
                            .Set(x => x.Rating, fund.RatingOverall)
                            .Set(x => x.Fundamental.InvestmentPolicy, fund.InvestmentStrategy)
                            .Set(x => x.Fundamental.Objective, fund.CategoryName)
                            .Set(x => x.Fundamental.InceptionDate, fund.InceptionDate)
                            .Set(x => x.Fundamental.FundSize, fund.ShareClassNetAssets)
                            .Set(x => x.Distribution.DividendPolicy, fund.DistributionFrequency)
                            .Set(x => x.Distribution.ExDivDate, fund.DividendDate)
                            .CurrentDate(x => x.LastModified);

                        await _fundRepository.UpdateFund(fund.ThailandFundCode, patch, ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error on SyncMorningstarFundBasicInfos, Symbol {Symbol}.",
                            fund.ThailandFundCode);
                    }
                }
            }

            _logger.LogInformation("Stop SyncMorningstarFundBasicInfos.");
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on {nameof(SyncMorningstarFundBasicInfos)}.");
        }

        _logger.LogInformation("Stop SyncMorningstarFundBasicInfos.");
        return [];
    }

    private async Task SyncMorningstarFundNavsAndPerformances(HashSet<string> authSymbols, CancellationToken ct)
    {
        _logger.LogInformation("Start SyncMorningstarFundPerformances.");
        try
        {
            var fundPerformances = await _morningstarService.GetFundPerformances(ct);

            foreach (var fund in fundPerformances)
            {
                if (!authSymbols.Contains(fund.ThailandFundCode))
                    continue;

                try
                {
                    NavChange navChange = new()
                    {
                        ValueChange = fund.NAVChange,
                        NavChangePercentage = fund.NAVChangePercentage
                    };

                    var patch = Builders<Fund>.Update
                        .Set(x => x.AssetValue.NavChange, navChange)
                        .Set(x => x.Performance.Yield1Y, fund.Yield1Yr)
                        .Set(x => x.Performance.AnnualizedHistoricalReturnPercentages, fund.GetHistoricalReturnPercentages())
                        .CurrentDate(x => x.LastModified);

                    await _fundRepository.UpdateFund(fund.ThailandFundCode, patch, ct);

                    var navPatch = Builders<Fund>.Update
                            .Set(x => x.AssetValue.Nav, fund.Nav)
                            .Set(x => x.AssetValue.AsOfDate, fund.NavAsOfDate)
                            .CurrentDate(x => x.LastModified);
                    await _fundRepository.UpdateFundNavFromMorningStar(fund.ThailandFundCode, fund.NavAsOfDate, navPatch, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error on SyncMorningstarFundPerformances, Symbol {Symbol}.",
                        fund.ThailandFundCode);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on {nameof(SyncMorningstarFundNavsAndPerformances)}.");
        }

        _logger.LogInformation("Stop SyncMorningstarFundPerformances.");
    }

    private async Task SyncMorningstarStockSectorAllocations(HashSet<string> authSymbols, CancellationToken ct)
    {
        _logger.LogInformation("Start SyncMorningstarStockSectorAllocations.");
        try
        {
            var sectorAllocations = await _morningstarService.GetStockSectorAllocations(ct);

            foreach (var fund in sectorAllocations)
            {
                if (!authSymbols.Contains(fund.ThailandFundCode))
                    continue;

                try
                {
                    var patch = Builders<Fund>.Update
                        .Set(x => x.AssetAllocation.SectorAllocations, fund.ToDictionary())
                        .Set(x => x.AssetAllocation.SectorAsOfDate, fund.PortfolioDate)
                        .CurrentDate(x => x.LastModified);

                    await _fundRepository.UpdateFund(fund.ThailandFundCode, patch, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error on SyncMorningstarStockSectorAllocations, Symbol {Symbol}.",
                        fund.ThailandFundCode);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on {nameof(SyncMorningstarStockSectorAllocations)}.");
        }

        _logger.LogInformation("Stop SyncMorningstarStockSectorAllocations.");
    }

    private async Task SyncMorningstarFeesAndExpenses(HashSet<string> authSymbols, CancellationToken ct)
    {
        _logger.LogInformation("Start SyncMorningstarFeesAndExpenses.");
        try
        {
            var feesAndExpenses = await _morningstarService.GetFeesAndExpenses(ct);

            foreach (var fund in feesAndExpenses)
            {
                if (!authSymbols.Contains(fund.ThailandFundCode))
                    continue;

                try
                {
                    Fee fee = new()
                    {
                        TotalExpense = fund.NetExpenseRatio,
                        ManagementFee = fund.ActualManagementFee,
                        FrontendFee = fund.ActualFrontLoad,
                        BackendFee = fund.ActualDeferLoad,
                        AsOfDate = fund.ProspectusDate
                    };

                    var patch = Builders<Fund>.Update
                        .Set(x => x.Fee, fee)
                        .CurrentDate(x => x.LastModified);

                    await _fundRepository.UpdateFund(fund.ThailandFundCode, patch, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error on SyncMorningstarFeesAndExpenses, Symbol {Symbol}.",
                        fund.ThailandFundCode);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on {nameof(SyncMorningstarFeesAndExpenses)}.");
        }

        _logger.LogInformation("Stop SyncMorningstarFeesAndExpenses.");
    }

    private async Task SyncMorningstarRegionalAllocations(HashSet<string> authSymbols, CancellationToken ct)
    {
        _logger.LogInformation("Start SyncMorningstarRegionalAllocations.");
        try
        {
            var regionalAllocations = await _morningstarService.GetRegionalAllocations(ct);

            foreach (var fund in regionalAllocations)
            {
                if (!authSymbols.Contains(fund.ThailandFundCode))
                    continue;

                try
                {
                    var patch = Builders<Fund>.Update
                        .Set(x => x.AssetAllocation.RegionalAllocations, fund.ToDictionary())
                        .Set(x => x.AssetAllocation.RegionalAsOfDate, fund.PortfolioDate)
                        .CurrentDate(x => x.LastModified);

                    await _fundRepository.UpdateFund(fund.ThailandFundCode, patch, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error on SyncMorningstarRegionalAllocations, Symbol {Symbol}.",
                        fund.ThailandFundCode);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on {nameof(SyncMorningstarRegionalAllocations)}.");
        }

        _logger.LogInformation("Stop SyncMorningstarRegionalAllocations.");
    }

    private async Task SyncMorningstarAssetClassAllocations(HashSet<string> authSymbols, CancellationToken ct)
    {
        _logger.LogInformation("Start SyncMorningstarAssetClassAllocations.");
        try
        {
            var assetClassAllocations = await _morningstarService.GetAssetClassAllocations(ct);

            foreach (var fund in assetClassAllocations)
            {
                if (!authSymbols.Contains(fund.ThailandFundCode))
                    continue;

                try
                {
                    var patch = Builders<Fund>.Update
                        .Set(x => x.AssetAllocation.AssetClassAllocations, fund.ToDictionary())
                        .Set(x => x.AssetAllocation.AssetClassAsOfDate, fund.PortfolioDate)
                        .CurrentDate(x => x.LastModified);

                    await _fundRepository.UpdateFund(fund.ThailandFundCode, patch, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error on SyncMorningstarAssetClassAllocations, Symbol {Symbol}.",
                        fund.ThailandFundCode);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on {nameof(SyncMorningstarAssetClassAllocations)}.");
        }

        _logger.LogInformation("Stop SyncMorningstarAssetClassAllocations.");
    }

    private async Task SyncMorningstarTop25UnderlyingHoldings(IEnumerable<string> mstarIds,
        CancellationToken ct)
    {
        _logger.LogInformation("Start SyncMorningstarTop25UnderlyingHoldings.");
        try
        {
            await BatchUtils.RunAsConcurrentBatchAsync(mstarIds,
                async (mstarId) =>
                {
                    try
                    {
                        var fund = await _morningstarService.GetTop25UnderlyingHolding(mstarId, ct);

                        if (fund is not null)
                        {
                            var topHoldings = fund.Holdings.Select(x =>
                                new KeyValuePair<string, double>(x.HoldingName, x.Weighting));

                            var patch = Builders<Fund>.Update
                                .Set(x => x.AssetAllocation.TopHoldings, topHoldings)
                                .Set(x => x.AssetAllocation.TopHoldingsAsOfDate, fund.PortfolioDate)
                                .CurrentDate(x => x.LastModified);

                            await _fundRepository.UpdateFund(fund.ThailandFundCode, patch, ct);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error on SyncMorningstarTop25UnderlyingHoldings, MStarId {MStarId}.",
                            mstarId);
                    }
                },
                10, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on {nameof(SyncMorningstarTop25UnderlyingHoldings)}.");
        }

        _logger.LogInformation("Stop SyncMorningstarTop25UnderlyingHoldings.");
    }

    private async Task SyncMorningstarHistoricalDistributions(DateTime dateTimeNow, IEnumerable<string> mstarIds,
        CancellationToken ct)
    {
        _logger.LogInformation("Start SyncMorningstarHistoricalDistributions.");
        try
        {
            var plus7Tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var utc = dateTimeNow;
            var endDate = TimeZoneInfo.ConvertTimeFromUtc(utc, plus7Tz);
            var startDate = endDate.AddYears(-10);

            await BatchUtils.RunAsConcurrentBatchAsync(mstarIds,
                async (mstarId) =>
                {
                    try
                    {
                        var fund = await _morningstarService.GetHistoricalDistribution(mstarId, startDate, endDate, ct);

                        if (fund?.Dividend is not null)
                        {
                            var historicalDividends = fund.Dividend.DividendDetails.Select(x =>
                                new HistoricalDividend { PayDate = x.PayDate, Dividend = x.TotalDividend });

                            var patch = Builders<Fund>.Update
                                .Set(x => x.Distribution.HistoricalDividends, historicalDividends)
                                .Set(x => x.Distribution.AsOfDate, utc)
                                .CurrentDate(x => x.LastModified);

                            await _fundRepository.UpdateFundByMorningstarId(mstarId, patch, ct);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error on SyncMorningstarHistoricalDistributions, MStarId {MStarId}.",
                            mstarId);
                    }
                },
                10, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on {nameof(SyncMorningstarHistoricalDistributions)}.");
        }

        _logger.LogInformation("Stop SyncMorningstarHistoricalDistributions.");
    }

    private async Task SyncMorningstarHistoricalNavsAndReturnPercentages(DateTime dateTimeNow,
        IEnumerable<(string mstarId, string symbol, DateTime? inceptionDate)> fundKeys,
        CancellationToken ct)
    {
        _logger.LogInformation("Start SyncMorningstarHistoricalNavsAndReturnPercentages.");

        try
        {
            var plus7Tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var utc = dateTimeNow;
            var endDate = TimeZoneInfo.ConvertTimeFromUtc(utc, plus7Tz);
            var startDate = endDate.AddYears(-5);

            await BatchUtils.RunAsConcurrentBatchAsync(fundKeys,
                async (fundKey) =>
                {
                    try
                    {
                        if (fundKey.inceptionDate != null)
                        {
                            startDate = TimeZoneInfo.ConvertTimeFromUtc(fundKey.inceptionDate.Value, plus7Tz);
                        }

                        var fund = await _morningstarService.GetHistoricalNav(fundKey.mstarId, startDate, endDate, ct);

                        bool isNavsNotExists = !fund?.Historical?.PriceList?.Any() ?? true;
                        if (isNavsNotExists)
                        {
                            _logger.LogWarning(
                                "Failed to replace historical NAVs for fund {Symbol}. The historical NAVs is null.",
                                fundKey.symbol);
                            return;
                        }

                        var historicalNavs = fund.Historical.PriceList.Select(
                            x => new HistoricalNav { Date = x.Date, Nav = x.Value, Symbol = fundKey.symbol }).ToArray();

                        await _fundHistNavRepository.BulkReplaceHistoricalNavs(fundKey.symbol, historicalNavs, ct);

                        var performance = new Performance(historicalNavs);
                        var patch = Builders<Fund>.Update
                            .Set(x => x.Performance.HistoricalReturnPercentages, performance.HistoricalReturnPercentages)
                            .CurrentDate(x => x.LastModified);

                        await _fundRepository.UpdateFund(fundKey.symbol, patch, ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error on SyncMorningstarHistoricalNavs, Symbol {Symbol}.",
                            fundKey.symbol);
                    }
                },
                10, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error on {nameof(SyncMorningstarHistoricalNavsAndReturnPercentages)}.");
        }

        _logger.LogInformation("Stop SyncMorningstarHistoricalNavs.");
    }
}
