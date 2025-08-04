using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi.BackofficeService.API.Factories;
using Pi.BackofficeService.API.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.Common.Http;

namespace Pi.BackofficeService.API.Controllers;

[Authorize]
[ApiController]
[Route("onboard")]
public class OnboardingController : ControllerBase
{
    private readonly IOnboardingQueries _onboardingQueries;

    public OnboardingController(IOnboardingQueries onboardingQueries)
    {
        _onboardingQueries = onboardingQueries;
    }


    /// <summary>
    /// This function returns a list of open accounts. It is used in the Back Office tool to identify the accounts that have been requested to be opened but require action.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Policy = "ApplicationSummaryAccess")]
    [HttpGet("OpenAccount/List")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OnboardingOpenAccountResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> OnboardingOpenAccounts([FromQuery] OnBoardingFilterRequest request)
    {
        try
        {
            var filters = DtoFactory.NewOnboardAccountFilter(request);
            var result = await _onboardingQueries.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filters);
            var model = result.OpenAccountInfos.Select(DtoFactory.NewOnboardingAccountResponse).ToList();

            return Ok(new ApiPaginateResponse<List<OnboardingOpenAccountResponse>>(model,
                result.Page,
                result.PageSize,
                result.Total,
                result.OrderBy,
                result.OrderDir
            ));
        }
        catch (Exception e)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
        }
    }
}