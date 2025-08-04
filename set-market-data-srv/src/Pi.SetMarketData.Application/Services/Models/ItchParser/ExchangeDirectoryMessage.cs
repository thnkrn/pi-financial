using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Services.Models.ItchParser;

public class ExchangeDirectoryParams
{
    public Numeric32 Nanos { get; set; }
    public Numeric8 ExchangeCode { get; set; }
    public Alpha ExchangeName { get; set; }
}

public class ExchangeDirectoryMessage : ItchMessage
{
    public Numeric32 Nanos { get; private set; }
    public Numeric8 ExchangeCode { get; private set; }
    public Alpha ExchangeName { get; private set; }

    public ExchangeDirectoryMessage(ExchangeDirectoryParams exchangeDirectoryParams)
    {
        MsgType = ItchMessageType.e;
        Nanos = exchangeDirectoryParams.Nanos;
        ExchangeCode = exchangeDirectoryParams.ExchangeCode;
        ExchangeName = exchangeDirectoryParams.ExchangeName;
    }

    public static ExchangeDirectoryMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length < 37)
        {
            throw new ArgumentException("Invalid data format for ExchangeDirectoryMessage.");
        }

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

    public override string ToString()
    {
        return $"ExchangeDirectoryMessage:\n"
            + $"Nanos: {Nanos},\n"
            + $"ExchangeCode: {ExchangeCode},\n"
            + $"ExchangeName: {ExchangeName}";
    }
}
