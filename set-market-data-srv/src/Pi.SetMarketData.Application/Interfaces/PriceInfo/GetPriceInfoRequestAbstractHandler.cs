using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.PriceInfo;

namespace Pi.SetMarketData.Application.Interfaces.PriceInfo;

public abstract class GetPriceInfoRequestAbstractHandler: RequestHandler<GetPriceInfoRequest, GetPriceInfoResponse>;