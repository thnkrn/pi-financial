using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Commands;

public record CreateCuratedListRequest(IEnumerable<CuratedList> CuratedList) : Request<CreateCuratedListResponse>;