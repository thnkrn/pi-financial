using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Domain.AggregatesModel.LogAggregate;
using Pi.WalletService.Domain.AggregatesModel.OnlineDirectDebitRegistrationAggregate;
using Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;
using Pi.WalletService.Domain.AggregatesModel.WalletAggregate;
using Pi.WalletService.Domain.SeedWork;
using Pi.WalletService.Infrastructure.EntityConfigs;
using IUnitOfWork = Pi.Common.SeedWork.IUnitOfWork;
namespace Pi.WalletService.Infrastructure;

public class WalletDbContext : DbContext, IUnitOfWork
{
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<TransactionHistory> TransactionHistories => Set<TransactionHistory>();
    public DbSet<GlobalWalletTransactionHistory> GlobalWalletTransactionHistories => Set<GlobalWalletTransactionHistory>();
    public DbSet<OnlineDirectDebitRegistration> OnlineDirectDebitRegistrations => Set<OnlineDirectDebitRegistration>();
    public DbSet<FreewillRequestLog> FreewillRequestLogs => Set<FreewillRequestLog>();
    public DbSet<Holiday> Holidays => Set<Holiday>();
    private IEncryptionProvider? EncryptionProvider => this.GetInfrastructure().GetService<IEncryptionProvider>();

    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
    {
    }

    protected IEnumerable<ISagaClassMap> SagaConfigurations => new ISagaClassMap[]
    {
        new CashDepositStateMap(),
        new CashWithdrawStateMap(),
        new DepositStateMap(),
        new GlobalWalletTransferStateMap(),
        new GlobalManualAllocationStateMap(),
        new WithdrawStateMap(),
        new RefundStateMap(),
        new DepositEntrypointStateMap(),
        new QrDepositStateMap(),
        new OddDepositStateMap(),
        new OddWithdrawStateMap(),
        new UpBackStateMap(),
        new GlobalTransferStateMap(),
        new WithdrawEntrypointStateMap(),
        new AtsDepositStateMap(),
        new AtsWithdrawStateMap(),
        new RecoveryStateMap(),
    };

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Saga state machine
        base.OnModelCreating(modelBuilder);
        foreach (var configuration in SagaConfigurations)
        {
            configuration.Configure(modelBuilder);
        }

        modelBuilder.UseCollation("utf8mb4_0900_ai_ci");
        // Apply configurations from assembly - applied all in the same namespace
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OnlineDirectDebitRegistrationMap).Assembly);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
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
