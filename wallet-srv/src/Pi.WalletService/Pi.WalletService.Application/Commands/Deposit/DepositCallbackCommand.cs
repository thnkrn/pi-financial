using Pi.WalletService.Application.Services.Freewill;
using MassTransit;

namespace Pi.WalletService.Application.Commands.Deposit;

public record DepositCallback(string Requester, string Application, string Token,
    string PreToken, string Message);

public class DepositCallbackConsumer : IConsumer<DepositCallback>
{
    private readonly ICallbackForwarderService _callbackForwarderService;

    public DepositCallbackConsumer(ICallbackForwarderService callbackForwarderService)
    {
        _callbackForwarderService = callbackForwarderService;
    }

    public async Task Consume(ConsumeContext<DepositCallback> context)
    {
        await _callbackForwarderService.ForwardDepositCashCallback(
            context.Message.Requester,
            context.Message.Application,
            context.Message.Token,
            context.Message.PreToken,
            context.Message.Message);
    }
}