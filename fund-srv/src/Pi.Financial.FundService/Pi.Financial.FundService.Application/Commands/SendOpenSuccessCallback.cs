using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Financial.FundService.Application.Models.Enums;
using Pi.Financial.FundService.Application.Services.OnboardService;

namespace Pi.Financial.FundService.Application.Commands;

public record SendOpenSuccessCallback(string CustCode, long? CustomerId, string OpenAccountRegisterUid, OpenFundAccountStatus Status);

public class SendOpenSuccessCallbackConsumer : IConsumer<SendOpenSuccessCallback>
{
    private readonly IOnboardService _onboardService;
    private readonly ILogger<SendOpenSuccessCallbackConsumer> _logger;

    public SendOpenSuccessCallbackConsumer(IOnboardService onboardService, ILogger<SendOpenSuccessCallbackConsumer> logger)
    {
        _onboardService = onboardService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendOpenSuccessCallback> context)
    {
        _logger.LogInformation("Init SendOpenSuccessCallback: {@Message}", context.Message);
        try
        {
            if (!string.IsNullOrEmpty(context.Message.OpenAccountRegisterUid))
            {
                await _onboardService.UpdateOpenFundAccountStatus(Guid.Parse(context.Message.OpenAccountRegisterUid), context.Message.Status);
            }
            else
            {
                _logger.LogError(
                    "Unable to Call OnboardService Parameter missing. CustCode: {CustCode}, OpenAccountRegisterUid: {OpenAccountRegisterUid}, Status: {Status}",
                    context.Message.CustCode,
                    context.Message.OpenAccountRegisterUid,
                    context.Message.Status
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while SendOpenSuccessCallback");
        }
    }
}
