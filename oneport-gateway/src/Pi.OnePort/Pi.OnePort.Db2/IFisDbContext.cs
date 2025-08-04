using Microsoft.EntityFrameworkCore;
using Pi.OnePort.Db2.EntityConfigs;

namespace Pi.OnePort.Db2;

public class IFisDbContext : DbContext
{
    public IFisDbContext(DbContextOptions<IFisDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DealMap).Assembly);
    }
}
