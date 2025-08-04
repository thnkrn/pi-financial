using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using Pi.BackofficeService.Application.Services.Measurement;

namespace Pi.BackofficeService.Infrastructure.Services;

public class MetricService : IMetric
{
    public const string MeterName = "BackofficeService";
    private const string MetricPrefix = "backoffice_srv";
    private readonly Meter _meter;
    private readonly ILogger<MetricService> _logger;

    public MetricService(ILogger<MetricService> logger)
    {
        _meter = new Meter(MeterName);
        _logger = logger;
    }

    public void Send(string metric, KeyValuePair<string, object?>? tag = null)
    {
        Send(metric, 1, tag);
    }

    public void Send(string metric, double value, KeyValuePair<string, object?>? tag = null)
    {
        if (tag == null)
        {
            _meter.CreateCounter<double>($"{MetricPrefix}.{metric}").Add(value);
        }
        else
        {
            _meter.CreateCounter<double>($"{MetricPrefix}.{metric}").Add(value, (KeyValuePair<string, object?>)tag);
        }

        _logger.LogInformation("Metric send: {Metric}", metric);
    }

    public void SendObservableGauge(string metric, KeyValuePair<string, object?>? tag = null)
    {
        SendObservableGauge(metric, 1, tag);
    }

    public void SendObservableGauge(string metric, double value, KeyValuePair<string, object?>? tag = null)
    {
        if (tag == null)
        {
            _meter.CreateObservableGauge($"{MetricPrefix}.{metric}", () => value);
        }
        else
        {
            _meter.CreateObservableGauge($"{MetricPrefix}.{metric}", () => new Measurement<double>(value, (KeyValuePair<string, object?>)tag));
        }
    }
}
