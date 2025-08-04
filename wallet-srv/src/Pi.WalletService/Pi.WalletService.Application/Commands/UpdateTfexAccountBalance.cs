using System.Runtime.Serialization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Services.SetTrade;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.TfexAccountEvents;
using Pi.WalletService.IntegrationEvents.UpBackEvents;

namespace Pi.WalletService.Application.Commands;

public record UpdateTfexAccountBalanceRequest(string UserId, string TransactionId, string AccountCode, decimal Amount, TransactionType TransactionType);
public record UpdateTfexAccountBalanceV2Request(Guid CorrelationId, TransactionType TransactionType);

[Serializable]
public class UpdateTfexAccountBalanceException : Exception
{
    public UpdateTfexAccountBalanceException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public UpdateTfexAccountBalanceException()
    {
    }

    public UpdateTfexAccountBalanceException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected UpdateTfexAccountBalanceException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

public class UpdateTfexAccountBalance :
    SagaConsumer,
    IConsumer<UpdateTfexAccountBalanceRequest>,
    IConsumer<UpdateTfexAccountBalanceV2Request>
{
    private readonly ISetTradeService _setTradeService;
    private readonly ILogger<UpdateTfexAccountBalance> _logger;

    public UpdateTfexAccountBalance(
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        ISetTradeService setTradeService,
        ILogger<UpdateTfexAccountBalance> logger) : base(depositEntrypointRepository, withdrawEntrypointRepository)
    {
        _setTradeService = setTradeService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UpdateTfexAccountBalanceRequest> context)
    {
        var resp = await UpdateCustomerTfexAccountBalance(
            context.Message.UserId,
            context.Message.TransactionId,
            context.Message.AccountCode,
            context.Message.Amount,
            context.Message.TransactionType
        );

        if (resp == null)
        {
            throw new UpdateTfexAccountBalanceException("Update Customer TFEX Account Failed");
        }

        await context.RespondAsync(resp);
    }

    public async Task Consume(ConsumeContext<UpdateTfexAccountBalanceV2Request> context)
    {
        string userId;
        string transactionNo;
        string accountCode;
        decimal requestedAmount;
        if (context.Message.TransactionType == TransactionType.Deposit)
        {
            var entrypoint = await GetDepositEntrypointByIdAsync(context.Message.CorrelationId);
            if (entrypoint == null)
            {
                throw new UpdateTfexAccountBalanceException("Deposit Entrypoint Not Found");
            }

            userId = entrypoint.UserId;
            transactionNo = entrypoint.TransactionNo!;
            accountCode = entrypoint.AccountCode;
            requestedAmount = entrypoint.RequestedAmount;
        }
        else
        {
            var entrypoint = await GetWithdrawEntrypointByIdAsync(context.Message.CorrelationId);
            if (entrypoint == null)
            {
                throw new UpdateTfexAccountBalanceException("Withdraw Entrypoint Not Found");
            }

            userId = entrypoint.UserId;
            transactionNo = entrypoint.TransactionNo!;
            accountCode = entrypoint.AccountCode;
            requestedAmount = entrypoint.RequestedAmount;
        }

        var resp = await UpdateCustomerTfexAccountBalance(
            userId,
            transactionNo!,
            accountCode,
            requestedAmount,
            context.Message.TransactionType
        );

        if (resp == null)
        {
            throw new UpdateTfexAccountBalanceException("Update Customer TFEX Account Failed");
        }

        await context.RespondAsync(new TradingPlatformCallbackSuccessEvent(
            resp.UserId,
            resp.TransactionNo,
            resp.SetTradeAccountNo,
            resp.Amount
        ));
    }


    private async Task<UpdateTfexAccountBalanceSuccessEvent?> UpdateCustomerTfexAccountBalance(
        string userId,
        string transactionId,
        string accountCode,
        decimal amount,
        TransactionType transactionType
        )
    {
        try
        {
            SetTradeDepositWithdrawResponse response;
            if (transactionType == TransactionType.Withdraw)
            {
                response = await _setTradeService.CashWithdraw(
                    userId,
                    transactionId,
                    accountCode,
                    amount);
            }
            else
            {
                response = await _setTradeService.CashDeposit(
                    transactionId,
                    transactionId,
                    accountCode,
                    amount);
            }

            if (!response.Success)
            {
                throw new Exception("Withdraw Update resp success = false");
            }

            return new UpdateTfexAccountBalanceSuccessEvent(
                userId,
                transactionId,
                accountCode,
                amount
            );
        }
        catch (Exception e)
        {
            _logger.LogError(e, "TransactionId: {TransactionId} UpdateTfexAccountBalanceConsumer: SetTrade Update Failed, Error: {ErrorMessage}"
                , transactionId, e.Message);
            throw new UpdateTfexAccountBalanceException("SetTrade Internal Error");
        }
    }
}
