using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;

namespace Pi.GlobalMarketData.Application.Interfaces.MarketDerivativeInformation;

public abstract class PostMarketDerivativeInformationAbstractHandler
    : RequestHandler<
        PostMarketDerivativeInformationRequest,
        PostMarketDerivativeInformationResponse
    >;
