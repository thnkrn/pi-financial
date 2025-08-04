using System.Net;
using System.Text;
using System.Text.Json;
using Pi.SetMarketData.MigrationProxy.Helpers;

namespace Pi.SetMarketData.MigrationProxy.Tests;

public class HttpResponseHelperTests
{
    [Fact]
    public async Task CombineResponses_CombinesHeadersCorrectly()
    {
        // Arrange
        var helper = new HttpResponseHelper();
        var responses = new List<HttpResponseMessage>
        {
            new HttpResponseMessage
            {
                Headers = { { "Header1", "Value1" } },
                Content = new StringContent("{\"key1\": \"value1\"}", Encoding.UTF8, "application/json")
            },
            new HttpResponseMessage
            {
                Headers = { { "Header2", "Value2" } },
                Content = new StringContent("{\"key2\": \"value2\"}", Encoding.UTF8, "application/json")
            }
        };

        // Act
        var result = await helper.CombineResponses(responses);

        // Assert
        Assert.True(result.Headers.Contains("Header1"));
        Assert.True(result.Headers.Contains("Header2"));
        Assert.Equal("Value1", result.Headers.GetValues("Header1").First());
        Assert.Equal("Value2", result.Headers.GetValues("Header2").First());
    }

    [Fact]
    public async Task CombineResponses_UpdatesStatusCodeCorrectly()
    {
        // Arrange
        var helper = new HttpResponseHelper();
        var responses = new List<HttpResponseMessage>
        {
            new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"key1\": \"value1\"}", Encoding.UTF8, "application/json")
            },
            new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{\"key2\": \"value2\"}", Encoding.UTF8, "application/json")
            }
        };

        // Act
        var result = await helper.CombineResponses(responses);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task CombineResponses_CombinesContentCorrectly()
    {
        // Arrange
        var helper = new HttpResponseHelper();
        var responses = new List<HttpResponseMessage>
        {
            new HttpResponseMessage
            {
                Content = new StringContent("{\"key1\": \"value1\", \"array\": [1]}", Encoding.UTF8, "application/json"),
                StatusCode = HttpStatusCode.OK
            },
            new HttpResponseMessage
            {
                Content = new StringContent("{\"key2\": \"value2\", \"array\": [2]}", Encoding.UTF8, "application/json"),
                StatusCode = HttpStatusCode.OK
            }
        };

        // Act
        var result = await helper.CombineResponses(responses);

        // Assert
        var content = await result.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content).RootElement;
        Assert.Equal("value1", json.GetProperty("key1").GetString());
        Assert.Equal("value2", json.GetProperty("key2").GetString());
        Assert.Equal(2, json.GetProperty("array").GetArrayLength());
        Assert.Equal(1, json.GetProperty("array")[0].GetInt32());
        Assert.Equal(2, json.GetProperty("array")[1].GetInt32());
    }

    [Fact]
    public async Task CombineResponses_HandlesNonSuccessStatusCodesCorrectly()
    {
        // Arrange
        var helper = new HttpResponseHelper();
        var responses = new List<HttpResponseMessage>
            {
                new HttpResponseMessage
                {
                    Content = new StringContent("{\"key1\": \"value1\"}", Encoding.UTF8, "application/json"),
                    StatusCode = HttpStatusCode.OK
                },
                new HttpResponseMessage
                {
                    Content = new StringContent("{\"error\": \"Bad Request\"}", Encoding.UTF8, "application/json"),
                    StatusCode = HttpStatusCode.BadRequest
                }
            };

        // Act
        var result = await helper.CombineResponses(responses);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        var content = await result.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content).RootElement;
        Assert.Equal("value1", json.GetProperty("key1").GetString());
        Assert.False(json.TryGetProperty("error", out _));
    }

    [Fact]
    public async Task CombineResponses_HandlesDuplicateHeadersCorrectly()
    {
        // Arrange
        var helper = new HttpResponseHelper();
        var responses = new List<HttpResponseMessage>
            {
                new HttpResponseMessage
                {
                    Headers = {
                        { "CommonHeader", "Value1" },
                        { "UniqueHeader1", "UniqueValue1" }
                    },
                    Content = new StringContent("{\"key1\": \"value1\"}", Encoding.UTF8, "application/json")
                },
                new HttpResponseMessage
                {
                    Headers = {
                        { "CommonHeader", "Value2" },
                        { "UniqueHeader2", "UniqueValue2" }
                    },
                    Content = new StringContent("{\"key2\": \"value2\"}", Encoding.UTF8, "application/json")
                }
            };

        // Act
        var result = await helper.CombineResponses(responses);

        // Assert
        Assert.True(result.Headers.Contains("CommonHeader"));
        Assert.True(result.Headers.Contains("UniqueHeader1"));
        Assert.True(result.Headers.Contains("UniqueHeader2"));

        var commonHeaderValues = result.Headers.GetValues("CommonHeader").ToList();
        Assert.Equal(2, commonHeaderValues.Count);
        Assert.Contains("Value1", commonHeaderValues);
        Assert.Contains("Value2", commonHeaderValues);

        Assert.Single(result.Headers.GetValues("UniqueHeader1"));
        Assert.Single(result.Headers.GetValues("UniqueHeader2"));
        Assert.Equal("UniqueValue1", result.Headers.GetValues("UniqueHeader1").First());
        Assert.Equal("UniqueValue2", result.Headers.GetValues("UniqueHeader2").First());
    }

    [Fact]
    public async Task CombineResponses_RemovesDuplicateValuesInHeaders()
    {
        // Arrange
        var helper = new HttpResponseHelper();
        var responses = new List<HttpResponseMessage>
            {
                new HttpResponseMessage
                {
                    Headers = { { "TestHeader", new[] { "Value1", "Value2" } } },
                    Content = new StringContent("{\"key1\": \"value1\"}", Encoding.UTF8, "application/json")
                },
                new HttpResponseMessage
                {
                    Headers = { { "TestHeader", new[] { "Value2", "Value3" } } },
                    Content = new StringContent("{\"key2\": \"value2\"}", Encoding.UTF8, "application/json")
                }
            };

        // Act
        var result = await helper.CombineResponses(responses);

        // Assert
        Assert.True(result.Headers.Contains("TestHeader"));
        var headerValues = result.Headers.GetValues("TestHeader").ToList();
        Assert.Equal(3, headerValues.Count);
        Assert.Contains("Value1", headerValues);
        Assert.Contains("Value2", headerValues);
        Assert.Contains("Value3", headerValues);
    }

    [Fact]
    public async Task CombineResponses_MergesArrayHeaderValues()
    {
        // Arrange
        var helper = new HttpResponseHelper();
        var responses = new List<HttpResponseMessage>
            {
                new HttpResponseMessage
                {
                    Headers = {
                        { "ArrayHeader", new[] { "Value1", "Value2" } },
                        { "SingleValueHeader", "SingleValue1" }
                    },
                    Content = new StringContent("{\"key1\": \"value1\"}", Encoding.UTF8, "application/json")
                },
                new HttpResponseMessage
                {
                    Headers = {
                        { "ArrayHeader", new[] { "Value2", "Value3" } },
                        { "SingleValueHeader", "SingleValue2" }
                    },
                    Content = new StringContent("{\"key2\": \"value2\"}", Encoding.UTF8, "application/json")
                }
            };

        // Act
        var result = await helper.CombineResponses(responses);

        // Assert
        Assert.True(result.Headers.Contains("ArrayHeader"));
        Assert.True(result.Headers.Contains("SingleValueHeader"));

        var arrayHeaderValues = result.Headers.GetValues("ArrayHeader").ToList();
        Assert.Equal(3, arrayHeaderValues.Count);
        Assert.Contains("Value1", arrayHeaderValues);
        Assert.Contains("Value2", arrayHeaderValues);
        Assert.Contains("Value3", arrayHeaderValues);

        var singleValueHeaderValues = result.Headers.GetValues("SingleValueHeader").ToList();
        Assert.Equal(2, singleValueHeaderValues.Count);
        Assert.Contains("SingleValue1", singleValueHeaderValues);
        Assert.Contains("SingleValue2", singleValueHeaderValues);
    }

    [Fact]
    public async Task CombineResponses_HandlesArrayAndSingleValueMixedHeaders()
    {
        // Arrange
        var helper = new HttpResponseHelper();
        var responses = new List<HttpResponseMessage>
            {
                new HttpResponseMessage
                {
                    Headers = { { "MixedHeader", "SingleValue" } },
                    Content = new StringContent("{\"key1\": \"value1\"}", Encoding.UTF8, "application/json")
                },
                new HttpResponseMessage
                {
                    Headers = { { "MixedHeader", new[] { "ArrayValue1", "ArrayValue2" } } },
                    Content = new StringContent("{\"key2\": \"value2\"}", Encoding.UTF8, "application/json")
                }
            };

        // Act
        var result = await helper.CombineResponses(responses);

        // Assert
        Assert.True(result.Headers.Contains("MixedHeader"));
        var mixedHeaderValues = result.Headers.GetValues("MixedHeader").ToList();
        Assert.Equal(3, mixedHeaderValues.Count);
        Assert.Contains("SingleValue", mixedHeaderValues);
        Assert.Contains("ArrayValue1", mixedHeaderValues);
        Assert.Contains("ArrayValue2", mixedHeaderValues);
    }

    [Fact]
    public async Task CombineResponses_MergesArrayValuesInContent()
    {
        // Arrange
        var helper = new HttpResponseHelper();
        var responses = new List<HttpResponseMessage>
            {
                new HttpResponseMessage
                {
                    Content = new StringContent("{\"key1\": \"value1\", \"arrayKey\": [1, 2]}", Encoding.UTF8, "application/json"),
                    StatusCode = HttpStatusCode.OK
                },
                new HttpResponseMessage
                {
                    Content = new StringContent("{\"key2\": \"value2\", \"arrayKey\": [2, 3]}", Encoding.UTF8, "application/json"),
                    StatusCode = HttpStatusCode.OK
                }
            };

        // Act
        var result = await helper.CombineResponses(responses);

        // Assert
        var content = await result.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content).RootElement;
        Assert.Equal("value1", json.GetProperty("key1").GetString());
        Assert.Equal("value2", json.GetProperty("key2").GetString());
        var arrayValues = json.GetProperty("arrayKey").EnumerateArray().Select(x => x.GetInt32()).ToList();
        Assert.Equal(4, arrayValues.Count);
        Assert.Contains(1, arrayValues);
        Assert.Contains(2, arrayValues);
        Assert.Contains(3, arrayValues);
    }

    [Fact]
    public async Task CombineResponses_MergesNestedObjectsInContent()
    {
        // Arrange
        var helper = new HttpResponseHelper();
        var responses = new List<HttpResponseMessage>
            {
                new HttpResponseMessage
                {
                    Content = new StringContent("{\"nested\": {\"key1\": \"value1\", \"arrayKey\": [1, 2]}}", Encoding.UTF8, "application/json"),
                    StatusCode = HttpStatusCode.OK
                },
                new HttpResponseMessage
                {
                    Content = new StringContent("{\"nested\": {\"key2\": \"value2\", \"arrayKey\": [2, 3]}}", Encoding.UTF8, "application/json"),
                    StatusCode = HttpStatusCode.OK
                }
            };

        // Act
        var result = await helper.CombineResponses(responses);

        // Assert
        var content = await result.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content).RootElement;
        var nested = json.GetProperty("nested");
        Assert.Equal("value1", nested.GetProperty("key1").GetString());
        Assert.Equal("value2", nested.GetProperty("key2").GetString());
        var arrayValues = nested.GetProperty("arrayKey").EnumerateArray().Select(x => x.GetInt32()).ToList();
        Assert.Equal(4, arrayValues.Count);
        Assert.Contains(1, arrayValues);
        Assert.Contains(2, arrayValues);
        Assert.Contains(3, arrayValues);
    }

    [Fact]
    public async Task CombineResponses_MergesArrayOfObjectsInContent()
    {
        // Arrange
        var helper = new HttpResponseHelper();
        var responses = new List<HttpResponseMessage>
    {
        new HttpResponseMessage
        {
            Content = new StringContent("{\"key1\": \"value1\", \"arrayKey\": [{\"id\": 1, \"name\": \"A\"}, {\"id\": 2, \"name\": \"B\"}]}", Encoding.UTF8, "application/json"),
            StatusCode = HttpStatusCode.OK
        },
        new HttpResponseMessage
        {
            Content = new StringContent("{\"key2\": \"value2\", \"arrayKey\": [{\"id\": 3, \"name\": \"C\"}, {\"id\": 4, \"name\": \"D\"}]}", Encoding.UTF8, "application/json"),
            StatusCode = HttpStatusCode.OK
        }
    };

        // Act
        var result = await helper.CombineResponses(responses);

        // Assert
        var content = await result.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content).RootElement;
        Assert.Equal("value1", json.GetProperty("key1").GetString());
        Assert.Equal("value2", json.GetProperty("key2").GetString());

        var array = json.GetProperty("arrayKey").EnumerateArray().Select(obj => new
        {
            id = obj.GetProperty("id").GetInt32(),
            name = obj.GetProperty("name").GetString()
        }).ToList();

        Assert.Equal(4, array.Count);
        Assert.Contains(array, item => item.id == 1 && item.name == "A");
        Assert.Contains(array, item => item.id == 2 && item.name == "B");
        Assert.Contains(array, item => item.id == 3 && item.name == "C");
        Assert.Contains(array, item => item.id == 4 && item.name == "D");
    }
}