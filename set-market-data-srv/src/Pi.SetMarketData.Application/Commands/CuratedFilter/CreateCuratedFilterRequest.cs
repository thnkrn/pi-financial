using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Commands;

public record CreateCuratedFilterRequest(IEnumerable<CuratedFilter> CuratedFilter) : Request<CreateCuratedFilterResponse>;