using MassTransit.Mediator;
using Pi.GlobalMarketData.Domain.Models.Request;

namespace Pi.GlobalMarketData.Application.Queries.MarketFilterInstruments;

public record PostMarketFilterInstrumentsRequest(MarketFilterInstrumentsRequest Data) : Request<PostMarketFilterInstrumentsResponse>;