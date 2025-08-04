using System.Runtime.Serialization;

namespace Pi.OnePort.TCP.Exceptions;

[Serializable]
public class PacketDeserializationException : Exception
{
    public PacketDeserializationException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public PacketDeserializationException()
    {
    }

    public PacketDeserializationException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected PacketDeserializationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
