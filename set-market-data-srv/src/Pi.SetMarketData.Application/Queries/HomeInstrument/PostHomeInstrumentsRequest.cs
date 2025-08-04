using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Models.Request;

namespace Pi.SetMarketData.Application.Queries;

public record PostHomeInstrumentsRequest(HomeInstrumentRequest Data) : Request<PostHomeInstrumentsResponse>;