using System;
using System.ComponentModel.DataAnnotations;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.User.API.Models;
using Pi.User.Application.Commands;
using Pi.User.Application.Queries;

namespace Pi.User.API.Controllers
{
    [ApiController]
    public class UserNotificationPreferenceController : ControllerBase
    {
        private readonly INotificationPreferenceQueries notificationPreferenceQueries;
        private readonly IBus bus;

        public UserNotificationPreferenceController(INotificationPreferenceQueries notificationPreferenceQueries, IBus bus)
        {
            this.notificationPreferenceQueries = notificationPreferenceQueries;
            this.bus = bus;
        }

        [HttpGet("secure/notification-preference")]
        [HttpGet("internal/notification-preference")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<DeviceResponse>>> GetNotificationPreference(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [FromHeader(Name = "deviceId")] [Required]
        string deviceId
    )
        {
            try
            {
                var res = await notificationPreferenceQueries.GetNotificationPreference(Guid.Parse(userId), Guid.Parse(deviceId));

                return Ok(new ApiResponse<DeviceResponse>(UserController.MapDeviceResponse(res)));
            }
            catch (Exception e)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: ErrorCodes.Usr0001.ToString().ToUpper(),
                    detail: e.Message);
            }
        }

        [HttpPut("secure/notification-preference")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<ApiResponse<NotificationPreferenceTicket>>> UpdateNotificationPreference(
            [FromHeader(Name = "user-id")] [Required]
        string userId,
            [FromHeader(Name = "deviceId")] [Required]
        string deviceId,
            [FromBody] [Required]
        NotificationPreferenceRequest notificationPreferenceRequest)
        {
            try
            {
                var ticketId = Guid.NewGuid();

                await bus.Publish(new UpdateNotificationPreference(
                    Guid.Parse(userId),
                    Guid.Parse(deviceId),
                    true,
                    notificationPreferenceRequest.Order,
                    notificationPreferenceRequest.Portfolio,
                    notificationPreferenceRequest.Wallet,
                    notificationPreferenceRequest.Market)
                );

                return Accepted(
                    new ApiResponse<NotificationPreferenceTicket>(new NotificationPreferenceTicket(ticketId)));
            }
            catch (Exception e)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: ErrorCodes.Usr0001.ToString().ToUpper(),
                    detail: e.Message);
            }
        }
    }
}

