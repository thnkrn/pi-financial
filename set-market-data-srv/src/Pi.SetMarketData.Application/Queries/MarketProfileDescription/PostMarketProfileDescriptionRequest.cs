using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Models.Request;

namespace Pi.SetMarketData.Application.Queries.MarketProfileDescription;

public record PostMarketProfileDescriptionRequest(MarketProfileDescriptionRequest Data)
    : Request<PostMarketProfileDescriptionResponse>;
