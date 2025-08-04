using Pi.WalletService.Domain.AggregatesModel.OnlineDirectDebitRegistrationAggregate;

namespace Pi.WalletService.Infrastructure.Repositories;

public class OnlineDirectDebitRegistrationRepository : IOnlineDirectDebitRegistrationRepository
{
    private readonly WalletDbContext _walletDbContext;

    public Common.SeedWork.IUnitOfWork UnitOfWork => _walletDbContext;
    public OnlineDirectDebitRegistrationRepository(WalletDbContext walletDbContext)
    {
        this._walletDbContext = walletDbContext;
    }

    public async Task AddAsync(OnlineDirectDebitRegistration onlineDirectDebitRegistration, CancellationToken cancellationToken = default(CancellationToken))
    {
        await this._walletDbContext.AddAsync(onlineDirectDebitRegistration, cancellationToken);
    }

    public async Task<OnlineDirectDebitRegistration?> GetAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await this._walletDbContext.FindAsync<OnlineDirectDebitRegistration>(id, cancellationToken);
    }

    public void Update(OnlineDirectDebitRegistration onlineDirectDebitRegistration)
    {
        this._walletDbContext.Update(onlineDirectDebitRegistration);
    }
}
