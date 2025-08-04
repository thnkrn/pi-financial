using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pi.TfexService.Domain.Models.ActivitiesLog;
using Pi.TfexService.Domain.Models.InitialMargin;
using Pi.TfexService.Domain.SeedWork;
using Pi.TfexService.Infrastructure.EntityConfigs;
using IUnitOfWork = Pi.Common.SeedWork.IUnitOfWork;

namespace Pi.TfexService.Infrastructure;

public class TfexDbContext(DbContextOptions<TfexDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<ActivitiesLog> ActivitiesLogs => Set<ActivitiesLog>();
    public DbSet<InitialMargin> InitialMargins => Set<InitialMargin>();
    private IEncryptionCryptoProvider? EncryptionProvider => this.GetInfrastructure().GetService<IEncryptionCryptoProvider>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("utf8mb4_0900_ai_ci");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EntityConfigMap).Assembly);
        modelBuilder.UseEncryption(EncryptionProvider);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .ConfigureWarnings(
                b => b.Log(
                    (RelationalEventId.CommandExecuted, LogLevel.Debug),
                    (CoreEventId.ContextInitialized, LogLevel.Debug)
                ));

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
            .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow; // current datetime

            if (entity.State == EntityState.Added)
            {
                ((BaseEntity)entity.Entity).CreatedAt = now;
            }
            ((BaseEntity)entity.Entity).UpdatedAt = now;
        }
    }
}