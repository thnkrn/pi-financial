using MassTransit;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Pi.Common.ExtensionMethods;
using Pi.SetService.Application.Exceptions;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;

namespace Pi.SetService.API.Middlewares;

public class ErrorResponseMiddleware(RequestDelegate next, ILogger<ErrorResponseMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (SetException e)
        {
            logger.LogError(e, "SetException on {RequestPath} endpoint", context.Request.Path);
            var problemDetailsFactory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();

            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            await context.Response.WriteAsJsonAsync(problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status422UnprocessableEntity,
                title: e.Code.ToString(),
                detail: e.Message));
        }
        catch (RequestFaultException exception)
        {
            logger.LogError(exception, "RequestFaultException on {RequestPath} endpoint", context.Request.Path);

            var setException =
                exception.Fault?.Exceptions.FirstOrDefault(e =>
                    e.ExceptionType.Equals(typeof(SetException).ToString()));
            var errorCodeStr = setException?.Data?["Code"];
            if (errorCodeStr == null || !Enum.TryParse((string)errorCodeStr, out SetErrorCode errorCode))
            {
                throw;
            }

            var problemDetailsFactory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            await context.Response.WriteAsJsonAsync(problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status422UnprocessableEntity,
                title: errorCode.ToString(),
                detail: setException?.Message ?? errorCode.GetEnumDescription()));
        }
        catch (Exception e)
        {
            logger.LogError(e, "{Exception} on {RequestPath} endpoint", nameof(e), context.Request.Path);
            throw;
        }
    }
}
