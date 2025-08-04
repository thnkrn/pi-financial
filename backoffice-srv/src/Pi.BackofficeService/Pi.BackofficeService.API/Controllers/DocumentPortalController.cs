using Microsoft.AspNetCore.Mvc;
using Pi.BackofficeService.API.Factories;
using Pi.BackofficeService.API.Models;
using Pi.BackofficeService.Application.Queries;
using Microsoft.AspNetCore.Authorization;


namespace Pi.BackofficeService.API.Controllers;

[Authorize]
[ApiController]
[Route("document_portal")]
public class DocumentPortalController : ControllerBase
{
    private readonly IOnboardingQueries _onboardingQueries;

    public DocumentPortalController(IOnboardingQueries backofficeQueries)
    {
        _onboardingQueries = backofficeQueries;
    }


    /// <summary>
    /// This function returns a list of open accounts. It is used in the Back Office tool to retrieve list of onboarding accounts via cust code.
    /// </summary>
    /// <param name="custCode"></param>
    /// <returns></returns>
    [Authorize(Policy = "DocumentPortalAccess")]
    [HttpGet("open_accounts/{custCode}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Common.Http.ApiResponse<List<OnboardingOpenAccountResponse>>))]
    public async Task<IActionResult> OnboardingOpenAccounts([FromRoute] string custCode)
    {
        try
        {
            var result = await _onboardingQueries.GetOpenAccountsByCustCode(custCode);
            if (result == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: "Sorry Customer Code not found, please try again");
            }

            var model = result.Select(DtoFactory.NewOnboardingAccountResponse).ToList();

            return Ok(new List<OnboardingOpenAccountResponse>(model));
        }
        catch (Exception ex)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: ex.Message);
        }
    }
}
