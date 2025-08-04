using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Models.Request;

namespace Pi.SetMarketData.Application.Queries;

public record PostMarketInitialMarginRequest(MarketInitialMarginRequest Data)
    : Request<PostMarketInitialMarginResponse>;
