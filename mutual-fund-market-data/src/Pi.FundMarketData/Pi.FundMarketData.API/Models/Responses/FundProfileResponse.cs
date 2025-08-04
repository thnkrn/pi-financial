#nullable enable

using System.Globalization;
using Pi.FundMarketData.DomainModels;
using Pi.FundMarketData.Utils;

namespace Pi.FundMarketData.API.Models.Responses;

public class FundProfileResponse
{
    public string Isin { get; init; }
    public string Symbol { get; init; }
    public string Name { get; init; }
    public string FundCategory { get; init; }
    public string FactSheetUrl { get; init; }
    public int? Rating { get; init; }
    public bool IsInLegacyMarket { get; init; }
    public bool? IsActive { get; init; }
    public DateTime AsOfDate { get; init; }
    public AmcProfileResponse? AmcProfile { get; init; }
    public FundamentalResponse? Fundamental { get; init; }
    public AssetValueResponse? AssetValue { get; init; }
    public AssetAllocationResponse? AssetAllocation { get; init; }
    public PurchaseDetailResponse? PurchaseDetail { get; init; }
    public FeeResponse? FeesAndExpense { get; init; }
    public PerformanceResponse? Performance { get; init; }
    public DistributionResponse? Distribution { get; init; }

    public FundProfileResponse(Fund model, AmcProfile? amcProfile, bool? isActive = null)
    {
        Isin = model.Isin;
        Symbol = model.Symbol;
        Name = model.Name;
        FundCategory = model.Category;
        FactSheetUrl = model.FactSheetUrl;
        Rating = model.Rating;
        IsInLegacyMarket = model.IsInLegacyMarket;
        IsActive = isActive;
        AsOfDate = model.LastModified;

        AmcProfile = model.AmcCode is not null
            ? new AmcProfileResponse(model.AmcCode, amcProfile)
            : null;
        Fundamental = model.Fundamental is not null
            ? new FundamentalResponse(model.Fundamental)
            : null;
        AssetValue = model.AssetValue is not null
            ? new AssetValueResponse(model.AssetValue)
            : null;
        AssetAllocation = model.AssetAllocation is not null
            ? new AssetAllocationResponse(model.AssetAllocation)
            : null;
        PurchaseDetail = model.Purchase is not null
            ? new PurchaseDetailResponse(model.Purchase)
            : null;
        FeesAndExpense = model.Fee is not null
            ? new FeeResponse(model.Fee)
            : null;
        Performance = model.Performance is not null
            ? new PerformanceResponse(model.Performance)
            : null;
        Distribution = model.Distribution is not null
            ? new DistributionResponse(model.Distribution)
            : null;
    }


    public class AmcProfileResponse
    {
        public string Code { get; init; }
        public string Name { get; init; }
        public string? Logo { get; init; }

        public AmcProfileResponse(string amcCode, AmcProfile? model)
        {
            Code = amcCode;
            Name = model?.Name ?? string.Empty;
            Logo = model?.Logo;
        }
    }

    public class FundamentalResponse
    {
        public int RiskLevel { get; init; }
        public bool IsForeignInvestment { get; init; }
        public string? InvestmentPolicy { get; init; }
        public bool HasCurrencyRisk { get; init; }
        public string? AssetClassFocus { get; init; }
        public bool AllowSwitchOut { get; init; }
        public DateTime? InceptionDate { get; init; }
        public string? Objective { get; init; }
        public decimal? FundSize { get; init; }
        public bool IsDividend { get; init; }
        public string? TaxType { get; init; }
        public string? Currency { get; init; }
        public string? ComplexFundUrl { get; init; }
        public string? ComplexFundRiskAckUrl { get; init; }
        public string? RedemptionType { get; init; }
        public DateTime AsOfDate { get; init; }

