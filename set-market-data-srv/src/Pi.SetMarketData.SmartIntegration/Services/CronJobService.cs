using NCrontab;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.SmartIntegration.Interfaces;

namespace Pi.SetMarketData.SmartIntegration.Services;

public class CronJobService : BackgroundService
{
    private readonly ILogger<CronJobService> _logger;
    private readonly IDatabaseTaskService _databaseTaskService;
    private readonly IHostApplicationLifetime _appLifetime;

    public CronJobService(
        ILogger<CronJobService> logger,
        IDatabaseTaskService databaseTaskService,
        IHostApplicationLifetime appLifetime
    )
    {
        _logger = logger;
        _databaseTaskService = databaseTaskService;
        _appLifetime = appLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cron job running at: {Time}", DateTime.Now);
        try{
            await _databaseTaskService.PerformDatabaseTask(null!);
        } catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to proceed Database task service.");
        } finally
        {
            _appLifetime.StopApplication();
        }
        


    }
}