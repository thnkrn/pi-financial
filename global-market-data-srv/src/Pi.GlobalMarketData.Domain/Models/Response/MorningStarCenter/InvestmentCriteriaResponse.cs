using Newtonsoft.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response.MorningStarCenter;

public class InvestmentCriteriaResponse
{
    public List<Data<InvestmentCriteria>>? Data { get; set; }

    public static InvestmentCriteriaResponse? FromJson(string json)
    {
        return JsonConvert.DeserializeObject<InvestmentCriteriaResponse>(json);
    }
}

public class InvestmentCriteria
{
    public string? Name { get; set; }
    public string? NarrativeLanguageId { get; set; }
    public string? NarrativeLanguageName { get; set; }
    public string? InvestmentStrategy { get; set; }
}
