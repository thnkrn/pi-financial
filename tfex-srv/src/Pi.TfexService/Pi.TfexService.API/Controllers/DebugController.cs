using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.TfexService.API.Models.Order;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.SetTrade;

namespace Pi.TfexService.API.Controllers;

[ApiController]
[Route("debug")]
public class DebugController(ISetTradeService setTradeService, IBus bus) : ControllerBase
{
    [HttpGet("settrade/auth-token")]
    public async Task<IActionResult> GetAuthToken()
    {
        var resp = await setTradeService.GetAccessToken();
        return Ok(resp);
    }

    [HttpGet("settrade/{accountNo}/orders")]
    public async Task<IActionResult> GetOrder([Required] string accountNo,
        [FromQuery] string sort,
        [FromQuery] Side? side,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        CancellationToken cancellationToken = default)
    {
        var timer = new Stopwatch();
        timer.Start();
        var resp = await setTradeService.GetOrders(accountNo, 1, int.MaxValue, sort, side, dateFrom, dateTo, cancellationToken);
        timer.Stop();
        return Ok(new { timer.ElapsedMilliseconds, resp.Orders.Count, resp });
    }

    [HttpGet("settrade/{accountNo}/orders/{orderNo}")]
    public async Task<IActionResult> GetOrderNo([Required] string accountNo, [Required] long orderNo,
        CancellationToken cancellationToken = default)
    {
        var resp = await setTradeService.GetOrderByNo(accountNo, orderNo, cancellationToken);
        return Ok(resp);
    }

    [HttpGet("settrade/{accountNo}/trades")]
    public async Task<IActionResult> GetTrades([Required] string accountNo, string? sort,
        CancellationToken cancellationToken = default)
    {
        var resp = await setTradeService.GetTrades(accountNo, sort ?? "orderNo:desc", cancellationToken);
        return Ok(resp);
    }

    [HttpGet("settrade/{accountNo}/account-info")]
    public async Task<IActionResult> GetAccountInfo([Required] string accountNo,
        CancellationToken cancellationToken = default)
    {
        var resp = await setTradeService.GetAccountInfo(accountNo, cancellationToken);
        return Ok(resp);
    }

    [HttpGet("settrade/{accountNo}/portfolio")]
    public async Task<IActionResult> GetPortfolio([Required] string accountNo,
        CancellationToken cancellationToken = default)
    {
        var resp = await setTradeService.GetPortfolio(accountNo, cancellationToken);
        return Ok(resp);
    }

    [HttpPost("settrade/test-notification")]
    public async Task<IActionResult> TestNotification(
        [FromBody] MockSetTradeOrderStatus body,
        CancellationToken cancellationToken)
    {
        await bus.Publish(new SetTradeOrderStatus(
            body.OrderNo,
            body.AccountNo,
            body.SeriesId,
            (SetTradeListenerOrderEnum.Side)Enum.Parse(typeof(SetTradeListenerOrderEnum.Side), body.Side),
            body.Price ?? 0,
            body.Volume ?? 0,
            body.BalanceVolume ?? 0,
            body.MatchedVolume ?? 0,
            body.CancelledVolume ?? 0,
            body.Status
            ), cancellationToken);
        return Ok();
    }
}