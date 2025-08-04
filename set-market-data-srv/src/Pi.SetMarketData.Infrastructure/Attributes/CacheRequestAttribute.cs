using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.SetMarketData.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CacheRequestAttribute : Attribute
{
    private readonly TimeSpan _ttl;
    public CacheRequestAttribute(int seconds = 300)
    {
        _ttl = TimeSpan.FromSeconds(seconds);
    }

    public static async Task<TResponse> ExecuteWithCaching<TRequest, TResponse>(
        Func<TRequest, CancellationToken, Task<TResponse>> originalMethod,
        TRequest request,
        CancellationToken cancellationToken,
        IServiceProvider serviceProvider)
    {
        var cacheService = serviceProvider.GetRequiredService<ICacheService>();

        // Generate a cache key from the request data
        string cacheKey = GenerateCacheKey(request);

        // Check if we have a valid cached response
        var cache = await cacheService.GetAsync<TResponse>(cacheKey);
        if (!EqualityComparer<TResponse>.Default.Equals(cache, default(TResponse)))
        {
            return cache;
        }

        // Execute the original method
        var response = await originalMethod(request, cancellationToken);

        // Get the TTL value from the attribute applied to the method
        var ttl = GetTtlFromCallingMethod();

        // Cache the response
        await cacheService.SetAsync(cacheKey, response, ttl);

        return response;
    }

    private static TimeSpan GetTtlFromCallingMethod()
    {
        // Get the calling method
        var stackFrame = new System.Diagnostics.StackFrame(2);
        var method = stackFrame.GetMethod();

        // Get the CacheRequestAttribute from the method
        var attribute = method.GetCustomAttribute<CacheRequestAttribute>();

        return attribute?._ttl ?? TimeSpan.FromMinutes(1); // Default to 1 minutes if not found
    }

    public static string GenerateCacheKey<T>(T request)
    {
        // Extract Data property from the request if it exists
        PropertyInfo dataProperty = typeof(T).GetProperty("Data");
        if (dataProperty != null)
        {
            var data = dataProperty.GetValue(request);
            if (data != null)
            {
                // Use serialized JSON of the Data object for a consistent cache key
                // with customized serialization options to ensure consistency
                var options = new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };

                string serializedData = JsonSerializer.Serialize(data, options);

                // Create a content-based hash that will be the same for identical content
                return $"{typeof(T).FullName}:{CreateConsistentHash(serializedData)}";
            }
        }

        // Fallback to serializing the entire request object
        string serializedRequest = JsonSerializer.Serialize(request);
        return $"{typeof(T).FullName}:{CreateConsistentHash(serializedRequest)}";
    }

    // Create a consistent hash from a string
    private static string CreateConsistentHash(string input)
    {
        using (var sha = System.Security.Cryptography.SHA256.Create())
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hashBytes = sha.ComputeHash(inputBytes);

            // Convert to a hex string
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}