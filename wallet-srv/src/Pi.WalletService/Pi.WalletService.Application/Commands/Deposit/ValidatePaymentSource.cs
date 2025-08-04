using System.Text.RegularExpressions;
using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.Domain.Exceptions;
namespace Pi.WalletService.Application.Commands.Deposit;

public record ValidatePaymentSource(string TransactionNo, string BankAccountName);
public record ValidatePaymentSourceV2(Guid CorrelationId);

public class ValidatePaymentSourceConsumer :
    SagaConsumer,
    IConsumer<ValidatePaymentSourceV2>,
    IConsumer<ValidatePaymentSource>
{
    private readonly static List<string> InvalidBankName = new()
    {
        "KTB G-WALLET", "บจก.ทรู มันนี่ เพื่อเก็บรักษาเงินรับล่วง"
    };

    public ValidatePaymentSourceConsumer(IDepositEntrypointRepository depositEntrypointRepository, IWithdrawEntrypointRepository withdrawEntrypointRepository) : base(depositEntrypointRepository, withdrawEntrypointRepository)
    {
    }

    public async Task Consume(ConsumeContext<ValidatePaymentSource> context)
    {
        if (InvalidBankName.Contains(context.Message.BankAccountName))
        {
            throw new InvalidBankSourceException();
        }

        await context.RespondAsync(new DepositValidatePaymentSourceSucceed(context.Message.TransactionNo));
    }

    public async Task Consume(ConsumeContext<ValidatePaymentSourceV2> context)
    {
        var depositEntrypointState = await GetDepositEntrypointByIdAsync(context.Message.CorrelationId);
        if (depositEntrypointState == null)
        {
            throw new Exception("Deposit Entrypoint Not Found");
        }

        if (InvalidBankName.Contains(depositEntrypointState.BankAccountName!))
        {
            throw new InvalidBankSourceException($"Invalid Bank Source");
        }

        await context.RespondAsync(new DepositValidatePaymentSourceSucceed(depositEntrypointState.TransactionNo!));
    }
}
