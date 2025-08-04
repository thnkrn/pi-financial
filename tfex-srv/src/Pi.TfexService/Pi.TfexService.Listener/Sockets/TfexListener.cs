using Google.Type;
using MassTransit;
using Microsoft.Extensions.Options;
using MQTTnet.Client;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Application.Utils;
using Pi.TfexService.Infrastructure.Options;
using Pi.TfexService.Listener.Models;
using Pi.TfexService.Listener.Utils;

namespace Pi.TfexService.Listener.Sockets;

public class TfexListener : BackgroundService
{
    private readonly ILogger<TfexListener> _logger;
    private readonly IBus _bus;
    private readonly IHostEnvironment _environment;
    private readonly ISetTradeService _setTradeService;
    private readonly IMqttClientFactory _mqttClientFactory;
    private readonly SetTradeStreamOptions _setTradeStreamOptions;
    private readonly FeaturesOptions _featuresOptions;

    private readonly TimeOnly _operatingHoursOpeningTime;
    private readonly TimeOnly _operatingHoursClosingTime;

    private IMqttClient _client = null!;
    private MqttClientSubscribeOptions _subscribeOptions = null!;
    private MqttClientOptions _brokerOptions = null!;
    private DateTime _operatingHoursOpeningDateTime;
    private DateTime _operatingHoursClosingDateTime;

    public CancellationTokenSource? HealthCheckCancellationTokenSource { get; set; }

