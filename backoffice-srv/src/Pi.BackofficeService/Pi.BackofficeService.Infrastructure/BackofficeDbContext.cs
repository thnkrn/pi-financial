using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.User;
using Pi.BackofficeService.Infrastructure.EntityConfig;

namespace Pi.BackofficeService.Infrastructure;

public class BackofficeDbContext : DbContext, Domain.SeedWork.IUnitOfWork
{
    public DbSet<ResponseCodeAction> ResponseCodeActions => Set<ResponseCodeAction>();
    public DbSet<ResponseCode> ResponseCodes => Set<ResponseCode>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Bank> Banks => Set<Bank>();
    public DbSet<BankChannel> BankChannels => Set<BankChannel>();

    private IEncryptionProvider? EncryptionProvider => this.GetInfrastructure().GetService<IEncryptionProvider>();

    public BackofficeDbContext(DbContextOptions<BackofficeDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("utf8mb4_0900_ai_ci");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfig).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ResponseCodeConfig).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankConfig).Assembly);
        modelBuilder.UseEncryption(EncryptionProvider);
    }
}
