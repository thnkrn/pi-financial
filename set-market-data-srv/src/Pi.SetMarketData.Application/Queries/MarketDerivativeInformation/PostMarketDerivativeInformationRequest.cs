using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Models.Request;

namespace Pi.SetMarketData.Application.Queries.MarketDerivativeInformation;

public record PostMarketDerivativeInformationRequest(
    MarketDerivativeInformationRequest Data,
    double InitialMarginMultiplier
) : Request<PostMarketDerivativeInformationResponse>;
