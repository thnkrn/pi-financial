namespace Pi.SetMarketData.Domain.Models.DataMigration;

public class MigrationData<T>
{
    public T? Response { get; set; }
    public MigrationJob? MigrationJob { get; set; }
}
