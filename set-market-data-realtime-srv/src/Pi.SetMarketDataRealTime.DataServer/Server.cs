using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;
using DotNetty.Transport.Channels.Sockets;
using Pi.SetMarketDataRealTime.DataServer.Handlers;
using Pi.SetMarketDataRealTime.DataServer.Services.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;
using Pi.SetMarketDataRealTime.Infrastructure.Services.SoupBinTcp.Codecs;

namespace Pi.SetMarketDataRealTime.DataServer;

public class Server
{
    private readonly CancellationToken _cancellationToken;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly DefaultChannelGroup _channelGroup = new("ALL", new SingleThreadEventLoop());
    private readonly Dictionary<string, IChannel> _channels = new();
    private readonly ServerListener _listener;
    private IChannel? _serverChannel;

    public Server(ServerListener listener)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _listener = listener;
    }

    private async Task RunServerAsync()
    {
        var parentGroup = new MultithreadEventLoopGroup(1);
        var childGroup = new MultithreadEventLoopGroup();

        try
        {
            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(parentGroup, childGroup)
                .Channel<TcpServerSocketChannel>()
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(
                    channel =>
                    {
                        var pipeline = channel.Pipeline;

                        _channels.Add(channel.Id.AsLongText(), channel);

                        pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian, ushort.MaxValue, 0, 2, 0,
                            2,
                            true));
                        pipeline.AddLast(new LengthFieldPrepender(ByteOrder.BigEndian, 2, 0, false));
                        pipeline.AddLast(new MessageDecoder());
                        pipeline.AddLast(new MessageEncoder());
                        pipeline.AddLast("LoginRequestFilter", new LoginRequestFilterHandler());
                        pipeline.AddLast(new IdleStateHandler(15, 1, 0));
                        pipeline.AddLast(new TimeoutHandler(_listener));
                        pipeline.AddLast("ServerHandshake", new HandshakeHandler(_listener));
                    }
                ));

            _serverChannel = await bootstrap.BindAsync(5501);
            await _listener.OnServerListening();
            _cancellationToken.WaitHandle.WaitOne();

            if (_serverChannel.Active)
            {
                await _channelGroup.WriteAndFlushAsync(new LogoutRequest());
                await _serverChannel.CloseAsync();
            }

            await _listener.OnServerDisconnect();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            await Task.WhenAll(
                parentGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                childGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1))
            );
        }
    }

    public void Start()
    {
        Task.Run(RunServerAsync, _cancellationToken);
    }

    public void Shutdown()
    {
        _cancellationTokenSource.Cancel();
    }
}