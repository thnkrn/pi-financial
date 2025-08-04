using Pi.WalletService.Application.Models;

namespace Pi.WalletService.Application.Services.Measurement;

public interface IMetric
{
    void Send(string name, double value, MetricTags? tags);
}