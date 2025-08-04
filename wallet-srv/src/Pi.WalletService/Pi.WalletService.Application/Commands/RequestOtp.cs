using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Client.OtpService.Api;
using Pi.Client.OtpService.Client;
using Pi.Client.OtpService.Model;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Exceptions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
namespace Pi.WalletService.Application.Commands;

public record RequestOtp(
    string UserId,
    Guid DeviceId
);

public record RequestOtpV2(Guid CorrelationId, TransactionType TransactionType);

public record RequestOtpSuccess(string RequestRef, Guid RequestId, string OtpRef);

public record RequestOtpV2Success(string TransactionNo, Guid RequestId, string RequestRef, Guid OtpRequestId, string OtpRef);

public class RequestOtpConsumer :
    SagaConsumer,
    IConsumer<RequestOtp>,
    IConsumer<RequestOtpV2>
{
    private readonly IOtpApi _otpApi;
    private readonly IUserService _userService;
    private readonly ILogger<RequestOtpConsumer> _logger;

    public RequestOtpConsumer(
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        ILogger<RequestOtpConsumer> logger,
        IUserService userService,
        IOtpApi otpApi) : base(depositEntrypointRepository, withdrawEntrypointRepository)
    {
        _logger = logger;
        _userService = userService;
        _otpApi = otpApi;
    }

    public async Task Consume(ConsumeContext<RequestOtp> context)
    {
        var resp = await RequestOtp(context.Message.UserId, context.Message.DeviceId);
        await context.RespondAsync(resp);
    }

    public async Task Consume(ConsumeContext<RequestOtpV2> context)
    {
        string userId;
        string transactionNo;
        Guid deviceId;
        Guid requestId;
        if (context.Message.TransactionType == TransactionType.Deposit)
        {
            var depositEntrypoint = await GetDepositEntrypointByIdAsync(context.Message.CorrelationId);
            if (depositEntrypoint == null)
            {
                throw new KeyNotFoundException("Deposit Entrypoint Not Found");
            }

            userId = depositEntrypoint.UserId;
            deviceId = depositEntrypoint.RequesterDeviceId!.Value;
            transactionNo = depositEntrypoint.TransactionNo!;
            requestId = depositEntrypoint.RequestId!.Value;
        }
        else if (context.Message.TransactionType == TransactionType.Withdraw)
        {
            var withdrawEntrypoint = await GetWithdrawEntrypointByIdAsync(context.Message.CorrelationId);
            if (withdrawEntrypoint == null)
            {
                throw new KeyNotFoundException("Withdraw Entrypoint Not Found");
            }

            userId = withdrawEntrypoint.UserId;
            deviceId = withdrawEntrypoint.RequesterDeviceId!.Value;
            transactionNo = withdrawEntrypoint.TransactionNo!;
            requestId = withdrawEntrypoint.RequestId!.Value;
        }
        else
        {
            throw new Exception("Invalid Transaction Type");
        }

        if (string.IsNullOrEmpty(userId) || deviceId.Equals(Guid.Empty))
        {
            throw new Exception("Invalid User Id or Device Id");
        }

        var resp = await RequestOtp(userId, deviceId);
        await context.RespondAsync(new RequestOtpV2Success(transactionNo, requestId, resp.RequestRef, resp.RequestId, resp.OtpRef));
    }

    private async Task<RequestOtpSuccess> RequestOtp(string userId, Guid deviceId)
    {
        var userInfo = await _userService.GetUserInfoById(userId);
        var requestRef = Guid.NewGuid().ToString();

        try
        {
            var resp = await _otpApi.InternalPlatformSendPostAsync(
                OtpPlatform.Sms,
                new SendOtpRequest(
                    Guid.Parse(userId),
                    deviceId,
                    userInfo.PhoneNumber,
                    requestRef
                )
            );

            return new RequestOtpSuccess(requestRef, resp.Data.RequestId, resp.Data.OtpRef);
        }
        catch (ApiException e)
        {
            _logger.LogError(e, "User Id: {UserId} Error sending otp request: {ErrorMessage}", userId, e.Message);

            var jsonErrorMessage = e.ErrorContent.ToString();
            // Can't seem to deserialize json to ProblemDetails so manually check string instead
            // ReSharper disable once InvertIf doesnt make sense
            if (jsonErrorMessage != null)
            {
                if (jsonErrorMessage.Contains("\"title\":\"OTP0003\""))
                {
                    throw new UserOtpVerificationLimitReachedException("OTP Verification Limit Reached");
                }
                if (jsonErrorMessage.Contains("\"title\":\"OTP0004\""))
                {
                    throw new UserOtpRequestLimitReachedException("OTP Request Limit Reached");
                }
            }

            throw;
        }
    }
}
