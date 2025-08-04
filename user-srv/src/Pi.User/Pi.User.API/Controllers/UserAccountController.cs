using System.ComponentModel.DataAnnotations;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.User.API.Models;
using Pi.User.Application.Commands;

namespace Pi.User.API.Controllers;

[ApiController]
public class UserAccountController(IPublishEndpoint bus) : ControllerBase
{

    [HttpPost("internal/user-account")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateUserAccount([FromHeader(Name = "user-id")] Guid userId, [FromBody][Required] CreateUserAccountRequest createUserRequest)
    {
        try
        {
            var command = new CreateUserAccount(userId, createUserRequest.UserAccountId, createUserRequest.UserAccountType);
            await bus.Publish(command);

            return Accepted();
        }
        catch (Exception ex)
        {
            return HandleInternalServerError(ex);
        }
    }

    private ObjectResult HandleInternalServerError(Exception e)
    {
        return Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: ErrorCodes.Usr0001.ToString().ToUpper(),
            detail: e.Message);
    }
}