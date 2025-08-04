using Newtonsoft.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response.MorningStarCenter;

public class FundShareClassBasicInfoResponse
{
    public List<Data<FundShareClassBasicInfo>>? Data { get; set; }

    public static FundShareClassBasicInfoResponse? FromJson(string json)
    {
        return JsonConvert.DeserializeObject<FundShareClassBasicInfoResponse>(json);
    }
}

public class FundShareClassBasicInfo
{
    public string? Name { get; set; }
    public string? MStarID { get; set; }
    public string? FundName { get; set; }
    public string? ExchangeID { get; set; }
    public string? Ticker { get; set; }
    public string? Isin { get; set; }
    public string? DomicileId { get; set; }
    public string? Domicile { get; set; }
    public string? SecurityType { get; set; }
    public string? CurrencyId { get; set; }
    public string? Currency { get; set; }
    public string? CategoryCode { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryCurrencyId { get; set; }
    public string? LegalStructure { get; set; }
    public string? PerformanceID { get; set; }
    public string? FundId { get; set; }
    public string? LegalName { get; set; }
    public DateTime InceptionDate { get; set; }
    public string? ProviderCompanyID { get; set; }
    public string? ProviderCompanyName { get; set; }
    public string? ProviderCompanyWebsite { get; set; }
    public string? ProviderCompanyPhoneNumber { get; set; }
    public string? AdvisoryCompanyID { get; set; }
    public string? AdvisoryCompanyName { get; set; }
    public string? CustodianCompanyID { get; set; }
    public string? CustodianCompanyName { get; set; }
    public List<MultilingualName>? MultilingualNames { get; set; }

    public string? BroadCategoryGroup { get; set; }
    public string? AggregatedCategoryName { get; set; }
}

public class MultilingualName
{
    public string? CategoryName { get; set; }
    public string? LanguageName { get; set; }
    public string? LanguageId { get; set; }
}
