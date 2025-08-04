using System.Globalization;
using System.Runtime.Serialization;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Services.FxService;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WalletAggregate;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.GlobalWalletTransfer;

public record ValidateFxRequest(
    string TransactionId,
    decimal RequestedFxAmount,
    decimal ConfirmedFxAmount,
    decimal FxMarkupRate
);

public record ValidateFxV2Request(
    Guid CorrelationId,
    decimal RequestedFxAmount,
    decimal ConfirmedFxAmount
);

[Serializable]
public class FxRateDiffOverThresholdException : Exception
{
    public FxRateDiffOverThresholdException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public FxRateDiffOverThresholdException()
    {
    }

    public FxRateDiffOverThresholdException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected FxRateDiffOverThresholdException(SerializationInfo info, StreamingContext ctx)
        : base(info, ctx)
    {
    }
}

public class ValidateFxRequestConsumer : IConsumer<ValidateFxRequest>
{
    private readonly ILogger<ValidateFxRequestConsumer> _logger;
    private readonly decimal _maxSlippage;

    public ValidateFxRequestConsumer(
        ILogger<ValidateFxRequestConsumer> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _maxSlippage = decimal.Parse(configuration["Fx:MaxSlippage"] ?? "1");
    }

    public async Task Consume(ConsumeContext<ValidateFxRequest> ctx)
    {
        var confirmedRate = ctx.Message.ConfirmedFxAmount;
        if (ctx.Message.FxMarkupRate > 0)
        {
            confirmedRate = FxSpreadUtils.CalculateMarkedUp(TransactionType.Deposit, confirmedRate, ctx.Message.FxMarkupRate);
        }

        if (Math.Abs(ctx.Message.RequestedFxAmount - confirmedRate) / ctx.Message.RequestedFxAmount * 100 > _maxSlippage)
        {
            throw new FxRateDiffOverThresholdException($"Fx rate exceed {_maxSlippage}%");
        }

        await ctx.RespondAsync(
            new ValidateFxRequestSucceed(ctx.Message.TransactionId)
        );
    }
}
