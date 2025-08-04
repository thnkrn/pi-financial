using System.Diagnostics.Metrics;
using Pi.Financial.FundService.Application.Models.Metric;
using Pi.Financial.FundService.Application.Services.Metric;

namespace Pi.Financial.FundService.Infrastructure.Services;

public class MetricService : IMetricService
{
    public const string MeterName = "FundService";
    private const string MetricPrefix = "fund_srv";
    private readonly Dictionary<string, Counter<double>> _counterMaps = new();
    private readonly Dictionary<string, UpDownCounter<double>> _upDownCountersMaps = new();

    public MetricService(IMetrics metrics)
    {
        var meter = new Meter(MeterName);
        foreach (var metricKey in metrics.GetMetricsCounter())
        {
            _counterMaps.Add(metricKey, meter.CreateCounter<double>($"{MetricPrefix}.{metricKey}"));
        }
        foreach (var metricKey in metrics.GetMetricsUpDownCounter())
        {
            _upDownCountersMaps.Add(metricKey, meter.CreateUpDownCounter<double>($"{MetricPrefix}.{metricKey}"));
        }
    }

    public void Send(string metric, double value, KeyValuePair<string, object?>[]? tags = null)
    {
        if (tags == null)
        {
            _counterMaps[metric].Add(value);
        }
        else
        {
            _counterMaps[metric].Add(value, tags);
        }
    }

    public void SendUpDown(string metric, double value, KeyValuePair<string, object?>[]? tags = null)
    {
        if (tags == null)
        {
            _upDownCountersMaps[metric].Add(value);
        }
        else
        {
            _upDownCountersMaps[metric].Add(value, tags);
        }
    }
}
