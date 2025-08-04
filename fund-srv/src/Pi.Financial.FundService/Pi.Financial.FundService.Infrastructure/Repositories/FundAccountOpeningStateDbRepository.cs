using Microsoft.EntityFrameworkCore;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;

namespace Pi.Financial.FundService.Infrastructure.Repositories;

public class FundAccountOpeningStateDbRepository : IFundAccountOpeningStateRepository
{
    private readonly FundDbContext _dbContext;

    public FundAccountOpeningStateDbRepository(FundDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<IEnumerable<FundAccountOpeningState>> GetFundAccountOpeningStatesByRequestDate(
        DateOnly requestReceivedDate, bool? ndid)
    {
        var startDate = requestReceivedDate.ToDateTime(TimeOnly.MinValue);
        var endDate = requestReceivedDate.ToDateTime(TimeOnly.MaxValue);

        IEnumerable<FundAccountOpeningState> result = _dbContext.Set<FundAccountOpeningState>().Where(
            r => r.RequestReceivedTime >= startDate && r.RequestReceivedTime <= endDate &&
                 (ndid == null || r.Ndid == ndid)
        );
        return Task.FromResult(result);
    }

    public async Task<FundAccountOpeningState?> GetFundAccountOpeningStatesByCustCode(string custCode)
    {
        //TODO: this will throw 500 when Sequence contains more than one element
        var result = await _dbContext.Set<FundAccountOpeningState>().Where(
            x => x.CustomerCode == custCode
        ).SingleOrDefaultAsync();

        return result;
    }

    public Task<IEnumerable<FundAccountOpeningState>> GetMultipleFundAccountOpeningStatesByCustCode(string custCode)
    {
        IEnumerable<FundAccountOpeningState> result = _dbContext.Set<FundAccountOpeningState>().Where(
            x => x.CustomerCode == custCode
        );
        return Task.FromResult(result);
    }
}
