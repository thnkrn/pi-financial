using Pi.Common.SeedWork;

namespace Pi.WalletService.Domain.AggregatesModel.OnlineDirectDebitRegistrationAggregate
{
    public interface IOnlineDirectDebitRegistrationRepository : IRepository<OnlineDirectDebitRegistration>
    {
        Task AddAsync(OnlineDirectDebitRegistration onlineDirectDebitRegistration, CancellationToken cancellationToken = default);
        Task<OnlineDirectDebitRegistration?> GetAsync(string id, CancellationToken cancellationToken = default);
        void Update(OnlineDirectDebitRegistration onlineDirectDebitRegistration);
    }
}

