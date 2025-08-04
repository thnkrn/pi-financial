using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Pi.GlobalMarketData.Domain.interfaces;

namespace Pi.GlobalMarketData.Domain.Models.Response.MorningStarCenter;

public class AccessCodeResponse : IMorningStarCenterAccessCodeResponse<API>
{
    [BsonId]
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public Status? Status { get; set; }

    [BsonElement("data")]
    public Data<API>? Data { get; set; }

    public static AccessCodeResponse? FromJson(string json)
    {
        return JsonConvert.DeserializeObject<AccessCodeResponse>(json);
    }
}

public class API
{
    [JsonProperty("AccessCode")]
    [BsonElement("code")]
    public string? Code { get; set; }

    [BsonElement("account_code")]
    public string? AccountCode { get; set; }

    [BsonElement("create_time")]
    public DateTime CreateTime { get; set; }

    [BsonElement("expire_time")]
    public DateTime ExpireTime { get; set; }

    [BsonElement("expire")]
    public bool Expired { get; set; }
}
