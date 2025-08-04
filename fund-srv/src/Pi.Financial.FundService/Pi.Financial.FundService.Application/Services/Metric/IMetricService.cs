namespace Pi.Financial.FundService.Application.Services.Metric;

public interface IMetricService
{
    void Send(string metric, double value, KeyValuePair<string, object?>[]? tags = null);
    void SendUpDown(string metric, double value, KeyValuePair<string, object?>[]? tags = null);
}
