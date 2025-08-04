using Newtonsoft.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response;

public class MorningstarToken
{
    public bool IsSuccess { set; get; } = false;
    public string? Token { set; get; } = string.Empty;
    public string? UserId { set; get; } = string.Empty;
    public DateTime? ExpireDate { set; get; }

    public static MorningstarToken? FromJson(string json) =>
        JsonConvert.DeserializeObject<MorningstarToken>(json);
}
