using System.Text;
using Newtonsoft.Json;
using System.Net.WebSockets;
using Microsoft.AspNetCore.SignalR.Client;
using Pi.SetMarketData.MigrationProxy.Interfaces;
using Pi.SetMarketData.MigrationProxy.ConstantConfigurations;
using Pi.SetMarketData.MigrationProxy.Models;
using Pi.SetMarketData.MigrationProxy.Enums;

namespace Pi.SetMarketData.MigrationProxy.Services;

public class ProxyService : IProxyService
{
    private readonly ILogger<ProxyService> _logger;
    private readonly IFeatureFlagService _featureFlagService;
    private readonly string _SEThubUrl;
    private readonly string _GEhubUrl;
    private readonly string _websocketUrl;
    private readonly HashSet<string> SETVenue = ["Equity"];
    private readonly HashSet<string> GEVenue = ["ARCA", "BATS", "HKEX", "NASDAQ", "NYSE"];
    private readonly HashSet<string> TFEXVenue = ["Derivative"];
    public ProxyService
    (
        ILogger<ProxyService> logger,
        IFeatureFlagService featureFlagService,
        IConfiguration configuration
    )
    {
        _logger = logger;
        _featureFlagService = featureFlagService;
        _SEThubUrl = configuration.GetValue<string>(ConfigurationKeys.SETSignalRHubURL)
            ?? throw new InvalidOperationException("SETSignalRHubURL is not configured.");
        _GEhubUrl = configuration.GetValue<string>(ConfigurationKeys.GESignalRHubURL)
            ?? throw new InvalidOperationException("GESignalRHubURL is not configured.");
        _websocketUrl = configuration.GetValue<string>(ConfigurationKeys.WebSocketURL)
            ?? throw new InvalidOperationException("WebSocketURL is not configured.");
    }

    public async Task SubscribeSignalRHub(WebSocket webSocket, RequestPayload? payload, SignalRHubEndpoint endpoint)
    {
        if (payload == null) return;

        var hubUrl = GetUrlEndpoint(endpoint);

        try
        {
            await using var connection = await CreateHubConnection(hubUrl, webSocket);
            _logger.LogInformation("Connected to the SignalR hub {HubUrl}",
                hubUrl
            );
            await connection.InvokeAsync("SubscribeToStreamDataAsync", payload);
            if (connection is { State: HubConnectionState.Connected })
            {
                await HandleHubMessages(connection, webSocket);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to SignalR hub");
        }
    }

    public async Task SubscribeWebsocket(WebSocket webSocketClientOut, RequestPayload? payload)
    {
        if (payload == null) return;

        try
        {
            using var webSocket = new ClientWebSocket();

            await webSocket.ConnectAsync(new Uri(_websocketUrl), CancellationToken.None);

            var message = JsonConvert.SerializeObject(payload);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync
            (
                new ArraySegment<byte>(messageBytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );

            _logger.LogInformation("Connected to WebSocket server {WebsocketUrl}", _websocketUrl);

            await HandleWebsocketMessages(webSocket, webSocketClientOut);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to WebSocket");
        }
    }

    private async Task HandleHubMessages(HubConnection connection, WebSocket webSocket)
    {
        await ListenForWebSocket(webSocket, async () =>
        {
            await connection.StopAsync();
        });
    }

    private async Task HandleWebsocketMessages(ClientWebSocket webSocket, WebSocket webSocketClientOut)
    {
        var receiveTask = ListenForClientMessages(webSocketClientOut, async () =>
        {
            await CloseWebsocketConnection(webSocket, webSocketClientOut);
        });

        await ListenForServerMessages(webSocket, webSocketClientOut);

        await receiveTask;
    }

    private async Task ListenForWebSocket(WebSocket webSocket, Func<Task> onClose)
    {
        while (webSocket.State == WebSocketState.Open)
        {
            try
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(new byte[1024 * 4]), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    break;
                }
            }
            catch (Exception ex) when (ex is WebSocketException or ObjectDisposedException)
            {
                _logger.LogError(ex, "WebSocket closed unexpectedly.");
                break;
            }
        }
        await onClose();
    }

