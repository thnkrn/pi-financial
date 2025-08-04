namespace Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate
{
    public interface IFundAccountOpeningStateRepository
    {
        Task<IEnumerable<FundAccountOpeningState>> GetFundAccountOpeningStatesByRequestDate(DateOnly requestReceivedDate, bool? ndid);
        Task<FundAccountOpeningState?> GetFundAccountOpeningStatesByCustCode(string custCode);
        Task<IEnumerable<FundAccountOpeningState>> GetMultipleFundAccountOpeningStatesByCustCode(string custCode);
    }
}
