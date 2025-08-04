using MassTransit.Mediator;
using Pi.SetMarketData.Domain.Models.Request.BrokerInfo;

namespace Pi.SetMarketData.Application.Commands.BrokerInfo;

public record PostBrokerInfoRequest(BrokerInfoRequest Data) : Request<PostBrokerInfoResponse>;
