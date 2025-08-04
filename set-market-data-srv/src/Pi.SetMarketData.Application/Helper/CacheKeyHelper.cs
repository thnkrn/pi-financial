using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Pi.SetMarketData.Application.Helper;

public static class CacheKeyHelper
{
    public static string GenerateCacheKey<T>(T @object)
    {
        string serializedRequest = JsonConvert.SerializeObject(@object);
        return $"{typeof(T).FullName}:{CreateConsistentHash(serializedRequest)}";
    }

    // Create a consistent hash from a string
    public static string CreateConsistentHash(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(inputBytes);

        // Convert to a hex string
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}