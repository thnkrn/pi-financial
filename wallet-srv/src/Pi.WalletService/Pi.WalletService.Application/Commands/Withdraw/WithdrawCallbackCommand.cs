using Pi.WalletService.Application.Services.Freewill;
using MassTransit;

namespace Pi.WalletService.Application.Commands.Withdraw;

public record WithdrawCallback(string Requester, string Application, string Token,
    string PreToken, string Message);

public class WithdrawCallbackConsumer : IConsumer<WithdrawCallback>
{
    private readonly ICallbackForwarderService _callbackForwarderService;

    public WithdrawCallbackConsumer(ICallbackForwarderService callbackForwarderService)
    {
        _callbackForwarderService = callbackForwarderService;

    }
    public async Task Consume(ConsumeContext<WithdrawCallback> context)
    {
        await _callbackForwarderService.ForwardWithdrawAnyPaytypeCallback(context.Message.Requester,
            context.Message.Application, context.Message.Token, context.Message.PreToken, context.Message.Message);
    }
}