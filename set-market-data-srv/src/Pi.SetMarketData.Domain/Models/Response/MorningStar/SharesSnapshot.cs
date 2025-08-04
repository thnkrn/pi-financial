using System.Text.Json;

namespace Pi.SetMarketData.Domain.Models.Response;

public class SharesSnapshotResponse : GeneralInfoData
{
    public SharesSnapshot? SharesSnapshotEntity { get; set; }

    public static SharesSnapshotResponse? FromJson(string json) =>
        JsonSerializer.Deserialize<SharesSnapshotResponse>(json);
}

public class SharesSnapshot
{
    public DateTime TSODate { get; set; }
    public double CompanyTSO { get; set; }
    public string? ShareLevelTSOSource { get; set; }
    public double ShareLevelTSO { get; set; }
    public DateTime FloatDate { get; set; }
    public double FloatShare { get; set; }
    public double InsiderHolding { get; set; }
    public double FloatShareToTotalSharesOutstanding { get; set; }
    public DateTime BalanceSheetDate { get; set; }
    public double ShareClassLevelTreasuryShareOutstanding { get; set; }
    public double SharesOutstandingWithBalanceSheetEndingDate { get; set; }
}
