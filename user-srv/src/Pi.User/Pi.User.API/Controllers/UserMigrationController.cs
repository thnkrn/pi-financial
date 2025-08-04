using System.ComponentModel.DataAnnotations;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Domain.AggregatesModel.ProductAggregate;
using Pi.Common.Http;
using Pi.User.API.Models;
using Pi.User.Application.Commands;
using Pi.User.Application.Queries;
using Pi.User.Application.Services.Customer;
using Pi.User.Application.Utils;
using Pi.User.Application.Models;

namespace Pi.User.API.Controllers;

[ApiController]
public class UserMigrationController : ControllerBase
{
    private readonly IBus _bus;
    private readonly ICustomerService _customerService;
    private readonly IMediator _mediator;
    private readonly IUserQueries _userQueries;
    private readonly IUserTradingAccountQueries _userTradingAccountQueries;

    public UserMigrationController(
        IBus bus,
        ICustomerService customerService,
        IUserQueries userQueries,
        IUserTradingAccountQueries userTradingAccountQueries,
        IMediator mediator)
    {
        _bus = bus;
        _customerService = customerService;
        _userQueries = userQueries;
        _userTradingAccountQueries = userTradingAccountQueries;
        _mediator = mediator;
    }

    [HttpPost("internal/v2/user", Name = "CreateUserV2")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CreateUserResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> CreateUser(
        [FromHeader(Name = "device")] string? device,
        [FromBody] CreateUserRequest createUserRequest
    )
    {
        try
        {
            var client = _bus.CreateRequestClient<CreateUserInfo>();

            var id = createUserRequest.Id ?? Guid.NewGuid();

            var result =
                await client.GetResponse<Application.Models.User>(new CreateUserInfo(id,
                    createUserRequest.CustomerId)
                {
                    Email = createUserRequest.Email,
                    PhoneNumber = createUserRequest.Mobile,
                    CitizenId = createUserRequest.CitizenId,
                    FirstnameEn = createUserRequest.FirstnameEn,
                    LastnameEn = createUserRequest.LastnameEn,
                    FirstnameTh = createUserRequest.FirstnameTh,
                    LastnameTh = createUserRequest.LastnameTh,
                    WealthType = createUserRequest.WealthType
                });

            return Ok(new ApiResponse<CreateUserResponse>(new CreateUserResponse(result.Message.Id)));
        }
        catch (Exception e)
        {
            return HandleInternalServerError(e);
        }
    }

    /// <summary>
    ///     Get user id by customer code for login.
    /// </summary>
    /// <param name="customerCode">The customer code.</param>
    /// <returns>UserInfoCitizenIdResponse</returns>
    [HttpGet("internal/user/login/{customerCode}", Name = "GetUserIdByCustCodeForLogin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserInfoForLoginResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> GetUserIdByCustCodeForLogin(
        [FromRoute] string customerCode
    )
    {
        try
        {
            var customerInfo = await _customerService.GetCustomerInfoByCustomerCode(customerCode) ??
                               throw new UserNotFoundException(
                                   $"Not found customer with customer code: {customerCode}");

            if (customerInfo is not { IsIndividualCustomer: true })
                throw new UserNotFoundException($"Customer is not individual: {customerCode}");

            var userInfo = await _userQueries.GetUserByCitizenId(customerInfo.IdentificationCard.Number);

            return Ok(new ApiResponse<UserInfoForLoginResponse>(new UserInfoForLoginResponse(userInfo.Id,
                customerInfo.IdentificationCard.Number,
                userInfo.CustomerId ?? throw new InvalidDataException("Customer id not found"))));
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
            return HandleInternalServerError(e);
        }
    }

    [HttpGet("secure/user/name")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserNameResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> GetUserName(
        [FromHeader(Name = "user-id")] string userId)
    {
        try
        {
            var user = await _userQueries.GetUser(new Guid(userId));

            return Ok(new ApiResponse<UserNameResponse>(new UserNameResponse(
                Guid.NewGuid(),
                user.FirstnameTh,
                user.LastnameTh,
                user.FirstnameEn,
                user.LastnameEn
            )));
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
                title: ErrorCodes.Usr0005.ToString().ToUpper(),
                detail: e.Message);
        }
    }

    /// <summary>
    ///     Get user by phone number
    /// </summary>
    /// <param name="phoneNumber">Phone Number</param>
    /// <returns>UserInfoResponse</returns>
    [HttpGet("internal/user/phone/by-phone")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserInfoResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> GetUserByPhoneNumber(
        [FromQuery] string phoneNumber)
    {
        try
        {
            var user = await _userQueries.GetUserByPhoneNumber(phoneNumber);
            var userInfoResponse = UserInfoResponse.MapUserInfoResponse(user);
            return Ok(new ApiResponse<UserInfoResponse>(userInfoResponse));
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
            return HandleInternalServerError(e);
        }
    }

    /// <summary>
    ///     Get user by customer id
    /// </summary>
    /// <param name="customerId">Customer Id</param>
    /// <returns>UserInfoResponse</returns>
    [HttpGet("internal/user/customer/{customerId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserInfoResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> GetUserByCustomerId(
        [FromRoute] string customerId)
    {
        try
        {
            var user = await _userQueries.GetUserByCustomerId(customerId);
            var userInfoResponse = UserInfoResponse.MapUserInfoResponse(user);
            return Ok(new ApiResponse<UserInfoResponse>(userInfoResponse));
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
            return HandleInternalServerError(e);
        }
    }

    [HttpPatch("internal/user", Name = "PartialUpdateUserInfo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> PartialUpdateUserInfo(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [FromBody] PartialUpdateUserInfoRequest partialUpdateUserInfoRequest
    )
    {
        try
        {
            var client = _mediator.CreateRequestClient<PartialUpdateUserInfo>();
            await client.GetResponse<PartialUpdateUserInfoResponse>(new PartialUpdateUserInfo(
                    Guid.Parse(userId),
                    partialUpdateUserInfoRequest.CitizenId,
                    partialUpdateUserInfoRequest.Email,
                    partialUpdateUserInfoRequest.FirstnameTh,
                    partialUpdateUserInfoRequest.LastnameTh,
                    partialUpdateUserInfoRequest.FirstnameEn,
                    partialUpdateUserInfoRequest.LastnameEn,
                    partialUpdateUserInfoRequest.PhoneNumber,
                    partialUpdateUserInfoRequest.PlaceOfBirthCity,
                    partialUpdateUserInfoRequest.PlaceOfBirthCountry,
                    partialUpdateUserInfoRequest.DateOfBirth,
                    partialUpdateUserInfoRequest.WealthType
                )
            );
            return Ok();
        }
        catch (RequestException e)
        {
            return e.InnerException switch
            {
                UserNotFoundException => Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: ErrorCodes.Usr0001.ToString().ToUpper(),
                    detail: ErrorCodes.Usr0001.ToDescriptionString()),
                _ => Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: ErrorCodes.Usr0000.ToString(),
                    detail: ErrorCodes.Usr0000.ToDescriptionString())
            };
        }
        catch (Exception e)
        {
            return HandleInternalServerError(e);
        }
    }

    [HttpGet("internal/user/id-by-customer-code")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Guid>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> GetUserIdByCustomerCode(
        [FromQuery] string customerCode)
    {
        try
        {
            var userId = await _userQueries.GetUserIdByCustomerCode(customerCode);

            return Ok(new ApiResponse<Guid>(userId));
        }
        catch (UserNotFoundException e)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: ErrorCodes.Usr0001.ToString().ToUpper(),
                detail: e.Message);
        }
    }

    /// <summary>
    /// Returns list of trading accounts belonging to <paramref name="userId"/>, grouped by customer code.
    /// </summary>
    /// <param name="userId">Guid user id to query trading accounts for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of trading accounts, grouped by customer code.</returns>
    [HttpGet("internal/v2/trading-accounts", Name = "InternalGetTradingAccountV2")]
    [HttpGet("secure/v2/trading-accounts", Name = "SecureGetTradingAccountV2")]
    [ProducesResponseType(StatusCodes.Status200OK,
        Type = typeof(ApiResponse<List<UserTradingAccountInfoWithExternalAccounts>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> InternalGetTradingAccountV2(
        [FromHeader(Name = "user-id")] [Required]
        Guid userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var tradingAccounts =
                await _userTradingAccountQueries.GetUserTradingAccountsWithExternalAccountsByUserId(
                    userId, cancellationToken);

            return Ok(new ApiResponse<List<UserTradingAccountInfoWithExternalAccounts>>(tradingAccounts));
        }
        catch (Exception e)
        {
            return HandleInternalServerError(e);
        }
    }

    [HttpGet("internal/migrate/users", Name = "InternalGetMigrateUsers")]
    [ProducesResponseType(StatusCodes.Status200OK,
        Type = typeof(ApiResponse<MigrateCustomerInfo>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<ActionResult> InternalGetMigrateUsers(
        [FromQuery(Name = "skip")] [Required]
        int skip,
        [FromQuery(Name = "limit")] [Required]
        int limit,
        CancellationToken cancellationToken)
    {
        try
        {
            var resp = await _userQueries.GetMigrateCustomerInfo(skip, limit);

            return Ok(new ApiResponse<MigrateCustomerInfo>(resp));
        }
        catch (Exception e)
        {
            return HandleInternalServerError(e);
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