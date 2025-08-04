using MassTransit.Mediator;
using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Commands;

public record UpdateCuratedFilterRequest(string id, CuratedFilter CuratedFilter) : Request<UpdateCuratedFilterResponse>;