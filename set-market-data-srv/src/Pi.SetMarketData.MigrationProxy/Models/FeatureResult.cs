using System.Net;
using GrowthBook;

namespace Pi.SetMarketData.MigrationProxy.Models;

public class FeaturesResult
{
    public HttpStatusCode Status { get; set; }
    public IDictionary<string, Feature>? Features { get; set; }
    public DateTimeOffset? DateUpdated { get; set; }
}