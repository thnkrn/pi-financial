using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class MarketDirectoryParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric8 MarketCode { get; init; }
    public required Alpha MarketName { get; init; }
    public required Alpha MarketDescription { get; init; }
}

public class MarketDirectoryMessage : ItchMessage
{
    public MarketDirectoryMessage(MarketDirectoryParams marketDirectoryParams)
    {
        MsgType = ItchMessageType.m;
        Nanos = marketDirectoryParams.Nanos;
        MarketCode = marketDirectoryParams.MarketCode;
        MarketName = marketDirectoryParams.MarketName;
        MarketDescription = marketDirectoryParams.MarketDescription;
    }

    public override Numeric32 Nanos { get; }
    public Numeric8 MarketCode { get; }
    public Alpha MarketName { get; }
    public Alpha MarketDescription { get; }

    public static MarketDirectoryMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        if (bytes.Length < 42)
            throw new ArgumentException("Invalid data format for MarketDirectoryMessage. Expected at least 42 bytes.",
                nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        var nanos = reader.ReadNumeric32();
        var marketCode = reader.ReadNumeric8();
        var marketName = reader.ReadAlpha(32);
        var marketDescription = reader.ReadAlpha(5);

        return new MarketDirectoryMessage(new MarketDirectoryParams
        {
            Nanos = nanos,
            MarketCode = marketCode,
            MarketName = marketName,
            MarketDescription = marketDescription
        });
    }

    public string ToStringUnitTest()
    {
        return $"""
                MarketDirectoryMessage:
                Nanos: {Nanos},
                MarketCode: {MarketCode},
                MarketName: {MarketName},
                MarketDescription: {MarketDescription}
                """;
    }
}