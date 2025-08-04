using Pi.BackofficeService.Application.Models;
using Pi.Client.OnboardService.Model;

namespace Pi.BackofficeService.Application.Services.OnboardService;

public interface IOnboardService
{
    Task<PaginateOpenAccountListResult> GetOpenAccounts(int pageNum, int pageSize, string? orderBy, string? orderDir, OnboardingAccountListFilter filters);
    Task<List<OpenAccountInfoDto>> GetOpenAccounts(string userId);
    Task<PiOnboardServiceAPIModelsAtsAtsRequestsPaginated> GetAtsUpdateRequests(
        string? atsUploadType,
        DateTime? requestDate,
        int? page = 1,
        int? pageSize = 10,
        CancellationToken cancellationToken = default);
    Task<PiOnboardServiceAPIModelsAtsAtsUploadResultDtoApiResponse> GetAtsUpdateResults(Guid atsRequestId, CancellationToken cancellationToken = default);
    Task AddAtsBankAccount(
        string fileName,
        string userName,
        string uploadType,
        List<PiOnboardServiceApplicationCommandsUpdateAtsBankEffectiveDateRow> atsBanks,
        CancellationToken cancellationToken = default);
}
