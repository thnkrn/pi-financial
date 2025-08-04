using System.Runtime.Serialization;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Services.GlobalEquities;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.Domain.Exceptions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.GlobalWalletTransfer;

public record TransferUsdMoneyFromSubAccountToMainAccount(string TransactionNo, string AccountId, Currency Currency, decimal Amount, decimal Fee);
public record TransferUsdMoneyFromSubAccountToMainAccountV2(Guid CorrelationId, string GlobalAccountId, Currency Currency, decimal Fee);

public record TransferUsdMoneyFromMainAccountToSubAccount(string TransactionNo, string AccountId, Currency Currency, decimal Amount, decimal Fee);
public record TransferUsdMoneyFromMainAccountToSubAccountV2(Guid CorrelationId, string GlobalAccountId, Currency Currency, decimal Amount, decimal Fee);

[Serializable]
public class TransferMoneyException : Exception
{
    public TransferMoneyException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public TransferMoneyException()
    {
    }

    public TransferMoneyException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected TransferMoneyException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

[Serializable]
public class TransferInsufficientBalanceException : Exception
{
    public TransferInsufficientBalanceException(string message, Exception? innerException) : base(message,
        innerException)
    {

    }

    public TransferInsufficientBalanceException()
    {

    }

    public TransferInsufficientBalanceException(string message) : base(message)
    {

    }

