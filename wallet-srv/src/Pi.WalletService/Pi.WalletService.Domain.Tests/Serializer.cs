using System.Runtime.Serialization.Formatters.Binary;

namespace Pi.WalletService.Domain.Tests;

static class Serializer
{
    public static TException SerializeAndDeserialize<TException>(TException exception)
    {
        var formatter = new BinaryFormatter();

        using var stream = new MemoryStream();

        // Use for testing only
#pragma warning disable SYSLIB0011
        formatter.Serialize(stream, exception!);
#pragma warning restore SYSLIB0011

        stream.Seek(0, 0);

#pragma warning disable SYSLIB0011
        return (TException)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
    }
}
