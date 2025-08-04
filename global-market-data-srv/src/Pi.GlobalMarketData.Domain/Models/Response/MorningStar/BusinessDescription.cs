using System.Text.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response;

public class BusinessDescriptionResponse : GeneralInfoData
{
    public BusinessDescriptionEntity? BusinessDescriptionEntity { get; set; }
    public BusinessDescription? BusinessDescription { get; set; }

    public static BusinessDescriptionResponse? FromJson(string json) =>
        JsonSerializer.Deserialize<BusinessDescriptionResponse>(json);
}

public class BusinessDescriptionEntity
{
    public string LongDescription { get; set; } = string.Empty;
}

public class BusinessDescriptionText
{
    public string LanguageCode { get; set; } = string.Empty;
    public string? EffectiveDate { get; set; } = string.Empty;
    public string? DocumentDate { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class BusinessDescription
{
    public List<string> ShortDescription { get; set; } = [];
    public List<string> MediumDescription { get; set; } = [];
    public List<BusinessDescriptionText> LongDescription { get; set; } = [];
}
