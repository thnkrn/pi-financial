using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries;

public record GetCuratedFilterRequest(string? GroupName, string? SubGroupName) : Request<GetCuratedFilterResponse>;