using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.MarketData.Application.Helpers;
using Pi.MarketData.Application.Interfaces;
using Pi.MarketData.Domain.ConstantConfigurations;
using Pi.MarketData.Domain.Enums;
using Pi.MarketData.Domain.Models;

namespace Pi.MarketData.Application.Services;

public class ProxyService : IProxyService
{
    private const string TimeFormat = "yyyyMMdd HH:mm:ss.fff";
    private const string OperationCanceledException = "Operation canceled exception.";
    private const string WebSocketException = "WebSocket exception.";
    private const string ObjectDisposedException = "WebSocket was disposed.";

    private static readonly HashSet<string> GeVenue = new(StringComparer.OrdinalIgnoreCase)
        { "ARCA", "BATS", "HKEX", "NASDAQ", "NYSE" };

    private static readonly HashSet<string> SetVenue = new(StringComparer.OrdinalIgnoreCase) { "Equity" };
    private static readonly HashSet<string> TfexVenue = new(StringComparer.OrdinalIgnoreCase) { "Derivative" };
    private readonly IFeatureFlagService _featureFlagService;
    private readonly string _geHubUrl;
    private readonly ILogger<ProxyService> _logger;
    private readonly string _setHubUrl;
    private readonly bool _websocketEnabled;
    private readonly string _websocketUrl;
    private CancellationTokenSource _cts;

    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="featureFlagService"></param>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public ProxyService(
        ILogger<ProxyService> logger,
        IFeatureFlagService featureFlagService,
        IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _featureFlagService = featureFlagService ?? throw new ArgumentNullException(nameof(featureFlagService));

        _setHubUrl = configuration.GetValue<string>(ConfigurationKeys.SetSignalRHubUrl) ??
                     throw new InvalidOperationException(
                         $"{nameof(ConfigurationKeys.SetSignalRHubUrl)} is not configured.");
        _geHubUrl = configuration.GetValue<string>(ConfigurationKeys.GeSignalRHubUrl) ??
                    throw new InvalidOperationException(
                        $"{nameof(ConfigurationKeys.GeSignalRHubUrl)} is not configured.");

        _websocketEnabled = false;
        _websocketUrl = configuration.GetValue<string>(ConfigurationKeys.WebSocketUrl) ??
                        throw new InvalidOperationException(
                            $"{nameof(ConfigurationKeys.WebSocketUrl)} is not configured.");

        _cts = new CancellationTokenSource();
    }

