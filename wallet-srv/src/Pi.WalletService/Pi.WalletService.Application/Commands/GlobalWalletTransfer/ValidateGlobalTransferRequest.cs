using MassTransit;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.GlobalWalletTransfer;

public record ValidateGlobalTransferRequest(
    string TransactionNo,
    TransactionType TransactionType,
    string UserId,
    string CustCode,
    decimal ExchangeRate,
    decimal RequestedAmount,
    Currency RequestedCurrency,
    Currency RequestedFxCurrency
);

public record ValidateGlobalTransferV2Request(
    Guid CorrelationId,
    TransactionType TransactionType,
    decimal ExchangeRate,
    Currency RequestedCurrency,
    Currency RequestedFxCurrency
);

public class ValidateGlobalTransferRequestConsumer :
    SagaConsumer,
    IConsumer<ValidateGlobalTransferRequest>,
    IConsumer<ValidateGlobalTransferV2Request>
{
    private readonly IWalletQueries _walletQueries;

    public ValidateGlobalTransferRequestConsumer(
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        IWalletQueries walletQueries) : base(depositEntrypointRepository, withdrawEntrypointRepository)
    {
        _walletQueries = walletQueries;
    }

    public async Task Consume(ConsumeContext<ValidateGlobalTransferRequest> context)
    {
        if (context.Message.RequestedCurrency != Currency.THB && context.Message.RequestedFxCurrency != Currency.THB)
        {
            throw new InvalidDataException($"Invalid currency pair, Got {context.Message.RequestedCurrency} and {context.Message.RequestedFxCurrency}");
        }

        switch (context.Message.TransactionType)
        {
            case TransactionType.Deposit:
                await ValidateDeposit(
                    context.Message.RequestedCurrency,
                    context.Message.RequestedAmount,
                    context.Message.ExchangeRate
                    );
                break;
            case TransactionType.Withdraw:
                await ValidateWithdraw(
                    context.Message.UserId,
                    context.Message.CustCode,
                    context.Message.RequestedCurrency,
                    context.Message.RequestedAmount
                    );
                break;
            default:
                throw new InvalidDataException($"Invalid Transaction Type, {context.Message.TransactionType}");
        }

        await context.RespondAsync(new GlobalTransferRequestValidationCompleted(context.Message.TransactionNo));
    }

    public async Task Consume(ConsumeContext<ValidateGlobalTransferV2Request> context)
    {
        if (context.Message.RequestedCurrency != Currency.THB && context.Message.RequestedFxCurrency != Currency.THB)
        {
            throw new InvalidDataException($"Invalid currency pair, Got {context.Message.RequestedCurrency} and {context.Message.RequestedFxCurrency}");
        }

        switch (context.Message.TransactionType)
        {
            case TransactionType.Deposit:
                var depositEntrypoint = await GetDepositEntrypointByIdAsync(context.Message.CorrelationId);
                if (depositEntrypoint == null)
                {
                    throw new Exception($"Deposit Entrypoint Not Found");
                }
                await ValidateDeposit(
                    context.Message.RequestedCurrency,
                    depositEntrypoint.RequestedAmount,
                    context.Message.ExchangeRate
                    );
                break;
            case TransactionType.Withdraw:
                var withdrawEntrypoint = await GetWithdrawEntrypointByIdAsync(context.Message.CorrelationId);
                if (withdrawEntrypoint == null)
                {
                    throw new Exception($"Withdraw Entrypoint Not Found");
                }
                await ValidateWithdraw(
                    withdrawEntrypoint.UserId,
                    withdrawEntrypoint.CustomerCode,
                    context.Message.RequestedCurrency,
                    withdrawEntrypoint.RequestedAmount
                    );
                break;
            default:
                throw new InvalidDataException($"Invalid Transaction Type, {context.Message.TransactionType}");
        }

        await context.RespondAsync(new GlobalTransferRequestValidationCompletedV2(context.Message.CorrelationId));
    }

    private static Task ValidateDeposit(
        Currency requestedCurrency,
        decimal requestedAmount,
        decimal exchangeRate
        )
    {
        var amountThb = RoundingUtils.RoundExchangeForLogic(
            requestedCurrency,
            requestedAmount,
            Currency.THB,
            exchangeRate
        );

        if (amountThb is < 1 or > 2_000_000)
        {
            throw new InvalidDataException("Requested amount should between 1 to 2,000,000");
        }
        return Task.CompletedTask;
    }

    private async Task ValidateWithdraw(
        string userId,
        string customerCode,
        Currency requestedCurrency,
        decimal requestedAmount
        )
    {
        await _walletQueries.VerifyUserGeBalance(
            userId,
            customerCode,
            requestedCurrency.ToString(),
            requestedAmount
        );
    }
}
