namespace Pi.User.Application.Services.LegacyUserInfo
{
    public interface IUserBankAccountService
    {
        public Task<BankAccountInfo?> GetBankAccountInfoAsync(string customerCode, CancellationToken cancellationToken = default);
        public Task UpdateBankAccountInfoAsync(BankAccountInfo bankAccountInfo, CancellationToken cancellationToken = default);
    }
}

