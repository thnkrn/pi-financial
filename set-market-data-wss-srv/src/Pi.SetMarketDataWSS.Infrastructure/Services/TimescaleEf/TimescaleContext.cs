using Microsoft.EntityFrameworkCore;

namespace Pi.SetMarketDataWSS.Infrastructure.Services.TimescaleEf;

public class TimescaleContext : DbContext
{
    public TimescaleContext(DbContextOptions<TimescaleContext> options) : base(options)
    {
    }

    // DbSet properties for your entities
    public DbSet<object> MarketStockDataEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure your entity mappings and relationships
    }
}