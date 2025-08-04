using System.ComponentModel.DataAnnotations;
using Amazon.SimpleNotificationService.Model;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Features;
using Pi.Common.Http;
using Pi.User.API.Models;
using Pi.User.Application.Commands;
using Pi.User.Application.Models;
using Pi.User.Application.Queries;
using Pi.User.Application.Utils;

namespace Pi.User.API.Controllers;

[ApiController]
public class UserDeviceController : ControllerBase
{
    private readonly IBus _bus;
    private readonly IUserQueries _userQueries;
    private readonly IFeatureService _featureService;

    public UserDeviceController(IBus bus, IUserQueries userQueries, IFeatureService featureService)
    {
        this._bus = bus;
        this._userQueries = userQueries;
        this._featureService = featureService;
    }

    [HttpGet("internal/user/{id}/device")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<ApiResponse<List<DeviceResponse>>>> GetDevice(
        [FromRoute] Guid id)
    {
        try
        {
            var res = await _userQueries.GetUser(id);

            var deviceRes = res.Devices.Select(UserController.MapDeviceResponse).ToList();

            return Ok(new ApiResponse<List<DeviceResponse>>(
                    deviceRes
                )
            );
        }
        catch (UserNotFoundException e)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: ErrorCodes.Usr0004.ToString().ToUpper(),
                detail: e.Message);
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.Usr0005.ToString().ToUpper(),
                detail: e.Message);
        }
    }

    /// <summary>
    /// Create or update device
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="deviceId"></param>
    /// <param name="platform"></param>
    /// <param name="sid"></param>
    /// <param name="createOrUpdateDeviceRequest"></param>
    /// <param name="lang"></param>
    /// <returns></returns>
    [HttpPost("secure/user/device")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<ApiResponse<UpdateUserInfoTicketId>>> CreateOrUpdateDevice(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [FromHeader(Name = "deviceId")] [Required]
        string deviceId,
        [FromHeader(Name = "device")] [Required]
        string platform,
        [FromHeader(Name = "Sid")]
        string? sid,
        [FromBody] [Required]
        CreateOrUpdateDeviceRequest createOrUpdateDeviceRequest,
        [FromHeader(Name = "Accept-Language")] [Required]
        string lang = "th-TH")
    {
        try
        {

            var id = Guid.Parse(userId);
            var deviceIdGuid = Guid.Parse(deviceId);

            var deviceReq = new CreateOrUpdateDevice(
                id,
                deviceIdGuid,
                createOrUpdateDeviceRequest.DeviceToken,
                lang,
                platform
            );

            await _bus.Publish(
                deviceReq,
                ctx =>
                {
                    if (ctx is AmazonSqsSendContext sqsContext)
                    {
                        sqsContext.SetDeduplicationId(deviceReq.GetDeduplicationId());
                        sqsContext.SetGroupId(nameof(CreateOrUpdateDevice));
                    }
                }
            );

            // Disable update to userinfo in Sirius code
            if (_featureService.IsOff(FeatureSwitch.SiriusAuthMigration))
            {
                await _bus.Publish(new UpdateUserInfo(id)
                {
                    Sid = sid,
                    CurrentDeviceId = deviceId,
                    CurrentPlatform = platform
                });
            }

            return Accepted(new ApiResponse<UpdateUserInfoTicketId>(new UpdateUserInfoTicketId(id)));
        }
        catch (InvalidParameterException)
        {
            // Duplicated DeduplicationId, ignore
            var parsed = Guid.TryParse(userId, out var id);
            return Accepted(new ApiResponse<UpdateUserInfoTicketId>(
                new UpdateUserInfoTicketId(
                    parsed
                        ? id
                        : new Guid())
            ));
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.Usr0002.ToString().ToUpper(),
                detail: e.Message);
        }
    }

    /// <summary>
    /// Internal Create or update device
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="deviceId"></param>
    /// <param name="platform"></param>
    /// <param name="createOrUpdateDeviceRequest"></param>
    /// <param name="lang"></param>
    /// <returns></returns>
    [HttpPost("internal/user/device")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<ApiResponse<UpdateUserInfoTicketId>>> InternalCreateOrUpdateDevice(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [FromHeader(Name = "deviceId")] [Required]
        string deviceId,
        [FromHeader(Name = "device")] [Required]
        string platform,
        [FromBody] [Required]
        CreateOrUpdateDeviceRequest createOrUpdateDeviceRequest,
        [FromHeader(Name = "Accept-Language")] [Required]
        string lang = "th-TH")
    {
        try
        {
            var id = Guid.Parse(userId);
            var deviceIdGuid = Guid.Parse(deviceId);

            var deviceReq = new CreateOrUpdateDevice(
                id,
                deviceIdGuid,
                createOrUpdateDeviceRequest.DeviceToken,
                lang,
                platform
            );

            var client = _bus.CreateRequestClient<CreateOrUpdateDevice>();
            var resp = await client.GetResponse<Application.Models.User>(deviceReq, x =>
            {
                x.UseExecute(ctx =>
                {
                    ctx.SetDeduplicationId(deviceReq.GetDeduplicationId());
                    ctx.SetGroupId(nameof(CreateOrUpdateDevice));
                });
            });

            return Ok(new ApiResponse<Application.Models.User>(resp.Message));
        }
        catch (RequestException e)
        {
            var errorCode = e.InnerException switch
            {
                InvalidDataException => ErrorCodes.Usr0004,
                _ => ErrorCodes.Usr0000
            };

            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: errorCode.ToString().ToUpper(),
                detail: e.InnerException?.Message ?? errorCode.ToDescriptionString()
            );
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.Usr0002.ToString().ToUpper(),
                detail: e.Message);
        }
    }

    [HttpDelete("secure/user/device")]
    [HttpDelete("internal/user/device")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<ApiResponse<UpdateUserInfoTicketId>>> DeregisterDevice(
        [FromHeader(Name = "user-id")] [Required]
        Guid userId,
        [FromHeader(Name = "deviceId")] [Required]
        Guid deviceId)
    {
        try
        {
            await _bus.Publish(
                new DeregisterDevice(
                    userId,
                    deviceId
                ));

            return Accepted(new ApiResponse<UpdateUserInfoTicketId>(new UpdateUserInfoTicketId(userId)));
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.Usr0003.ToString().ToUpper(),
                detail: e.Message);
        }
    }
}
