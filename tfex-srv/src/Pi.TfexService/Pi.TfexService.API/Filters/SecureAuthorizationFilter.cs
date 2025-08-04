using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pi.Common.Features;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.UserService;
using Pi.TfexService.Application.Utils;

namespace Pi.TfexService.API.Filters;

public class SecureAuthorizationFilter(IUserService userService, IUserV2Service userV2Service, IFeatureService featureService) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            if (context.HttpContext.Request.Headers.TryGetValue("user-id", out var userId))
            {
                // Retrieve accountCode from route values
                if (context.ActionArguments.TryGetValue("accountCode", out var accountCodeObj) &&
                    accountCodeObj is string accountCode)
                {
                    // Call user service and verify trading account
                    var user = featureService.IsOn(Features.MigrateUserV2)
                        ? await userV2Service.GetUserById(userId!)
                        : await userService.GetUserById(userId.ToString());

                    if (!user.TradingAccountNoList.Contains(accountCode))
                    {
                        throw new UnauthorizedAccessException("User does not have permission to access this account");
                    }
                }
                else
                {
                    throw new ArgumentException("AccountCode is required");
                }
            }
            else
            {
                throw new ArgumentException("UserId is required");
            }

            // If authorization passed, continue to the next action/middleware
            await next();
        }
        catch (Exception e)
        {
            var errorResponse = ExceptionUtils.HandleException(e);
            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                Title = errorResponse.title,
                Status = errorResponse.statusCode,
                Detail = errorResponse.detail
            };
            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }
}