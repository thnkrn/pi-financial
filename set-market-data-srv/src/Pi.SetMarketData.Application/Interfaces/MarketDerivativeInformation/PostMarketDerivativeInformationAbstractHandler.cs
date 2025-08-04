using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.MarketDerivativeInformation;

namespace Pi.SetMarketData.Application.Interfaces.MarketDerivativeInformation;

public abstract class PostMarketDerivativeInformationAbstractHandler
    : RequestHandler<PostMarketDerivativeInformationRequest, PostMarketDerivativeInformationResponse>;