        public FundamentalResponse(Fundamental? model = null)
        {
            if (model is null)
                return;

            RiskLevel = model.RiskLevel;
            IsForeignInvestment = model.IsForeignInvestment;
            InvestmentPolicy = model.InvestmentPolicy;
            HasCurrencyRisk = model.HasCurrencyRisk;
            AssetClassFocus = model.AssetClassFocus;
            AllowSwitchOut = model.AllowSwitchOut;
            InceptionDate = model.InceptionDate;
            Objective = model.Objective;
            FundSize = model.FundSize;
            IsDividend = model.IsDividend;
            TaxType = model.TaxType;
            Currency = model.Currency;
            ComplexFundUrl = model.ComplexFundUrl;
            ComplexFundRiskAckUrl = model.ComplexFundRiskAckUrl;
            RedemptionType = model.RedemptionType;
            AsOfDate = model.AsOfDate;
        }
    }

    public class AssetValueResponse
    {
        public decimal Nav { get; init; }
        public decimal? NavChange { get; init; }
        public double? NavChangePercentage { get; init; }
        public DateTime AsOfDate { get; init; }

        public AssetValueResponse(AssetValue? model = null)
        {
            if (model is null)
                return;

            Nav = model.Nav ?? 0;
            NavChange = model.NavChange?.ValueChange;
            NavChangePercentage = model.NavChange?.NavChangePercentage;
            AsOfDate = model.AsOfDate ?? default;
        }
    }

    public class AssetAllocationResponse
    {
        public DateTime? AsOfDate { get; init; }

        public IEnumerable<CategoricalRecord<string, double>> AssetClassAllocations { get; init; } = Enumerable.Empty<CategoricalRecord<string, double>>();
        public IEnumerable<CategoricalRecord<string, double>> RegionalAllocations { get; init; } = Enumerable.Empty<CategoricalRecord<string, double>>();
        public IEnumerable<CategoricalRecord<string, double>> SectorAllocations { get; init; } = Enumerable.Empty<CategoricalRecord<string, double>>();
        public IEnumerable<CategoricalRecord<string, double>> TopHoldings { get; init; } = Enumerable.Empty<CategoricalRecord<string, double>>();

        public AssetAllocationResponse(AssetAllocation? model = null)
        {
            if (model is null)
                return;

            AsOfDate = new[] { model.AssetClassAsOfDate, model.RegionalAsOfDate, model.SectorAsOfDate, model.TopHoldingsAsOfDate }.Max();

            var assetClassAllocations = model.AssetClassAllocations
                ?.Select(x => new CategoricalRecord<string, double>(x.Key, x.Value));
            var regionalAllocations = model.RegionalAllocations
                ?.Select(x => new CategoricalRecord<string, double>(x.Key, x.Value));
            var sectorAllocations = model.SectorAllocations
                ?.Select(x => new CategoricalRecord<string, double>(x.Key, x.Value));
            var topHoldings = model.TopHoldings
                ?.Select(x => new CategoricalRecord<string, double>(x.Key, x.Value));

            AssetClassAllocations = assetClassAllocations ?? Enumerable.Empty<CategoricalRecord<string, double>>();
            RegionalAllocations = regionalAllocations ?? Enumerable.Empty<CategoricalRecord<string, double>>();
            SectorAllocations = sectorAllocations ?? Enumerable.Empty<CategoricalRecord<string, double>>();
            TopHoldings = topHoldings ?? Enumerable.Empty<CategoricalRecord<string, double>>();
        }
    }

    public class PurchaseDetailResponse
    {
        public decimal? MinInitialBuy { get; init; }
        public decimal? MinAdditionalBuy { get; init; }
        public decimal? MinSellUnit { get; init; }
        public decimal? MinSellAmount { get; init; }
        public decimal? MinHoldUnit { get; init; }
        public decimal? MinHoldAmount { get; init; }
        public int? SettlementPeriod { get; init; }
        public string PiBuyCutOffTime { get; init; } = string.Empty;
        public string PiSellCutOffTime { get; init; } = string.Empty;
        public DateTime AsOfDate { get; init; }

