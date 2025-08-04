using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Financial.FundService.Application.Models.Metric;
using Pi.Financial.FundService.Application.Services.Metric;

namespace Pi.Financial.FundService.Application.Commands;

public record SendMetric(string Name, double Value, KeyValuePair<string, object?>[]? Tags);

public class SendMetricConsumer : IConsumer<SendMetric>
{
    private readonly IMetricService _metricService;
    private readonly IMetrics _metrics;
    private readonly ILogger<SendMetricConsumer> _logger;

    public SendMetricConsumer(IMetricService metricService, IMetrics metrics, ILogger<SendMetricConsumer> logger)
    {
        _metricService = metricService;
        _metrics = metrics;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendMetric> context)
    {
        try
        {
            if (_metrics.GetMetricsCounter().Contains(context.Message.Name))
            {
                _metricService.Send(context.Message.Name, context.Message.Value, context.Message.Tags);
            }
            else if (_metrics.GetMetricsUpDownCounter().Contains(context.Message.Name))
            {
                _metricService.SendUpDown(context.Message.Name, context.Message.Value, context.Message.Tags);
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddMetricConsumer: Unable to add metric {Context} with Exception: {Message}", context.Message, ex.Message);
        }
    }
}
