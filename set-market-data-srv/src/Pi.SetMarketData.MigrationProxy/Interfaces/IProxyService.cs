using System.Net.WebSockets;
using Pi.SetMarketData.MigrationProxy.Models;
using Pi.SetMarketData.MigrationProxy.Enums;

namespace Pi.SetMarketData.MigrationProxy.Interfaces;

public interface IProxyService
{
    Task SubscribeSignalRHub(WebSocket webSocket, RequestPayload? payload, SignalRHubEndpoint endpoint);
    Task SubscribeWebsocket(WebSocket webSocketClientOut, RequestPayload? payload);
    (RequestPayload? SETPayload, RequestPayload? GEPayload, RequestPayload? WsPayload) DetermineSubscriptionList(RequestPayload? payload);
}