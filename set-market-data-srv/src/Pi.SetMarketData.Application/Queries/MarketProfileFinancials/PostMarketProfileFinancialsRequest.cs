using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Models.Request;

namespace Pi.SetMarketData.Application.Queries.MarketProfileFinancials;

public record PostMarketProfileFinancialsRequest(MarketProfileDescriptionRequest Data)
    : Request<PostMarketProfileFinancialsResponse>;
