using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pi.MarketData.Infrastructure.Middleware;
public class SwaggerRedirectMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SwaggerRedirectMiddleware> _logger;
    public SwaggerRedirectMiddleware(RequestDelegate next, ILogger<SwaggerRedirectMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    /// <summary>
    /// Middleware to redirect swagger destination of staging and above to point the valid swagger.json path
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
     public async Task Invoke(HttpContext context)
    {
       var originalPath = context.Request.Path.Value;
        Console.WriteLine($"Request to : {originalPath}");
        Console.WriteLine($"Path base : {context.Request.PathBase}");
        Console.WriteLine($"Path : {context.Request.Path}");
        // if (originalPath.StartsWith("v1/swagger"))
        // {
        //     var newPath = originalPath.Replace("v1/swagger", "marketdata-migrationproxy/v1/swagger");
        //     context.Response.Redirect(newPath, permanent: false);
        //     Console.WriteLine($"Redirected to {newPath}");
        //     return;
        // }

        await _next(context);
    }
}
