using System.Net;
using NCrontab;
using Newtonsoft.Json;
using Pi.SetMarketDataRealTime.Application.Helpers;
using Pi.SetMarketDataRealTime.Application.Interfaces.Holiday;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Interfaces.MessageValidator;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.DataHandler.Exceptions;
using Pi.SetMarketDataRealTime.DataHandler.Helpers;
using Pi.SetMarketDataRealTime.DataHandler.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.DataHandler.Models;
using Pi.SetMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.AmazonS3;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;
using Polly;

namespace Pi.SetMarketDataRealTime.DataHandler.Services.SoupBinTcp;

public class ClientSubscriptionServiceAutoRestart : BackgroundService, IDisconnectionHandler
{
    private const string ErrorMessageTemplate = "{0}.{1}";
    private static readonly TimeSpan HolidayCacheDuration = TimeSpan.FromMinutes(15);
    private readonly IClientFactory _clientFactory;
    private readonly IClientListener _clientListener;
    private readonly IConfiguration _configuration;
    private readonly bool _holidayApiIsActivated;
    private readonly ILogger<ClientSubscriptionServiceAutoRestart> _logger;
    private readonly IMemoryCacheHelper _memoryCacheHelper;
    private readonly IMessageValidator _messageValidator;
    private readonly SemaphoreSlim _reconnectionSemaphore = new(1, 1);
    private readonly bool _runLocalMode;
    private readonly IAmazonS3Service _s3Service;
    private readonly IServiceProvider _serviceProvider;
    private readonly SettradeGatewaySettings _settradeGatewaySettings = new();
    private readonly SemaphoreSlim _taskManagementSemaphore = new(1, 1);
    private CancellationTokenSource _clientCts = new();
    private CrontabSchedule _closeSchedule;

    private volatile bool _forceRunOnStartup;
    private volatile bool _isClientRunning;
    private volatile bool _isSubscriptionRunning;

    private IClient? _localClient;

    private CancellationTokenSource _loginRejectionCts = new();
    private DateTime _nextCloseRun;
    private DateTime _nextOpenRun;
    private CrontabSchedule _openSchedule;
    private IClient? _productionClient;
    private Task? _subscriptionTask;

    public ClientSubscriptionServiceAutoRestart(
        ClientSubscriptionDependencies dependencies,
        IConfiguration configuration,
        ILogger<ClientSubscriptionServiceAutoRestart> logger,
        IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(dependencies);

        // Its purpose is to indicate whether the application should force itself to run on startup
        _forceRunOnStartup = configuration.GetValue(ConfigurationKeys.SettradeRunOnStartup, true);

        _clientFactory = dependencies.ClientFactory ?? throw new ArgumentNullException(
            string.Format(ErrorMessageTemplate, nameof(dependencies), nameof(dependencies.ClientFactory)));
        _clientListener = dependencies.ClientListener ?? throw new ArgumentNullException(
            string.Format(ErrorMessageTemplate, nameof(dependencies), nameof(dependencies.ClientListener)));
        _messageValidator = dependencies.MessageValidator ?? throw new ArgumentNullException(
            string.Format(ErrorMessageTemplate, nameof(dependencies), nameof(dependencies.MessageValidator)));
        _memoryCacheHelper = dependencies.MemoryCacheHelper ?? throw new ArgumentNullException(
            string.Format(ErrorMessageTemplate, nameof(dependencies), nameof(dependencies.MemoryCacheHelper)));
        _s3Service = dependencies.S3Service ?? throw new ArgumentNullException(
            string.Format(ErrorMessageTemplate, nameof(dependencies), nameof(dependencies.S3Service)));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _runLocalMode = configuration.GetValue(ConfigurationKeys.SettradeRunLocalMode, false);
        _holidayApiIsActivated = true;

        // Initialize with default values
        _openSchedule =
            CrontabSchedule.Parse("0 6 * * 1-5", new CrontabSchedule.ParseOptions { IncludingSeconds = false });
        _closeSchedule =
            CrontabSchedule.Parse("30 3 * * 1-6", new CrontabSchedule.ParseOptions { IncludingSeconds = false });

        // Initialize server configurations
        InitializeServerConfigurations();
    }

