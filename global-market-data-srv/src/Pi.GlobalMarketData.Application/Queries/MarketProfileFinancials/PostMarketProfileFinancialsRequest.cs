using MassTransit.Mediator;
using Pi.GlobalMarketData.Domain.Models.Request;

namespace Pi.GlobalMarketData.Application.Queries;

public record PostMarketProfileFinancialsRequest(MarketProfileDescriptionRequest Data)
    : Request<PostMarketProfileFinancialsResponse>;
