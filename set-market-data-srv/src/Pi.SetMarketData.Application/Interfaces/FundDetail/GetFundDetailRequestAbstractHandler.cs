using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.FundDetail;

namespace Pi.SetMarketData.Application.Interfaces.FundDetail;

public abstract class GetFundDetailRequestAbstractHandler: RequestHandler<GetFundDetailRequest, GetFundDetailResponse>;