using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
namespace Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;

public interface IGlobalTransferRepository : IGenericRepository<GlobalTransferState>
{
    Task<GlobalTransferState?> Get(Guid correlationId);
    Task UpdateExchangeData(Guid correlationId, decimal exchangeAmount, Currency exchangeCurrency);
    /// <summary>
    /// For mocking test case on UAT only.
    /// </summary>
    Task UpdateRequestedFxAmountById(Guid correlationId, decimal requestedFxAmount);

    /// <summary>
    /// For mocking test case on UAT only.
    /// </summary>
    Task UpdateGlobalAccountById(Guid correlationId, string globalAccount);

    /// <summary>
    /// For mocking test case on UAT only.
    /// </summary>
    Task<bool> UpdateGlobalAccountByIdAndState(Guid correlationId, string state, string globalAccount);

    /// <summary>
    /// For mocking test case on UAT only.
    /// </summary>
    Task<bool> UpdateCurrencyAndState(Guid correlationId, string state, Currency requestedCurrency, Currency requestedFxCurrency);

    /// <summary>
    /// For mocking test case on UAT only.
    /// </summary>
    Task<bool> UpdateFxTransactionIdAndState(Guid correlationId, string state, string fxTransactionId);
}