    public async Task SubscribeSignalRHub(WebSocket webSocket, RequestPayload? payload, SignalRHubEndpoint endpoint,
        CancellationToken cancellationToken)
    {
        if (payload == null) return;

        var hubUrl = GetUrlEndpoint(endpoint);

        try
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cts.Token);
            await using var connection = await CreateHubConnection(hubUrl, webSocket, linkedCts.Token);
            _logger.LogDebug("Connected to the SignalR hub {HubUrl}", hubUrl);
            await connection.InvokeAsync("SubscribeToStreamDataAsync", payload, linkedCts.Token);
            if (connection.State == HubConnectionState.Connected)
                await HandleHubMessages(connection, webSocket, linkedCts.Token);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogDebug(ex, OperationCanceledException);
            StopAllSubscriptions();
        }
        catch (WebSocketException ex)
        {
            WebSocketExceptionHandler(ex);
            StopAllSubscriptions();
        }
        catch (ObjectDisposedException ex)
        {
            _logger.LogDebug(ex, ObjectDisposedException);
            StopAllSubscriptions();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to SignalR hub: {HubUrl}", hubUrl);
            StopAllSubscriptions();
        }
    }

    public async Task SubscribeWebsocket(WebSocket webSocketClientOut, RequestPayload? payload,
        CancellationToken cancellationToken)
    {
        if (payload == null) return;

        try
        {
            if (_websocketEnabled)
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cts.Token);
                using var webSocket = new ClientWebSocket();

                await webSocket.ConnectAsync(new Uri(_websocketUrl), linkedCts.Token);

                var message = JsonConvert.SerializeObject(payload);
                var messageBytes = Encoding.UTF8.GetBytes(message);

                _logger.LogDebug(
                    "Connected to WebSocket server {WebsocketUrl} and started to send payload at time: {Time}",
                    _websocketUrl, DateTime.Now.ToString(TimeFormat));

                await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true,
                    linkedCts.Token);

                _logger.LogDebug("Sent payload to subscribe message at time: {Time}",
                    DateTime.Now.ToString(TimeFormat));

                await HandleWebsocketMessages(webSocket, webSocketClientOut, linkedCts.Token);
            }
            else
            {
                await Task.CompletedTask;
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogDebug(ex, OperationCanceledException);
            StopAllSubscriptions();
        }
        catch (WebSocketException ex)
        {
            WebSocketExceptionHandler(ex);
            StopAllSubscriptions();
        }
        catch (ObjectDisposedException ex)
        {
            _logger.LogDebug(ex, ObjectDisposedException);
            StopAllSubscriptions();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to WebSocket");
            StopAllSubscriptions();
        }
    }

    public (RequestPayload? setPayload, RequestPayload? gePayload, RequestPayload? wsPayload)
        DetermineSubscriptionList(RequestPayload? payload)
    {
        if (payload?.Data?.Param == null) return (null, null, null);

        var parameters = payload.Data.Param;
        var flags = GetFeatureFlags();
        var (setPayload, gePayload) = CreateVenuePayloads(payload, parameters, flags);
        var wsPayload = CreateWsPayload(payload, parameters, flags);

        return (setPayload, gePayload, wsPayload);
    }

    private async Task HandleHubMessages(HubConnection connection, WebSocket webSocket,
        CancellationToken cancellationToken)
    {
        try
        {
            await ListenForWebSocket(webSocket, async () =>
            {
                await connection.StopAsync(cancellationToken);
                StopAllSubscriptions();
            }, cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogDebug(ex, OperationCanceledException);
            StopAllSubscriptions();
        }
        catch (WebSocketException ex)
        {
            WebSocketExceptionHandler(ex);
            StopAllSubscriptions();
        }
        catch (ObjectDisposedException ex)
        {
            _logger.LogDebug(ex, ObjectDisposedException);
            StopAllSubscriptions();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling hub messages");
            StopAllSubscriptions();
        }
    }

    private async Task HandleWebsocketMessages(ClientWebSocket webSocket, WebSocket webSocketClientOut,
        CancellationToken cancellationToken)
    {
        try
        {
            var receiveTask = ListenForClientMessages(webSocketClientOut, async () =>
            {
                await CloseWebsocketConnection(webSocket, webSocketClientOut, cancellationToken);
                StopAllSubscriptions();
            }, cancellationToken);

            await ListenForServerMessages(webSocket, webSocketClientOut, cancellationToken);
            await receiveTask;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogDebug(ex, OperationCanceledException);
            StopAllSubscriptions();
        }
        catch (WebSocketException ex)
        {
            WebSocketExceptionHandler(ex);
            StopAllSubscriptions();
        }
        catch (ObjectDisposedException ex)
        {
            _logger.LogDebug(ex, ObjectDisposedException);
            StopAllSubscriptions();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling websocket messages");
            StopAllSubscriptions();
        }
    }

    private void StopAllSubscriptions()
    {
        _cts.Cancel();
        _cts.Dispose();
        _cts = new CancellationTokenSource();
    }

    private async Task ListenForWebSocket(WebSocket webSocket, Func<Task> onClose, CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            try
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
                    break;
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug(ex, OperationCanceledException);
                break;
            }
            catch (WebSocketException ex)
            {
                WebSocketExceptionHandler(ex);
                break;
            }
            catch (ObjectDisposedException ex)
            {
                _logger.LogDebug(ex, ObjectDisposedException);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebSocket closed unexpectedly.");
                break;
            }

        await onClose();
    }

    private async Task ListenForClientMessages(WebSocket webSocketClientOut, Func<Task> onClose,
        CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];
        while (webSocketClientOut.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            try
            {
                var result = await webSocketClientOut.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocketClientOut.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty,
                        cancellationToken);
                    break;
                }

                var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                _logger.LogDebug("[{Time}] [WS>>] Received | {ReceivedMessage}",
                    DateTime.Now.ToString(TimeFormat), receivedMessage);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug(ex, OperationCanceledException);
                break;
            }
            catch (WebSocketException ex)
            {
                WebSocketExceptionHandler(ex);
                break;
            }
            catch (ObjectDisposedException ex)
            {
                _logger.LogDebug(ex, ObjectDisposedException);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in receiving message: {Message}", ex.Message);
                break;
            }

        await onClose();
    }

    private async Task ListenForServerMessages(ClientWebSocket webSocket, WebSocket webSocketClientOut,
        CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            try
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                _logger.LogDebug("[{Time}] [+WS] Got | {ReceivedMessage}",
                    DateTime.Now.ToString(TimeFormat), receivedMessage);

                if (webSocketClientOut.State == WebSocketState.Open)
                {
                    await SendWebSocketMessage(webSocketClientOut, receivedMessage, result.MessageType,
                        result.EndOfMessage, cancellationToken);
                    _logger.LogDebug("[{Time}] [WS>>] Sent | {ReceivedMessage}",
                        DateTime.Now.ToString(TimeFormat), receivedMessage);
                }
                else
                {
                    break;
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug(ex, OperationCanceledException);
                break;
            }
            catch (WebSocketException ex)
            {
                WebSocketExceptionHandler(ex);
                break;
            }
            catch (ObjectDisposedException ex)
            {
                _logger.LogDebug(ex, ObjectDisposedException);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in processing message: {Message}", ex.Message);
                break;
            }

        await CloseWebsocketConnection(webSocket, webSocketClientOut, cancellationToken);
    }

    private static async Task SendWebSocketMessage(WebSocket webSocket, string message,
        WebSocketMessageType messageType = WebSocketMessageType.Text, bool endOfMessage = true,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(message) && webSocket.State == WebSocketState.Open)
            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), messageType,
                endOfMessage, cancellationToken);
    }

    private async Task CloseWebsocketConnection(ClientWebSocket webSocket, WebSocket webSocketClientOut,
        CancellationToken cancellationToken)
    {
        if (webSocketClientOut.State != WebSocketState.Closed && webSocketClientOut.State != WebSocketState.Aborted)
            await CloseWebsocketConnectionHandler(webSocketClientOut, cancellationToken);

        if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
            await CloseWebsocketConnectionHandler(webSocket, cancellationToken);
    }

    private async Task CloseWebsocketConnectionHandler(WebSocket webSocketClientOut,
        CancellationToken cancellationToken)
    {
        try
        {
            await webSocketClientOut.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty,
                cancellationToken);
            _logger.LogDebug("Client disconnected");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogDebug(ex, OperationCanceledException);
        }
        catch (WebSocketException ex)
        {
            WebSocketExceptionHandler(ex);
        }
        catch (ObjectDisposedException ex)
        {
            _logger.LogDebug(ex, ObjectDisposedException);
        }
    }

    private void WebSocketExceptionHandler(WebSocketException ex)
    {
        if (ex.Message.Equals(
                "The remote party closed the WebSocket connection without completing the close handshake.",
                StringComparison.CurrentCultureIgnoreCase))
            _logger.LogDebug(ex, WebSocketException);
        else
            _logger.LogWarning(ex, WebSocketException);
    }

    private async Task<HubConnection> CreateHubConnection(string url, WebSocket webSocket,
        CancellationToken cancellationToken)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl(url)
            .WithAutomaticReconnect([
                TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)
            ])
            .Build();

        connection.On<object>("ReceiveMarketData", async data =>
        {
            var receivedString = data.ToString() ?? "";
            _logger.LogDebug("[+Hub] Got | {Data}", receivedString);

            try
            {
                if (CompressionHelper.IsBase64String(receivedString))
                {
                    try
                    {
                        try
                        {
                            var compressedData = CompressionHelper.FromBase64String(receivedString);
                            var decompressedJson = CompressionHelper.DecompressData(compressedData);

                            if (webSocket.State ==
                                WebSocketState.Open) // Check WebSocket state before attempting to send
                            {
                                await SendWebSocketMessage(webSocket, decompressedJson,
                                    cancellationToken: cancellationToken);
                                _logger.LogDebug("[Hub>>] Sent | {Data}", decompressedJson);
                            }
                            else
                            {
                                _logger.LogWarning("WebSocket is not in Open state. Current state: {State}",
                                    webSocket.State);
                            }
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogError(ex, "Failed to decode JSON string. Attempting to send original string.");
                            if (webSocket.State == WebSocketState.Open)
                                await SendWebSocketMessage(webSocket, receivedString,
                                    cancellationToken: cancellationToken);
                        }
                    }
                    catch (FormatException ex)
                    {
                        // If not Base64/compressed, send the original string
                        _logger.LogError(ex, "Failed to decode Base64 string. Attempting to send original string.");
                        if (webSocket.State == WebSocketState.Open)
                            await SendWebSocketMessage(webSocket, receivedString,
                                cancellationToken: cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to decompress data: {ReceivedString}", receivedString);
                    }
                }
                else
                {
                    // If not Base64/compressed, send the original string
                    if (webSocket.State == WebSocketState.Open)
                        await SendWebSocketMessage(webSocket, receivedString,
                            cancellationToken: cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error processing received data: {ReceivedString}", receivedString);
            }
        });
        await connection.StartAsync(cancellationToken);
        return connection;
    }

    private string GetUrlEndpoint(SignalRHubEndpoint endpoint)
    {
        return endpoint switch
        {
            SignalRHubEndpoint.Set => _setHubUrl,
            SignalRHubEndpoint.Ge => _geHubUrl,
            _ => throw new InvalidOperationException($"Endpoint value was {endpoint}. No valid return value provided.")
        };
    }

    private (bool setFlag, bool geFlag, bool tfexFlag) GetFeatureFlags()
    {
        var setProxyEnabled = _featureFlagService.IsSetWebsocketProxyEnabled();
        var geProxyEnabled = _featureFlagService.IsGeWebsocketProxyEnabled();
        var tfexProxyEnabled = _featureFlagService.IsTfexWebsocketProxyEnabled();

        _logger.LogDebug(
            ">SetFlag: {SetFlag} | GeFlag: {GeFlag} | TfexFlag: {TfexFlag}{NewLine}",
            setProxyEnabled, geProxyEnabled, tfexProxyEnabled, Environment.NewLine);

        if (_websocketEnabled)
            _logger.LogDebug(
                ">SetHubUrl: {SetHubUrl}{NewLine1}>GeHubUrl: {GeHubUrl}{NewLine2}>websocketUrl: {WebSocketHubUrl}{NewLine3}",
                _setHubUrl, Environment.NewLine,
                _geHubUrl, Environment.NewLine, 
                _websocketUrl, Environment.NewLine);
        else
            _logger.LogDebug(
                ">SetHubUrl: {SetHubUrl}{NewLine1}>GeHubUrl: {GeHubUrl}{NewLine2}",
                _setHubUrl, Environment.NewLine,
                _geHubUrl, Environment.NewLine);

        return (
            setFlag: setProxyEnabled,
            geFlag: geProxyEnabled,
            tfexFlag: tfexProxyEnabled
        );
    }

    private static (RequestPayload? setPayload, RequestPayload? gePayload) CreateVenuePayloads
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
            if (item.Market == null || item.Symbol == null) continue;
            if (SetVenue.Contains(item.Market) && flags.SET)
                setPayload.Data.Param.Add(item);
            else if (GeVenue.Contains(item.Market) && flags.GE && !item.Symbol.EndsWith(".INDEX"))
                gePayload.Data.Param.Add(item);
            else if (TfexVenue.Contains(item.Market) && flags.TFEX) setPayload.Data.Param.Add(item);
        }

        return (setPayload, gePayload);
    }

    private static RequestPayload CreateWsPayload(RequestPayload payload, List<Parameter> parameters,
        (bool setFlag, bool geFlag, bool tfexFlag) flags)
    {
        var wsParameters = GetWsParameters(parameters, flags.setFlag, flags.geFlag, flags.tfexFlag);

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

    private static List<Parameter> GetWsParameters(List<Parameter> param, bool setFlag, bool geFlag, bool tfexFlag)
    {
        var wsParameters = new List<Parameter>();
        wsParameters.AddRange(param.Where(item => item.Market != null && !setFlag && item.Market == "Equity")); // SET
        wsParameters.AddRange(param.Where(item =>
            item.Market != null && !geFlag && GeVenue.Contains(item.Market))); // GE
        wsParameters.AddRange(param.Where(item =>
            item.Market != null && !tfexFlag && item.Market == "Derivative")); // TFEX
        wsParameters.AddRange(param.Where(item =>
            item.Market != null &&
            GeVenue.Contains(item.Market) &&
            item.Symbol != null &&
            item.Symbol.EndsWith(".INDEX"))); // Symbol with "*.INDEX"
        wsParameters.AddRange(param.Where(item =>
            item.Market != null &&
            !GeVenue.Contains(item.Market) &&
            item.Market != "Equity" &&
            item.Market != "Derivative"));
        return wsParameters;
    }
}