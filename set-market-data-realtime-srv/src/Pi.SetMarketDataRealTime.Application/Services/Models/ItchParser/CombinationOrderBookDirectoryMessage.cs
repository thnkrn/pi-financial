using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class CombinationOrderBookDirectoryParams
{
    public required Numeric32 Nanos { get; init; }
    public required Numeric32 CombinationOrderBookId { get; init; }
    public required Numeric32 LegOrderBookId { get; init; }
    public required Alpha LegSide { get; init; }
    public required Numeric32 LegRatio { get; init; }
}

public class CombinationOrderBookDirectoryMessage : ItchMessage
{
    public CombinationOrderBookDirectoryMessage(CombinationOrderBookDirectoryParams messageParams)
    {
        MsgType = ItchMessageType.M;
        Nanos = messageParams.Nanos;
        CombinationOrderBookId = messageParams.CombinationOrderBookId;
        LegOrderBookId = messageParams.LegOrderBookId;
        LegSide = messageParams.LegSide;
        LegRatio = messageParams.LegRatio;
    }

    public override Numeric32 Nanos { get; }
    public Numeric32 CombinationOrderBookId { get; }
    public Numeric32 LegOrderBookId { get; }
    public Alpha LegSide { get; }
    public Numeric32 LegRatio { get; }

    public static CombinationOrderBookDirectoryMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < 17)
            throw new ArgumentException("Invalid data format for CombinationOrderBookDirectoryMessage.", nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        var nanos = reader.ReadNumeric32();
        var combinationOrderBookId = reader.ReadNumeric32();
        var legOrderBookId = reader.ReadNumeric32();
        var legSide = reader.ReadAlpha(1);
        var legRatio = reader.ReadNumeric32();

        return new CombinationOrderBookDirectoryMessage(new CombinationOrderBookDirectoryParams
        {
            Nanos = nanos,
            CombinationOrderBookId = combinationOrderBookId,
            LegOrderBookId = legOrderBookId,
            LegSide = legSide,
            LegRatio = legRatio
        });
    }

    public string ToStringUnitTest()
    {
        return $"""
                CombinationOrderBookDirectoryMessage:
                Nanos: {Nanos},
                CombinationOrderBookId: {CombinationOrderBookId},
                LegOrderBookId: {LegOrderBookId},
                LegSide: {LegSide},
                LegRatio: {LegRatio}
                """;
    }
}