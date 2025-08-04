using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Pi.GlobalEquities.Errors;

namespace Pi.GlobalEquities.API.Models;

public class CustomProblemDetailsFactory : ProblemDetailsFactory
{
    private readonly ILogger<CustomProblemDetailsFactory> _logger;

    public CustomProblemDetailsFactory(ILogger<CustomProblemDetailsFactory> logger)
    {
        _logger = logger;
    }

    public override ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string title = null,
        string type = null,
        string detail = null,
        string instance = null)
    {
        return InternalCreateProblemDetails(
            httpContext: httpContext,
            statusCode: statusCode,
            title: title,
            type: type,
            detail: detail,
            instance: instance);
    }

    private static ProblemDetails InternalCreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string title = null,
        string type = null,
        string detail = null,
        string instance = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode ?? (int)HttpStatusCode.InternalServerError,
            Title = title ?? "An error occurred while processing your request",
            Type = type ?? "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Detail = detail,
            Instance = instance
        };

        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext?.TraceIdentifier;

        return problemDetails;
    }

    public override ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ModelStateDictionary modelStateDictionary,
        int? statusCode = null,
        string title = null,
        string type = null,
        string detail = null,
        string instance = null)
    {
        return InternalCreateValidationProblemDetails(
            _logger,
            httpContext: httpContext,
            modelStateDictionary: modelStateDictionary,
            statusCode: statusCode,
            title: title,
            type: type,
            detail: detail,
            instance: instance);
    }

    private static ValidationProblemDetails InternalCreateValidationProblemDetails(
        ILogger<CustomProblemDetailsFactory> logger,
        HttpContext httpContext,
        ModelStateDictionary modelStateDictionary,
        int? statusCode = null,
        string title = null,
        string type = null,
        string detail = null,
        string instance = null)
    {
        var vpd = new ValidationProblemDetails(modelStateDictionary ?? new())
        {
            Status = statusCode ?? (int)HttpStatusCode.BadRequest,
            Title = title ?? "One or more validation errors occurred.",
            Type = type ?? "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Detail = detail,
            Instance = instance
        };

        vpd.Extensions["traceId"] = Activity.Current?.Id ?? httpContext?.TraceIdentifier;

        logger.LogError("Validation error, " +
            "Detail: {Detail}, Title: {Title}, Status: {Status}, Errors: {@Errors}, TraceId: {TraceId}",
            vpd.Detail, vpd.Title, vpd.Status, vpd.Errors, vpd.Extensions["traceId"]);

        return vpd;
    }

    public static PiProblemResult CreateProblemDetails(
        Error error,
        int? statusCode = null)
    {
        var pd = InternalCreateProblemDetails(
            httpContext: null,
            statusCode: statusCode,
            title: error.Code.ToString(),
            detail: error.Description);

        return new PiProblemResult(pd);
    }

    public static ValidationProblemDetails CreateValidationProblemDetails(
        ILogger<CustomProblemDetailsFactory> logger,
        Error error,
        int? statusCode = null)
    {
        return InternalCreateValidationProblemDetails(
            logger,
            httpContext: null,
            modelStateDictionary: null,
            statusCode: statusCode,
            title: error.Code.ToString(),
            detail: error.Description);
    }

    public static PiValidationResult CreateValidationResult(
        ILogger<CustomProblemDetailsFactory> logger,
        Error error,
        int? statusCode = null)
    {
        var pd = InternalCreateValidationProblemDetails(
            logger,
            httpContext: null,
            modelStateDictionary: null,
            statusCode: statusCode,
            title: error.Code.ToString(),
            detail: error.Description);

        return new PiValidationResult(pd);
    }
}

public class PiValidationResult : ValidationProblemDetails
{
    public PiValidationResult(ProblemDetails pd)
    {
        Title = pd.Title;
        Status = pd.Status;
        Detail = pd.Detail;
        Instance = pd.Instance;
        Type = pd.Type;

        foreach (var kvp in pd.Extensions)
        {
            Extensions[kvp.Key] = kvp.Value;
        }
    }

    public static implicit operator ObjectResult(PiValidationResult result)
    {
        return new ObjectResult(result) { StatusCode = result.Status };
    }

    public ObjectResult ToObjectResult() => this;
}

public class PiProblemResult : ProblemDetails
{
    public PiProblemResult(ProblemDetails pd)
    {
        Title = pd.Title;
        Status = pd.Status;
        Detail = pd.Detail;
        Instance = pd.Instance;
        Type = pd.Type;

        foreach (var kvp in pd.Extensions)
        {
            Extensions[kvp.Key] = kvp.Value;
        }
    }

    public static implicit operator ObjectResult(PiProblemResult result)
    {
        return new ObjectResult(result) { StatusCode = result.Status };
    }

    public ObjectResult ToObjectResult() => this;
}
