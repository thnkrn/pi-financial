using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Pi.MarketData.MigrationProxy.API.Filters;

public class PathPrefixInsertDocumentFilter : IDocumentFilter
{
    private readonly string _pathPrefix;

    /// <summary>
    /// </summary>
    /// <param name="prefix"></param>
    public PathPrefixInsertDocumentFilter(string prefix)
    {
        _pathPrefix = prefix;
    }
    
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // Set OpenAPI info version
        swaggerDoc.Info = new OpenApiInfo
        {
            Version = "3.0.1",
            Title = "MarketData Migration Proxy API",
            Description = "Market Data Migration Proxy API Documentation"
        };
    
        var paths = swaggerDoc.Paths.ToDictionary(
            entry => _pathPrefix + entry.Key,
            entry => entry.Value
        );

        swaggerDoc.Paths = new OpenApiPaths();
        foreach (var (key, value) in paths)
        {
            swaggerDoc.Paths.Add(key, value);
        }

        swaggerDoc.Servers = new List<OpenApiServer>
        {
            new() { Url = _pathPrefix }
        };
    }
}