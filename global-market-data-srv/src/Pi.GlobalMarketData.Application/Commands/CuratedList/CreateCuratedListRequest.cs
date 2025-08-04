using MassTransit.Mediator;
using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Commands;

public record CreateCuratedListRequest(IEnumerable<CuratedList> CuratedList) : Request<CreateCuratedListResponse>;