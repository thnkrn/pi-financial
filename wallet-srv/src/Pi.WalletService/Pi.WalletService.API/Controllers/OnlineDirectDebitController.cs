using System.ComponentModel.DataAnnotations;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.WalletService.API.Models;
using Pi.WalletService.Application.Commands.ODD;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Domain.Events.ODD;

namespace Pi.WalletService.API.Controllers;

[ApiController]
public class OnlineDirectDebitController : ControllerBase
{
    private readonly IRequestClient<RequestOnlineDirectDebitRegistration> requestOnlineDirectDebitRegistrationClient;
    private readonly IBus bus;

    public OnlineDirectDebitController(IRequestClient<RequestOnlineDirectDebitRegistration> requestOnlineDirectDebitRegistrationClient, IBus bus)
    {
        this.requestOnlineDirectDebitRegistrationClient = requestOnlineDirectDebitRegistrationClient;
        this.bus = bus;
    }

    [HttpPost("secure/odd/registration", Name = "RequestOnlineDirectDebitRegistration")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<OnlineDirectDebitRegistrationResultDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterOnlineDirectDebit(
        [FromHeader(Name = "user-id")] [Required]
        Guid userId,
        [FromBody] RegisterOnlineDirectDebitInfo info
    )
    {
        try
        {
            var refCode = DateTime.Now.Ticks;

            Response response = await requestOnlineDirectDebitRegistrationClient.GetResponse<OnlineDirectDebitRegistrationRequestSuccess, OnlineDirectDebitRegistrationRequestFailed>(
                new RequestOnlineDirectDebitRegistration(userId, info.BankPrefix, refCode.ToString()));

            switch (response)
            {
                case (_, OnlineDirectDebitRegistrationRequestSuccess a):
                    {
                        return Ok(new ApiResponse<OnlineDirectDebitRegistrationResultDto>(new OnlineDirectDebitRegistrationResultDto(a.WebUrl, true)));
                    }

                case (_, OnlineDirectDebitRegistrationRequestFailed b):
                    {
                        if (b.ErrorCode == ErrorCodes.InvalidUserId)
                        {
                            return Problem(statusCode: StatusCodes.Status400BadRequest, title: b.ErrorCode);
                        }

                        return Problem(statusCode: StatusCodes.Status500InternalServerError, title: b.ErrorCode);
                    }
            }

            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ErrorCodes.InternalServerError);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ErrorCodes.InternalServerError, detail: ex.Message);
        }
    }



    [HttpPost("public/odd/registration/callback", Name = "OnlineDirectDebitRegistrationCallback")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<OnlineDirectDebitRegistrationResultDto>))]
    public async Task<IActionResult> OnlineDirectDebitRegistrationCallback([FromBody] OnlineDirectDebitRegistrationCallbackDto dto)
    {
        await this.bus.Send(new UpdateOnlineDirectDebitRegistration(dto.Data.RegistrationRefCode, dto.Data.BankAccountNo, dto.Status.Status, dto.Status.ExternalStatusCode, dto.Status.ExternalStatusDescription));
        return Ok();
    }
}
