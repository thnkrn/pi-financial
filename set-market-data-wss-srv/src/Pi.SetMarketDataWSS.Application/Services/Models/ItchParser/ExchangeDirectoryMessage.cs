using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Application.Utils;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

public class ExchangeDirectoryParams
{
    public Numeric32 Nanos { get; set; }
    public Numeric8 ExchangeCode { get; set; }
    public Alpha ExchangeName { get; set; }
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

    public Numeric32 Nanos { get; }
    public Numeric8 ExchangeCode { get; }
    public Alpha ExchangeName { get; }

    public static ExchangeDirectoryMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length < 37)
            throw new ArgumentException("Invalid data format for ExchangeDirectoryMessage.");

        using (var reader = new ItchMessageByteReader(new Memory<byte>(bytes)))
        {
            var nanos = reader.ReadNumeric32();
            var exchangeCode = reader.ReadNumeric8();
            var exchangeName = reader.ReadAlpha(32);

            var exchangeDirectoryParams = new ExchangeDirectoryParams
            {
                Nanos = nanos,
                ExchangeCode = exchangeCode,
                ExchangeName = exchangeName
            };

            return new ExchangeDirectoryMessage(exchangeDirectoryParams);
        }
    }

    public string ToStringUnitTest()
    {
        return $"ExchangeDirectoryMessage:\n"
               + $"Nanos: {Nanos},\n"
               + $"ExchangeCode: {ExchangeCode},\n"
               + $"ExchangeName: {ExchangeName}";
    }
}