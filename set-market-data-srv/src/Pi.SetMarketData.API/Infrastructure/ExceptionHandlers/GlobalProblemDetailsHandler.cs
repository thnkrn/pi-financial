using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pi.SetMarketData.API.Infrastructure.Exceptions;
using Pi.SetMarketData.API.Infrastructure.Mappers;

namespace Pi.SetMarketData.API.Infrastructure.ExceptionHandlers;

public class GlobalProblemDetailsHandler : IExceptionHandler
{
    private const string TraceIdKey = "traceId";

    public static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private readonly ILogger<GlobalProblemDetailsHandler> _logger;

    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    public GlobalProblemDetailsHandler(ILogger<GlobalProblemDetailsHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext? httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext == null) return false;

        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        _logger.LogError(exception, "traceId:{TraceId} detail:{Message}", traceId, exception.Message);

        var (statusCode, type) = GetExceptionDetails(exception);
        var problemDetails = new ProblemDetails
        {
            Type = type,
            Title = null,
            Status = null,
            Detail = exception.InnerException?.Message ?? string.Empty,
            Instance = null,
            Extensions =
            {
                ["code"] = statusCode,
                ["message"] = exception.Message,
                ["title"] = httpContext.Request.Path.Value,
                ["instance"] = ExtractTextInParentheses(exception.Source),
                ["response"] = GetExceptionData(exception)
            }
        };

        var json = JsonSerializer.Serialize(problemDetails, SerializerOptions);

        httpContext.Response.Headers.Append(TraceIdKey, traceId);
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsync(json, cancellationToken);

        return true;
    }

    public static void CustomizeProblemDetails(ProblemDetailsContext ctx)
    {
        var traceId = string.Empty;
        var problemDetail = ctx.ProblemDetails;

        if (problemDetail.Extensions.TryGetValue(TraceIdKey, out var value))
        {
            traceId = value?.ToString() ?? string.Empty;
            problemDetail.Extensions.Remove(TraceIdKey);
        }

        problemDetail.Title = null;
        problemDetail.Instance = null;

        if (problemDetail.Status.HasValue)
        {
            problemDetail.Extensions["code"] = problemDetail.Status;
            problemDetail.Status = null;
        }

        if (!string.IsNullOrEmpty(problemDetail.Detail))
        {
            var (message, detail) = SplitDetail(problemDetail.Detail);
            problemDetail.Extensions["message"] = message;
            problemDetail.Detail = detail;
        }
        else
        {
            problemDetail.Detail = string.Empty;
        }

        problemDetail.Type = MapRfcUrlToExceptionType.Map(problemDetail.Type ?? string.Empty);
        problemDetail.Extensions["title"] = ctx.HttpContext.Request.Path.Value;
        problemDetail.Extensions["instance"] = ExtractTextInParentheses(ctx.HttpContext.GetEndpoint()?.DisplayName);
        problemDetail.Extensions["response"] = null;

        HandleValidationProblemDetails(problemDetail);

        if (problemDetail.Extensions.TryGetValue("code", out var code) &&
            int.TryParse(code?.ToString(), out var status))
            ctx.HttpContext.Response.StatusCode = status;

        ctx.HttpContext.Response.Headers.Append(TraceIdKey, traceId);
    }

    private static (int StatusCode, string Type) GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            BadRequestException => ((int)HttpStatusCode.BadRequest, nameof(BadRequestException)),
            UnauthorizedErrorException => ((int)HttpStatusCode.Unauthorized, nameof(UnauthorizedErrorException)),
            NotFoundException => ((int)HttpStatusCode.NotFound, nameof(NotFoundException)),
            _ => ((int)HttpStatusCode.InternalServerError, nameof(InternalServerErrorException))
        };
    }

    private static object? GetExceptionData(Exception exception)
    {
        return exception.Data.Count > 0 ? exception.Data : null;
    }

    private static void HandleValidationProblemDetails(ProblemDetails problemDetail)
    {
        if (problemDetail is not ValidationProblemDetails validationProblemDetails)
            return;

        var (message, detail) = ExtractMessageAndDetail(validationProblemDetails.Errors);

        validationProblemDetails.Extensions["message"] = message;
        validationProblemDetails.Detail = detail;
        validationProblemDetails.Errors.Clear();
    }

    private static (string Message, string Detail) ExtractMessageAndDetail(IDictionary<string, string[]> errors)
    {
        var messageBuilder = new StringBuilder();
        var detailBuilder = new StringBuilder();
        var isFirstError = true;

        foreach (var errorMessages in errors.Select(error => error.Value).Where(v => v.Length > 0))
        {
            ProcessError(errorMessages, isFirstError, messageBuilder, detailBuilder);
            isFirstError = false;
        }

        return (messageBuilder.ToString(), detailBuilder.ToString());
    }

    private static void ProcessError(IReadOnlyList<string> errorMessages, bool isFirstError,
        StringBuilder messageBuilder, StringBuilder detailBuilder)
    {
        if (isFirstError)
        {
            messageBuilder.Append(errorMessages[0]);
            AppendAdditionalMessages(errorMessages.Skip(1), detailBuilder);
        }
        else
        {
            AppendAdditionalMessages(errorMessages, detailBuilder);
        }
    }

    private static void AppendAdditionalMessages(IEnumerable<string> messages, StringBuilder builder)
    {
        foreach (var message in messages)
        {
            if (builder.Length > 0)
                builder.Append(", ");
            builder.Append(message);
        }
    }

    private static (string Message, string Detail) SplitDetail(string detail)
    {
        if (detail.Contains('|'))
        {
            var parts = detail.Split('|', 2);
            return (parts[0], parts[1]);
        }

        return (string.Empty, detail);
    }

    private static string ExtractTextInParentheses(string? input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            const string pattern = @"\(([^)]*)\)";
            var match = Regex.Match(input, pattern, RegexOptions.None, TimeSpan.FromMilliseconds(500));

            return match.Success ? match.Groups[1].Value : input;
        }

        return string.Empty;
    }
}