using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Services.Models.ItchParser;

public class SystemEventMessageParams
{
    public Numeric32 Nanos { get; set; }
    public required Alpha EventCode { get; set; }
}

public class SystemEventMessage : ItchMessage
{
    public Numeric32 Nanos { get; private set; }
    public Alpha EventCode { get; private set; }

    public SystemEventMessage(SystemEventMessageParams systemEventMessageParams)
    {
        MsgType = ItchMessageType.S;
        Nanos = systemEventMessageParams.Nanos;
        EventCode = systemEventMessageParams.EventCode;
    }

    public static SystemEventMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 5) // Expecting exactly 5 bytes for the SystemEventMessage
        {
            throw new ArgumentException("Invalid data format for SystemEventMessage.");
        }

        var reader = new ItchMessageByteReader(new Memory<byte>(bytes));
        var nanos = reader.ReadNumeric32();
        var eventCode = reader.ReadAlpha(1);

        var systemEventMessageParams = new SystemEventMessageParams
        {
            Nanos = nanos,
            EventCode = eventCode
        };

        return new SystemEventMessage(systemEventMessageParams);
    }

    public override string ToString()
    {
        return $"SystemEventMessage:\n" + $"Nanos: {Nanos},\n" + $"EventCode: {EventCode},\n";
    }
}