    public async Task HandleLoginRejectedDisconnectionAsync(CancellationToken cancellationToken = default)
    {
        await _reconnectionSemaphore.WaitAsync(cancellationToken);
        try
        {
            await StopSubscriptionTask();
            await _loginRejectionCts.CancelAsync();

            _logger.LogWarning("Subscription and client stopped due to login rejection");
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Login rejection handling was cancelled");
            throw new SubscriptionServiceException("Login rejection handling was cancelled", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while handling login rejection");
            throw new SubscriptionServiceException("Error occurred while handling login rejection", ex);
        }
        finally
        {
            _reconnectionSemaphore.Release();
        }
    }

    public async Task HandleUnexpectedDisconnectionAsync(CancellationToken cancellationToken = default)
    {
        await _reconnectionSemaphore.WaitAsync(cancellationToken);
        try
        {
            _logger.LogWarning("Handling unexpected disconnection");
            await StopSubscriptionTask();
        }
        finally
        {
            _reconnectionSemaphore.Release();
        }
    }

    private void InitializeServerConfigurations()
    {
        try
        {
            // Initialize server configurations
            _configuration.GetSection(ConfigurationKeys.SettradeGatewaySettings).Bind(_settradeGatewaySettings);

            // Environment Variables Injection
            var servers = _configuration.GetValue(ConfigurationKeys.SettradeGatewaySettingsServers, string.Empty)?
                .SimpleCleanJsonMessage();

            if (!string.IsNullOrEmpty(servers))
            {
                _settradeGatewaySettings.SERVERS = [];
                _settradeGatewaySettings.SERVERS = JsonConvert.DeserializeObject<List<ServerConfig?>>(servers);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing server gateway configurations.");
            throw new InvalidOperationException("Error deserializing server gateway configurations.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error binding server gateway configurations.");
            throw new InvalidOperationException("Error binding server gateway configurations.");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var combinedCts =
                CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, _loginRejectionCts.Token);

            InitializeSchedules();
            UpdateNextRunTimes(ClientSubscriptionServiceHelper.GetCurrentThailandTime());

            if (_holidayApiIsActivated)
                await InitializeHolidayInfo();

            while (!combinedCts.IsCancellationRequested)
                try
                {
                    var thaiNow = ClientSubscriptionServiceHelper.GetCurrentThailandTime();

                    UpdateNextRunTimes(thaiNow);

                    _logger.LogDebug("Checking schedule at {ThaiTime}",
                        ClientSubscriptionServiceHelper.FormatThaiDateTime(thaiNow));

                    if (!await IsNotBusinessDays())
                        await HandleSubscriptionStart(thaiNow, combinedCts.Token);
                    else
                        await HandleSubscriptionStopIfIsNotBusinessDays();

                    // The code below is used to check if it's time to shut down the subscription service every day
                    await HandleSubscriptionStop(thaiNow);
                    await Task.Delay(TimeSpan.FromSeconds(10), combinedCts.Token);
                }
                catch (OperationCanceledException ex)
                {
                    var message = _loginRejectionCts.IsCancellationRequested
                        ? "Service is stopping due to login rejection."
                        : "Service is stopping due to cancellation request.";

                    _logger.LogWarning(ex, "{Message}", message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the main execution loop. Continuing execution.");
                    await Task.Delay(TimeSpan.FromSeconds(1), combinedCts.Token);
                }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An unhandled exception occurred in the main execution loop");
        }
        finally
        {
            _logger.LogDebug("ExecuteAsync completed.");
            await StopSubscriptionTask();
        }
    }

    private async Task HandleSubscriptionStopIfIsNotBusinessDays()
    {
        var dateNow = ClientSubscriptionServiceHelper.GetCurrentThailandTime();
        var closeSchedule = CrontabSchedule.Parse(
            _configuration[ConfigurationKeys.SettradeStockMarketStopCronJob],
            new CrontabSchedule.ParseOptions { IncludingSeconds = false });
        var nextOccurrence = closeSchedule.GetNextOccurrence(dateNow);

        _logger.LogDebug("ThaiNowTime: {ThaiNowTime} NextOccurrenceTime: {NextOccurrenceTime}",
            dateNow.TimeOfDay, nextOccurrence.TimeOfDay);

        if (dateNow.TimeOfDay >= nextOccurrence.TimeOfDay)
            await StopSubscriptionTask();
    }

    private async Task InitializeHolidayInfo()
    {
        try
        {
            bool isNotBusinessDays;
            var cacheKey = ClientSubscriptionServiceHelper.GetCurrentThailandTime().ToString("yyyyMMdd");
            using (var scope = _serviceProvider.CreateScope())
            {
                var holidayQuery = scope.ServiceProvider.GetRequiredService<IHolidayApiQuery>();
                isNotBusinessDays = await holidayQuery.IsNotBusinessDays();
            }

            await _memoryCacheHelper.SetCurrentIsNotBusinessDaysAsync(cacheKey, isNotBusinessDays.ToString(),
                HolidayCacheDuration);

            _logger.LogDebug("Initialized holiday information: IsNotBusinessDays = {IsNotBusinessDays}",
                isNotBusinessDays);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing business days. Service may not function correctly.");

            // Depending on how critical this initialization is, the application might want to:
            // 1. Rethrow the exception if it's absolutely necessary for the service to function
            // 2. Set a default value and continue
            throw new BusinessHourInitializationException("Failed to initialize business days. Cannot continue.", ex);
        }
    }

    private async Task<bool> IsNotBusinessDays(DateTime? date = null)
    {
        if (!_holidayApiIsActivated)
            return false;

        try
        {
            var cacheKey = date?.ToString("yyyyMMdd") ??
                           ClientSubscriptionServiceHelper.GetCurrentThailandTime().ToString("yyyyMMdd");
            var isNotBusinessDaysCache = await _memoryCacheHelper.GetCurrentIsNotBusinessDaysAsync(cacheKey);

            if (string.IsNullOrEmpty(isNotBusinessDaysCache))
            {
                bool isNotBusinessDays;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var holidayQuery = scope.ServiceProvider.GetRequiredService<IHolidayApiQuery>();
                    isNotBusinessDays = await holidayQuery.IsNotBusinessDays(date);
                }

                await _memoryCacheHelper.SetCurrentIsNotBusinessDaysAsync(cacheKey, isNotBusinessDays.ToString(),
                    HolidayCacheDuration);

                return isNotBusinessDays;
            }

            return bool.TryParse(isNotBusinessDaysCache, out var result) && result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error determining business days.");
            return false;
        }
    }

    private void InitializeSchedules()
    {
        try
        {
            // Parse original cron expressions
            _openSchedule = CrontabSchedule.Parse(
                _configuration[ConfigurationKeys.SettradeStockMarketStartCronJob],
                new CrontabSchedule.ParseOptions { IncludingSeconds = false });

            _closeSchedule = CrontabSchedule.Parse(
                _configuration[ConfigurationKeys.SettradeStockMarketStopCronJob],
                new CrontabSchedule.ParseOptions { IncludingSeconds = false });

            var thaiNow = ClientSubscriptionServiceHelper.GetCurrentThailandTime();

            // Validate schedules
            ValidateSchedules(thaiNow);

            _logger.LogDebug("Schedules initialized with expressions. Open: {OpenSchedule}, Close: {CloseSchedule}",
                _configuration[ConfigurationKeys.SettradeStockMarketStartCronJob],
                _configuration[ConfigurationKeys.SettradeStockMarketStopCronJob]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing schedules.");
            throw new InvalidOperationException("Error initializing schedules.", ex);
        }
    }

    private void ValidateSchedules(DateTime thaiNow)
    {
        var currentDate = thaiNow.Date;
        var openTime = _openSchedule.GetNextOccurrence(currentDate);
        var closeTime = _closeSchedule.GetNextOccurrence(openTime);

        // Log the actual times that will be used
        _logger.LogDebug("Schedule validation - Next open time: {OpenTime}, Next close time: {CloseTime}",
            ClientSubscriptionServiceHelper.FormatThaiDateTime(openTime),
            ClientSubscriptionServiceHelper.FormatThaiDateTime(closeTime));

        // Verify the schedules make sense
        if (closeTime <= openTime)
        {
            var message =
                $"Invalid schedule configuration. Close time ({closeTime}) must be after open time ({openTime})";
            _logger.LogError("{Message}", message);
            throw new InvalidOperationException(message);
        }
    }

    private async Task HandleSubscriptionStart(DateTime thaiNow, CancellationToken cancellationToken)
    {
        await _reconnectionSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (ShouldStartSubscription(thaiNow, true) && await HandleSubscriptionStartOnDayOfWeek())
            {
                _logger.LogDebug("Stock market opened at {ThaiTime}. Starting data subscription...",
                    ClientSubscriptionServiceHelper.FormatThaiDateTime(thaiNow));
                await StartSubscriptionTask();

                UpdateNextRunTimes(thaiNow);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Failed to start subscription");
        }
        finally
        {
            _reconnectionSemaphore.Release();
        }
    }

    private async Task HandleSubscriptionStop(DateTime thaiNow)
    {
        if (ShouldStopSubscription(thaiNow))
        {
            _logger.LogDebug("Stock market closed at {ThaiTime}. Stopping data subscription...",
                ClientSubscriptionServiceHelper.FormatThaiDateTime(thaiNow));

            await StopSubscriptionTask();
            await HouseKeepingService();

            UpdateNextRunTimes(thaiNow);
        }
    }

    private bool ShouldStartSubscription(DateTime thaiNow, bool useForceRunOnStartup)
    {
        if (_isSubscriptionRunning)
        {
            _logger.LogDebug("Subscription is already running. Not starting a new one.");
            return false;
        }

        var (isWithinValidTimeRange, hasMissedStart, _) =
            ClientSubscriptionServiceHelper.EvaluateSchedule(thaiNow, _nextOpenRun, _nextCloseRun,
                _isSubscriptionRunning);

        var isEquals = ClientSubscriptionServiceHelper.IsEquals(thaiNow, _nextOpenRun);
        var shouldStart = isEquals || (hasMissedStart && isWithinValidTimeRange);

        if (!shouldStart)
        {
            // Its purpose is to indicate whether the application should force itself to run on startup
            if (_forceRunOnStartup && useForceRunOnStartup)
            {
                _logger.LogWarning(
                    "The application is configured to force execution on startup. Bypassing standard initialization checks.");
                return true;
            }

            return shouldStart;
        }

        if (isEquals)
            _logger.LogDebug(
                "Current time {CurrentTime} is within one minute of next open time {NextOpenTime}. Starting subscription.",
                ClientSubscriptionServiceHelper.FormatThaiDateTime(thaiNow),
                ClientSubscriptionServiceHelper.FormatThaiDateTime(_nextOpenRun));
        else
            _logger.LogWarning(
                "Missed start detected. Current time {CurrentTime} is past the scheduled open time {NextOpenTime}. Starting subscription now.",
                ClientSubscriptionServiceHelper.FormatThaiDateTime(thaiNow),
                ClientSubscriptionServiceHelper.FormatThaiDateTime(_nextOpenRun));

        return shouldStart;
    }

    private async Task<bool> HandleSubscriptionStartOnDayOfWeek()
    {
        var dateNow = ClientSubscriptionServiceHelper.GetCurrentThailandTime();

        // Weekend check (Saturday, Sunday)
        if (dateNow.DayOfWeek is DayOfWeek.Sunday or DayOfWeek.Saturday)
        {
            _logger.LogDebug("Weekend detected: {DayOfWeek}", dateNow.DayOfWeek.ToString("G"));
            return false;
        }

        // For Monday
        if (dateNow.DayOfWeek is DayOfWeek.Monday)
        {
            _logger.LogDebug("Monday check - Current time: {CurrentTime}", dateNow.TimeOfDay);
            return ValidateStockMarketStartTime();
        }

        // For other weekdays (Tuesday-Friday)
        var yesterday = dateNow.AddDays(-1);
        var yesterdayIsNotBusinessDays = await IsNotBusinessDays(yesterday);

        if (yesterdayIsNotBusinessDays)
        {
            _logger.LogDebug("Previous day was not a business day. Checking market open time for {DayOfWeek}",
                dateNow.DayOfWeek.ToString("G"));
            return ValidateStockMarketStartTime();
        }

        _logger.LogDebug("Regular business day detected: {DayOfWeek}", dateNow.DayOfWeek.ToString("G"));
        return true;

        bool ValidateStockMarketStartTime()
        {
            var openSchedule = CrontabSchedule.Parse(_configuration[ConfigurationKeys.SettradeStockMarketStartCronJob],
                new CrontabSchedule.ParseOptions { IncludingSeconds = false });
            var nextOccurrence = openSchedule.GetNextOccurrence(dateNow);

            _logger.LogDebug("{DayOfWeek} ThaiNowTime: {ThaiNowTime} NextOccurrenceTime: {NextOccurrenceTime}",
                dateNow.DayOfWeek.ToString("G"), dateNow.TimeOfDay, nextOccurrence.TimeOfDay);

            return dateNow.TimeOfDay >= nextOccurrence.TimeOfDay;
        }
    }

    private bool ShouldStopSubscription(DateTime thaiNow)
    {
        if (!_isSubscriptionRunning)
        {
            _logger.LogDebug("No subscription is running. Nothing to stop.");

            if (_subscriptionTask is
                {
                    Status: not (TaskStatus.WaitingForActivation
                    or TaskStatus.WaitingToRun
                    or TaskStatus.WaitingForChildrenToComplete)
                })
            {
                _logger.LogWarning(
                    "Inconsistent state detected: subscription task exists but isRunning flag is false. Task status: {Status}",
                    _subscriptionTask.Status);

                _ = Task.Run(async () => await ReleaseSubscriptionTask());
            }

            return false;
        }

        var (isWithinValidTimeRange, _, hasMissedStop) =
            ClientSubscriptionServiceHelper.EvaluateSchedule(thaiNow, _nextOpenRun, _nextCloseRun,
                _isSubscriptionRunning);
        var isEquals = ClientSubscriptionServiceHelper.IsEquals(thaiNow, _nextCloseRun);

        // We should stop if:
        // 1. We're within one minute of the scheduled close time, OR
        // 2. We've missed the stop time, and we're no longer in the valid time range
        var shouldStop = isEquals || (hasMissedStop && !isWithinValidTimeRange);

        if (!shouldStop) return shouldStop;
        if (isEquals)
            _logger.LogDebug(
                "Current time {CurrentTime} is within one minute of next close time {NextCloseTime}. Stopping subscription.",
                ClientSubscriptionServiceHelper.FormatThaiDateTime(thaiNow),
                ClientSubscriptionServiceHelper.FormatThaiDateTime(_nextCloseRun));
        else
            _logger.LogWarning(
                "Missed stop detected and outside valid time range. Current time {CurrentTime} is past the scheduled close time {NextCloseTime}. Stopping subscription now.",
                ClientSubscriptionServiceHelper.FormatThaiDateTime(thaiNow),
                ClientSubscriptionServiceHelper.FormatThaiDateTime(_nextCloseRun));

        return shouldStop;
    }

    private void UpdateNextRunTimes(DateTime thaiNow)
    {
        var localNow = TimeZoneInfo.ConvertTime(thaiNow, ClientSubscriptionServiceHelper.ThailandTimeZone,
            TimeZoneInfo.Local);
        var nextOpenLocal = _openSchedule.GetNextOccurrence(localNow);
        var nextCloseLocal = _closeSchedule.GetNextOccurrence(localNow);

        _nextOpenRun = TimeZoneInfo.ConvertTime(nextOpenLocal, TimeZoneInfo.Local,
            ClientSubscriptionServiceHelper.ThailandTimeZone);
        _nextCloseRun = TimeZoneInfo.ConvertTime(nextCloseLocal, TimeZoneInfo.Local,
            ClientSubscriptionServiceHelper.ThailandTimeZone);

        var (isWithinValidTimeRange, hasMissedStart, hasMissedStop) =
            ClientSubscriptionServiceHelper.EvaluateSchedule(thaiNow, _nextOpenRun, _nextCloseRun,
                _isSubscriptionRunning);

        if (isWithinValidTimeRange && hasMissedStart && !_isSubscriptionRunning)
        {
            _nextOpenRun = thaiNow;
            _logger.LogWarning(
                "Missed start time detected. Temporarily adjusting next open time to time: {CurrentTime}. This does not change the crontab setting.",
                ClientSubscriptionServiceHelper.FormatThaiDateTime(thaiNow));
        }

        if (!isWithinValidTimeRange && hasMissedStop && _isSubscriptionRunning)
        {
            _nextCloseRun = thaiNow;
            _logger.LogWarning(
                "Missed stop time detected. Temporarily adjusting next close time to time: {CurrentTime}. This does not change the crontab setting.",
                ClientSubscriptionServiceHelper.FormatThaiDateTime(thaiNow));
        }

        _logger.LogDebug(
            "Updated next run times. Current time (Thai): {ThaiNow}, Next market open (Thai): {OpenTime}, Next market close (Thai): {CloseTime}",
            ClientSubscriptionServiceHelper.FormatThaiDateTime(thaiNow),
            ClientSubscriptionServiceHelper.FormatThaiDateTime(_nextOpenRun),
            ClientSubscriptionServiceHelper.FormatThaiDateTime(_nextCloseRun));
    }

    private async Task StartSubscriptionTask()
    {
        try
        {
            if (_isSubscriptionRunning)
            {
                _logger.LogWarning("The subscription task is already running. Skipping start request.");
                return;
            }

            _logger.LogDebug("The subscription task is not running. Trying start request.");

            // Reset the login rejection token
            if (_loginRejectionCts.IsCancellationRequested)
            {
                _loginRejectionCts.Dispose();
                _loginRejectionCts = new CancellationTokenSource();
            }

            if (_runLocalMode)
                await ReleaseSubscriptionTask();

            if (_subscriptionTask == null)
            {
                _clientCts = new CancellationTokenSource();
                _subscriptionTask = Task.Run(async () =>
                {
                    try
                    {
                        await StartClient(_clientCts.Token);
                    }
                    catch (OperationCanceledException ex)
                    {
                        _isSubscriptionRunning = false;
                        _logger.LogWarning(ex, "Subscription task was canceled.");
                    }
                    catch (Exception ex)
                    {
                        _isSubscriptionRunning = false;
                        _logger.LogError(ex, "An error occurred in the subscription task.");
                    }
                }, _clientCts.Token);
            }
            else
            {
                _logger.LogWarning("The subscription task is not null and cannot initialize the new subscription.");
            }

            await Task.CompletedTask;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Failed to start subscription");
        }
    }

    private async Task StopSubscriptionTask()
    {
        if (!_isSubscriptionRunning)
        {
            _logger.LogWarning("The subscription task is not running. Skipping stop request.");
            return;
        }

        try
        {
            await StopClient();
            await _clientCts.CancelAsync();

            if (_subscriptionTask != null)
            {
                // Wait with a longer timeout
                var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(300));
                await Task.WhenAny(_subscriptionTask, timeoutTask);

                // Set to null regardless of completion
                _subscriptionTask = null;
            }

            _isSubscriptionRunning = false;
            _isClientRunning = false;

            await _s3Service.UploadBinLogToS3Async(DateTime.Now.ToString("HH_mm_ss_fff"));
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "The subscription task cancellation completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while stopping the subscription task.");
        }
        finally
        {
            _isSubscriptionRunning = false;
            _isClientRunning = false;
            _clientCts.Dispose();
            _clientCts = new CancellationTokenSource();
            _subscriptionTask = null;
        }
    }

