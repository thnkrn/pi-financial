using System.Text.Json;

namespace Pi.SetMarketData.Domain.Models.Response;

public class ProfitabilityRatiosResponse : GeneralInfoData
{
    public List<ProfitabilityRatios>? ProfitabilityEntityList { get; set; }

    public static ProfitabilityRatiosResponse? FromJson(string json) =>
        JsonSerializer.Deserialize<ProfitabilityRatiosResponse>(json);
}

public class ProfitabilityRatios : ReportInfo
{
    public double GrossMargin { get; set; }
    public double OperatingMargin { get; set; }
    public double EBTMargin { get; set; }
    public double TaxRate { get; set; }
    public double NetMargin { get; set; }
    public double SalesPerEmployee { get; set; }
    public double EBITMargin { get; set; }
    public double EBITDAMargin { get; set; }
    public double NormalizedNetProfitMargin { get; set; }
    public double InterestCoverage { get; set; }
    public double IncPerEmployeeTotOps { get; set; }
    public double NetIncomePerFullTimeEmployee { get; set; }
    public double SolvencyRatio { get; set; }
}
