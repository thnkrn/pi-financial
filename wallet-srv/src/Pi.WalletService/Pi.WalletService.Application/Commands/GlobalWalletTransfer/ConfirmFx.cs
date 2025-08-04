using System.Runtime.Serialization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Services.FxService;
using Pi.WalletService.Domain.Events.ForeignExchange;
namespace Pi.WalletService.Application.Commands.GlobalWalletTransfer;

public record ConfirmFxRequest(string TransactionId);

[Serializable]
public class UnableToConfirmFxException : Exception
{
    public UnableToConfirmFxException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public UnableToConfirmFxException()
    {
    }

    public UnableToConfirmFxException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected UnableToConfirmFxException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

public class ConfirmFxConsumer : IConsumer<ConfirmFxRequest>
{
    private readonly ILogger<ConfirmFxConsumer> _logger;
    private readonly IFxService _fxService;

    public ConfirmFxConsumer(ILogger<ConfirmFxConsumer> logger,
        IFxService fxService)
    {
        _logger = logger;
        _fxService = fxService;
    }

    public async Task Consume(ConsumeContext<ConfirmFxRequest> context)
    {
        try
        {
            await _fxService.Confirm(new ConfirmRequest(context.Message.TransactionId));
            var confirmedTime = DateTime.Now;

            await context.RespondAsync(new FxConfirmed(context.Message.TransactionId, confirmedTime));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "TransactionId: {TransactionId} ConfirmFxConsumer: Cannot confirm fx rate with Exception: {Message}"
                , context.Message.TransactionId, e.Message);
            throw new UnableToConfirmFxException("Cannot Confirm Fx Rate");
        }
    }
}