using System.Net;
using GrowthBook;

namespace Pi.MarketData.Domain.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global
public class FeaturesResult
{
    public HttpStatusCode Status { get; set; }
    public IDictionary<string, Feature>? Features { get; set; }
    public DateTimeOffset? DateUpdated { get; set; }
}