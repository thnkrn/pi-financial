using MassTransit.Mediator;
using Pi.GlobalMarketData.Domain.Models.Request;

namespace Pi.GlobalMarketData.Application.Queries;

public record PostHomeInstrumentRequest(HomeInstrumentRequest Data) : Request<PostHomeInstrumentResponse>;