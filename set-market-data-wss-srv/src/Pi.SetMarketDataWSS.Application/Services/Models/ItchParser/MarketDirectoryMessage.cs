using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Application.Utils;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

public class MarketDirectoryParams
{
    public Numeric32 Nanos { get; set; }
    public Numeric8 MarketCode { get; set; }
    public required Alpha MarketName { get; set; }
    public required Alpha MarketDescription { get; set; }
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

    public Numeric32 Nanos { get; }
    public Numeric8 MarketCode { get; }
    public Alpha MarketName { get; }
    public Alpha MarketDescription { get; }

    public static MarketDirectoryMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length < 42)
            throw new ArgumentException("Invalid data format for MarketDirectoryMessage.");

        using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
        {
            var nanos = reader.ReadNumeric32();
            var marketCode = reader.ReadNumeric8();
            var marketName = reader.ReadAlpha(32);
            var marketDescription = reader.ReadAlpha(5);

            var marketDirectoryParams = new MarketDirectoryParams
            {
                Nanos = nanos,
                MarketCode = marketCode,
                MarketName = marketName,
                MarketDescription = marketDescription
            };

            return new MarketDirectoryMessage(marketDirectoryParams);
        }
    }

    public string ToStringUnitTest()
    {
        return $"MarketDirectoryMessage:\n"
               + $"Nanos: {Nanos},\n"
               + $"MarketCode: {MarketCode},\n"
               + $"MarketName: {MarketName},\n"
               + $"MarketDescription: {MarketDescription}";
    }
}