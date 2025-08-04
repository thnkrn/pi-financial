using System.Runtime.Serialization;

namespace Pi.OnePort.TCP.Exceptions;

[Serializable]
public class OutOfOperatingHoursException : Exception
{
    public OutOfOperatingHoursException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public OutOfOperatingHoursException()
    {
    }

    public OutOfOperatingHoursException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected OutOfOperatingHoursException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
