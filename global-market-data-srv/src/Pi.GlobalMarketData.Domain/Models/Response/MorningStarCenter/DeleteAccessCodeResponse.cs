using Newtonsoft.Json;
using Pi.GlobalMarketData.Domain.interfaces;

namespace Pi.GlobalMarketData.Domain.Models.Response.MorningStarCenter;

public class DeleteAccessCodeResponse : IMorningStarCenterAccessCodeResponse<object>
{
    public Status? Status { get; set; }
    public Data<object>? Data { get; set; }

    public static DeleteAccessCodeResponse? FromJson(string json)
    {
        return JsonConvert.DeserializeObject<DeleteAccessCodeResponse>(json);
    }
}
