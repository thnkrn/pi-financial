using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pi.Common.Generators.Number;
using Pi.Common.SeedWork;
using Pi.Financial.FundService.Domain.AggregatesModel.CustomerDataAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Infrastructure.EntityConfigs;
using Pi.Financial.FundService.Infrastructure.Options;

namespace Pi.Financial.FundService.Infrastructure
{
    public class FundDbContext : DbContext, IUnitOfWork
    {
        public DbSet<UnitHolder> UnitHolders => Set<UnitHolder>();
        public DbSet<FundOrderState> FundOrderState => Set<FundOrderState>();
        public DbSet<NumberGenerator> NumberGenerators => Set<NumberGenerator>();
        public DbSet<CustomerDataSyncHistory> CustomerDataSyncHistories => Set<CustomerDataSyncHistory>();

        public FundDbContext(DbContextOptions<FundDbContext> options) : base(options)
        {
        }

        protected IEnumerable<ISagaClassMap> SagaConfigurations => new ISagaClassMap[]
        {
            new FundAccountOpeningStateMap(),
            new FundOrderStateMap()
        };

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Saga state machine
            base.OnModelCreating(modelBuilder);
            foreach (var configuration in SagaConfigurations)
            {
                configuration.Configure(modelBuilder);
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UnitHolderMap).Assembly);
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
}
