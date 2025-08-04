using MassTransit.Mediator;
using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Commands;

public record CreateCuratedFilterRequest(IEnumerable<CuratedFilter> CuratedFilter) : Request<CreateCuratedFilterResponse>;