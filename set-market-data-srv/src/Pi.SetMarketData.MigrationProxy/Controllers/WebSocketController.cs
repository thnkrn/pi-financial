using Microsoft.AspNetCore.Mvc;
using Pi.SetMarketData.MigrationProxy.Interfaces;

namespace Pi.SetMarketData.MigrationProxy.Controllers;

[ApiController]
[Route("marketdata")]
public class WebSocketController : ControllerBase
{
    private readonly IWebSocketService _webSocketService;
    public WebSocketController
    (
        IWebSocketService webSocketService
    )
    {
        _webSocketService = webSocketService;
    }

    [Route("streaming")]
    [HttpGet]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _webSocketService.AddClient(webSocket);
            await _webSocketService.HandleWebSocketConnection(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}