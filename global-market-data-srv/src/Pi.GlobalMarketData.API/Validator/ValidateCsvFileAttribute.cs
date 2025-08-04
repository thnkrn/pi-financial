using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Pi.GlobalMarketData.API.Validator;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ValidateCsvFileAttribute : ActionFilterAttribute
{
    private readonly string _parameterName;
    private readonly string[] _allowedExtensions;

    public ValidateCsvFileAttribute(string parameterName = "file", string[]? allowedExtensions = null)
    {
        _parameterName = parameterName;
        _allowedExtensions = allowedExtensions ?? [".csv"];
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ActionArguments.TryGetValue(_parameterName, out var fileObj) || 
            fileObj is not IFormFile file)
        {
            context.Result = new BadRequestObjectResult(
                new ProblemDetails { 
                    Status = 400, 
                    Title = $"{_parameterName} is required" 
                });
            return;
        }

        if (file.Length == 0)
        {
            context.Result = new BadRequestObjectResult(
                new ProblemDetails { 
                    Status = 400, 
                    Title = $"{_parameterName} is empty" 
                });
            return;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            context.Result = new BadRequestObjectResult(
                new ProblemDetails { 
                    Status = 400, 
                    Title = $"Only {string.Join(", ", _allowedExtensions)} files are supported" 
                });
            return;
        }

        base.OnActionExecuting(context);
    }
}