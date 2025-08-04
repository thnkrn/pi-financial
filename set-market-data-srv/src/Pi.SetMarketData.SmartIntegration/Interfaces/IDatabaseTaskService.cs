using Pi.SetMarketData.SmartIntegration.Configurations;

namespace Pi.SetMarketData.SmartIntegration.Interfaces;

public interface IDatabaseTaskService
{
    Task PerformDatabaseTask(BatchUpdateOptions options);
}