    private async Task StartClient(CancellationToken stoppingToken)
    {
        const int maxRetries = 10;
        const int baseDelay = 60;

        if (_isClientRunning)
        {
            _logger.LogWarning("The client is already running. Skipping start request.");
            return;
        }

        for (var attempt = 0; attempt < maxRetries; attempt++)
            try
            {
                await AttemptClientStart(stoppingToken);
                return;
            }
            catch (Exception ex) when (ex is OperationCanceledException or TimeoutException)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning(ex, "The client start operation was canceled due to service stop request.");
                    return;
                }

                await HandleRetry(attempt, maxRetries, baseDelay, ex, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An unrecoverable error occurred while starting the client. (Attempt {Attempt}/{MaxRetries})",
                    attempt + 1, maxRetries);
                break;
            }

        _isSubscriptionRunning = false;
        _logger.LogCritical("Failed to start the client after {MaxRetries} attempts.", maxRetries);
    }

    private async Task AttemptClientStart(CancellationToken stoppingToken)
    {
        _logger.LogDebug("Attempt to Starting client . . .");

        try
        {
            if (_runLocalMode)
                await RunLocalMode(stoppingToken);
            else
                await RunProductionMode(stoppingToken);

            _logger.LogDebug("The client started successfully");
        }
        catch (SubscriptionServiceException ex)
        {
            // Rollback operation values
            _isClientRunning = false;
            _isSubscriptionRunning = false;

            // Log error message
            _logger.LogWarning(ex, "The subscription service failed.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Attempt to start client failed.");
        }
    }

    private async Task ReleaseSubscriptionTask(CancellationToken stoppingToken = default)
    {
        await _taskManagementSemaphore.WaitAsync(stoppingToken);
        try
        {
            if (_subscriptionTask != null)
            {
                if (!_clientCts.IsCancellationRequested)
                    await _clientCts.CancelAsync();

                // Wait with a longer timeout
                var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(300), stoppingToken);
                await Task.WhenAny(_subscriptionTask, timeoutTask);

                // Set to null regardless of completion
                _subscriptionTask = null;

                // Dispose and recreate cancellation token source
                _clientCts.Dispose();
                _clientCts = new CancellationTokenSource();

                _isSubscriptionRunning = false;
                _isClientRunning = false;
            }
        }
        finally
        {
            _taskManagementSemaphore.Release();
        }
    }

    private async Task HandleRetry(int attempt, int maxRetries, int baseDelay, Exception ex,
        CancellationToken stoppingToken)
    {
        var waitingSeconds = (int)Math.Pow(2, attempt) * baseDelay;
        _logger.LogWarning(ex,
            "The client start operation failed. Retrying in {WaitingSeconds} seconds. (Attempt {Attempt}/{MaxRetries})",
            waitingSeconds, attempt + 1, maxRetries);

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(waitingSeconds), stoppingToken);
        }
        catch (OperationCanceledException err)
        {
            _logger.LogWarning(err, "Retry delay was interrupted due to cancellation request.");
        }
    }

    private async Task StopClient()
    {
        if (!_isClientRunning)
        {
            _logger.LogWarning("The client is not running. Skipping stop request.");
            return;
        }

        _logger.LogDebug("Stopping client");

        try
        {
            await _clientCts.CancelAsync();

            if (_localClient != null)
                await ClientSubscriptionServiceHelper.SafeLogout(_localClient, _logger);

            if (_productionClient != null)
                await ClientSubscriptionServiceHelper.SafeLogout(_productionClient, _logger);

            // Wait for a short time to allow ongoing operations to complete
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            _logger.LogDebug("The client logged out successfully");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "The client stop operation was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during client logout");
        }
        finally
        {
            _isClientRunning = false;
            _isSubscriptionRunning = false;
        }
    }

    public async Task RunLocalClientAsync(IClient client, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var thaiNow = ClientSubscriptionServiceHelper.GetCurrentThailandTime();
            await HandleSubscriptionStop(thaiNow);

            var cmd = Console.ReadLine();
            if (string.IsNullOrEmpty(cmd)) continue;

            Message? message = null;

            if (cmd.Equals("d", StringComparison.OrdinalIgnoreCase))
                message = new Debug("test debug");
            if (cmd.Equals("s", StringComparison.OrdinalIgnoreCase))
                message = new UnSequencedData("request data"u8.ToArray());

            if (message != null)
                await client.SendAsync(message.Bytes, stoppingToken);
        }
    }

    public async Task SubscribeLocalClientAsync(IClient client, CancellationToken stoppingToken)
    {
        var message = new UnSequencedData("request data"u8.ToArray());

        while (!stoppingToken.IsCancellationRequested)
        {
            var thaiNow = ClientSubscriptionServiceHelper.GetCurrentThailandTime();
            await HandleSubscriptionStop(thaiNow);

            if (message != null)
                await client.SendAsync(message.Bytes, stoppingToken);

            message = null;
        }
    }

    private async Task RunLocalMode(CancellationToken stoppingToken)
    {
        _localClient = _clientFactory.CreateClient();
        var loginDetails = new LoginDetails();
        _configuration.GetSection(ConfigurationKeys.SettradeClientConfigLoginDetails).Bind(loginDetails);

        try
        {
            var thaiNow = ClientSubscriptionServiceHelper.GetCurrentThailandTime();
            var localNow = TimeZoneInfo.ConvertTime(thaiNow, ClientSubscriptionServiceHelper.ThailandTimeZone,
                TimeZoneInfo.Local);
            var nextOpenLocal = _openSchedule.GetNextOccurrence(localNow);
            var nextCloseLocal = _closeSchedule.GetNextOccurrence(localNow);
            var (isWithinValidTimeRange, _, _) =
                ClientSubscriptionServiceHelper.EvaluateSchedule(thaiNow, nextOpenLocal, nextCloseLocal,
                    _isSubscriptionRunning);

            if (_forceRunOnStartup || (!await IsNotBusinessDays() && isWithinValidTimeRange))
            {
                var server = _configuration[ConfigurationKeys.SettradeClientConfigIpAddress] ?? string.Empty;
                var port = int.Parse(_configuration[ConfigurationKeys.SettradeClientConfigPort] ?? "0");
                var reconnectDelayMs =
                    int.Parse(_configuration[ConfigurationKeys.SettradeClientConfigReconnectDelayMs] ?? "0");

                if (!await ClientSubscriptionServiceHelper.QuickConnectivityCheck(server, port, _logger))
                    throw new SubscriptionServiceException("Quick connectivity check failed for LOCAL_GATEWAY.");

                _logger.LogDebug("Attempting to connect to server: {ServerName}", server);

                await _localClient.SetupAsync(
                    IPAddress.Parse(server),
                    port,
                    reconnectDelayMs,
                    loginDetails);

                try
                {
                    // Set operation values
                    _isClientRunning = true;
                    _isSubscriptionRunning = true;
                    _forceRunOnStartup = false;

                    await _localClient.StartAsync(stoppingToken);

                    // Start to request data
                    var autoRequest =
                        _configuration.GetValue(ConfigurationKeys.SettradeClientConfigRequestDataAfterLogon, false);
                    if (autoRequest)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                        await SubscribeLocalClientAsync(_localClient, stoppingToken);
                    }
                    else
                    {
                        await RunLocalClientAsync(_localClient, stoppingToken);
                    }
                }
                catch (Exception)
                {
                    // Rollback operation values
                    _isClientRunning = false;
                    _isSubscriptionRunning = false;
                }
            }

            _logger.LogDebug(
                "Unable to connect to the {ServerName} server due to an invalid time range or non-business day",
                IPAddress.Parse(_configuration[ConfigurationKeys.SettradeClientConfigIpAddress] ?? string.Empty));
        }
        finally
        {
            await ClientSubscriptionServiceHelper.SafeLogout(_localClient, _logger, stoppingToken);
        }
    }

    private async Task RunProductionMode(CancellationToken stoppingToken)
    {
        ArgumentNullException.ThrowIfNull(_settradeGatewaySettings.SERVERS);

        _logger.LogDebug("Found {Count} server configurations", _settradeGatewaySettings.SERVERS.Count);

        var currentServerIndex = 0;
        var glimpsePolicy = ClientSubscriptionServiceHelper.CreateRetryPolicy(_logger);
        var itchPolicy = ClientSubscriptionServiceHelper.CreateRetryPolicy(_logger);

        while (!stoppingToken.IsCancellationRequested)
        {
            var server = _settradeGatewaySettings.SERVERS[currentServerIndex];

            if (server == null)
                throw new SubscriptionServiceException(nameof(server));

            if (string.IsNullOrEmpty(server.NAME))
                server.NAME = string.Empty;

            try
            {
                await Task.Delay(_settradeGatewaySettings.RECONNECT_DELAY_MS, stoppingToken);

                var thaiNow = ClientSubscriptionServiceHelper.GetCurrentThailandTime();
                var localNow = TimeZoneInfo.ConvertTime(thaiNow, ClientSubscriptionServiceHelper.ThailandTimeZone,
                    TimeZoneInfo.Local);
                var nextOpenLocal = _openSchedule.GetNextOccurrence(localNow);
                var nextCloseLocal = _closeSchedule.GetNextOccurrence(localNow);
                var (isWithinValidTimeRange, _, _) =
                    ClientSubscriptionServiceHelper.EvaluateSchedule(thaiNow, nextOpenLocal, nextCloseLocal,
                        _isSubscriptionRunning);

                if (_forceRunOnStartup || (!await IsNotBusinessDays() && isWithinValidTimeRange))
                {
                    _logger.LogDebug("Attempting to connect to server: {ServerName}", server.NAME);

                    await ConnectToServer(server, glimpsePolicy, itchPolicy, stoppingToken);
                    break; // Successfully connected and processed data
                }

                _logger.LogDebug(
                    "Unable to connect to the {ServerName} server due to an invalid time range or non-business day",
                    server.NAME);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred with server {ServerName}. Moving to next server.", server.NAME);
                currentServerIndex = (currentServerIndex + 1) % _settradeGatewaySettings.SERVERS.Count;
            }

            _logger.LogDebug("Waiting for {Delay}ms before trying next server...",
                _settradeGatewaySettings.FAIL_OVER_DELAY_MS);

            await Task.Delay(_settradeGatewaySettings.FAIL_OVER_DELAY_MS, stoppingToken);
        }
    }

    private async Task ConnectToServer(ServerConfig server, IAsyncPolicy glimpsePolicy,
        IAsyncPolicy itchPolicy, CancellationToken stoppingToken)
    {
        ArgumentNullException.ThrowIfNull(server);
        ArgumentNullException.ThrowIfNull(server.GLIMPSE_GATEWAY);
        ArgumentNullException.ThrowIfNull(server.ITCH_GATEWAY);
        ArgumentNullException.ThrowIfNull(server.GLIMPSE_GATEWAY.IP_ADDRESS);
        ArgumentNullException.ThrowIfNull(server.ITCH_GATEWAY.IP_ADDRESS);

        if (string.IsNullOrEmpty(server.NAME))
            server.NAME = string.Empty;

        if (!await ClientSubscriptionServiceHelper.QuickConnectivityCheck(server.GLIMPSE_GATEWAY.IP_ADDRESS,
                server.GLIMPSE_GATEWAY.PORT, _logger))
            throw new SubscriptionServiceException("Quick connectivity check failed for GLIMPSE_GATEWAY.");

        try
        {
            // Set operation values
            _isClientRunning = true;
            _isSubscriptionRunning = true;
            _forceRunOnStartup = false;

            var lastGlimpseSequenceNumber = await glimpsePolicy.ExecuteAsync(async () =>
            {
                var glimpseLoginDetails =
                    ClientSubscriptionServiceHelper.CreateLoginDetails(server.GLIMPSE_GATEWAY, "Glimpse");
                _clientListener.LogPrefix = $"glimpse-{server.NAME.ToLower()}";

                return await RetrieveSnapshotDataFromGlimpseServer(server.GLIMPSE_GATEWAY, glimpseLoginDetails,
                    stoppingToken);
            });

            _logger.LogDebug("Latest SequenceNumber from Glimpse service: {SequenceNumber}",
                lastGlimpseSequenceNumber);

            // Set current GlimpseSequenceNumber to local memory
            await _memoryCacheHelper.SetCurrentGlimpseSequenceNoAsync(lastGlimpseSequenceNumber);

            if (!await ClientSubscriptionServiceHelper.QuickConnectivityCheck(server.ITCH_GATEWAY.IP_ADDRESS,
                    server.ITCH_GATEWAY.PORT, _logger))
                throw new SubscriptionServiceException("Quick connectivity check failed for ITCH_GATEWAY.");

            await itchPolicy.ExecuteAsync(async () =>
            {
                // Start manual counting. ItchSequenceNumber
                var lastItchSequenceNumber = lastGlimpseSequenceNumber + 1;
                var itchLoginDetails =
                    ClientSubscriptionServiceHelper.CreateLoginDetails(server.ITCH_GATEWAY, "Itch",
                        lastItchSequenceNumber);
                _clientListener.LogPrefix = $"itch-{server.NAME.ToLower()}";

                await RetrieveRealTimeDataFromItchServer(server.ITCH_GATEWAY, itchLoginDetails, stoppingToken);
            });
        }
        catch (Exception)
        {
            // Rollback operation values
            _isClientRunning = false;
            _isSubscriptionRunning = false;
        }
    }

    private async Task HouseKeepingService(bool resetSeqNo = true)
    {
        if (resetSeqNo)
        {
            // Reset SequenceNumber of Glimpse and Itch services
            await _memoryCacheHelper.SetCurrentGlimpseSequenceNoAsync(1);
            await _memoryCacheHelper.SetCurrentItchSequenceNoAsync(1);
        }

        // Reset all statuses and counters
        _isSubscriptionRunning = false;
        _isClientRunning = false;

        // Upload BinLogs to S3
        await _s3Service.UploadBinLogToS3Async(DateTime.Now.ToString("HH_mm_ss_fff"));
    }

    private async Task<ulong> RetrieveSnapshotDataFromGlimpseServer(Gateway config, LoginDetails loginDetails,
        CancellationToken stoppingToken)
    {
        ArgumentNullException.ThrowIfNull(config.IP_ADDRESS);

        _productionClient = _clientFactory.CreateClient();
        if (IPAddress.TryParse(config.IP_ADDRESS, out var ipAddress))
            await _productionClient.SetupAsync(ipAddress, config.PORT, int.Parse(
                    _configuration[ConfigurationKeys.SettradeGatewaySettingsReconnectDelayMs] ??
                    throw new InvalidOperationException("Reconnect delay is not set")),
                loginDetails);

        await _productionClient.StartAsync(stoppingToken);

        ulong lastSequenceNumber = 0;
        var endOfSnapshotReceived = false;

        try
        {
            while (!stoppingToken.IsCancellationRequested && !endOfSnapshotReceived)
            {
                var message = await _clientListener.ReceiveMessage(stoppingToken);
                if (message == null)
                {
                    _logger.LogWarning("Received null message, possibly due to cancellation.");
                    break;
                }

                _logger.LogDebug("Received message of type: {MessageType}", message.MsgType);

                switch (message)
                {
                    case EndOfSnapshotMessage endOfSnapshotMessage:
                        lastSequenceNumber = ulong.Parse(endOfSnapshotMessage.SequenceNumber);
                        _logger.LogDebug("Received EndOfSnapshotMessage with sequence number: {SequenceNumber}",
                            lastSequenceNumber);
                        endOfSnapshotReceived = true;
                        break;
                }
            }
        }
        finally
        {
            await ClientSubscriptionServiceHelper.SafeLogout(_productionClient, _logger, stoppingToken);
        }

        return lastSequenceNumber;
    }

    private async Task RetrieveRealTimeDataFromItchServer(Gateway config, LoginDetails loginDetails,
        CancellationToken stoppingToken)
    {
        ArgumentNullException.ThrowIfNull(config.IP_ADDRESS);

        _productionClient = _clientFactory.CreateClient();
        if (IPAddress.TryParse(config.IP_ADDRESS, out var ipAddress))
            await _productionClient.SetupAsync(ipAddress, config.PORT, int.Parse(
                    _configuration[ConfigurationKeys.SettradeGatewaySettingsReconnectDelayMs] ??
                    throw new InvalidOperationException("Reconnect delay is not set")),
                loginDetails);

        await _productionClient.StartAsync(stoppingToken);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await _clientListener.ReceiveMessage(stoppingToken);
                if (message == null)
                {
                    _logger.LogWarning("Received null message, possibly due to cancellation.");
                    break;
                }

                _logger.LogDebug("Received real-time message of type: {MessageType}", message.MsgType);

                switch (message)
                {
                    case SystemEventMessage systemEventMessage
                        when _messageValidator.IsEndOfMessage(systemEventMessage):
                        _logger.LogWarning(
                            "Received SystemEventMessage (EndOfMessageEventCode) message, possibly due to cancellation.");
                        await HouseKeepingService();
                        return;
                }
            }
        }
        finally
        {
            await ClientSubscriptionServiceHelper.SafeLogout(_productionClient, _logger, stoppingToken);
        }
    }
}