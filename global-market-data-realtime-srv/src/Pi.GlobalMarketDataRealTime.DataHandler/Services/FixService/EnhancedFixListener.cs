using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks.Dataflow;
using Pi.GlobalMarketDataRealTime.DataHandler.Interfaces;
using Pi.GlobalMarketDataRealTime.Domain.Models.Fix;
using Pi.GlobalMarketDataRealTime.Infrastructure.Helpers;
using Pi.GlobalMarketDataRealTime.Infrastructure.Services.Fix;
using Pi.GlobalMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;
using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;
using Message = QuickFix.Message;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Services.FixService;

// ReSharper disable InconsistentNaming
public sealed class EnhancedFixListener : MessageCracker, IFixListener, IDisposable
{
    private const string Unknown = "Unknown";
    private const int _maxConsecutiveHeartbeats = 50;
    private const int _maxRejectCount = 700;
    private readonly ConcurrentQueue<MarketDataSnapshotFullRefresh> _batchBuffer;
    private readonly SemaphoreSlim _batchSemaphore = new(1, 1);
    private readonly int _batchSize;
    private readonly Timer _batchTimer;
    private readonly IDataRecoveryService _dataRecoveryService;
    private readonly object _heartbeatLock = new();

    private readonly TimeSpan
        _heartbeatResetTimeout = TimeSpan.FromMinutes(30);

    private readonly Timer _heartbeatTimer;
    private readonly bool _isDataRecovery;
    private readonly ILogger<EnhancedFixListener> _logger;
    private readonly TimeSpan _maxBatchDelay = TimeSpan.FromMilliseconds(100);
    private readonly TimeSpan _performanceReportInterval = TimeSpan.FromSeconds(30);
    private readonly ActionBlock<MarketDataSnapshotFullRefresh> _processingPipeline;
    private readonly object _rejectCountLock = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly SessionSettings _settings;
    private readonly IStockDataOptimizedV2Publisher _stockPublisher;
    private int _consecutiveErrorCount;
    private int _consecutiveHeartbeatCount;
    private bool _disposed;
    private volatile bool _isNeedToRestart;
    private volatile bool _isSessionLogin;
    private DateTime _lastMessageTime = DateTime.UtcNow;
    private DateTime _lastPerformanceReport = DateTime.UtcNow;
    private long _lastReportedCount;
    private long _messageCount;
    private int _rejectCount;
    private SessionID? _sessionId;

    public EnhancedFixListener(
        string configFilePath,
        IDataRecoveryService dataRecoveryService,
        IStockDataOptimizedV2Publisher stockPublisher,
        ILogger<EnhancedFixListener> logger,
        IServiceProvider serviceProvider,
        int batchSize = 150,
        int maxDegreeOfParallelism = 15)
    {
        _isSessionLogin = false;
        _isNeedToRestart = false;
        _isDataRecovery = false;
        _batchSize = batchSize;
        _consecutiveErrorCount = 0;
        _rejectCount = 0;

        if (string.IsNullOrEmpty(configFilePath))
            throw new ArgumentNullException(nameof(configFilePath), "Configuration file path cannot be null or empty.");

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dataRecoveryService = dataRecoveryService ?? throw new ArgumentNullException(nameof(dataRecoveryService));
        _stockPublisher = stockPublisher ?? throw new ArgumentNullException(nameof(stockPublisher));
        _serviceProvider = serviceProvider;
        using var reader = new StreamReader(configFilePath);
        _settings = new SessionSettings(reader);

        // Initialize performance monitoring
        _messageCount = 0;
        _lastReportedCount = 0;

        // Initialize batching with ConcurrentQueue for better performance
        _batchBuffer = new ConcurrentQueue<MarketDataSnapshotFullRefresh>();

        // Initialize batch timer to process partial batches (properly stored in field)
        _batchTimer = new Timer(ProcessBatchTimerCallback, null, _maxBatchDelay, _maxBatchDelay);

        // Configure dataflow pipeline for parallel processing with optimized settings
        var dataflowOptions = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism,
            // Allow some buffering
            BoundedCapacity = batchSize * maxDegreeOfParallelism,
            SingleProducerConstrained = false,
            EnsureOrdered = true
        };

