namespace Pi.SetMarketData.DataMigrationJobProducer.interfaces;

public interface IMigrationJobsProducerService
{
    Task ProduceMigrationJobsAsync(DateTime migrationDateFrom, DateTime migrationDateTo, string venue, string[] stockSymbols);
}