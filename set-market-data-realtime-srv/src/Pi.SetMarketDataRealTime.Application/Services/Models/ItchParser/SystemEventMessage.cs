using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class SystemEventMessageParams
{
    public required Numeric32 Nanos { get; init; }
    public required Alpha EventCode { get; init; }
}

public class SystemEventMessage : ItchMessage
{
    public SystemEventMessage(SystemEventMessageParams systemEventMessageParams)
    {
        MsgType = ItchMessageType.S;
        Nanos = systemEventMessageParams.Nanos;
        EventCode = systemEventMessageParams.EventCode;
    }

    public override Numeric32 Nanos { get; }
    public Alpha EventCode { get; }

    public static SystemEventMessage Parse(byte[] bytes)
    {
        if (bytes is not { Length: 5 }) // Expecting exactly 5 bytes for the SystemEventMessage
            throw new ArgumentException("Invalid data format for SystemEventMessage.");

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

    public string ToStringUnitTest()
    {
        return $"SystemEventMessage:\n" + $"Nanos: {Nanos},\n" + $"EventCode: {EventCode},\n";
    }
}