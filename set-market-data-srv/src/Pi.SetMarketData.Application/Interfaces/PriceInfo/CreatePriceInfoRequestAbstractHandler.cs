using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.PriceInfo;

namespace Pi.SetMarketData.Application.Interfaces.PriceInfo;

public abstract class CreatePriceInfoRequestAbstractHandler: RequestHandler<CreatePriceInfoRequest, CreatePriceInfoResponse>;