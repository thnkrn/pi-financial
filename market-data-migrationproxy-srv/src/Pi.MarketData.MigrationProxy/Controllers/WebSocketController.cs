using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using Pi.MarketData.Application.Interfaces;
using Pi.MarketDataWSS.Domain.Models.Request;
using Pi.SMarketDataWSS.Domain.Models.Response;
using Swashbuckle.AspNetCore.Annotations;

namespace Pi.MarketData.MigrationProxy.API.Controllers;

[ApiController]
[Route("market-data")]
[Route("marketdata-migrationproxy/market-data")]
public class WebSocketController : ControllerBase
{
    private const string WebSocketException = "WebSocket exception.";
    private const string ObjectDisposedException = "WebSocket was disposed.";
    private readonly ILogger<WebSocketController> _logger;
    private readonly IWebSocketService _webSocketService;

    /// <summary>
    /// </summary>
    /// <param name="webSocketService"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public WebSocketController(
        IWebSocketService webSocketService,
        ILogger<WebSocketController> logger)
    {
        _webSocketService = webSocketService ?? throw new ArgumentNullException(nameof(webSocketService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Route("streaming")]
    [HttpGet]
    [SwaggerOperation(Summary = "WebSocket Endpoint for Market Data",
        Description =
            "Use this endpoint to establish a WebSocket connection and use following request body as a subscription request message over SignalR.")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketStreamingResponse)
    )]
    public async Task Get(
        [FromBody] [SwaggerParameter("Example message to request subscription.")]
        MarketStreamingRequest? optionalParam = null)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            try
            {
                _webSocketService.AddClient(webSocket);
                await _webSocketService.HandleWebSocketConnection(webSocket);
            }
            catch (WebSocketException ex)
            {
                if (ex.Message.Equals(
                        "The remote party closed the WebSocket connection without completing the close handshake.",
                        StringComparison.CurrentCultureIgnoreCase))
                    _logger.LogDebug(ex, WebSocketException);
                else
                    _logger.LogWarning(ex, WebSocketException);

                await CloseWebSocketWithError(webSocket, "A WebSocket error occurred");
            }
            catch (ObjectDisposedException ex)
            {
                _logger.LogDebug(ex, ObjectDisposedException);
                await CloseWebSocketWithError(webSocket, "A WebSocket error occurred");
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug(ex, "WebSocket operation was cancelled");
                await CloseWebSocketWithError(webSocket, "Operation cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error handling WebSocket connection");
                await CloseWebSocketWithError(webSocket, "An unexpected error occurred");
            }
            finally
            {
                _webSocketService.RemoveClient(webSocket);
            }
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private static async Task CloseWebSocketWithError(WebSocket webSocket, string message)
    {
        if (webSocket.State == WebSocketState.Open)
            await webSocket.CloseAsync(
                WebSocketCloseStatus.InternalServerError,
                message,
                CancellationToken.None);
    }
}