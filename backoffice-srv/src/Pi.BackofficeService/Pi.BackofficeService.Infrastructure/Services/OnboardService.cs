using Microsoft.Extensions.Logging;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Services.OnboardService;
using Pi.BackofficeService.Infrastructure.Factories;
using Pi.Client.OnboardService.Api;
using Pi.Client.OnboardService.Client;
using Pi.Client.OnboardService.Model;

namespace Pi.BackofficeService.Infrastructure.Services;

public class OnboardService : IOnboardService
{
    private readonly IOpenAccountApi _openAccountApi;
    private readonly IAtsApi _atsApi;
    private readonly ILogger<OnboardService> _logger;

    public OnboardService(
        IOpenAccountApi openAccountApi,
        IAtsApi atsApi,
        ILogger<OnboardService> logger)
    {
        _openAccountApi = openAccountApi;
        _atsApi = atsApi;
        _logger = logger;
    }

    public async Task<PaginateOpenAccountListResult> GetOpenAccounts(int pageNum, int pageSize, string? orderBy, string? orderDir, OnboardingAccountListFilter filters)
    {
        try
        {
            var response = await _openAccountApi.InternalGetOpenAccountListAsync(filters.Status, filters.CitizenId, filters.Custcode, filters.UserId, filters.Date, filters.BpmReceived, pageNum, pageSize, orderBy, orderDir);
            var results = EntityFactory.NewOpenAccounts(response.Data);
            return new PaginateOpenAccountListResult(results, response.Page, response.PageSize, response.Total, response.OrderBy, response.OrderDir);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Error - calling InternalGetOpenAccountListAsync");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error - calling InternalGetOpenAccountListAsync in OnBoarding Service and mapping to DTO");
            throw;
        }
    }

    public async Task<List<Application.Models.OpenAccountInfoDto>> GetOpenAccounts(string userId)
    {
        try
        {
            var response = await _openAccountApi.InternalGetOpenAccountAsync(userId);
            var results = EntityFactory.NewOpenAccounts(response.Data);
            return results;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Error - calling InternalGetOpenAccountAsync");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error - calling InternalGetOpenAccountAsync in OnBoarding Service and mapping to DTO");
            throw;
        }
    }

    public async Task<PiOnboardServiceAPIModelsAtsAtsRequestsPaginated> GetAtsUpdateRequests(
        string? atsUploadType,
        DateTime? requestDate,
        int? page = 1,
        int? pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await _atsApi.InternalGetAtsUpdateRequestsAsync(atsUploadType, requestDate, page, pageSize, cancellationToken: cancellationToken);
        return response;
    }

    public async Task<PiOnboardServiceAPIModelsAtsAtsUploadResultDtoApiResponse> GetAtsUpdateResults(Guid atsRequestId, CancellationToken cancellationToken = default)
    {
        var response = await _atsApi.InternalGetAtsUpdateResultAsync(atsRequestId, cancellationToken);
        return response;
    }

    public async Task AddAtsBankAccount(
        string fileName,
        string userName,
        string uploadType,
        List<PiOnboardServiceApplicationCommandsUpdateAtsBankEffectiveDateRow> atsBanks,
        CancellationToken cancellationToken = default)
    {
        var atsUploadType =
            Enum.Parse<PiOnboardServiceApplicationCommandsUpdateAtsBankEffectiveDateRequest.UploadTypeEnum>(uploadType);
        await _atsApi.InternalAddAtsUpdateRequestAsync(new PiOnboardServiceApplicationCommandsUpdateAtsBankEffectiveDateRequest(fileName, userName, atsUploadType, atsBanks), cancellationToken);
    }
}
