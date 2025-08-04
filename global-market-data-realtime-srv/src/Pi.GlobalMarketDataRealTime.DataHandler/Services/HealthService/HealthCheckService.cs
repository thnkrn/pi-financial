using System.Net;
using Pi.GlobalMarketDataRealTime.DataHandler.Exceptions;
using Pi.GlobalMarketDataRealTime.Domain.Entities;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Redis;
using Pi.GlobalMarketDataRealTime.Infrastructure.Services.Fix;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Services.HealthService;

public class HealthCheckService : BackgroundService
{
    private readonly IClient _client;
    private readonly IConfiguration _configuration;
    private readonly IMongoService<GeInstrument> _geInstrumentService;
    private readonly HttpListener _httpListener;
    private readonly ILogger<HealthCheckService> _logger;
    private readonly IRedisV2Publisher _redisCache;

    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="geInstrumentService"></param>
    /// <param name="redisCache"></param>
    /// <param name="client"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public HealthCheckService(IConfiguration configuration, 
        IMongoService<GeInstrument> geInstrumentService,
        IRedisV2Publisher redisCache, 
        IClient client, 
        ILogger<HealthCheckService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _geInstrumentService = geInstrumentService ?? throw new ArgumentNullException(nameof(geInstrumentService));
        _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpListener = new HttpListener();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ConfigureHttpListener();
        _logger.LogDebug("Health check service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var ctx = await GetContextSafelyAsync();
            await HandleRequestAsync(ctx);
        }
    }

    private void ConfigureHttpListener()
    {
        var (readinessPath, readinessPort) = GetConfigValue("READINESS");
        var (livenessPath, livenessPort) = GetConfigValue("LIVENESS");

        _httpListener.Prefixes.Add($"http://*:{readinessPort}{readinessPath}");
        _httpListener.Prefixes.Add($"http://*:{livenessPort}{livenessPath}");
        _httpListener.Start();

        _logger.LogDebug("Readiness Probe is {ReadinessPath} Port: {ReadinessPort}", readinessPath,
            readinessPort);
        _logger.LogDebug("Liveness Probe is {LivenessPath} Port: {LivenessPort}", livenessPath, livenessPort);
    }

    private (string path, int port) GetConfigValue(string probeType)
    {
        var path = _configuration[$"HEALTHY:{probeType}_PATH"]
                   ?? throw new ArgumentNullException($"HEALTHY:{probeType}_PATH");

        path = path[0].Equals('/') ? path : $"/{path}";

        _ = int.TryParse(_configuration[$"HEALTHY:{probeType}_PORT"]
                         ?? throw new ArgumentNullException($"HEALTHY:{probeType}_PORT"), out var port);

        return (path, port);
    }

    private async Task<HttpListenerContext?> GetContextSafelyAsync()
    {
        try
        {
            return await _httpListener.GetContextAsync();
        }
        catch (HttpListenerException ex)
        {
            if (ex.ErrorCode == 995) return null;
            _logger.LogError(ex, "Error getting HTTP context: {Message}", ex.Message);
            return null;
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext? ctx)
    {
        if (ctx != null)
        {
            var response = ctx.Response;
            SetResponseHeaders(response);

            if (ctx.Request.Url != null && ctx.Request.Url.ToString().Contains("/liveness"))
                await HandleLivenessProbeAsync(response);

            response.Close();
        }
    }

    private static void SetResponseHeaders(HttpListenerResponse response)
    {
        response.ContentType = "text/plain";
        response.Headers.Add(HttpResponseHeader.CacheControl, "no-store, no-cache");
        response.StatusCode = (int)HttpStatusCode.OK;
    }

    private async Task HandleLivenessProbeAsync(HttpListenerResponse response)
    {
        try
        {
            await CheckMongoDbConnectionAsync();
            await CheckRedisConnectionAsync();

            CheckFixClientConnection();
        }
        catch (Exception ex)
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            _logger.LogError(ex, "Liveness probe failed: {Message}", ex.Message);
        }
    }

    private async Task CheckMongoDbConnectionAsync()
    {
        _logger.LogDebug("Liveness: Checking MongoDB connection...");
        await _geInstrumentService.GetAllAsync();
        _logger.LogDebug("Liveness: MongoDB Connected");
    }

    private async Task CheckRedisConnectionAsync()
    {
        if (!bool.TryParse(_configuration["REDIS:ENABLED"], out var isRedisEnabled) || !isRedisEnabled)
            return;

        _logger.LogDebug("Liveness: Checking Redis connection...");
        await _redisCache.GetAsync<string>("T");
        _logger.LogDebug("Liveness: Redis available");
    }

    private void CheckFixClientConnection()
    {
        if (_client.State != ClientState.Connected)
            throw new SubscriptionServiceException("FIX client offline");
    }
}