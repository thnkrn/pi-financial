using Serilog.Context;

namespace Pi.Financial.FundService.API.Middlewares;

public class CustomLoggingContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomLoggingContextMiddleware> _logger;

    public CustomLoggingContextMiddleware(RequestDelegate next, ILogger<CustomLoggingContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using (LogContext.PushProperty("UserId", context.Request.Headers["user-id"].FirstOrDefault()))
        {
            await _next(context);
        }
    }
}
