using System.Text.Json;

namespace Pi.SetMarketData.Domain.Models.Response;

public class CashDividendResponse : GeneralInfoData
{
    public List<CashDividend>? CashDividendEntityList { get; set; }

    public static CashDividendResponse? FromJson(string json) =>
        JsonSerializer.Deserialize<CashDividendResponse>(json);
}

public class CashDividend
{
    public DateTime ExcludingDate { get; set; }
    public DateTime DeclarationDate { get; set; }
    public DateTime RecordDate { get; set; }
    public DateTime PayDate { get; set; }
    public double AmountPaid { get; set; }
    public string? CurrencyId { get; set; } = string.Empty;
    public int DistributionFrequency { get; set; }
    public DateTime FiscalYearEnd { get; set; }
}
