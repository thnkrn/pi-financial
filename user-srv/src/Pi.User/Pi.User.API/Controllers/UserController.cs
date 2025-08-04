using System.ComponentModel.DataAnnotations;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.User.API.Models;
using Pi.User.Application.Commands;
using Pi.User.Application.Queries;
using Pi.User.Application.Models;

namespace Pi.User.API.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserQueries _userQueries;
    private readonly IBus _bus;

    public UserController(
        IUserQueries userQueries,
        IBus bus)
    {
        _userQueries = userQueries;
        _bus = bus;
    }

    [HttpGet("secure/user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserInfoResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> GetById(
        [FromHeader(Name = "user-id")]
        string userId)
    {
        try
        {
            var user = await _userQueries.GetUser(new Guid(userId));
            return this.Ok(new ApiResponse<UserInfoResponse>(MapUserInfoResponse(user)));
        }
        catch (UserNotFoundException e)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: ErrorCodes.Usr0001.ToString().ToUpper(),
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
    /// Get or create user v2
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet("internal/v2/user", Name = "GetOrCreateUserV2")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Application.Models.User>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<ApiResponse<Application.Models.User>>> GetOrCreateUserV2(
        [FromHeader(Name = "customer-id")]
        string customerId
    )
    {
        try
        {
            var user = await _userQueries.CreateUserIfNotExist(customerId);
            return this.Ok(new ApiResponse<Application.Models.User>(user));
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
    /// Get user by user id or customer code v2
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isCustCode"></param>
    /// <returns></returns>
    [HttpGet("internal/v2/user/{id}", Name = "GetUserByIdOrCustomerCodeV2")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Application.Models.User>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<ApiResponse<Application.Models.User>>> GetUserByIdOrCustomerCodeV2(
        [FromRoute] string id,
        [FromQuery(Name = "isCustCode")]
        bool isCustCode = false)
    {
        try
        {
            var user = isCustCode
                ? await _userQueries.GetUserByCustCode(id)
                : await _userQueries.GetUser(new Guid(id));
            return this.Ok(new ApiResponse<Application.Models.User>(user));
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: ErrorCodes.Usr0001.ToString().ToUpper(),
                detail: e.Message);
        }
    }

    [HttpGet("internal/user/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserInfoResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> GetUserByIdOrCustomerCode(
        [FromRoute] string id,
        [FromQuery(Name = "isCustCode")]
        bool isCustCode = false)
    {
        try
        {
            var user = isCustCode
                ? await _userQueries.GetUserByCustCode(id)
                : await _userQueries.GetUser(new Guid(id));
            return this.Ok(new ApiResponse<UserInfoResponse>(MapUserInfoResponse(user)));
        }
        catch (UserNotFoundException e)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: ErrorCodes.Usr0001.ToString().ToUpper(),
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

    [HttpGet("internal/user/email/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserInfoResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> GetUserByEmail(
        [FromRoute] string email)
    {
        try
        {
            var user = await _userQueries.GetUserByEmail(email);
            return this.Ok(new ApiResponse<UserInfoResponse>(MapUserInfoResponse(user)));
        }
        catch (UserNotFoundException e)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: ErrorCodes.Usr0001.ToString().ToUpper(),
                detail: e.Message);
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.Usr0001.ToString().ToUpper(),
                detail: e.Message);
        }
    }

    [HttpGet("internal/user/citizenId/{citizenId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserInfoResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> GetUserByCitizenId(
        [FromRoute] string citizenId)
    {
        try
        {
            var user = await _userQueries.GetUserByCitizenId(citizenId);
            return this.Ok(new ApiResponse<UserInfoResponse>(MapUserInfoResponse(user)));
        }
        catch (UserNotFoundException e)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: ErrorCodes.Usr0001.ToString().ToUpper(),
                detail: e.Message);
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.Usr0001.ToString().ToUpper(),
                detail: e.Message);
        }
    }

    [HttpGet("internal/user/bulk")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<UserInfoResponse>>))]
    public async Task<ActionResult> GetUsersByIdsOrCustomerCodes(
        [Required] [FromQuery(Name = "ids")]
        IEnumerable<string> ids,
        [FromQuery(Name = "isCustCode")]
        bool isCustCode = false)
    {
        try
        {
            var users = isCustCode
                ? await _userQueries.GetBulkUserByCustCode(ids)
                : await _userQueries.GetBulkUser(ids.Select(id => new Guid(id)));
            return this.Ok(new ApiResponse<IEnumerable<UserInfoResponse>>(users.Select(MapUserInfoResponse)));
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.Usr0005.ToString().ToUpper(),
                detail: e.Message);
        }
    }

    [HttpPost("internal/user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UserTicketId>>> CreateUserInfo(
        [FromHeader(Name = "customer-id")] [Required]
        string customerId,
        [FromHeader(Name = "device")] [Required]
        string platform,
        [FromBody] [Required]
        UserInfoRequest userInfoRequest)
    {
        try
        {
            var deviceInfoList = userInfoRequest.Devices.Select(d => new DeviceInfo(
                Guid.Parse(d.DeviceId),
                d.DeviceToken,
                d.Language,
                platform)
            ).ToList();

            var newId = Guid.NewGuid();

            await _bus.Publish(new CreateUserInfo(
                newId,
                customerId
            )
            {
                Devices = deviceInfoList,
                CustCodes = userInfoRequest.CustCodes,
                TradingAccounts = userInfoRequest.TradingAccounts,
                CitizenId = userInfoRequest.CitizenId,
                PhoneNumber = userInfoRequest.PhoneNumber,
                GlobalAccount = userInfoRequest.GlobalAccount,
                WealthType = userInfoRequest.WealthType
            });

            return Accepted(new ApiResponse<UserTicketId>(new UserTicketId(newId)));
        }
        catch (Exception e)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.Usr0002.ToString().ToUpper(),
                detail: e.Message);
        }
    }

    [HttpGet("internal/user/{id}/citizen-id")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserInfoCitizenIdResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<ApiResponse<UserInfoCitizenIdResponse>>> GetCitizenId(
        [FromRoute] string id)
    {
        try
        {
            var guid = new Guid(id);
            var res = await _userQueries.GetUserWithCitizenId(guid);

            return Ok(new ApiResponse<UserInfoCitizenIdResponse>(
                    new UserInfoCitizenIdResponse(
                        res.Id,
                        res.CitizenId
                    )
                )
            );
        }
        catch (InvalidDataException e)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: ErrorCodes.Usr0004.ToString().ToUpper(),
                detail: e.Message);
        }
    }

    private static UserInfoResponse MapUserInfoResponse(Application.Models.User user)
    {
        return new UserInfoResponse(
            user.Id,
            user.Devices.Select(MapDeviceResponse).ToList(),
            user.CustomerCodes.Select(c => c.Code).ToList(),
            user.TradingAccounts.Select(t => t.TradingAccountId).ToList(),
            user.FirstnameTh,
            user.LastnameTh,
            user.FirstnameEn,
            user.LastnameEn,
            user.PhoneNumber,
            user.GlobalAccount,
            user.Email,
            user.CustomerId,
            user.CitizenId,
            user.WealthType
        );
    }

    public static DeviceResponse MapDeviceResponse(Device device)
    {
        return new DeviceResponse(
            device.DeviceId,
            device.DeviceToken,
            device.DeviceIdentifier,
            device.Language,
            device.Platform,
            device.NotificationPreference != null
                ? new NotificatonPreferenceResponse(
                    device.NotificationPreference.Important,
                    device.NotificationPreference.Order,
                    device.NotificationPreference.Market,
                    device.NotificationPreference.Portfolio,
                    device.NotificationPreference.Wallet
                )
                : null
        );
    }
}