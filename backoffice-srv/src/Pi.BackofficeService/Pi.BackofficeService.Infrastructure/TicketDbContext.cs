using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Pi.BackofficeService.Domain.SeedWork;
using Pi.BackofficeService.Infrastructure.EntityConfig;

namespace Pi.BackofficeService.Infrastructure;

public class TicketDbContext : SagaDbContext, IUnitOfWork
{
    private IEncryptionProvider? EncryptionProvider => this.GetInfrastructure().GetService<IEncryptionProvider>();

    public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseEncryption(EncryptionProvider);
    }

    protected override IEnumerable<ISagaClassMap> Configurations => new[] { new TicketStateMap() };
}
