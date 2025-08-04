using System.Runtime.Serialization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Services.CustomerService;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.UpBack;
using Pi.WalletService.Domain.Events.Withdraw;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands;

public record UpdateTradingAccountBalanceRequest(Guid TicketId, string TransactionId, decimal Amount, string CustomerCode, string AccountCode, string BankCode, Channel Channel, TransactionType TransactionType);

public record UpdateTradingAccountBalanceV2Request(Guid CorrelationId, TransactionType TransactionType);

[Serializable]
public class UpdateTradingAccountBalanceException : Exception
{
    public UpdateTradingAccountBalanceException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public UpdateTradingAccountBalanceException()
    {
    }

    public UpdateTradingAccountBalanceException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected UpdateTradingAccountBalanceException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

public class UpdateTradingAccountBalance :
    SagaConsumer,
    IConsumer<UpdateTradingAccountBalanceRequest>,
    IConsumer<UpdateTradingAccountBalanceV2Request>
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<UpdateTradingAccountBalance> _logger;

    public UpdateTradingAccountBalance(
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        ICustomerService customerService,
        ILogger<UpdateTradingAccountBalance> logger) : base(depositEntrypointRepository, withdrawEntrypointRepository)
    {
        _customerService = customerService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UpdateTradingAccountBalanceRequest> context)
    {
        var resp = await UpdateCustomerTradingAccountBalance(
            context.Message.TransactionId,
            context.Message.CustomerCode,
            context.Message.AccountCode,
            context.Message.Amount,
            context.Message.BankCode,
            context.Message.Channel,
            context.Message.TransactionType
        );

        if (resp == null)
        {
            _logger.LogError("TicketId: {TicketId} UpdateTradingAccountBalanceConsumer: Exception encountered during {TransactionType}. Exception: Null response from customer service"
                , context.Message.TicketId
                , context.Message.TransactionType.ToString());
            throw new UpdateTradingAccountBalanceException("Null response from customer service");
        }

        await context.RespondAsync(
            new UpdateTradingAccountBalanceSuccess(
                resp.ReferId,
                resp.TransId,
                resp.ResultCode,
                resp.Reason,
                resp.SendDate,
                resp.SendTime
            )
        );
    }

    public async Task Consume(ConsumeContext<UpdateTradingAccountBalanceV2Request> context)
    {
        string transactionNo = string.Empty;
        string customerCode = string.Empty;
        string accountCode = string.Empty;
        decimal amount = 0;
        string bankName = string.Empty;
        Channel channel = Channel.Unknown;
        if (context.Message.TransactionType == TransactionType.Deposit)
        {
            var depositEntrypoint = await GetDepositEntrypointByIdAsync(context.Message.CorrelationId);
            if (depositEntrypoint == null)
            {
                throw new KeyNotFoundException("Deposit Entrypoint Not Found");
            }

            transactionNo = depositEntrypoint.TransactionNo!;
            customerCode = depositEntrypoint.CustomerCode;
            accountCode = depositEntrypoint.AccountCode;
            amount = depositEntrypoint.RequestedAmount;
            bankName = depositEntrypoint.BankName!;
            channel = depositEntrypoint.Channel;
        }
        else if (context.Message.TransactionType == TransactionType.Withdraw)
        {
            var withdrawEntrypoint = await GetWithdrawEntrypointByIdAsync(context.Message.CorrelationId);
            if (withdrawEntrypoint == null)
            {
                throw new KeyNotFoundException("Withdraw Entrypoint Not Found");
            }

            transactionNo = withdrawEntrypoint.TransactionNo!;
            customerCode = withdrawEntrypoint.CustomerCode;
            accountCode = withdrawEntrypoint.AccountCode;
            amount = withdrawEntrypoint.RequestedAmount;
            bankName = withdrawEntrypoint.BankName!;
            channel = withdrawEntrypoint.Channel;
        }

        if (string.IsNullOrEmpty(transactionNo) || string.IsNullOrEmpty(customerCode) || string.IsNullOrEmpty(accountCode) || amount == 0 || string.IsNullOrEmpty(bankName) || channel == Channel.Unknown)
        {
            _logger.LogError("TicketId: {TicketId} UpdateTradingAccountBalanceConsumer: Invalid Request Parameters", context.Message.CorrelationId);
            throw new Exception("Invalid Transaction No, Customer Code, Account Code, Amount, Bank Code or Channel");
        }

        var resp = await UpdateCustomerTradingAccountBalance(
            transactionNo,
            customerCode,
            accountCode,
            amount,
            bankName,
            channel,
            context.Message.TransactionType
        );

        if (resp == null)
        {
            _logger.LogError("TicketId: {TicketId} UpdateTradingAccountBalanceConsumer: Null Response From Freewill", context.Message.CorrelationId);
            throw new UpdateTradingAccountBalanceException("Null Response From Freewill");
        }

        await context.RespondAsync(
            new GatewayUpdateAccountBalanceSuccessEvent(
                resp.ReferId,
                resp.TransId,
                resp.ResultCode,
                resp.Reason,
                resp.SendDate,
                resp.SendTime
            )
        );
    }

    private async Task<ICustomerService.CustomerServiceResponse?> UpdateCustomerTradingAccountBalance(
        string transactionId,
        string customerCode,
        string accountCode,
        decimal amount,
        string bankName,
        Channel channel,
        TransactionType transactionType
        )
    {
        try
        {
            var bankCode = _customerService.GetBankCode(bankName, channel);
            var paymentType = _customerService.GetPaymentTypeCode(channel, transactionType);

            ICustomerService.CustomerServiceResponse resp;

            _logger.LogInformation("Consume UpdateTradingAccountBalanceConsumer: {TransactionType} for {CustomerCode} with {Amount} {AccountCode} {BankCode} {Channel}",
                transactionType.ToString(),
                customerCode,
                amount,
                accountCode,
                bankCode,
                channel.ToString());

            if (transactionType == TransactionType.Deposit)
            {
                resp = await _customerService.DepositCashAsync(
                    customerCode,
                    transactionId,
                    accountCode,
                    amount,
                    Purpose.Collateral,
                    paymentType,
                    bankCode,
                    GetDepositRemark(channel, bankCode, transactionId));
            }
            else
            {
                resp = await _customerService.WithdrawAnyPayTypeAsync(
                    transactionId,
                    accountCode,
                    amount,
                    paymentType,
                    bankCode,
                    string.Empty);
            }

            // todo: put this condition in Freewill Service Layer
            if (resp.ResultCode is not ("_000" or "_001"))
            {
                _logger.LogError("Transaction: {transactionId} received invalid result code from freewill {ResultCode}",
                    transactionId, resp.ResultCode);
                throw new Exception($"Result Code Invalid ({resp.ResultCode})");
            }

            return resp;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "UpdateTradingAccountBalanceConsumer: {TransactionType} failed. Error: {ErrorMessage}", transactionType.ToString(), e.Message);
            throw new UpdateTradingAccountBalanceException(e.Message);
        }
    }

    private static string GetDepositRemark(Channel channel, string bankCode, string transId)
    {
        return channel switch
        {
            Channel.SetTrade => $"STT-{transId}",
            Channel.ODD => $"ODD-FINNET({bankCode})",
            Channel.QR => string.Empty,
            Channel.ATS => string.Empty,
            Channel.OnlineViaKKP => string.Empty,
            Channel.EForm => string.Empty,
            Channel.TransferApp => string.Empty,
            Channel.Unknown => string.Empty,
            _ => string.Empty
        };
    }
}
