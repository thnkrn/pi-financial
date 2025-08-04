using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class ExchangeDirectoryParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric8 ExchangeCode { get; init; }
    public required Alpha ExchangeName { get; init; }
}

public class ExchangeDirectoryMessage : ItchMessage
{
    public ExchangeDirectoryMessage(ExchangeDirectoryParams exchangeDirectoryParams)
    {
        MsgType = ItchMessageType.e;
        Nanos = exchangeDirectoryParams.Nanos;
        ExchangeCode = exchangeDirectoryParams.ExchangeCode;
        ExchangeName = exchangeDirectoryParams.ExchangeName;
    }

    public override Numeric32 Nanos { get; }
    public Numeric8 ExchangeCode { get; }
    public Alpha ExchangeName { get; }

    public static ExchangeDirectoryMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < 37)
            throw new ArgumentException("Invalid data format for ExchangeDirectoryMessage.", nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        var nanos = reader.ReadNumeric32();
        var exchangeCode = reader.ReadNumeric8();
        var exchangeName = reader.ReadAlpha(32);

        return new ExchangeDirectoryMessage(new ExchangeDirectoryParams
        {
            Nanos = nanos,
            ExchangeCode = exchangeCode,
            ExchangeName = exchangeName
        });
    }

    public string ToStringUnitTest()
    {
        return $"""
                ExchangeDirectoryMessage:
                Nanos: {Nanos},
                ExchangeCode: {ExchangeCode},
                ExchangeName: {ExchangeName}
                """;
    }
}