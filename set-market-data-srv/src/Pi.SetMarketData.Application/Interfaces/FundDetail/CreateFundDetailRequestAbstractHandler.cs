using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.FundDetail;

namespace Pi.SetMarketData.Application.Interfaces.FundDetail;

public abstract class CreateFundDetailRequestAbstractHandler: RequestHandler<CreateFundDetailRequest, CreateFundDetailResponse>;