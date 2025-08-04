using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Pi.GlobalMarketData.API.Attributes;

/// <summary>
/// Attribute to enable caching for API endpoints
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CacheAttribute : Attribute, IAsyncActionFilter
{
    /// <summary>
    /// Duration to cache the response in seconds
    /// </summary>
    public int Duration { get; set; } = 60;

    /// <summary>
    /// Async method to implement caching logic
    /// </summary>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Determine the actual return type of the method
        var returnType = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo.ReturnType;

        // If the return type is Task<T>, extract the generic type
        if (returnType?.IsGenericType == true && returnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            returnType = returnType.GetGenericArguments()[0];
        }

        // If the return type is IActionResult or similar, use object
        if (returnType == typeof(IActionResult) || returnType == typeof(ActionResult) ||
            returnType == typeof(Task<IActionResult>) || returnType == typeof(Task<ActionResult>))
        {
            returnType = typeof(object);
        }

        // Use reflection to call the generic GetAsync method
        var cacheService = context.HttpContext.RequestServices.GetService(typeof(ICacheService)) as ICacheService;

        // Generate a unique cache key based on request parameters
        var cacheKey = GenerateCacheKey(context);

        // Use reflection to call GetAsync with the correct generic type
        var getMethod = typeof(ICacheService).GetMethod(nameof(ICacheService.GetAsync))
            ?.MakeGenericMethod(returnType);

        var cachedResponse = await (Task<object>)getMethod.Invoke(cacheService, [cacheKey]);
        var response = (JObject)cachedResponse;

        if (cachedResponse != null)
        {
            // Return the cached response as JSON
            context.Result = new ContentResult
            {
                Content = response["Value"]?.ToString(),
                ContentType = "application/json",
                StatusCode = 200
            };
            return;
        }

        // If no cached response, proceed with the request
        var executedContext = await next();

        // Only cache successful responses
        if (executedContext.Result is ObjectResult successResult && successResult.StatusCode == 200)
        {
            await cacheService.SetAsync(cacheKey, successResult, TimeSpan.FromSeconds(Duration));
        }
    }

    /// <summary>
    /// Generate a unique cache key based on request context
    /// </summary>
    private static string GenerateCacheKey(ActionExecutingContext context)
    {
        // Start with the controller and action names
        var keyParts = new List<string>
        {
            context.HttpContext.Request.Method,
            // context.Controller.GetType().FullName ?? "",
            context.HttpContext.Request.Path.ToString()
        };

        // Add query parameters
        var queryParams = context.HttpContext.Request.Query
            .OrderBy(q => q.Key)
            .Select(q => $"{q.Key}:{q.Value}");
        keyParts.AddRange(queryParams);

        // If it's a POST/PUT request, add request body
        if (HttpMethods.IsPost(context.HttpContext.Request.Method) ||
            HttpMethods.IsPut(context.HttpContext.Request.Method))
        {
            string body = TryGetRequestBody(context);

            // Add body hash if not empty
            if (!string.IsNullOrEmpty(body))
                keyParts.Add(CreateConsistentHash(body));
        }

        // Combine all parts and create a hash
        var fullKey = string.Join(":", keyParts);
        return fullKey;
    }

    private static string TryGetRequestBody(ActionExecutingContext context)
    {
        var actionArguments = context.ActionArguments;
        if (actionArguments != null && actionArguments.Count > 0)
        {
            // Try to serialize the first non-null argument
            var firstArg = actionArguments.FirstOrDefault(arg => arg.Value != null);
            if (firstArg.Value != null)
            {
                try
                {
                    return JsonConvert.SerializeObject(firstArg.Value);
                }
                catch
                {
                    return "";
                }
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// Create a consistent hash from a string
    /// </summary>
    private static string CreateConsistentHash(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(inputBytes);

        // Convert to a hex string
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}