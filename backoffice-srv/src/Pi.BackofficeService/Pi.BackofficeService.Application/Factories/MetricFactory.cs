namespace Pi.BackofficeService.Application.Factories;

public static class MetricFactory
{
    public static KeyValuePair<string, object?> NewTag(string key, object? value)
    {
        return new KeyValuePair<string, object?>(key, value);
    }
}
