using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Pi.SetMarketData.MigrationProxy.Handlers;

public class HandleExceptionFilter : IAsyncActionFilter
{
    private readonly ILogger<HandleExceptionFilter> _logger;
    private const string _logError = "Error occurred while handling request";
    private const string _statusCodeError = "An error occurred while processing your request.";

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
            _logger.LogError(ex, _logError);

            // Return a 500 status code with the error message
            context.Result = new ObjectResult(_statusCodeError)
            {
                StatusCode = 500
            };
        }
    }
}