#nullable enable

using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.API.Models.Responses;

public class FundTradingProfileResponse
{
    public string Symbol { get; init; }
    public string Name { get; init; }
    public string FundCategory { get; init; }
    public FundamentalTradingResponse? Fundamental { get; init; }
    public AmcProfileTradingResponse? AmcProfile { get; init; }
    public PurchaseDetailTradingResponse? PurchaseDetail { get; init; }
    public decimal Nav { get; init; }

    public FundTradingProfileResponse(Fund model, AmcProfile? amcProfile)
    {
        Symbol = model.Symbol;
        Name = model.Name;
        FundCategory = model.Category;
        Nav = model.AssetValue?.Nav ?? 0;
        Fundamental = model.Fundamental is not null ? new FundamentalTradingResponse(model.Fundamental) : null;
        AmcProfile = model.AmcCode is not null ? new AmcProfileTradingResponse(amcProfile) : null;
        PurchaseDetail = model.Purchase is not null ? new PurchaseDetailTradingResponse(model.Purchase) : null;
    }

    public class FundamentalTradingResponse
    {
        public string? TaxType { get; init; }
        public int? FundRiskLevel { get; init; }
        public FundType? FundType { get; init; }
        public ProjectType? ProjectType { get; init; }
        public bool? IsFatcaAllow { get; init; }
        public bool? HasCurrencyRisk { get; init; }
        public bool? IsDerivative { get; init; }
        public bool? HasHealthInsuranceBenefit { get; init; }
        public IEnumerable<InvestorAlert>? InvestorAlerts { get; init; }
        public FundamentalTradingResponse(Fundamental? fundamental)
        {
            if (fundamental == null) return;
            TaxType = fundamental.TaxType;
            FundRiskLevel = fundamental.RiskLevel;
            FundType = fundamental.FundType;
            ProjectType = fundamental.ProjectType;
            IsFatcaAllow = fundamental.IsFatcaAllow;
            HasCurrencyRisk = fundamental.HasCurrencyRisk;
            IsDerivative = fundamental.IsDerivative;
            HasHealthInsuranceBenefit = fundamental.HasHealthInsuranceBenefit;
            InvestorAlerts = fundamental.InvestorAlerts;
        }
    }

    public class AmcProfileTradingResponse
    {
        public string? AmcCode { get; init; }
        public string? AmcLogo { get; init; }
        public AmcProfileTradingResponse(AmcProfile? amcProfile)
        {
            if (amcProfile == null) return;
            AmcCode = amcProfile.Code;
            AmcLogo = amcProfile.Logo;
        }
    }

    public class PurchaseDetailTradingResponse
    {
        public decimal? MinInitialBuy { get; init; }
        public decimal? MinAdditionalBuy { get; init; }
        public decimal? MinSellUnit { get; init; }
        public decimal? MinSellAmount { get; init; }
        public decimal? MinHoldUnit { get; init; }
        public decimal? MinHoldAmount { get; init; }
        public DateTime? BuyCutOffTime { get; init; }
        public DateTime? SellCutOffTime { get; init; }
        public DateTime PiBuyCutOffTime { get; init; }
        public DateTime PiSellCutOffTime { get; init; }
        public PurchaseDetailTradingResponse(Purchase? model = null)
        {
            if (model is null) return;
            var thTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var buyCutOffTimeLocal = model.GetBuyCutOffTimeLocal();
            var sellCutOffTime = model.GetSellCutOffTimeLocal();
            var piBuyCutOffTime = model.GetPiBuyCutOffTimeLocal();
            var piSellCutOffTime = model.GetPiSellCutOffTimeLocal();
            MinInitialBuy = model.MinimumInitialBuy;
            MinAdditionalBuy = model.MinimumSubsequentBuy;
            MinSellUnit = model.MinimumSellUnit;
            MinSellAmount = model.MinimumSellAmount;
            MinHoldUnit = model.MinHoldUnit;
            MinHoldAmount = model.MinHoldAmount;
            BuyCutOffTime = buyCutOffTimeLocal == null ? null : TimeZoneInfo.ConvertTimeToUtc(buyCutOffTimeLocal.Value, thTimeZone);
            SellCutOffTime = sellCutOffTime == null ? null : TimeZoneInfo.ConvertTimeToUtc(sellCutOffTime.Value, thTimeZone);
            PiBuyCutOffTime = TimeZoneInfo.ConvertTimeToUtc(piBuyCutOffTime, thTimeZone);
            PiSellCutOffTime = TimeZoneInfo.ConvertTimeToUtc(piSellCutOffTime, thTimeZone);
        }
    }
}