    protected TransferInsufficientBalanceException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {

    }
}

public class TransferUsdMoneyConsumer :
    SagaConsumer,
    IConsumer<TransferUsdMoneyFromSubAccountToMainAccount>,
    IConsumer<TransferUsdMoneyFromSubAccountToMainAccountV2>,
    IConsumer<TransferUsdMoneyFromMainAccountToSubAccount>,
    IConsumer<TransferUsdMoneyFromMainAccountToSubAccountV2>
{
    private readonly IGlobalTradeService _globalTradeService;
    private readonly IGlobalUserManagementService _globalUserManagementService;
    private readonly ILogger<TransferUsdMoneyConsumer> _logger;
    private readonly string _mainAccountId;

    public TransferUsdMoneyConsumer(
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        IGlobalUserManagementService globalUserManagementService,
        IGlobalTradeService globalTradeService,
        ILogger<TransferUsdMoneyConsumer> logger,
        IConfiguration configuration) : base(depositEntrypointRepository, withdrawEntrypointRepository)
    {
        _globalTradeService = globalTradeService;
        _globalUserManagementService = globalUserManagementService;
        _logger = logger;
        _mainAccountId = configuration["Exante:MainAccountId"] ?? string.Empty;
    }

    public async Task Consume(ConsumeContext<TransferUsdMoneyFromSubAccountToMainAccount> context)
    {
        try
        {
            var sourceAccountSummary =
                await _globalTradeService.GetAccountSummary(context.Message.AccountId, context.Message.Currency.ToString());

            if (decimal.Parse(sourceAccountSummary.AvailableBalance) >= context.Message.Amount)
            {
                var requestedTime = DateTime.Now;
                await _globalUserManagementService.TransferMoney(
                    context.Message.AccountId,
                    _mainAccountId,
                    context.Message.Currency.ToString(),
                    context.Message.Amount);

                await context.RespondAsync(
                    new TransferUsdMoneyToMainSucceeded(
                        context.Message.TransactionNo,
                        context.Message.AccountId,
                        _mainAccountId,
                        context.Message.Amount,
                        context.Message.Currency,
                        requestedTime,
                        DateTime.Now,
                        context.Message.Fee
                    )
                );

                _logger.LogInformation(
                    "TransactionNo: {TransactionNo} AllocateMoneyConsumer: Allocated {Amount} {Currency} from {SubAccountId} to {MainAccountId}",
                    context.Message.TransactionNo,
                    context.Message.Amount,
                    context.Message.Currency,
                    context.Message.AccountId,
                    _mainAccountId
                );
            }
            else
            {
                throw new TransferInsufficientBalanceException("Insufficient account balance");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "TransactionNo: {TransactionNo} AllocateMoneyConsumer: Unable to allocate money from {SubAccountId} to {MainAccountId} with Exception: {Message}",
                context.Message.TransactionNo, context.Message.AccountId, _mainAccountId, e.Message);

            if (e is TransferInsufficientBalanceException)
            {
                throw;
            }

            throw new TransferMoneyException($"Unable to allocate money from {context.Message.AccountId} to {_mainAccountId}");
        }
    }

    public async Task Consume(ConsumeContext<TransferUsdMoneyFromSubAccountToMainAccountV2> context)
    {
        var withdrawEntrypoint = await GetWithdrawEntrypointByIdAsync(context.Message.CorrelationId);
        if (withdrawEntrypoint is null)
        {
            throw new Exception($"Withdraw Entrypoint Not Found");
        }

        var result = await TransferToMain(
            withdrawEntrypoint.TransactionNo!,
            context.Message.GlobalAccountId,
            context.Message.Currency,
            withdrawEntrypoint.RequestedAmount,
            context.Message.Fee
        );

        if (result is null)
        {
            throw new Exception($"Unable to transfer money from {context.Message.GlobalAccountId} to {_mainAccountId}");
        }

        await context.RespondAsync(result);

        _logger.LogInformation(
            "TicketId: {TicketId} AllocateMoneyConsumer: Allocated {Amount} {Currency} from {SubAccountId} to {MainAccountId}",
            context.Message.CorrelationId,
            withdrawEntrypoint.RequestedAmount,
            context.Message.Currency,
            context.Message.GlobalAccountId,
            _mainAccountId
        );
    }

    public async Task Consume(ConsumeContext<TransferUsdMoneyFromMainAccountToSubAccount> context)
    {
        var result = await TransferToSub(
            context.Message.TransactionNo,
            context.Message.AccountId,
            context.Message.Currency,
            context.Message.Amount,
            context.Message.Fee
        );

        if (result is null)
        {
            throw new Exception($"Unable to transfer money from {_mainAccountId} to {context.Message.AccountId}");
        }

        await context.RespondAsync(result);

        _logger.LogInformation(
            "TransactionNo: {TransactionNo} AllocateMoneyConsumer: Allocated {Amount} {Currency} from {MainAccountId} to {SubAccountId}",
            context.Message.TransactionNo,
            context.Message.Amount,
            context.Message.Currency,
            _mainAccountId,
            context.Message.AccountId
        );
    }

    public async Task Consume(ConsumeContext<TransferUsdMoneyFromMainAccountToSubAccountV2> context)
    {
        var entryPoint = await GetAnyEntryPointByIdAsync(context.Message.CorrelationId);
        if (entryPoint == null)
        {
            throw new Exception("Entrypoint Not Found");
        }

        var result = await TransferToSub(
            entryPoint.TransactionNo!,
            context.Message.GlobalAccountId,
            context.Message.Currency,
            context.Message.Amount,
            context.Message.Fee
        );

        if (result is null)
        {
            throw new Exception($"Unable to transfer money from {_mainAccountId} to {context.Message.GlobalAccountId}");
        }

        await context.RespondAsync(result);

        _logger.LogInformation(
            "TicketId: {TicketId} AllocateMoneyConsumer: Allocated {Amount} {Currency} from {MainAccountId} to {SubAccountId}",
            context.Message.CorrelationId,
            context.Message.Amount,
            context.Message.Currency,
            _mainAccountId,
            context.Message.GlobalAccountId
        );
    }

    private async Task<TransferUsdMoneyToMainSucceeded?> TransferToMain(
        string transactionNo,
        string globalAccountId,
        Currency currency,
        decimal amount,
        decimal fee
        )
    {
        try
        {
            var sourceAccountSummary =
                await _globalTradeService.GetAccountSummary(globalAccountId, currency.ToString());

            if (decimal.Parse(sourceAccountSummary.AvailableBalance) < amount)
            {
                throw new TransferInsufficientBalanceException("Insufficient account balance");
            }

            var requestedTime = DateTime.Now;
            await _globalUserManagementService.TransferMoney(
                globalAccountId,
                _mainAccountId,
                currency.ToString(),
                amount);

            return new TransferUsdMoneyToMainSucceeded(
                transactionNo,
                globalAccountId,
                _mainAccountId,
                amount,
                currency,
                requestedTime,
                DateTime.Now,
                fee
            );
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "TransactionNo: {TransactionNo} AllocateMoneyConsumer: Unable to allocate money from {SubAccountId} to {MainAccountId} with Exception: {Message}",
                transactionNo, globalAccountId, _mainAccountId, e.Message);

            if (e is TransferInsufficientBalanceException)
            {
                throw;
            }

            throw new TransferMoneyException($"Unable to allocate money from {globalAccountId} to {_mainAccountId}");
        }
    }

    private async Task<TransferUsdMoneyToSubSucceeded?> TransferToSub(
        string transactionNo,
        string globalAccountId,
        Currency currency,
        decimal amount,
        decimal fee
        )
    {
        try
        {
            var requestedTime = DateTime.Now;
            var netAmount = amount - fee;

            if (netAmount < 0)
            {
                throw new TransferMoneyException("Insufficient Balance after deduct Fee");
            }

            await _globalUserManagementService.TransferMoney(
                _mainAccountId,
                globalAccountId,
                currency.ToString(),
                netAmount
            );

            return new TransferUsdMoneyToSubSucceeded(
                transactionNo,
                _mainAccountId,
                globalAccountId,
                netAmount,
                currency,
                requestedTime,
                DateTime.Now,
                fee
            );


        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "TransactionNo: {TransactionNo} AllocateMoneyConsumer: Unable to allocate money from {MainAccountId} to {SubAccountId} with Exception: {Message}",
                transactionNo, _mainAccountId, globalAccountId, e.Message);

            if (e is TransferInsufficientBalanceException)
            {
                throw;
            }

            throw new TransferMoneyException($"Unable to allocate money from {_mainAccountId} to {globalAccountId}");
        }
    }
}