        _heartbeatTimer = new Timer(CheckMessageActivity, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        _processingPipeline = new ActionBlock<MarketDataSnapshotFullRefresh>(
            ProcessMarketDataMessageAsync,
            dataflowOptions);

        _logger.LogDebug(
            "EnhancedFixListener initialized with batch size {BatchSize} and parallelism degree {Parallelism}",
            _batchSize, maxDegreeOfParallelism);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public bool IsInitialService { get; set; } = false;

    public void SendMessage(Message message)
    {
        try
        {
            if (_sessionId != null)
            {
                var session = Session.LookupSession(_sessionId);
                if (session is { IsLoggedOn: true })
                {
                    session.Send(message);
                    _logger.LogDebug("Message sent.");
                }
                else
                {
                    _logger.LogDebug("Session is not logged on.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message: {Message}", ex.Message);
        }
    }

    public void FromApp(Message message, SessionID sessionID)
    {
        try
        {
            // Reset heartbeat counter when any application message is received
            HandleNonHeartbeatMessage();
            Crack(message, sessionID);
        }
        catch (SocketException ex) when (ex.SocketErrorCode is SocketError.ConnectionAborted
                                             or SocketError.ConnectionRefused
                                             or SocketError.ConnectionReset)
        {
            NeedToRestart(sessionID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message: {Message}", ex.Message);
        }
    }

    public void OnCreate(SessionID sessionID)
    {
        _logger.LogInformation("Session created: {SessionID}", sessionID);
        _sessionId = sessionID;
    }

    public void OnLogout(SessionID sessionID)
    {
        _logger.LogInformation("Session logout: {SessionID}", sessionID);
        _isSessionLogin = false;

        // Execute synchronously
        FlushPendingMessages().GetAwaiter().GetResult();

        if (_isNeedToRestart)
        {
            _isNeedToRestart = false;

            // Execute synchronously
            Thread.Sleep(1000);
            NotifySubscriptionServiceLogout(sessionID).GetAwaiter().GetResult();
        }
    }

    public void OnLogon(SessionID sessionID)
    {
        _logger.LogInformation("Session logon successful: {SessionID}", sessionID);
        _isSessionLogin = true;

        if (!IsInitialService && _isDataRecovery)
            Task.Run(async () => await _dataRecoveryService.RecoverData());
    }

    public void FromAdmin(Message message, SessionID sessionID)
    {
        // Update last message time for all admin messages
        _lastMessageTime = DateTime.UtcNow;

        var msgType = message.Header.GetString(Tags.MsgType);
        _logger.LogDebug("Received admin message of type {MsgType}", msgType);

        if (msgType.Equals(MsgType.HEARTBEAT, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogDebug("Received heartbeat from counterparty");
            HeartbeatHandler(sessionID);
        }
        else
        {
            if (msgType.Equals(MsgType.REJECT, StringComparison.OrdinalIgnoreCase))
            {
                var refTagID = message.IsSetField(Tags.RefTagID) ? message.GetInt(Tags.RefTagID) : -1;
                var refMsgType = message.IsSetField(Tags.RefMsgType) ? message.GetString(Tags.RefMsgType) : Unknown;

                _logger.LogError("Admin Reject: Tag {TagID}, MsgType {MsgType}", refTagID, refMsgType);
            }
            else if (msgType.Equals(MsgType.LOGON, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Received logon message from counterparty");
            }
            else if (msgType.Equals(MsgType.SEQUENCE_RESET, StringComparison.OrdinalIgnoreCase))
            {
                var newSeqNum = message.GetInt(Tags.NewSeqNo);
                _logger.LogWarning("Received Sequence Reset. New sequence number: {NewSeqNum}", newSeqNum);
            }

            // Reset the heartbeat counter for any non-heartbeat message
            HandleNonHeartbeatMessage();
        }
    }

    public void ToAdmin(Message message, SessionID sessionID)
    {
        var msgType = message.Header.GetString(Tags.MsgType);
        _logger.LogDebug("Sending admin message of type {MsgType}", msgType);

        switch (msgType)
        {
            case MsgType.LOGON:
                _logger.LogInformation("Sending logon message with reset sequence flag");
                if (_sessionId != null)
                    message.SetField(new ResetSeqNumFlag(true)); // Force sequence reset

                try
                {
                    var password = _settings.Get(sessionID).GetString("Password");
                    message.SetField(new Password(password));
                    _logger.LogDebug("Password field set in logon message");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error setting password in logon message");
                }

                break;

            case MsgType.LOGOUT:
                message.SetField(new Text("*logout*"));
                break;

            case MsgType.TEST_REQUEST:
                _ = message.GetString(Tags.TestReqID);
                FixUtil.IsFixAvailable = true;
                break;

            case MsgType.REJECT:
                var refTagField = message.IsSetField(Tags.RefTagID) ? message.GetString(Tags.RefTagID) : Unknown;
                var refMsgType = message.IsSetField(Tags.RefMsgType) ? message.GetString(Tags.RefMsgType) : Unknown;
                var symbol = string.Empty;

                if (message.IsSetField(Tags.Symbol)) // Check if tag 55 exists
                    symbol = message.GetString(Tags.Symbol);

                if (!string.IsNullOrEmpty(symbol))
                    _logger.LogWarning("To Admin (REJECT) - Rejected Tag: {Tag}, MsgType: {MsgType}, {Symbol}",
                        refTagField,
                        refMsgType,
                        symbol);
                else
                    _logger.LogWarning("To Admin (REJECT) - Rejected Tag: {Tag}, MsgType: {MsgType}",
                        refTagField,
                        refMsgType);

                RejectedHandler(sessionID);
                break;
        }
    }

    public void ToApp(Message message, SessionID sessionID)
    {
        try
        {
            // Directly use DateTime.UtcNow for SendingTime
            var sendingTimeUtc = DateTime.UtcNow;
            message.Header.SetField(new SendingTime(sendingTimeUtc));

            if (message.Header.IsSetField(Tags.PossDupFlag) && message.Header.GetBoolean(Tags.PossDupFlag))
            {
                var origSendingTime = message.Header.GetDateTime(Tags.SendingTime);
                message.Header.SetField(new OrigSendingTime(origSendingTime));

                // Update SendingTime again in case of duplicate message
                message.Header.SetField(new SendingTime(sendingTimeUtc));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message: {Message}", ex.Message);
        }
    }

    public bool CheckSession()
    {
        return _isSessionLogin;
    }

    private void RejectedHandler(SessionID sessionID)
    {
        var shouldRestart = false;
        lock (_rejectCountLock)
        {
            _rejectCount++;
            if (_rejectCount >= _maxRejectCount)
            {
                _rejectCount = 0;
                shouldRestart = true;
            }
        }

        if (shouldRestart)
            NeedToRestart(sessionID);
    }

    private void HeartbeatHandler(SessionID sessionID)
    {
        // Check for timeout between heartbeats
        var shouldRestart = false;
        lock (_heartbeatLock)
        {
            // Increment the consecutive heartbeat counter
            _consecutiveHeartbeatCount++;
            _logger.LogInformation("Consecutive heartbeat count: {HeartbeatCount}", _consecutiveHeartbeatCount);

            if (_consecutiveHeartbeatCount >= _maxConsecutiveHeartbeats)
            {
                shouldRestart = true;
                _consecutiveHeartbeatCount = 0;
                _logger.LogWarning(
                    "Reached {MaxHeartbeats} consecutive heartbeats without other messages, triggering service restart",
                    _maxConsecutiveHeartbeats);
            }
        }

        if (shouldRestart)
            NeedToRestart(sessionID);
    }

    private void ResetConsecutiveHeartbeatCounter(string reason)
    {
        lock (_heartbeatLock)
        {
            if (_consecutiveHeartbeatCount > 0)
                _logger.LogDebug("Resetting consecutive heartbeat counter from {Count} to 0. Reason: {Reason}",
                    _consecutiveHeartbeatCount, reason);

            _consecutiveHeartbeatCount = 0;
        }
    }

    private void HandleNonHeartbeatMessage()
    {
        ResetConsecutiveHeartbeatCounter("Received non-heartbeat message");
    }

    private void CheckMessageActivity(object? state)
    {
        try
        {
            var now = DateTime.UtcNow;
            var elapsed = now - _lastMessageTime;

            // If there's been recent activity, no need to check further
            if (elapsed < _heartbeatResetTimeout)
                return;

            // If we've had consecutive heartbeats but there's been a gap, reset the counter
            lock (_heartbeatLock)
            {
                if (_consecutiveHeartbeatCount > 0)
                    ResetConsecutiveHeartbeatCounter($"Timeout: No messages for {elapsed.TotalSeconds:F1} seconds");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in heartbeat activity monitor");
        }
    }

    private void NeedToRestart(SessionID sessionID)
    {
        _isNeedToRestart = true;
        _isSessionLogin = true;
        OnLogout(sessionID);
    }

    private ILogoutNotificationHandler LogoutNotificationHandler()
    {
        return _serviceProvider.GetRequiredService<ILogoutNotificationHandler>();
    }

    private async Task NotifySubscriptionServiceLogout(SessionID sessionID)
    {
        try
        {
            var _logoutNotificationHandler = LogoutNotificationHandler();
            await _logoutNotificationHandler.NotifyLogout(sessionID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying subscription service about logout");
        }
    }

    private async Task ProcessMarketDataMessageAsync(MarketDataSnapshotFullRefresh message)
    {
        if (_disposed)
            return;

        try
        {
            // Add to batch buffer
            _batchBuffer.Enqueue(message);

            // Check if we have enough messages to process a batch
            if (_batchBuffer.Count >= _batchSize)
                await ProcessBatchAsync();

            // Reset error counter after successful processing
            _consecutiveErrorCount = 0;
        }
        catch (Exception ex)
        {
            // Increment error counter
            Interlocked.Increment(ref _consecutiveErrorCount);

            _logger.LogError(ex, "Error enqueueing market data message for symbol {Symbol}",
                message.GetString(Tags.Symbol));

            // Check if we've hit too many consecutive errors
            if (_consecutiveErrorCount > 10)
            {
                _logger.LogCritical("Too many consecutive errors processing market data messages");

                // Try to recover by flushing the buffer
                try
                {
                    await FlushPendingMessages();
                    _consecutiveErrorCount = 0;
                }
                catch (Exception flushEx)
                {
                    _logger.LogError(flushEx, "Failed to flush buffer during error recovery");
                }
            }

            // Fallback: try to process individually
            try
            {
                await _stockPublisher.EnqueueMessageAsync(message);
            }
            catch (Exception pubEx)
            {
                _logger.LogError(pubEx, "Fallback individual processing failed for symbol {Symbol}",
                    message.GetString(Tags.Symbol));
            }
        }
    }

    private void ProcessBatchTimerCallback(object? state)
    {
        if (_disposed)
            return;

        try
        {
            // Only process if there are messages and not already processing another batch
            if (!_batchBuffer.IsEmpty && _batchSemaphore.CurrentCount > 0)
            {
                // Capture current time before processing
                var startTime = Stopwatch.GetTimestamp();
                ProcessBatchAsync().GetAwaiter().GetResult();

                // Log processing time for monitoring if it takes too long
                var elapsed = Stopwatch.GetElapsedTime(startTime);
                if (elapsed > TimeSpan.FromMilliseconds(50))
                    _logger.LogWarning("Batch processing took {ElapsedMs}ms", elapsed.TotalMilliseconds);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in batch timer callback");
        }
    }

    private async Task ProcessBatchAsync()
    {
        if (_disposed)
            return;

        // Use semaphore to ensure only one batch processing at a time
        // Reduced wait time for better throughput
        var lockAcquired = await _batchSemaphore.WaitAsync(TimeSpan.FromMilliseconds(5));
        if (!lockAcquired) // Another thread is already processing, we'll try again later
            return;

        try
        {
            var batchMessages = new List<MarketDataSnapshotFullRefresh>(_batchSize);

            // Drain the current batch
            while (batchMessages.Count < _batchSize && _batchBuffer.TryDequeue(out var message))
                batchMessages.Add(message);

            if (batchMessages.Count == 0)
                return;

            // Process the batch
            var batchStopwatch = Stopwatch.StartNew();

            // Process in parallel but with proper error handling for each message
            var tasks = new List<Task>(_batchSize);
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var message in batchMessages)
                tasks.Add(SafeEnqueueMessageAsync(message));

            // Wait for all messages to be enqueued with timeout
            var allTasksCompleted = Task.WaitAll(tasks.ToArray(), TimeSpan.FromMilliseconds(500));
            if (!allTasksCompleted)
                _logger.LogWarning("Batch processing timed out after 500ms for {Count} messages", batchMessages.Count);

            batchStopwatch.Stop();

            // Update message count for performance monitoring
            Interlocked.Add(ref _messageCount, batchMessages.Count);

            // Check if we should report performance metrics
            ReportPerformanceIfNeeded();

            _logger.LogDebug(
                "Processed batch of {Count} messages in {ElapsedMs}ms ({AvgMs}ms/message)",
                batchMessages.Count,
                batchStopwatch.ElapsedMilliseconds,
                batchStopwatch.ElapsedMilliseconds / (double)batchMessages.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing batch");
            Interlocked.Increment(ref _consecutiveErrorCount);
        }
        finally
        {
            _batchSemaphore.Release();
        }
    }

    private async Task SafeEnqueueMessageAsync(MarketDataSnapshotFullRefresh message)
    {
        try
        {
            await _stockPublisher.EnqueueMessageAsync(message).AsTask();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message for symbol {Symbol}",
                message.GetString(Tags.Symbol));
        }
    }

    private void ReportPerformanceIfNeeded()
    {
        var now = DateTime.UtcNow;
        if (now - _lastPerformanceReport >= _performanceReportInterval)
        {
            var currentCount = Interlocked.Read(ref _messageCount);
            var newMessages = currentCount - _lastReportedCount;

            // Calculate messages per second
            var messagesPerSecond = newMessages / _performanceReportInterval.TotalSeconds;

            _logger.LogInformation(
                "Performance: {MPS:F1} messages/sec (Total: {Total}, Period: {Period}s, Backlog: {Backlog})",
                messagesPerSecond,
                currentCount,
                _performanceReportInterval.TotalSeconds,
                _batchBuffer.Count);

            _lastReportedCount = currentCount;
            _lastPerformanceReport = now;
        }
    }

    private async Task FlushPendingMessages()
    {
        _logger.LogDebug("Flushing pending messages");

        // Process remaining batches
        while (!_batchBuffer.IsEmpty && !_disposed)
            await ProcessBatchAsync();

        // Complete the pipeline
        if (!_disposed)
        {
            _processingPipeline.Complete();

            try
            {
                // Wait with timeout to avoid blocking indefinitely
                var completedTask = await Task.WhenAny(
                    _processingPipeline.Completion,
                    Task.Delay(TimeSpan.FromMinutes(20))
                );

                if (completedTask != _processingPipeline.Completion)
                    _logger.LogWarning("Pipeline completion timed out after 3 seconds");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error waiting for pipeline completion");
            }
        }

        _logger.LogDebug("All pending messages processed");
    }
#pragma warning disable CS1998
    public async void OnMessage(MarketDataSnapshotFullRefresh message, SessionID sessionID)
#pragma warning restore CS1998
    {
        try
        {
            // Track message reception (even before successful processing)
            Interlocked.Increment(ref _messageCount);

            // Fast path for when pipeline isn't backed up
            // This optimizes for low latency in normal conditions
            if (_batchBuffer.Count < _batchSize / 2)
            {
                // Post to the processing pipeline for parallel processing
                var success = _processingPipeline.Post(message);
                if (!success)
                {
                    // If pipeline is full, add to batch buffer
                    _logger.LogDebug("Processing pipeline full, adding to batch buffer");
                    _batchBuffer.Enqueue(message);
                }
            }
            else
            {
                // Add directly to batch buffer when we're already accumulating a batch
                _batchBuffer.Enqueue(message);

                // If buffer is getting too large, trigger processing
                if (_batchBuffer.Count >= _batchSize && _batchSemaphore.CurrentCount > 0)
                    _ = Task.Run(async () => await ProcessBatchAsync());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing market data snapshot for symbol {Symbol} in session {Session}",
                message.GetString(Tags.Symbol), sessionID);

            // Attempt direct processing as last resort
            try
            {
                await _stockPublisher.EnqueueMessageAsync(message);
            }
            catch (Exception pubEx)
            {
                _logger.LogError(pubEx, "Emergency direct processing failed for {Symbol}",
                    message.GetString(Tags.Symbol));
            }
        }
    }
#pragma warning disable CS1998
    public async void OnMessage(SecurityList message, SessionID sessionID)
#pragma warning restore CS1998
    {
        try
        {
            _ = message.GetString(Tags.SecurityReqID);
            var noRelatedSym = FixListenerOptimizedHelper.GetNoRelatedSym(message);

            // Process securities asynchronously
            var securities = await Task.Run(() => ProcessSecurities(message, noRelatedSym));
            var jsonResponse = FixListenerOptimizedHelper.SerializeSecurities(securities);

            _logger.LogWarning("HandleResponse message {Message}", jsonResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SecurityList message: {Message} for {Session}",
                ex.Message, sessionID);
        }
    }
#pragma warning disable CS1998
    public async void OnMessage(BusinessMessageReject message, SessionID sessionID)
#pragma warning restore CS1998
    {
        try
        {
            var refSeqNum = message.GetInt(Tags.RefSeqNum);
            var refMsgType = message.GetString(Tags.RefMsgType);
            var businessRejectReason = message.GetInt(Tags.BusinessRejectReason);
            var text = message.IsSetField(Tags.Text) ? message.GetString(Tags.Text) : string.Empty;
            var response =
                $"Received Business Message Reject: RefSeqNum={refSeqNum}, RefMsgType={refMsgType}, BusinessRejectReason={businessRejectReason}, Text={text}";

            _logger.LogWarning("The response: {Response} for {Session}", response, sessionID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing BusinessMessageReject for {Session}: {Message}",
                sessionID, ex.Message);
        }
    }
#pragma warning disable CS1998
    public async void OnMessage(MarketDataRequestReject message, SessionID sessionID)
#pragma warning restore CS1998
    {
        try
        {
            var text = message.IsSetField(Tags.Text) ? message.GetString(Tags.Text) : string.Empty;
            var mdReqID = message.IsSetField(Tags.MDReqID) ? message.GetString(Tags.MDReqID) : Unknown;
            var symbol = message.IsSetField(Tags.Symbol) ? message.GetString(Tags.Symbol) : Unknown;
            var rejectReason = -1;

            if (message.IsSetField(Tags.MDReqRejReason))
            {
                var rejectReasonStr = message.GetString(Tags.MDReqRejReason);
                if (!int.TryParse(rejectReasonStr, out rejectReason))
                {
                    _logger.LogWarning("Received non-numeric MDReqRejReason: {RejectReasonStr}", rejectReasonStr);
                    rejectReason = -1;
                }
            }

            // Log the rejection details
            _logger.LogWarning(
                "Market Data Request Rejected: ReqID={MDReqID}, Symbol={Symbol}, Reason={RejectReason}, Text={Text}",
                mdReqID, symbol, rejectReason, text);

            // Handle specific reject reasons based on FIX 4.4 spec values
            switch (rejectReason)
            {
                case 0:
                    _logger.LogError("Unknown symbol in request {MDReqID}", mdReqID);
                    break;
                case 1:
                    _logger.LogError("Duplicate MDReqID {MDReqID}", mdReqID);
                    break;
                case 2:
                    _logger.LogError("Insufficient bandwidth for request {MDReqID}", mdReqID);
                    break;
                case 3:
                    _logger.LogError("Insufficient permissions for request {MDReqID}", mdReqID);
                    break;
                case 4:
                    _logger.LogError("Unsupported subscription request type for request {MDReqID}", mdReqID);
                    break;
                case 5:
                    _logger.LogError("Unsupported market depth for request {MDReqID}", mdReqID);
                    break;
                case 6:
                    _logger.LogError(
                        "Unsupported MDUpdateType for request {MDReqID}. Only values 0 (FULL_REFRESH) and 1 (INCREMENTAL_REFRESH) are supported",
                        mdReqID);
                    break;
                case 7:
                    _logger.LogError("Unsupported aggregated book for request {MDReqID}", mdReqID);
                    break;
                case 8:
                    _logger.LogError("Unsupported MDEntryType for request {MDReqID}", mdReqID);
                    break;
                case 9:
                    _logger.LogError("Unsupported TradingSessionID for request {MDReqID}", mdReqID);
                    break;
                case 10:
                    _logger.LogError("Unsupported scope for request {MDReqID}", mdReqID);
                    break;
                case 11:
                    _logger.LogError("Unsupported open/close settle flag for request {MDReqID}", mdReqID);
                    break;
                case 12:
                    _logger.LogError("Unsupported MDImplicitDelete for request {MDReqID}", mdReqID);
                    break;
                default:
                    _logger.LogError("Market data request {MDReqID} rejected with reason code {RejectReason}",
                        mdReqID, rejectReason);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MarketDataRequestReject message for session {Session}",
                sessionID);
        }
    }
#pragma warning disable CS1998
    public async void OnMessage(Reject message, SessionID sessionID)
#pragma warning restore CS1998
    {
        try
        {
            var refSeqNum = message.GetInt(Tags.RefSeqNum);
            var refMsgType = message.GetString(Tags.RefMsgType);
            var rejectReason = message.GetInt(Tags.SessionRejectReason);
            var text = message.IsSetField(Tags.Text) ? message.GetString(Tags.Text) : string.Empty;

            var response =
                $"Received Message Reject: RefSeqNum={refSeqNum}, RefMsgType={refMsgType}, SessionRejectReason={rejectReason}, Text={text}";

            _logger.LogWarning("The response: {Response} for {Session}", response, sessionID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Reject for {Session}: {Message}",
                sessionID, ex.Message);
        }
    }

    private static List<SecurityInfo> ProcessSecurities(FieldMap message, int noRelatedSym)
    {
        var securities = new List<SecurityInfo>();

        if (noRelatedSym == 0)
            return securities;

        var group = new SecurityList.NoRelatedSymGroup();
        for (var i = 1; i <= noRelatedSym; i++)
        {
            message.GetGroup(i, group);
            securities.Add(FixListenerOptimizedHelper.CreateSecurityInfo(group));
        }

        return securities;
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
            try
            {
                _disposed = true;
                _batchTimer.Dispose();
                _processingPipeline.Complete();

                // Give the pipeline a chance to complete but don't wait too long
                try
                {
                    Task.WaitAll([_processingPipeline.Completion], TimeSpan.FromSeconds(3));
                }
                catch (TimeoutException ex)
                {
                    _logger.LogWarning(ex, "Timed out waiting for processing pipeline to complete");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during pipeline shutdown");
                }

                _batchSemaphore.Dispose();
                _heartbeatTimer.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during EnhancedFixListener disposal");
            }

        _disposed = true;
    }

    ~EnhancedFixListener()
    {
        Dispose(false);
    }
}