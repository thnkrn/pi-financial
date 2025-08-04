using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Pi.MarketData.MigrationProxy.API.Handlers;

public class HandleExceptionFilter : IAsyncActionFilter
{
    private const string LogError = "Error occurred while handling request";
    private const string StatusCodeError = "An error occurred while processing your request.";
    private readonly ILogger<HandleExceptionFilter> _logger;

    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    public HandleExceptionFilter(ILogger<HandleExceptionFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            // Proceed with the action method execution
            await next();
        }
        catch (Exception ex)
        {
            // Log the error
            _logger.LogError(ex, LogError);

            // Return a 500 status code with the error message
            context.Result = new ObjectResult(StatusCodeError)
            {
                StatusCode = 500
            };
        }
    }
}