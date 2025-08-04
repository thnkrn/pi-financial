using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace Pi.GlobalEquities.API.Middlewares;

public class LogEnricher
{
    private readonly RequestDelegate _next;

    public LogEnricher(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue("correlation-id", out var correlationIdValues)
            ? correlationIdValues.ToString()
            : Guid.NewGuid().ToString();
        var userId = context.Request.Headers.TryGetValue(RequestHeader.UserId, out StringValues userIdValues)
            ? userIdValues.FirstOrDefault()
            : null;
        var queryParameter = context.Request.Query;

        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("QueryParams", queryParameter))
        using (LogContext.PushProperty("CorrelationId", correlationId))
            await _next(context);
    }
}
