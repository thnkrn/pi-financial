using Pi.GlobalMarketDataWSS.Domain.Models.Fix;
using Pi.GlobalMarketDataWSS.Domain.Models.Response;

namespace Pi.GlobalMarketDataWSS.Application.Interfaces.FixMapper;

public interface IPriceInfoMapperService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="streamingBody"></param>
    /// <param name="marketSession"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public StreamingBody Map(StreamingBody streamingBody, string marketSession, Entry entry);
}