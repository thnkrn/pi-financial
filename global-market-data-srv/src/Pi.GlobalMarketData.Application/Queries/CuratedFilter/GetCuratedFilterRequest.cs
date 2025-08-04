using MassTransit.Mediator;

namespace Pi.GlobalMarketData.Application.Queries;

public record GetCuratedFilterRequest(string? GroupName, string? SubGroupName) : Request<GetCuratedFilterResponse>;