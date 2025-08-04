using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Application.Commands;

public record UpdateCuratedListRequest(string id, CuratedList CuratedList) : Request<UpdateCuratedListResponse>;