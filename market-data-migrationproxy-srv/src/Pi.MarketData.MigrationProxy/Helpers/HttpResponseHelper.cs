using System.Net;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Pi.MarketData.Application.Interfaces;

namespace Pi.MarketData.MigrationProxy.API.Helpers;

public class HttpResponseHelper : IHttpResponseHelper
{
    private readonly Dictionary<string, object> _combinedData = new();
    private readonly Dictionary<string, HashSet<string>> _combinedHeaders = new();
    private HttpStatusCode _overallStatusCode = HttpStatusCode.OK;

    public async Task<HttpResponseMessage> CombineResponses(List<HttpResponseMessage> responses)
    {
        foreach (var response in responses)
        {
            CombineHeaders(response);
            UpdateStatusCode(response);
            if (response.IsSuccessStatusCode) await CombineContent(response);
        }

        return CreateCombinedResponse();
    }

    private void CombineHeaders(HttpResponseMessage response)
    {
        foreach (var (key, values) in response.Headers)
        {
            if (string.Equals(key, "Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
                continue;

            _combinedHeaders.TryAdd(key, new HashSet<string>());
            foreach (var value in values) _combinedHeaders[key].Add(value);
        }
    }

    private void UpdateStatusCode(HttpResponseMessage response)
    {
        _overallStatusCode = response.StatusCode != HttpStatusCode.OK ? response.StatusCode : _overallStatusCode;
    }

    private async Task CombineContent(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        using var jsonDocument = JsonDocument.Parse(content);
        var rootElement = jsonDocument.RootElement;

        if (rootElement.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in rootElement.EnumerateObject())
            {
                if (_combinedData.TryGetValue(property.Name, out var existing))
                {
                    _combinedData[property.Name] = MergeValues(existing, ConvertJsonElementToObject(property.Value));
                }
                else
                {
                    _combinedData[property.Name] = ConvertJsonElementToObject(property.Value);
                }
            }
        }
    }

    private static object ConvertJsonElementToObject(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => element.EnumerateObject()
                .ToDictionary(p => p.Name, p => ConvertJsonElementToObject(p.Value)),
            JsonValueKind.Array => element.EnumerateArray()
                .Select(ConvertJsonElementToObject).ToList(),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => throw new ArgumentException($"Unexpected JSON value kind: {element.ValueKind}")
        };
    }

    private static object MergeValues(object existing, object newValue)
    {
        if (existing is List<object> existingList && newValue is List<object> newList)
        {
            return existingList.Concat(newList).ToList();
        }

        if (existing is Dictionary<string, object> existingDict && 
            newValue is Dictionary<string, object> newDict)
        {
            var result = new Dictionary<string, object>(existingDict);
            foreach (var (key, value) in newDict)
            {
                if (result.TryGetValue(key, out var existingValue))
                {
                    result[key] = MergeValues(existingValue, value);
                }
                else
                {
                    result[key] = value;
                }
            }
            return result;
        }

        return newValue;
    }

    private HttpResponseMessage CreateCombinedResponse()
    {
        var combinedContent = JsonSerializer.Serialize(_combinedData);
        var combinedResponse = new HttpResponseMessage(_overallStatusCode)
        {
            Content = new StringContent(combinedContent, Encoding.UTF8, "application/json")
        };

        foreach (var (key, values) in _combinedHeaders)
        {
            if (!string.Equals(key, "Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
                combinedResponse.Headers.TryAddWithoutValidation(key, values);
        }

        combinedResponse.Headers.Remove("Transfer-Encoding");

        return combinedResponse;
    }

    public static JObject? ParseResponseContent(string content)
    {
        try
        {
            return JObject.Parse(content);
        }
        catch
        {
            return null;
        }
    }
}