using Newtonsoft.Json;

namespace Pi.SetMarketData.Domain.Models.Response;

public class MorningStarToken
{
    public bool IsSuccess { set; get; } = false;
    public string? Token { set; get; } = string.Empty;
    public string? UserId { set; get; } = string.Empty;
    public DateTime? ExpireDate { set; get; }

    public static MorningStarToken? FromJson(string json) =>
        JsonConvert.DeserializeObject<MorningStarToken>(json);
}
