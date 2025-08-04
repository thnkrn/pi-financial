using System.Net.WebSockets;
using Pi.MarketData.Domain.Enums;
using Pi.MarketData.Domain.Models;

namespace Pi.MarketData.Application.Interfaces;

public interface IProxyService
{
    Task SubscribeSignalRHub(WebSocket webSocket, RequestPayload? payload, SignalRHubEndpoint endpoint,
        CancellationToken cancellationToken);

    Task SubscribeWebsocket(WebSocket webSocketClientOut, RequestPayload? payload, CancellationToken cancellationToken);

    (RequestPayload? setPayload, RequestPayload? gePayload, RequestPayload? wsPayload) DetermineSubscriptionList(
        RequestPayload? payload);
}