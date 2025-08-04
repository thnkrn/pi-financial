using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Services.Models.ItchParser;

public class CombinationOrderBookDirectoryParams
{
    public Numeric32 Nanos { get; set; }
    public Numeric32 CombinationOrderBookId { get; set; }
    public Numeric32 LegOrderBookId { get; set; }
    public Alpha LegSide { get; set; }
    public Numeric32 LegRatio { get; set; }
}

public class CombinationOrderBookDirectoryMessage : ItchMessage
{
    public Numeric32 Nanos { get; private set; }
    public Numeric32 CombinationOrderBookId { get; private set; }
    public Numeric32 LegOrderBookId { get; private set; }
    public Alpha LegSide { get; private set; }
    public Numeric32 LegRatio { get; private set; }

    public CombinationOrderBookDirectoryMessage(CombinationOrderBookDirectoryParams messageParams)
    {
        MsgType = ItchMessageType.M; // Message type for Combination Order Book Directory
        Nanos = messageParams.Nanos;
        CombinationOrderBookId = messageParams.CombinationOrderBookId;
        LegOrderBookId = messageParams.LegOrderBookId;
        LegSide = messageParams.LegSide;
        LegRatio = messageParams.LegRatio;
    }

    public static CombinationOrderBookDirectoryMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length < 17) // Minimum length check
        {
            throw new ArgumentException(
                "Invalid data format for CombinationOrderBookDirectoryMessage."
            );
        }

        using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
        {
            var nanos = reader.ReadNumeric32();
            var combinationOrderBookId = reader.ReadNumeric32();
            var legOrderBookId = reader.ReadNumeric32();
            var legSide = reader.ReadAlpha(1);
            var legRatio = reader.ReadNumeric32();

            var combinationOrderBookDirectoryParams = new CombinationOrderBookDirectoryParams
            {
                Nanos = nanos,
                CombinationOrderBookId = combinationOrderBookId,
                LegOrderBookId = legOrderBookId,
                LegSide = legSide,
                LegRatio = legRatio
            };

            return new CombinationOrderBookDirectoryMessage(combinationOrderBookDirectoryParams);
        }
    }

    public override string ToString()
    {
        return $"CombinationOrderBookDirectoryMessage:\n"
            + $"Nanos: {Nanos},\n"
            + $"CombinationOrderBookId: {CombinationOrderBookId},\n"
            + $"LegOrderBookId: {LegOrderBookId},\n"
            + $"LegSide: {LegSide},\n"
            + $"LegRatio: {LegRatio}";
    }
}
