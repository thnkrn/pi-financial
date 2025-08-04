using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Models.Request;

namespace Pi.SetMarketData.Application.Queries.MarketProfileFundamentals;

public record PostMarketProfileFundamentalsRequest(MarketProfileDescriptionRequest Data)
    : Request<PostMarketProfileFundamentalsResponse>;
