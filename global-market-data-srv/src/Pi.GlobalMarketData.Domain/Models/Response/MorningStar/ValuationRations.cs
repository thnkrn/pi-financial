using Newtonsoft.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response;

public class ValuationRationsResponse : GeneralInfoData
{
    public List<ValuationRations>? ValuationRatioEntityList { get; set; }

    public static ValuationRationsResponse? FromJson(string json) =>
        JsonConvert.DeserializeObject<ValuationRationsResponse>(json);
}

public class ValuationRations
{
    public DateTime AsOfDate { get; set; }
    public double SalesPerShare { get; set; }
    public double GrowthAnnSalesPerShare5Year { get; set; }
    public double BookValuePerShare { get; set; }
    public double CFPerShare { get; set; }
    public double FCFPerShare { get; set; }
    public double PriceToEPS { get; set; }
    public double RatioPE5YearHigh { get; set; }
    public double RatioPE5YearLow { get; set; }
    public double PriceToBook { get; set; }
    public double PriceToSales { get; set; }
    public double PriceToCashFlow { get; set; }
    public double PriceToFreeCashFlow { get; set; }
    public double DivRate { get; set; }
    public double DividendYield { get; set; }
    public double DivPayoutTotOps { get; set; }
    public double DivPayout5Year { get; set; }
    public double DivYield5Year { get; set; }
    public double PayoutRatio { get; set; }
    public double SustainableGrowthRate { get; set; }
    public double CashReturn { get; set; }
    public double ForwardEarningYield { get; set; }
    public double PEGRatio { get; set; }
    public double PEGPayback { get; set; }
    public double ForwardDividendYield { get; set; }
    public double ForwardPERatio { get; set; }
    public double TangibleBookValuePerShare { get; set; }
    public double TangibleBVPerShare3YrAvg { get; set; }
    public double TangibleBVPerShare5YrAvg { get; set; }
    public double EVToEBITDA { get; set; }
    public double RatioPE5YearAverage { get; set; }
    public double NormalizedPERatio { get; set; }
    public double FCFYield { get; set; }
    public double EVToForwardEBIT { get; set; }
    public double EVToForwardEBITDA { get; set; }
    public double EVToForwardRevenue { get; set; }
    public double TwoYearsEVToForwardEBIT { get; set; }
    public double TwoYearsEVToForwardEBITDA { get; set; }
    public double FirstYearEstimatedEPSGrowth { get; set; }
    public double SecondYearEstimatedEPSGrowth { get; set; }
    public double NormalizedPEG { get; set; }
    public double EarningYield { get; set; }
    public double SalesYield { get; set; }
    public double BookValueYield { get; set; }
    public double CashFlowYield { get; set; }
    public double WorkingCapitalPerShare { get; set; }
    public double WorkingCapitalPerShare3YrAvg { get; set; }
    public double WorkingCapitalPerShare5YrAvg { get; set; }
    public double BuyBackYield { get; set; }
    public double TotalYield { get; set; }
    public double PricetoEBITDA { get; set; }
    public double ForwardROE { get; set; }
    public double ForwardROA { get; set; }
    public double TwoYearsForwardEarningYield { get; set; }
    public double TwoYearsForwardPERatio { get; set; }
    public double TotalAssetPerShare { get; set; }
    public double EVtoRevenue { get; set; }
    public double EVtoPreTaxIncome { get; set; }
    public double EVtoTotalAssets { get; set; }
    public double EVtoFCF { get; set; }
    public double EVtoEBIT { get; set; }
    public double PricetoCashRatio { get; set; }
    public double CAPERatio { get; set; }
}
