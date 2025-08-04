using System.Text.Json;

namespace Pi.BackofficeService.Domain.Tests;

internal static class Serializer
{
    public static TException SerializeAndDeserialize<TException>(TException exception)
    {
        var exceptionStr = JsonSerializer.Serialize(exception);

        return JsonSerializer.Deserialize<TException>(exceptionStr) ?? throw new InvalidOperationException();
    }
}
