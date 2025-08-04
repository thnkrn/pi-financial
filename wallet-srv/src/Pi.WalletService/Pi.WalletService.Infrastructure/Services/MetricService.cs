using System.Diagnostics.Metrics;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Services.Measurement;
using Pi.WalletService.Application.Utilities;

namespace Pi.WalletService.Infrastructure.Services;

public class MetricService : IMetric
{
    public const string MeterName = "WalletService";
    private const string MetricPrefix = "wallet_srv";
    private readonly Dictionary<string, UpDownCounter<double>> _counterMaps;
    public readonly Meter Meter;

    public MetricService()
    {
        Meter = new Meter(MeterName);
        _counterMaps = new Dictionary<string, UpDownCounter<double>>();
        foreach (var metric in typeof(Metrics).GetAllPublicConstValues<string>())
        {
            _counterMaps.Add(metric, Meter.CreateUpDownCounter<double>($"{MetricPrefix}.{metric}"));
        }
    }

    public void Send(string name, double value, MetricTags? tags)
    {
        if (tags == null)
        {
            _counterMaps[name].Add(value);
        }
        else
        {
            _counterMaps[name].Add(value, tags.GetValues());
        }

    }
}