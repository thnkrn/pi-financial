using Microsoft.Extensions.Diagnostics.Metrics.Testing;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Infrastructure.Services;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.Tests.Services;

public class MetricServiceTests
{

    private readonly MetricService _metricService;
    private readonly string _testMetricName;
    private readonly MetricCollector<double> _collector;

    public MetricServiceTests()
    {
        _metricService = new MetricService();
        _testMetricName = Metrics.DepositAmount;
        _collector = new MetricCollector<double>(_metricService.Meter, $"wallet_srv.{Metrics.DepositAmount}");
    }

    [Fact]
    public void Should_RecordOnlyMetricValue_When_TagsIsEmpty()
    {
        // Arrange
        var value = 100.00;

        // Act
        _metricService.Send(_testMetricName, value, null);

        // Asserts
        var measurements = _collector.GetMeasurementSnapshot();
        Assert.Equal(1, measurements.Count);
        Assert.Equal(value, measurements[0].Value);
        Assert.Equal(0, measurements[0].Tags.Count);
    }

    [Fact]
    public void Should_RecordMetricValueAndTags_When_TagsIsNotEmpty()
    {
        // Arrange
        var value = 1;
        var tags = new MetricTags(Product.Cash, Channel.QR, null, null);

        // Act
        _metricService.Send(_testMetricName, value, tags);

        // Asserts
        var measurements = _collector.GetMeasurementSnapshot();
        Assert.Equal(1, measurements.Count);
        Assert.Equal(value, measurements[0].Value);
        Assert.Equal(2, measurements[0].Tags.Count);
    }

}