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
public class FixListener : MessageCracker, IFixListener
{
    private const string Unknown = "Unknown";
    private readonly IDataRecoveryService _dataRecoveryService;
    private readonly bool _isDataRecovery;
    private readonly ILogger<FixListener> _logger;
    private readonly SessionSettings _settings;
    private readonly IStockDataOptimizedV2Publisher _stockPublisher;
    private bool _isSessionLogin;
    private SessionID? _sessionId;

    public FixListener(
        string configFilePath,
        IDataRecoveryService dataRecoveryService,
        IStockDataOptimizedV2Publisher stockPublisher,
        ILogger<FixListener> logger)
    {
        _isSessionLogin = false;
        _isDataRecovery = false;

        if (string.IsNullOrEmpty(configFilePath))
            throw new ArgumentNullException(nameof(configFilePath), "Configuration file path cannot be null or empty.");

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dataRecoveryService = dataRecoveryService ?? throw new ArgumentNullException(nameof(dataRecoveryService));
        _stockPublisher =
            stockPublisher ?? throw new ArgumentNullException(nameof(stockPublisher));

        using var reader = new StreamReader(configFilePath);
        _settings = new SessionSettings(reader);
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
            Crack(message, sessionID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message: {Message}", ex.Message);
        }
    }

    public void OnCreate(SessionID sessionID)
    {
        _sessionId = sessionID;
    }

    public void OnLogout(SessionID sessionID)
    {
        _isSessionLogin = false;
    }

    public void OnLogon(SessionID sessionID)
    {
        _isSessionLogin = true;

        if (!IsInitialService && _isDataRecovery)
            Task.Run(async () => await _dataRecoveryService.RecoverData());
    }

    public void FromAdmin(Message message, SessionID sessionID)
    {
        var msgType = message.Header.GetString(Tags.MsgType);
        if (msgType.Equals(MsgType.REJECT, StringComparison.OrdinalIgnoreCase))
        {
            // Set session login state to false

            var refTagID = message.IsSetField(Tags.RefTagID) ? message.GetInt(Tags.RefTagID) : -1;
            var refMsgType = message.IsSetField(Tags.RefMsgType) ? message.GetString(Tags.RefMsgType) : Unknown;

            _logger.LogError("Admin Reject: Tag {TagID}, MsgType {MsgType}", refTagID, refMsgType);
        }

        if (msgType.Equals(MsgType.SEQUENCE_RESET, StringComparison.OrdinalIgnoreCase))
        {
            var newSeqNum = message.GetInt(Tags.NewSeqNo);
            _logger.LogWarning("Received Sequence Reset. New sequence number: {NewSeqNum}", newSeqNum);
        }
    }

    public void ToAdmin(Message message, SessionID sessionID)
    {
        switch (message.Header.GetString(Tags.MsgType))
        {
            case MsgType.LOGON:
                if (_sessionId != null)
                    message.SetField(new ResetSeqNumFlag(true)); // Force sequence reset
                message.SetField(new Password(_settings.Get(sessionID).GetString("Password")));
                break;
            case MsgType.LOGOUT:
                message.SetField(new Text("*logout*"));
                break;
            case MsgType.TEST_REQUEST:
                _ = message.GetString(Tags.TestReqID);
                FixUtil.IsFixAvailable = true;
                break;
            case MsgType.REJECT:
                // Set session login state to false

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
                break;
        }
    }

    public void ToApp(Message message, SessionID sessionID)
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

    public bool CheckSession()
    {
        return _isSessionLogin;
    }

    public async void OnMessage(MarketDataSnapshotFullRefresh message, SessionID sessionID)
    {
        try
        {
            // Enqueue message
            await _stockPublisher.EnqueueMessageAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing market data snapshot for symbol {Symbol} in session {Session}",
                message.GetString(Tags.Symbol), sessionID);
        }
    }

    public async void OnMessage(SecurityList message, SessionID sessionID)
    {
        try
        {
            _ = message.GetString(Tags.SecurityReqID);
            var noRelatedSym = FixListenerOptimizedHelper.GetNoRelatedSym(message);

            // Process securities asynchronously
            var securities = await Task.Run(() => ProcessSecurities(message, noRelatedSym));
            var jsonResponse = FixListenerOptimizedHelper.SerializeSecurities(securities);

            _logger.LogWarning("HandleResponse message {Message}", jsonResponse);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SecurityList message: {Message} for {Session}",
                ex.Message, sessionID);
        }
    }

    public async void OnMessage(BusinessMessageReject message, SessionID sessionID)
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

            // Set session login state to false

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing BusinessMessageReject for {Session}: {Message}",
                sessionID, ex.Message);
        }
    }

    public void OnMessage(MarketDataRequestReject message, SessionID sessionID)
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

    public async void OnMessage(Reject message, SessionID sessionID)
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

            // Set session login state to false

            await Task.CompletedTask;
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
}