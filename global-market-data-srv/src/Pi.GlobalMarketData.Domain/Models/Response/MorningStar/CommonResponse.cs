using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response;

public class MorningStarStatementType
{
    private MorningStarStatementType(string value)
    {
        Value = value;
    }

    public string Value { get; }
    public static MorningStarStatementType Quarterly => new("Quarterly");
    public static MorningStarStatementType SemiAnnual => new("Semi-Annual");
    public static MorningStarStatementType Annual => new("Annual");

    public override string ToString() => Value;
}

public class Status
{
    public int Code { get; set; }

    public string? Message { get; set; }
}

public class GeneralInfoData
{
    [BsonId]
    [BsonElement("_id")]
    public ObjectId Id { get; set; }

    public GeneralInfo? GeneralInfo { get; set; }
}

public class GeneralInfo
{
    public string? ShareClassId { get; set; } = string.Empty;

    public string? CompanyName { get; set; } = string.Empty;

    public string? ExchangeId { get; set; } = string.Empty;

    public string? Symbol { get; set; } = string.Empty;

    public string? CIK { get; set; } = string.Empty;

    public string? CountryId { get; set; } = string.Empty;

    public string? CompanyLEI { get; set; } = string.Empty;
}

public class ReportInfo
{
    public DateTime ReportDate { get; set; }
    public DateTime PeriodEndingDate { get; set; }
    public DateTime FileDate { get; set; }
    public string? StatementType { get; set; }
    public string? DataType { get; set; }
    public string? Interim { get; set; }
    public string? CurrencyId { get; set; }
    public string? AccessionNumber { get; set; }
    public string? FormType { get; set; }
}

public class Data<T>
{
    [JsonProperty("_idtype")]
    public string? IdType { get; set; }

    [JsonProperty("_id")]
    public string? Id { get; set; }

    [JsonProperty("_MstarId")]
    public string? MstarId { get; set; }

    [JsonProperty("_CurrencyId")]
    public string? CurrencyId { get; set; }

    [JsonProperty("api")]
    public T? Api { get; set; }

    public static List<Data<T>>? FromJson(string json) =>
        JsonConvert.DeserializeObject<List<Data<T>>>(json);
}
