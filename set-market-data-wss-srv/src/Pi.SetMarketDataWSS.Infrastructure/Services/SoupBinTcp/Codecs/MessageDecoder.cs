using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Pi.SetMarketDataWSS.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataWSS.Infrastructure.Services.SoupBinTcp.Codecs;

public class MessageDecoder : ByteToMessageDecoder
{
    protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
    {
        if (!input.IsReadable()) return;

        var bytes = new byte[input.ReadableBytes];
        input.ReadBytes(bytes);

        var type = bytes[0];

        switch (type)
        {
            case 43:
                output.Add(new Debug(bytes));
                break;
            case 76:
                output.Add(new LoginRequest(bytes));
                break;
            case 65:
                output.Add(new LoginAccepted(bytes));
                break;
            case 74:
                output.Add(new LoginRejected(bytes));
                break;
            case 79:
                output.Add(new LogoutRequest(bytes));
                break;
            case 83:
                output.Add(new SequencedData(bytes, true));
                break;
            case 85:
                output.Add(new UnSequencedData(bytes, true));
                break;
            case 72:
                Console.WriteLine($"Type: {type} {Encoding.ASCII.GetString(bytes)}");
                output.Add(new ServerHeartbeat(bytes));
                break;
            case 82:
                Console.WriteLine($"Type: {type} {Encoding.ASCII.GetString(bytes)}");
                output.Add(new ClientHeartbeat(bytes));
                break;
            default:
                Console.WriteLine($"Type: {type} {Encoding.ASCII.GetString(bytes)}");
                output.Add(new ClientHeartbeat(bytes));
                break;
        }
    }
}