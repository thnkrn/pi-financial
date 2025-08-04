namespace Pi.BackofficeService.Application.Services.Measurement;

public interface IMetric
{
    void Send(string metric, KeyValuePair<string, object?>? tag = null);
    void Send(string metric, double value, KeyValuePair<string, object?>? tag = null);
    void SendObservableGauge(string metric, KeyValuePair<string, object?>? tag = null);
    void SendObservableGauge(string metric, double value, KeyValuePair<string, object?>? tag = null);
}
