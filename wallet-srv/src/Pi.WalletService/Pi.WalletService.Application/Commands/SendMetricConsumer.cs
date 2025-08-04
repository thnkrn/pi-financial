using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Services.Measurement;

namespace Pi.WalletService.Application.Commands;

public record SendMetric(string Name, double Value, MetricTags? Tags);


public class SendMetricConsumer : IConsumer<SendMetric>
{

    private readonly IMetric _metricService;
    private readonly ILogger<SendMetricConsumer> _logger;

    public SendMetricConsumer(IMetric metricService, ILogger<SendMetricConsumer> logger)
    {
        _metricService = metricService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendMetric> context)
    {
        try
        {
            _metricService.Send(context.Message.Name, context.Message.Value, context.Message.Tags);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddMetricConsumer: Unable to add metric {Context} with Exception: {Message}", context.Message, ex.Message);
        }
    }
}