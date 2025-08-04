using Microsoft.AspNetCore.Mvc.Infrastructure;
using Pi.BackofficeService.Application.Exceptions;

namespace Pi.BackofficeService.API.Middlewares;

public class ErrorResponseMiddleware(RequestDelegate next, ILogger<ErrorResponseMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ExternalApiException e)
        {
            logger.LogError(e, "{Exception} on {RequestPath} endpoint", nameof(e), context.Request.Path);
            var problemDetailsFactory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();

            context.Response.StatusCode = e.Status;
            await context.Response.WriteAsJsonAsync(problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode: e.Status,
                title: e.Title,
                detail: e.Detail));
        }
        catch (Exception e)
        {
            logger.LogError(e, "{Exception} on {RequestPath} endpoint", nameof(e), context.Request.Path);
            throw;
        }
    }
}
