using System.Runtime.Serialization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.CustomerService;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.ATS;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.Ats;

public record DepositAtsRequest(Guid CorrelationId);

[Serializable]
public class DepositAtsException : Exception
{
    public DepositAtsException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public DepositAtsException()
    {
    }

    public DepositAtsException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected DepositAtsException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}


public class DepositAtsRequestConsumer : SagaConsumer, IConsumer<DepositAtsRequest>
{
    private readonly ICustomerService _customerService;
    private readonly IWalletQueries _walletQueries;
    private readonly ILogger<DepositAtsRequestConsumer> _logger;

    public DepositAtsRequestConsumer(
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        ICustomerService customerService,
        ILogger<DepositAtsRequestConsumer> logger, IWalletQueries walletQueries) : base(depositEntrypointRepository, withdrawEntrypointRepository)
    {
        _customerService = customerService;
        _logger = logger;
        _walletQueries = walletQueries;
    }

    public async Task Consume(ConsumeContext<DepositAtsRequest> context)
    {
        try
        {
            var depositEntrypoint = await GetDepositEntrypointByIdAsync(context.Message.CorrelationId);
            if (depositEntrypoint == null || string.IsNullOrEmpty(depositEntrypoint.TransactionNo))
            {
                throw new InvalidDataException("Deposit Entrypoint Not Found");
            }

            var bankInfo = await _walletQueries.GetBankAccount(
                depositEntrypoint.UserId,
                depositEntrypoint.CustomerCode,
                depositEntrypoint.Product,
                TransactionType.Deposit);
            if (bankInfo == null)
            {
                throw new InvalidDataException("User Bank Account Not Found");
            }

            var resp = await _customerService.DepositAtsAsync(
                depositEntrypoint.TransactionNo,
                depositEntrypoint.AccountCode,
                depositEntrypoint.RequestedAmount,
                bankInfo.Code,
                bankInfo.AccountNo,
                string.Empty,
                bankInfo.BranchCode);

            if (resp == null)
            {
                throw new InvalidDataException("Received null response from Freewill");
            }

            await context.RespondAsync(
                new AtsDepositRequestSuccess(
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
            _logger.LogError(ex, "TicketId: {TicketId} DepositAtsRequestConsumer: Failed with Exception {Exception}", context.Message.CorrelationId, ex.Message);
            throw new DepositAtsException("Internal Errors");
        }
    }
}