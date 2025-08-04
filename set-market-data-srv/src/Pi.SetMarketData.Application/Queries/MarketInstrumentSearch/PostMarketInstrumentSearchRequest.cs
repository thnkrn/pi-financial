using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Models.Request;
namespace Pi.SetMarketData.Application.Queries.MarketInstrumentSearch;

public record PostMarketInstrumentSearchRequest(MarketInstrumentSearchRequest data)
    : Request<PostMarketInstrumentSearchResponse>;