    public TfexListener(
        ILogger<TfexListener> logger,
        IBus bus,
        IHostEnvironment environment,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<SetTradeStreamOptions> setTradeStreamOptions,
        IOptions<OperationHoursOptions> operationHoursOptions,
        IOptions<FeaturesOptions> featuresOptions)
    {
        _logger = logger;
        _bus = bus;
        _environment = environment;

        var serviceScope = serviceScopeFactory.CreateScope();
        _setTradeService = serviceScope.ServiceProvider.GetRequiredService<ISetTradeService>();
        _mqttClientFactory = serviceScope.ServiceProvider.GetRequiredService<IMqttClientFactory>();
        _setTradeStreamOptions = setTradeStreamOptions.Value;
        _featuresOptions = featuresOptions.Value;

        _operatingHoursOpeningTime = TimeOnly.Parse(operationHoursOptions.Value.Start);
        _operatingHoursClosingTime = TimeOnly.Parse(operationHoursOptions.Value.End);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await CheckOperatingHours(cancellationToken);

            try
            {
                if (!_client?.IsConnected ?? true)
                {
                    await Connect(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            await Task.Delay(TimeSpan.FromSeconds(_setTradeStreamOptions.ConnectionDelaySeconds), cancellationToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await CleanConnection(cancellationToken);
    }

    private async Task CheckOperatingHours(CancellationToken cancellationToken)
    {
        var now = DateUtils.GetThDateTimeNow();

        (_operatingHoursOpeningDateTime, _operatingHoursClosingDateTime) = OperatingHoursUtils
            .GetOperatingDateTime(
                now,
                _operatingHoursOpeningTime,
                _operatingHoursClosingTime
            );

        if (now < _operatingHoursOpeningDateTime && now > _operatingHoursClosingDateTime)
        {
            await CleanConnection(cancellationToken);

            var timeDiff = _operatingHoursOpeningDateTime - now;

            _logger.LogInformation("Await for opening hour ({TimeDiff})", timeDiff);

            await Task.Delay(timeDiff, cancellationToken);

            (_operatingHoursOpeningDateTime, _operatingHoursClosingDateTime) = OperatingHoursUtils
                .GetOperatingDateTime(
                    now,
                    _operatingHoursOpeningTime,
                    _operatingHoursClosingTime
                );
        }
    }

    private async Task InitializeConfig(CancellationToken cancellationToken)
    {
        var url = await GetSetTradeUrl(cancellationToken);
        _subscribeOptions = _mqttClientFactory.CreateSubscribeOptions(_setTradeStreamOptions.Topic);
        _brokerOptions = _mqttClientFactory.CreateBrokerOptions(url);
        _client = _mqttClientFactory.CreateMqttClient();

        _client.ConnectedAsync += async e => await HandleListenerConnected(e, cancellationToken);
        _client.DisconnectedAsync += async e => await HandleListenerDisconnected(e, cancellationToken);
        _client.ApplicationMessageReceivedAsync += async e => await HandleListenerMessageReceived(e);
    }

    private async Task<string> GetSetTradeUrl(CancellationToken cancellationToken)
    {
        try
        {
            if (_environment.IsDevelopment())
            {
                var accessToken = await _setTradeService.GetAccessToken();
                _logger.LogDebug("Current access token: {token}", accessToken);
            }

            var setTradeStreamInfo = await _setTradeService.GetSetTradeStreamInfo(cancellationToken);
            var host = setTradeStreamInfo.Hosts.FirstOrDefault();
            var path = _setTradeStreamOptions.Path;
            var token = setTradeStreamInfo.Token;
            var url = $"{host}{path}{token}";

            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get SetTrade url");
            throw;
        }
    }

    private async Task HandleListenerConnected(MqttClientConnectedEventArgs e, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Connected to SetTrade with Result: {Result}", e.ConnectResult?.ResultCode);
        await _mqttClientFactory.Subscribe(_client, _subscribeOptions, cancellationToken);
    }

    private async Task HandleListenerDisconnected(MqttClientDisconnectedEventArgs e, CancellationToken cancellationToken)
    {
        if (!e.Exception?.Message.Contains("401") ?? true)
        {
            return;
        }

        _logger.LogWarning("Disconnected from SetTrade with Reason: {Reason}", e.Exception?.Message);

        await Task.Delay(TimeSpan.FromSeconds(_setTradeStreamOptions.ConnectionDelaySeconds), cancellationToken);

        await CleanConnection(cancellationToken);
    }

    private async Task HandleListenerMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        var payloadSegment = e.ApplicationMessage.PayloadSegment;
        var message = OrderDerivV3.Parser.ParseFrom(payloadSegment);


        // _logger.LogInformation("Received message: {Message}", message);

        if (!_featuresOptions.IsTfexListenerNotificationEnabled)
        {
            return;
        }

        await _bus.Publish(
            new SetTradeOrderStatus(
                message.OrderNo,
                message.AccountNo,
                message.SeriesId,
                (SetTradeListenerOrderEnum.Side)message.Side,
                SetTradeOrderUtils.GetOrderPrice(message.Price.Units, message.Price.Nanos),
                message.Volume,
                message.BalanceVolume,
                message.MatchedVolume,
                message.CancelledVolume,
                message.Status
            )
        );
    }

    private void HandleException(Exception ex)
    {
        if (ex.Message.Contains("401"))
        {
            _logger.LogError(ex, "Unable to connect to SetTrade, Error: Unauthorized");
            _setTradeService.GetAccessToken(true);
            return;
        }

        _logger.LogError(ex, "Unable to connect to SetTrade");
    }

    private async Task Connect(CancellationToken cancellationToken)
    {
        await InitializeConfig(cancellationToken);
        await _mqttClientFactory.Connect(_client, _brokerOptions, cancellationToken);
        ListenerHealthCheck();
    }

    private void ListenerHealthCheck()
    {
        if (HealthCheckCancellationTokenSource != null)
        {
            return;
        }

        HealthCheckCancellationTokenSource = new CancellationTokenSource();
        Task.Run(_ListenerHealthCheck, HealthCheckCancellationTokenSource.Token);
    }

    private async Task _ListenerHealthCheck()
    {
        while (!HealthCheckCancellationTokenSource?.Token.IsCancellationRequested ?? true)
        {
            try
            {
                await _mqttClientFactory.Ping(_client, HealthCheckCancellationTokenSource!.Token);
                _logger.LogInformation("Health check success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(_setTradeStreamOptions.HealthCheckSeconds), HealthCheckCancellationTokenSource!.Token);
            }
        }
    }

    private async Task CleanConnection(CancellationToken cancellationToken)
    {
        if (_client?.IsConnected ?? false)
        {
            await _mqttClientFactory.Disconnect(_client, new MqttClientDisconnectOptions(), cancellationToken);
        }

        if (HealthCheckCancellationTokenSource != null)
        {
            await HealthCheckCancellationTokenSource.CancelAsync();
            HealthCheckCancellationTokenSource.Dispose();
            HealthCheckCancellationTokenSource = null;
        }
    }
}