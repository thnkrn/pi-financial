using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Domain.Events.Withdraw;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.Withdraw;

[EntityName("kkp-withdraw-ats.fifo")]
public record KkpWithdraw(
    bool IsSuccess,
    double Amount,
    string CustomerCode,
    string Product,
    string TransactionNo,
    string TransactionRefCode
);

public class ProcessKkpWithdrawCallbackConsumer : IConsumer<KkpWithdraw>
{
    private readonly IBus _bus;
    private readonly ILogger<ProcessKkpWithdrawCallbackConsumer> _logger;

    public ProcessKkpWithdrawCallbackConsumer(IBus bus, ILogger<ProcessKkpWithdrawCallbackConsumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<KkpWithdraw> context)
    {
        if (!string.Equals(context.Message.Product, Product.GlobalEquities.ToString(), StringComparison.InvariantCultureIgnoreCase))
        {
            // Ignore KKP withdraw callback for NonGE"
            _logger.LogInformation("Ignore KKP withdraw callback for NonGE Withdraw V2");
            return;
        }

        if (context.Message.IsSuccess)
        {
            await _bus.Publish(new WithdrawCallbackReceived(context.Message.TransactionNo));
        }
        else
        {
            await _bus.Publish(new WithdrawCallbackFailed(context.Message.TransactionNo));
        }
    }
}