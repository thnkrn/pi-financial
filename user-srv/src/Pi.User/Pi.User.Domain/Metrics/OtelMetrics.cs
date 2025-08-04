using System.Diagnostics.Metrics;

namespace Pi.User.Domain.Metrics;

public class OtelMetrics
{
    private Counter<int> UserCreatedCounter { get; }
    private Counter<int> UserUpdatedCounter { get; }
    public const string MetricName = "UserService";
    public OtelMetrics(string meterName = "UserService")
    {
        // https://www.mytechramblings.com/posts/getting-started-with-opentelemetry-metrics-and-dotnet-part-2/
        var meter = new Meter(meterName);
        UserCreatedCounter = meter.CreateCounter<int>("user_srv.users_created", "user");
        UserUpdatedCounter = meter.CreateCounter<int>("user_srv.users_updated", "user");
    }

    public void CreateUser() => UserCreatedCounter.Add(1);
    public void UpdateUser() => UserUpdatedCounter.Add(1);
}