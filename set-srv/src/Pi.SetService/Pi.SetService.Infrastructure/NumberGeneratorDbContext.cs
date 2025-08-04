using Microsoft.EntityFrameworkCore;
using Pi.Common.Generators.Number;
using Pi.Common.SeedWork;
using Pi.SetService.Infrastructure.EntityConfigs;

namespace Pi.SetService.Infrastructure;

public class NumberGeneratorDbContext : DbContext, IUnitOfWork
{
    public DbSet<NumberGenerator> NumberGenerators => Set<NumberGenerator>();

    public NumberGeneratorDbContext(DbContextOptions<NumberGeneratorDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Saga state machine
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EquityOrderStateMap).Assembly);
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
