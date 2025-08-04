using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.Domain.interfaces;

public interface IMorningStarCenterAccessCodeResponse<T>
{
    public Status? Status { get; set; }
    public Data<T>? Data { get; set; }
}
