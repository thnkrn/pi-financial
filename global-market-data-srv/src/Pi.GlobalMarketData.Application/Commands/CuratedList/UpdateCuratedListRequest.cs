using MassTransit.Mediator;
using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Application.Commands;

public record UpdateCuratedListRequest(string id, CuratedList CuratedList) : Request<UpdateCuratedListResponse>;