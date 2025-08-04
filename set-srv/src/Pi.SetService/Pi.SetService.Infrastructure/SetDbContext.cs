using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pi.Common.SeedWork;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using Pi.SetService.Infrastructure.EntityConfigs;
using Pi.SetService.Infrastructure.Options;

namespace Pi.SetService.Infrastructure;

public class SetDbContext : DbContext, IUnitOfWork
{
    public DbSet<EquityOrderState> EquityOrderState => Set<EquityOrderState>();
    public DbSet<EquityInfo> EquityInfos => Set<EquityInfo>();
    public DbSet<EquityInitialMargin> EquityInitialMargins => Set<EquityInitialMargin>();
    public DbSet<SblInstrument> SblInstruments => Set<SblInstrument>();
    public DbSet<SblOrder> SblOrders => Set<SblOrder>();

    public SetDbContext(DbContextOptions<SetDbContext> options) : base(options)
    {
    }

    protected IEnumerable<ISagaClassMap> SagaConfigurations => new ISagaClassMap[]
    {
        new EquityOrderStateMap()
    };

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Saga state machine
        base.OnModelCreating(modelBuilder);
        foreach (var configuration in SagaConfigurations)
        {
            configuration.Configure(modelBuilder);
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EquityOrderStateMap).Assembly);
        modelBuilder.UseEncryption(AesProvider());
    }

    private AesProvider AesProvider()
    {
        var dbOptions = this.GetInfrastructure().GetRequiredService<IOptions<DatabaseOptions>>();
        var key = Convert.FromBase64String(dbOptions.Value.AesKey);
        var iv = Convert.FromBase64String(dbOptions.Value.AesIV);
        return new AesProvider(key, iv);
    }

    public override int SaveChanges()
    {
        AddTimestamp();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        AddTimestamp();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamp()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is IAuditableEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow;

            if (entity.State == EntityState.Added)
            {
                ((IAuditableEntity)entity.Entity).CreatedAt = now;
            }
            ((IAuditableEntity)entity.Entity).UpdatedAt = now;
        }
    }
}
