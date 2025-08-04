using System.Threading.Channels;
using Newtonsoft.Json;
using Pi.SetMarketDataRealTime.Application.Helpers;
using Pi.SetMarketDataRealTime.Application.Interfaces.ItchParser;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Interfaces.WriteBinlogData;
using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.DataHandler.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;

namespace Pi.SetMarketDataRealTime.DataHandler.Services.SoupBinTcp;

public sealed class ClientListener : IClientListener
{
    private const string ErrorMessageTemplate = "{0}.{1}";
    private readonly IDisconnectionHandlerFactory _disconnectionHandlerFactory;
    private readonly IItchParserService _itchParserService;
    private readonly IStockDataOptimizedV2Publisher _kafkaPublisher;
    private readonly ILogger<ClientListener> _logger;
    private readonly IMemoryCacheHelper _memoryCacheHelper;
    private readonly Channel<ItchMessage> _messageChannel;
    private readonly IWriteBinLogsData _writeBinLogsData;
    private bool _disposed;

    public ClientListener(
        ClientListenerDependencies dependencies,
        ILogger<ClientListener> logger)
    {
        ArgumentNullException.ThrowIfNull(dependencies);

        _itchParserService = dependencies.ItchParserService ?? throw new ArgumentNullException(
            string.Format(ErrorMessageTemplate, nameof(dependencies), nameof(dependencies.ItchParserService)));
        _kafkaPublisher = dependencies.KafkaPublisher ?? throw new ArgumentNullException(
            string.Format(ErrorMessageTemplate, nameof(dependencies), nameof(dependencies.KafkaPublisher)));
        _writeBinLogsData = dependencies.WriteBinLogsData ?? throw new ArgumentNullException(
            string.Format(ErrorMessageTemplate, nameof(dependencies), nameof(dependencies.WriteBinLogsData)));
        _memoryCacheHelper = dependencies.MemoryCacheHelper ?? throw new ArgumentNullException(
            string.Format(ErrorMessageTemplate, nameof(dependencies), nameof(dependencies.MemoryCacheHelper)));
        _disconnectionHandlerFactory = dependencies.DisconnectionHandlerFactory ?? throw new ArgumentNullException(
            string.Format(ErrorMessageTemplate, nameof(dependencies),
                nameof(dependencies.DisconnectionHandlerFactory)));
        _messageChannel = Channel.CreateUnbounded<ItchMessage>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true
        });
        _logger = logger;
    }

    public string LogPrefix { get; set; } = string.Empty;

    public async Task OnConnect()
    {
        _logger.LogDebug("Connected to the server");
        await Task.CompletedTask;
    }

    public async Task OnMessage(byte[] message)
    {
        ItchMessage itchMessage;

        try
        {
            // Fire and forget operation!
            _ = _writeBinLogsData.WriteBinLogsDataAsync(message, LogPrefix);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing binary logs");
        }

        try
        {
            itchMessage = await _itchParserService.Parse(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing message");
            return;
        }

        _logger.LogDebug("Message of type {MsgType} parsed", itchMessage.MsgType);

        if (itchMessage.MsgType is ItchMessageType.G or ItchMessageType.S)
        {
            // Immediately queue the parsed message
            await _messageChannel.Writer.WriteAsync(itchMessage);
            _logger.LogDebug("Message queued for processing");
        }

        var serializedItchMessage = JsonConvert.SerializeObject(itchMessage);

        if (string.IsNullOrEmpty(serializedItchMessage.Trim()))
        {
            _logger.LogError("Error serializing message: {ItchMessage}", itchMessage);
            return;
        }

        var cleanMessage = serializedItchMessage.SimpleCleanJsonMessage();
        if (string.IsNullOrEmpty(cleanMessage.Trim()))
        {
            _logger.LogError("Error cleaning message: {SerializedItchMessage}", serializedItchMessage);
            return;
        }

        _logger.LogDebug("Processed message: {CleanMessage}", cleanMessage);

        try
        {
            await _kafkaPublisher.EnqueueMessageAsync(itchMessage, cleanMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in message processing tasks");
        }
    }

    public async Task<ItchMessage?> ReceiveMessage(CancellationToken cancellationToken)
    {
        try
        {
            var message = await _messageChannel.Reader.ReadAsync(cancellationToken);
            _logger.LogDebug("Message of type {MsgType} dequeued for processing", message.MsgType);
            return message;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "ReceiveMessage operation cancelled");
            return null;
        }
    }

    public async Task OnDebug(string message)
    {
        _logger.LogDebug("Debug message received: {Message}", message);
        await Task.CompletedTask;
    }

    public async Task OnLoginAccept(string session, ulong sequenceNumber)
    {
        _logger.LogDebug(
            "Login accepted. Session: {Session}, Sequence Number: {SequenceNumber}, Date: {DateTime}", session,
            sequenceNumber, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

        var cleanSession = session.Trim();
        // Set current Session to local memory
        await _memoryCacheHelper.SetCurrentSessionAsync(cleanSession);
        // Set current ItchSequenceNumber to local memory
        await _memoryCacheHelper.SetCurrentItchSequenceNoAsync(sequenceNumber);

        _logger.LogDebug("Latest SequenceNumber from Itch service: {SequenceNumber}",
            sequenceNumber);
    }

    public async Task OnLoginReject(char rejectReasonCode)
    {
        _logger.LogWarning("Login rejected, the RejectReasonCode: {RejectReasonCode}", rejectReasonCode);
        var handler = _disconnectionHandlerFactory.CreateHandler();
        await handler.HandleLoginRejectedDisconnectionAsync();
    }

    public async Task OnDisconnect(bool isUnexpectedDisconnection)
    {
        if (isUnexpectedDisconnection)
        {
            _logger.LogWarning("Unexpected disconnection from the server");
            var handler = _disconnectionHandlerFactory.CreateHandler();
            await handler.HandleUnexpectedDisconnectionAsync();
        }
        else
        {
            _logger.LogDebug("Disconnected from the server");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
            // Dispose managed resources
            _writeBinLogsData.Dispose();

        // Dispose unmanaged resources here, if any
        _disposed = true;
    }

    ~ClientListener()
    {
        Dispose(false);
    }
}