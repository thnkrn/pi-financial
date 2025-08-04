using Microsoft.AspNetCore.Mvc.Infrastructure;
using Pi.GlobalEquities.API.Controllers;
using Pi.GlobalEquities.Application.Exceptions;
using Pi.GlobalEquities.Errors;


namespace Pi.GlobalEquities.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly ILogger<CustomProblemDetailsFactory> _problemLogger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, ILoggerFactory loggerFact)
    {
        _next = next;
        _logger = logger;
        _problemLogger = loggerFact.CreateLogger<CustomProblemDetailsFactory>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (GeException ex)
        {
            if (ex.Error == OrderErrors.UserAccessDenied || ex.Error == AccountErrors.NotExist)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(CustomProblemDetailsFactory.CreateValidationResult(_problemLogger,
                    ex.Error, StatusCodes.Status403Forbidden));
            }
            else if (ex.Error == OrderErrors.MarketClose)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(CustomProblemDetailsFactory
                    .CreateProblemDetails(ex.Error, StatusCodes.Status500InternalServerError));
            }
            else if (ex.Error == AccountErrors.InsufficientBalance || ex.Error == PositionErrors.InsufficientHoldings)
            {
                context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
                await context.Response.WriteAsJsonAsync(
                    CustomProblemDetailsFactory.CreateValidationResult(_problemLogger, ex.Error,
                        StatusCodes.Status402PaymentRequired));
            }
            else if (ex.Error == OrderErrors.OrderNotFound)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(
                    CustomProblemDetailsFactory.CreateValidationResult(_problemLogger, ex.Error,
                        StatusCodes.Status404NotFound));
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(
                    CustomProblemDetailsFactory.CreateValidationProblemDetails(_problemLogger, ex.Error));
            }
        }
    }
}
