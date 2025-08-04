using Pi.WalletService.Application.Services.Freewill;
using MassTransit;

namespace Pi.WalletService.Application.Commands.Ats;

public record DepositAtsCallback(string Requester, string Application, string Token, string PreToken, string Message);

public class DepositAtsCallbackConsumer : IConsumer<DepositAtsCallback>
{
    private readonly ICallbackForwarderService _callbackForwarderService;

    public DepositAtsCallbackConsumer(ICallbackForwarderService callbackForwarderService)
    {
        _callbackForwarderService = callbackForwarderService;
    }

    public async Task Consume(ConsumeContext<DepositAtsCallback> context)
    {
        await _callbackForwarderService.ForwardDepositAtsCallback(
            context.Message.Requester,
            context.Message.Application,
            context.Message.Token,
            context.Message.PreToken,
            context.Message.Message);
    }
}