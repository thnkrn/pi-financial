using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.Domain.interfaces;

public interface IMorningStarCenterApiDataResponse<T>
{
    public Data<T>? Data { get; set; }
}