        public PurchaseDetailResponse(Purchase? model = null)
        {
            if (model is null)
                return;

            MinInitialBuy = model.MinimumInitialBuy;
            MinAdditionalBuy = model.MinimumSubsequentBuy;
            MinSellUnit = model.MinimumSellUnit;
            MinSellAmount = model.MinimumSellAmount;
            MinHoldUnit = model.MinHoldUnit;
            MinHoldAmount = model.MinHoldAmount;
            SettlementPeriod = model.SellSettlementDay;
            PiBuyCutOffTime = model.GetPiBuyCutOffTimeLocal().ToString("s", CultureInfo.InvariantCulture);
            PiSellCutOffTime = model.GetPiSellCutOffTimeLocal().ToString("s", CultureInfo.InvariantCulture);
            AsOfDate = model.AsOfDate;
        }
    }

    public class FeeResponse
    {
        public FeeUnit? FeeUnit { get; init; }
        public decimal? SwitchInFee { get; init; }
        public double? ManagementFee { get; init; }
        public double? FrontendFee { get; init; }
        public double? BackendFee { get; init; }
        public double? TotalExpense { get; init; }
        public DateTime? AsOfDate { get; init; }

        public FeeResponse(Fee? model = null)
        {
            if (model is null)
                return;

            FeeUnit = model.SwitchInFeeUnit;
            SwitchInFee = model.SwitchInFee;
            ManagementFee = model.ManagementFee;
            FrontendFee = model.FrontendFee;
            BackendFee = model.BackendFee;
            TotalExpense = model.TotalExpense;
            AsOfDate = model.AsOfDate;
        }
    }

    public class PerformanceResponse
    {
        public double? Yield1Y { get; init; }
        public IEnumerable<HistoricalReturnPercentageResponse> HistoricalReturnPercentages { get; init; } = Enumerable.Empty<HistoricalReturnPercentageResponse>();

        public PerformanceResponse(Performance? model = null)
        {
            if (model is null)
                return;

            Yield1Y = model.Yield1Y;

            var historicalReturnPercentages = model.AnnualizedHistoricalReturnPercentages?.Select(x => new HistoricalReturnPercentageResponse(x));
            HistoricalReturnPercentages = historicalReturnPercentages ?? Enumerable.Empty<HistoricalReturnPercentageResponse>();
        }
    }

    public class HistoricalReturnPercentageResponse
    {
        public string Interval { get; init; } = string.Empty;
        public double? ReturnPercentage { get; init; }

        public HistoricalReturnPercentageResponse(HistoricalReturnPercentage? model = null)
        {
            if (model is null)
                return;

            Interval = EnumUtils.GetEnumMemberValue(model.Interval);
            ReturnPercentage = model.Value;
        }
    }

    public class DistributionResponse
    {
        public string DividendPolicy { get; init; } = string.Empty;
        public DateTime? ExDivDate { get; init; }
        public DateTime AsOfDate { get; init; }
        public IEnumerable<HistoricalDividendResponse> HistoricalDividends { get; init; } = Enumerable.Empty<HistoricalDividendResponse>();

        public DistributionResponse(Distribution? model = null)
        {
            if (model is null)
                return;

            DividendPolicy = model.DividendPolicy;
            ExDivDate = model.ExDivDate;
            AsOfDate = model.AsOfDate;

            var historicalDividends = model.HistoricalDividends?.Select(x => new HistoricalDividendResponse(x));
            HistoricalDividends = historicalDividends ?? Enumerable.Empty<HistoricalDividendResponse>();
        }
    }

    public class HistoricalDividendResponse
    {
        public DateTime PayDate { get; init; }
        public double Dividend { get; init; }

        public HistoricalDividendResponse(HistoricalDividend? model = null)
        {
            if (model is null)
                return;

            PayDate = model.PayDate;
            Dividend = model.Dividend;
        }
    }
}
