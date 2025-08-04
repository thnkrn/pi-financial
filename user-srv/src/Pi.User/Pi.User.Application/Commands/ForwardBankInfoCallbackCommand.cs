using MassTransit;
using Pi.User.Application.Services.LegacyUserInfo;

namespace Pi.User.Application.Commands;

public record ForwardBankInfoCallbackCommand(string Requester, string Application, string Token, string PreToken, string Message);

public class ForwardBankInfoCallbackCommandConsumer : IConsumer<ForwardBankInfoCallbackCommand>
{
    private readonly IUserInfoService _userInfoService;

    public ForwardBankInfoCallbackCommandConsumer(IUserInfoService userInfoService)
    {
        this._userInfoService = userInfoService;

    }
    public async Task Consume(ConsumeContext<ForwardBankInfoCallbackCommand> context)
    {
        await this._userInfoService.NotifyBankAccountInfo(context.Message.Requester, context.Message.Application, context.Message.Token, context.Message.PreToken, context.Message.Message);
    }
}