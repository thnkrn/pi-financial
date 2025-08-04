using Serilog.Context;

namespace Pi.PortfolioService.API.Middlewares;

public class CustomLoggingContextMiddleware
{
    private readonly RequestDelegate _next;

    public CustomLoggingContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using (LogContext.PushProperty("UserId", context.Request.Headers["user-id"].FirstOrDefault()))
        {
            await _next(context);
        }
    }
}
