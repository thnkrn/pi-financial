using Microsoft.AspNetCore.Mvc.Controllers;
using Pi.GlobalEquities.API.Controllers;

namespace Pi.GlobalEquities.API.Middlewares;

public class RequestBodyLogger
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestBodyLogger> _logger;

    private static readonly HashSet<string> _enabledLogControllerActions =
        new(OrderController.EnabledLogRequestBodyActions, StringComparer.OrdinalIgnoreCase);

    public RequestBodyLogger(RequestDelegate next, ILogger<RequestBodyLogger> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!IsEnabledLogRequestBody(context))
        {
            await _next(context);
            return;
        }

        context.Request.EnableBuffering();

        await _next(context);

        if (context.Response.StatusCode >= 400)
        {
            var originalResponseBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            context.Request.Body.Position = 0;
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            _logger.LogInformation("Request Body: {@RequestBody}", requestBody);

            await responseBody.CopyToAsync(originalResponseBodyStream);
        }
    }

    private bool IsEnabledLogRequestBody(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

        return actionDescriptor != null
               && _enabledLogControllerActions.Contains(actionDescriptor.ActionName);
    }
}
