using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Commands;

public record UpdateCuratedFilterRequest(string id, CuratedFilter CuratedFilter) : Request<UpdateCuratedFilterResponse>;