using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Pi.Common.SeedWork;
using Pi.User.Domain.AggregatesModel.AddressAggregate;
using Pi.User.Domain.AggregatesModel.BankAccountAggregate;
using Pi.User.Domain.AggregatesModel.DocumentAggregate;
using Pi.User.Domain.AggregatesModel.ExamAggregate;
using Pi.User.Domain.AggregatesModel.ExternalAccountAggregate;
using Pi.User.Domain.AggregatesModel.KycAggregate;
using Pi.User.Domain.AggregatesModel.SuitabilityTestAggregate;
using Pi.User.Domain.AggregatesModel.TradeAccountAggregate;
using Pi.User.Domain.AggregatesModel.TransactionIdAggregate;
using Pi.User.Domain.AggregatesModel.UserAccountAggregate;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
using Pi.User.Domain.SeedWork;
using Pi.User.Infrastructure.EntityConfigs;

namespace Pi.User.Infrastructure;

public class UserDbContext : DbContext, IUnitOfWork
{
    public DbSet<UserInfo> UserInfos => Set<UserInfo>();
    public DbSet<CustCode> CustCodes => Set<CustCode>();
    public DbSet<TradingAccount> TradingAccounts => Set<TradingAccount>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
    public DbSet<TransactionId> TransactionIds => Set<TransactionId>();
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Examination> Examinations => Set<Examination>();

    // New structure
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Kyc> Kycs => Set<Kyc>();
    public DbSet<BankAccountV2> BankAccountV2s => Set<BankAccountV2>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<SuitabilityTest> SuitabilityTests => Set<SuitabilityTest>();
    public DbSet<TradeAccount> TradeAccounts => Set<TradeAccount>();
    public DbSet<ExternalAccount> ExternalAccounts => Set<ExternalAccount>();


    private IEncryptionProvider? _encryptionProvider;

    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // order matter
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransactionIdEntityConfig).Assembly);
        _encryptionProvider = this.GetInfrastructure().GetService<IEncryptionProvider>();
        modelBuilder.UseEncryption(_encryptionProvider);
    }

    public override int SaveChanges()
    {
        AddTimestamp();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken token = default)
    {
        AddTimestamp();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, token);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        AddTimestamp();
        var result = await base.SaveChangesAsync(cancellationToken);
        return true;
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