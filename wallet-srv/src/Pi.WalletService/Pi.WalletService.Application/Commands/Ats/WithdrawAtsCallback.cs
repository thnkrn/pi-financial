using Pi.WalletService.Application.Services.Freewill;
using MassTransit;

namespace Pi.WalletService.Application.Commands.Ats;

public record WithdrawAtsCallback(string Requester, string Application, string Token, string PreToken, string Message);

public class WithdrawAtsCallbackConsumer : IConsumer<WithdrawAtsCallback>
{
    private readonly ICallbackForwarderService _callbackForwarderService;

    public WithdrawAtsCallbackConsumer(ICallbackForwarderService callbackForwarderService)
    {
        _callbackForwarderService = callbackForwarderService;
    }

    public async Task Consume(ConsumeContext<WithdrawAtsCallback> context)
    {
        await _callbackForwarderService.ForwardWithdrawAtsCallback(
            context.Message.Requester,
            context.Message.Application,
            context.Message.Token,
            context.Message.PreToken,
            context.Message.Message);
    }
}