using Pi.BackofficeService.Application.Factories;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Services.OnboardService;
using Pi.BackofficeService.Application.Services.UserService;

namespace Pi.BackofficeService.Application.Queries;

public class OnboardingQueries : IOnboardingQueries
{
    private readonly IOnboardService _onboardService;
    private readonly IUserService _userService;

    public OnboardingQueries(IOnboardService onboardService, IUserService userService)
    {
        _onboardService = onboardService;
        _userService = userService;
    }

    public async Task<PaginateOpenAccountListResult> GetOpenAccountsPaginate(int pageNum, int pageSize, string? orderBy, string? orderDir, OnboardingAccountListFilter filters)
    {
        var response = await _onboardService.GetOpenAccounts(pageNum, pageSize, orderBy, orderDir, filters);
        return response;
    }

    public async Task<List<OpenAccountInfoDto>?> GetOpenAccountsByCustCode(string custCode)
    {
        var user = await _userService.GetUserByIdOrCustomerCode(custCode, true);
        if (user == null)
            return null;

        var response = await _onboardService.GetOpenAccounts(user.Id);
        foreach (var info in response)
        {
            if (info.Identification != null)
            {
                info.Identification.UserId = user.Id;
                info.Identification.Email = user.Email;
                info.Identification.Phone = user.PhoneNumber;
                info.CustCode = custCode;
            }
        }

        return response;
    }
}
