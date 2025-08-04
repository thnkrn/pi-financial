using MassTransit;
using Pi.BackofficeService.Application.Factories;
using Pi.BackofficeService.Application.Services.UserService;
using Pi.BackofficeService.Application.Services.WalletService;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransferCashAggregate;
using Pi.BackofficeService.Domain.Exceptions;

namespace Pi.BackofficeService.Application.Commands.Transaction;

public record FetchTransactionMessage(string TransactionNo, TransactionType TransactionType);

public record FetchTransactionResponse(
    Guid TransactionId,
    string TransactionNo,
    string TransactionState,
    string? CustomerName,
    string CustomerCode,
    Guid? ResponseCodeId);

public class FetchTransactionConsumer : IConsumer<FetchTransactionMessage>
{
    private readonly IDepositWithdrawService _depositWithdrawService;
    private readonly ITransferCashService _transferCashService;
    private readonly IUserService _userService;
    private readonly IResponseCodeRepository _responseCodeRepository;

    public FetchTransactionConsumer(
        IDepositWithdrawService depositWithdrawService,
        ITransferCashService transferCashService,
        IUserService userService,
        IResponseCodeRepository responseCodeRepository)
    {
        _depositWithdrawService = depositWithdrawService;
        _transferCashService = transferCashService;
        _responseCodeRepository = responseCodeRepository;
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<FetchTransactionMessage> context)
    {
        if (!new[] { TransactionType.Deposit, TransactionType.Withdraw, TransactionType.TransferCash }.Contains(context.Message.TransactionType))
        {
            throw new NotImplementedException();
        }

        ResponseCode? responseCode = null;
        var (transaction, transferCash) = await FetchTransactionData(context);

        if (transaction != null)
        {
            var machine = EntityFactory.NewMachine(transaction.TransactionType);

            if (machine != null && transaction.State != null)
            {
                var productType = EntityFactory.NewProductType(transaction.Product!.Value);
                responseCode =
                    await _responseCodeRepository.GetByStateMachine((Machine)machine, transaction.State, productType) ??
                    await _responseCodeRepository.GetByStateMachine((Machine)machine, transaction.State, null);
            }

            var customer = await _userService.GetUserByIdOrCustomerCode(transaction.CustomerCode!, true);
            var customerName = customer != null
                ? $"{customer.FirstnameTh} {customer.LastnameTh} {customer.FirstnameEn} {customer.LastnameEn}"
                : null;

            await context.RespondAsync(
                new FetchTransactionResponse(
                    transaction.Id,
                    transaction.TransactionNo!,
                    transaction.State!,
                    customerName,
                    transaction.CustomerCode!,
                    responseCode?.Id
                )
            );
        }
        else if (transferCash != null)
        {
            if (transferCash.State != null)
            {
                responseCode =
                    await _responseCodeRepository.GetByStateMachine(Machine.TransferCash, transferCash.State, null);
            }

            await context.RespondAsync(
                new FetchTransactionResponse(
                    transferCash.Id,
                    transferCash.TransactionNo!,
                    transferCash.State!,
                    transferCash.CustomerName,
                    transferCash.TransferFromAccountCode![..^1],
                    responseCode?.Id
                )
            );
        }
        else
        {
            throw new NotFoundException();
        }
    }

    private async Task<(TransactionV2?, TransferCash?)> FetchTransactionData(ConsumeContext<FetchTransactionMessage> context)
    {
        return context.Message.TransactionType switch
        {
            TransactionType.Deposit or TransactionType.Withdraw =>
                (await _depositWithdrawService.GetTransactionV2ByTransactionNo(context.Message.TransactionNo), null),
            TransactionType.TransferCash =>
                (null, await _transferCashService.GetTransferCashByTransactionNo(context.Message.TransactionNo)),
            _ => throw new NotSupportedException()
        };
    }
}