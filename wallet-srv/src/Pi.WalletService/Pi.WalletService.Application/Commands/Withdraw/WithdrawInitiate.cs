using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.Bank;
using Pi.WalletService.Application.Services.CustomerService;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.Withdraw;

public record WithdrawInitiateRequest(
    string UserId,
    string CustomerCode,
    Product Product
);

public record WithdrawInitiateResponse(string BankAccountNo, string BankName, string BankCode);

public class WithdrawInitiate : IConsumer<WithdrawInitiateRequest>
{
    private readonly IWalletQueries _walletQueries;

    public WithdrawInitiate(
        IWalletQueries walletQueries
    )
    {
        _walletQueries = walletQueries;
    }

    public async Task Consume(ConsumeContext<WithdrawInitiateRequest> context)
    {
        var customerBank = await _walletQueries.GetBankAccount(
            context.Message.UserId,
            context.Message.CustomerCode,
            context.Message.Product,
            TransactionType.Withdraw);

        await context.RespondAsync(
            new WithdrawInitiateResponse(
                customerBank.AccountNo,
                customerBank.ShortName,
                customerBank.Code
            ));
    }
}