using Pi.BackofficeService.Application.Models;

namespace Pi.BackofficeService.Application.Queries
{
    public interface IOnboardingQueries
    {
        Task<List<OpenAccountInfoDto>?> GetOpenAccountsByCustCode(string custCode);

        Task<PaginateOpenAccountListResult> GetOpenAccountsPaginate(int pageNum, int pageSize, string? orderBy, string? orderDir, OnboardingAccountListFilter filters);
    }
}