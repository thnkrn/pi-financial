using System.Text.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response;

public class CurrentMarketCapitalizationResponse : GeneralInfoData
{
    public List<CurrentMarketCapitalization>? MarketCapitalizationEntityList { get; set; }

    public static CurrentMarketCapitalizationResponse? FromJson(string json) =>
        JsonSerializer.Deserialize<CurrentMarketCapitalizationResponse>(json);
}

public class CurrentMarketCapitalization
{
    public DateTime MarketCapDate { get; set; }
    public double MarketCap { get; set; }
    public double EnterpriseValue { get; set; }
    public string? CurrencyId { get; set; }
    public double SharesOutstanding { get; set; }
    public DateTime SharesDate { get; set; }
}
