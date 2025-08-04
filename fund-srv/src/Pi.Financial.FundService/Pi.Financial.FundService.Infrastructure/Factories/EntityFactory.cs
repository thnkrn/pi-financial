using Pi.Client.FundMarketData.Model;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.Financial.FundService.Infrastructure.Factories;

public static class EntityFactory
{
    public static FundInfo NewFundInfo(PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse marketData)
    {
        return new FundInfo(marketData.Name, marketData.Symbol, marketData.AmcProfile.AmcLogo, marketData.AmcProfile.AmcCode)
        {
            InstrumentCategory = marketData.FundCategory,
            Nav = marketData.Nav,
            FirstMinBuyAmount = Convert.ToDecimal(marketData.PurchaseDetail.MinInitialBuy),
            NextMinBuyAmount = Convert.ToDecimal(marketData.PurchaseDetail.MinAdditionalBuy),
            MinSellAmount = Convert.ToDecimal(marketData.PurchaseDetail.MinSellAmount),
            MinSellUnit = Convert.ToDecimal(marketData.PurchaseDetail.MinSellUnit),
            MinBalanceAmount = Convert.ToDecimal(marketData.PurchaseDetail.MinHoldAmount),
            MinBalanceUnit = Convert.ToDecimal(marketData.PurchaseDetail.MinHoldUnit),
            TaxType = marketData.Fundamental.TaxType,
            PiBuyCutOffTime = marketData.PurchaseDetail.PiBuyCutOffTime,
            PiSellCutOffTime = marketData.PurchaseDetail.PiSellCutOffTime,
            ProjectType = marketData.Fundamental.ProjectType != null ? NewProjectType((PiFundMarketDataConstantsProjectType)marketData.Fundamental.ProjectType) : null,
        };
    }

    private static FundProjectType NewProjectType(PiFundMarketDataConstantsProjectType projectType)
    {
        return projectType switch
        {
            PiFundMarketDataConstantsProjectType.Unknown => FundProjectType.Unknown,
            PiFundMarketDataConstantsProjectType.GeneralInvestorsFund => FundProjectType.GeneralInvestorsFund,
            PiFundMarketDataConstantsProjectType.InstitutionalInvestorsFund => FundProjectType.InstitutionalInvestorsFund,
            PiFundMarketDataConstantsProjectType.NonRetailInvestorsFund => FundProjectType.NonRetailInvestorsFund,
            PiFundMarketDataConstantsProjectType.NonRetailAndHighNetWorthInvestorsFund => FundProjectType.NonRetailAndHighNetWorthInvestorsFund,
            PiFundMarketDataConstantsProjectType.InstitutionalAndSpecialLargeInvestorsFund => FundProjectType.InstitutionalAndSpecialLargeInvestorsFund,
            PiFundMarketDataConstantsProjectType.HighNetWorthInvestorsFund => FundProjectType.HighNetWorthInvestorsFund,
            PiFundMarketDataConstantsProjectType.CorporateInvestorsFund => FundProjectType.CorporateInvestorsFund,
            PiFundMarketDataConstantsProjectType.ThaiGovernmentFund => FundProjectType.ThaiGovernmentFund,
            PiFundMarketDataConstantsProjectType.PensionReserveFund => FundProjectType.PensionReserveFund,
            _ => throw new ArgumentOutOfRangeException(nameof(projectType), projectType, null)
        };
    }
}
