using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Models.Request;

namespace Pi.SetMarketData.Application.Queries.MarketFilterInstruments;

public record PostMarketFilterInstrumentsRequest(MarketFilterInstrumentsRequest Data) : Request<PostMarketFilterInstrumentsResponse>;