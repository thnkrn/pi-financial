using System.Net;
using System.Text.Json;
using Pi.SetMarketData.MigrationProxy.Interfaces;

namespace Pi.SetMarketData.MigrationProxy.Helpers;

public class HttpResponseHelper : IHttpResponseHelper
{
    private readonly Dictionary<string, JsonElement> _combinedData = new();
    private readonly Dictionary<string, HashSet<string>> _combinedHeaders = new();
    private HttpStatusCode _overallStatusCode = HttpStatusCode.OK;

    public async Task<HttpResponseMessage> CombineResponses(List<HttpResponseMessage> responses)
    {
        foreach (var response in responses)
        {
            CombineHeaders(response);
            UpdateStatusCode(response);
            if (response.IsSuccessStatusCode)
            {
                await CombineContent(response);
            }
        }

        return CreateCombinedResponse();
    }

    private void CombineHeaders(HttpResponseMessage response)
    {
        foreach (var (key, values) in response.Headers)
        {
            _combinedHeaders.TryAdd(key, new HashSet<string>());
            foreach (var value in values)
            {
                _combinedHeaders[key].Add(value);
            }
        }
    }

    private void UpdateStatusCode(HttpResponseMessage response) =>
        _overallStatusCode = response.StatusCode != HttpStatusCode.OK ? response.StatusCode : _overallStatusCode;

    private async Task CombineContent(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        using var jsonDocument = JsonDocument.Parse(content);
        var rootElement = jsonDocument.RootElement;

        if (rootElement.ValueKind == JsonValueKind.Object)
        {
            CombineJsonObjects(rootElement);
        }
    }

    private void CombineJsonObjects(JsonElement rootElement)
    {
        foreach (var property in rootElement.EnumerateObject())
        {
            _combinedData[property.Name] = _combinedData.TryGetValue(property.Name, out var existing)
                ? MergeJsonElements(existing, property.Value)
                : property.Value.Clone();
        }
    }

    private static JsonElement MergeJsonElements(JsonElement existing, JsonElement newElement) =>
        (existing.ValueKind, newElement.ValueKind) switch
        {
            (JsonValueKind.Array, JsonValueKind.Array) => MergeArrays(existing, newElement),
            (JsonValueKind.Object, JsonValueKind.Object) => MergeObjects(existing, newElement),
            _ => newElement
        };

    private static JsonElement MergeArrays(JsonElement existing, JsonElement newElement)
    {
        var mergedArray = existing.EnumerateArray().Concat(newElement.EnumerateArray()).ToList();
        return JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(mergedArray));
    }

    private static JsonElement MergeObjects(JsonElement existing, JsonElement newElement)
    {
        var mergedObject = existing.EnumerateObject()
            .ToDictionary(p => p.Name, p => p.Value);

        foreach (var property in newElement.EnumerateObject())
        {
            mergedObject[property.Name] = mergedObject.TryGetValue(property.Name, out var existingValue)
                ? MergeJsonElements(existingValue, property.Value)
                : property.Value;
        }

        return JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(mergedObject));
    }

    private HttpResponseMessage CreateCombinedResponse()
    {
        var combinedContent = JsonSerializer.Serialize(_combinedData);
        var combinedResponse = new HttpResponseMessage(_overallStatusCode)
        {
            Content = new StringContent(combinedContent, System.Text.Encoding.UTF8, "application/json")
        };

        foreach (var (key, values) in _combinedHeaders)
        {
            combinedResponse.Headers.TryAddWithoutValidation(key, values);
        }

        return combinedResponse;
    }
}