using System.Runtime.Serialization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.CustomerService;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.ATS;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.Ats;

public record WithdrawAtsRequest(Guid CorrelationId);

[Serializable]
public class WithdrawAtsException : Exception
{
    public WithdrawAtsException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public WithdrawAtsException()
    {
    }

    public WithdrawAtsException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected WithdrawAtsException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

public class WithdrawAtsRequestConsumer : SagaConsumer, IConsumer<WithdrawAtsRequest>
{
    private readonly ICustomerService _customerService;
    private readonly IWalletQueries _walletQueries;
    private readonly ILogger<WithdrawAtsRequestConsumer> _logger;

    public WithdrawAtsRequestConsumer(
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        ICustomerService customerService,
        IWalletQueries walletQueries,
        ILogger<WithdrawAtsRequestConsumer> logger) : base(depositEntrypointRepository, withdrawEntrypointRepository)
    {
        _customerService = customerService;
        _logger = logger;
        _walletQueries = walletQueries;
    }

    public async Task Consume(ConsumeContext<WithdrawAtsRequest> context)
    {
        try
        {
            var withdrawEntrypoint = await GetWithdrawEntrypointByIdAsync(context.Message.CorrelationId);
            if (withdrawEntrypoint == null || string.IsNullOrEmpty(withdrawEntrypoint.TransactionNo))
            {
                throw new InvalidDataException("Withdraw Entrypoint Not Found");
            }

            var bankInfo = await _walletQueries.GetBankAccount(
                withdrawEntrypoint.UserId,
                withdrawEntrypoint.CustomerCode,
                withdrawEntrypoint.Product,
                TransactionType.Withdraw);
            if (bankInfo == null)
            {
                throw new InvalidDataException("User Bank Account Not Found");
            }

            var resp = await _customerService.WithdrawAtsAsync(
                withdrawEntrypoint.TransactionNo,
                withdrawEntrypoint.AccountCode,
                withdrawEntrypoint.ConfirmedAmount,
                bankInfo.Code,
                bankInfo.AccountNo,
                string.Empty,
                DateOnly.FromDateTime(DateUtils.GetThDateTimeFromUtc(withdrawEntrypoint.EffectiveDate)),
                bankInfo.BranchCode);

            if (resp == null)
            {
                throw new InvalidDataException("Received null response from Freewill");
            }

            await context.RespondAsync(
                new AtsWithdrawRequestSuccess(
                    resp.ReferId,
                    resp.TransId,
                    resp.ResultCode,
                    resp.Reason,
                    resp.SendDate,
                    resp.SendTime,
                    0 // in case we have fee
                )
            );
        }
        catch (Exception ex) when (ex is not InvalidDataException)
        {
            _logger.LogError(ex, "TicketId: {TicketId} WithdrawAtsRequestConsumer: Failed with Exception {Exception}", context.Message.CorrelationId, ex.Message);
            throw new WithdrawAtsException("Internal Errors");
        }
    }
}