    private async Task ListenForClientMessages(WebSocket webSocketClientOut, Func<Task> onClose)
    {
        while (webSocketClientOut.State == WebSocketState.Open)
        {
            try
            {
                var clientBuffer = new byte[1024 * 4];
                var clientResult = await webSocketClientOut.ReceiveAsync(new ArraySegment<byte>(clientBuffer), CancellationToken.None);

                if (clientResult.MessageType == WebSocketMessageType.Close)
                {
                    await webSocketClientOut.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    break;
                }
                var receivedMessage = Encoding.UTF8.GetString(clientBuffer, 0, clientResult.Count);
                _logger.LogInformation("[WS>>] Received | {ReceivedMessage}", receivedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in receiving message: {Message}", ex.Message);
            }
        }
        await onClose();
    }

    private async Task ListenForServerMessages(ClientWebSocket webSocket, WebSocket webSocketClientOut)
    {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            try
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                _logger.LogInformation("[+WS] Got | {ReceivedMessage}", receivedMessage);

                if (webSocketClientOut.State == WebSocketState.Open)
                {
                    await SendWebSocketMessage(webSocketClientOut, receivedMessage, result.MessageType, result.EndOfMessage);
                    _logger.LogInformation("[WS>>] Sent | {ReceivedMessage}", receivedMessage);
                }
                else
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in processing message: {Message}", ex.Message);
            }
        }
        await CloseWebsocketConnection(webSocket, webSocketClientOut);
    }

    private static async Task SendWebSocketMessage
    (
        WebSocket webSocket,
        string message,
        WebSocketMessageType messageType = WebSocketMessageType.Text,
        bool endOfMessage = true
    )
    {
        await webSocket.SendAsync(
            new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),
            messageType,
            endOfMessage,
            CancellationToken.None
        );
    }

    private async Task CloseWebsocketConnection(ClientWebSocket webSocket, WebSocket webSocketClientOut)
    {
        if (webSocketClientOut.State != WebSocketState.Closed && webSocketClientOut.State != WebSocketState.Aborted)
        {
            try
            {
                await webSocketClientOut.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                _logger.LogInformation("----------------------- Client disconnected");
            }
            catch (WebSocketException ex)
            {
                _logger.LogWarning(ex, "Error closing webSocketClientOut: {Message}", ex.Message);
            }
        }

        if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
        {
            try
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                _logger.LogWarning(ex, "Error closing webSocket: {Message}", ex.Message);
            }
        }
    }

    private async Task<HubConnection> CreateHubConnection(string url, WebSocket webSocket)
    {
        var connection = new HubConnectionBuilder()
           .WithUrl(url)
           .WithAutomaticReconnect([TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)])
           .Build();

        connection.On<object>("ReceiveMarketData", async (data) =>
        {
            _logger.LogInformation("[+Hub] Got | {Payload}", data);
            await SendWebSocketMessage(webSocket, data.ToString()!);
            _logger.LogInformation("[Hub>>] Sent | {Payload}", data);
        });
        await connection.StartAsync();
        return connection;
    }

    private string GetUrlEndpoint(SignalRHubEndpoint endpoint)
    {
        return endpoint switch
        {
            SignalRHubEndpoint.SET => _SEThubUrl,
            SignalRHubEndpoint.GE => _GEhubUrl,
            _ => throw new InvalidOperationException($"Endpoint value was {endpoint}. No valid return value provided.")
        };
    }

    public (RequestPayload? SETPayload, RequestPayload? GEPayload, RequestPayload? WsPayload)
    DetermineSubscriptionList(RequestPayload? payload)
    {
        if (payload?.Data?.Param == null)
        {
            return (null, null, null);
        }

        var parameters = payload.Data.Param;
        var flags = GetFeatureFlags();
        var (SETPayload, GEPayload) = CreateVenuePayloads(payload, parameters, flags);
        var wsPayload = CreateWsPayload(payload, parameters, flags);

        return (SETPayload, GEPayload, wsPayload);
    }

    private (bool SET, bool GE, bool TFEX) GetFeatureFlags()
    {
        // return (
        //     SET: _featureFlagService.IsSETProxyEnabled(),
        //     GE: _featureFlagService.IsGEProxyEnabled(),
        //     TFEX: _featureFlagService.IsTFEXProxyEnabled()
        // );

        return (
            SET: true,
            GE: true,
            TFEX: true
        );
    }

    private (RequestPayload? SETPayload, RequestPayload? GEPayload) CreateVenuePayloads
    (
        RequestPayload payload,
        List<Parameter> parameters,
        (bool SET, bool GE, bool TFEX) flags
    )
    {
        var setPayload = new RequestPayload
        {
            Data = new Data
            {
                Param = [],
                SubscribeType = payload.Data?.SubscribeType
            },
            Op = payload.Op,
            SessionId = payload.SessionId
        };
        var gePayload = new RequestPayload
        {
            Data = new Data
            {
                Param = [],
                SubscribeType = payload.Data?.SubscribeType
            },
            Op = payload.Op,
            SessionId = payload.SessionId
        };

        foreach (var item in parameters)
        {
            if ((item.Market == null) || (item.Symbol == null)) continue;
            if (SETVenue.Contains(item.Market) && flags.SET)
            {
                setPayload.Data.Param.Add(item);
            }
            else if (GEVenue.Contains(item.Market) && flags.GE && !item.Symbol.EndsWith(".INDEX"))
            {
                gePayload.Data.Param.Add(item);
            }
            else if (TFEXVenue.Contains(item.Market) && flags.TFEX)
            {
                setPayload.Data.Param.Add(item);
            }
        }
        return (setPayload, gePayload);
    }

    private RequestPayload CreateWsPayload(RequestPayload payload, List<Parameter> parameters, (bool SET, bool GE, bool TFEX) flags)
    {
        var wsParameters = GetWSParameters(parameters, flags.SET, flags.GE, flags.TFEX);

        return new RequestPayload
        {
            Data = new Data
            {
                Param = wsParameters,
                SubscribeType = payload.Data?.SubscribeType
            },
            Op = payload.Op,
            SessionId = payload.SessionId
        };
    }

    private List<Parameter> GetWSParameters(List<Parameter> param, bool SETflag, bool GEflag, bool TFEXflag)
    {
        List<Parameter> wsParameters =
        [
            .. param.Where(item => item?.Market != null && !SETflag && (item.Market == "Equity")).ToList(),         // SET
            .. param.Where(item => item?.Market != null && !GEflag && GEVenue.Contains(item.Market)).ToList(),      // GE
            .. param.Where(item => item?.Market != null && item.Symbol.EndsWith(".INDEX")).ToList(),                // Symbol with "*.INDEX"
            .. param.Where(item => item?.Market != null && !TFEXflag && (item.Market == "Derivative")).ToList(),    // TFEX
            .. param.Where(item => item?.Market != null && !GEVenue.Contains(item.Market) && (item.Market != "Equity") && (item.Market != "Derivative")).ToList(),  // Others
        ];
        return wsParameters;
    }
}