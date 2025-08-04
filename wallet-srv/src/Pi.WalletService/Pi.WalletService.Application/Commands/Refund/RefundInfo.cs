using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.Exceptions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.Refund;

public record RefundInfoRequest(Guid CorrelationId, string DepositTransactionNo);
public record RefundInfoResponse(Guid CorrelationId,
    string DepositTransactionNo,
    string CustomerCode,
    decimal RefundAmount,
    string BankAccountNo,
    string BankName,
    string BankCode,
    decimal? BankFee,
    string UserId,
    string AccountCode,
    Product Product);
public class RefundInfoConsumer : IConsumer<RefundInfoRequest>
{
    private readonly IDepositRepository _depositRepository;

    public RefundInfoConsumer(IDepositRepository depositRepository)
    {
        _depositRepository = depositRepository;
    }

    public async Task Consume(ConsumeContext<RefundInfoRequest> context)
    {
        var deposit = await _depositRepository.GetByTransactionNo(context.Message.DepositTransactionNo);

        if (deposit == null)
        {
            throw new CannotRefundException("Deposit Transaction Not Found");
        }
        if (deposit.PaymentReceivedAmount == null || deposit.BankAccountNo == null || deposit.BankCode == null)
        {
            throw new CannotRefundException("Deposit payment not complete yet");
        }

        await context.RespondAsync(
            new RefundInfoResponse(
                context.Message.CorrelationId,
                deposit.TransactionNo!,
                deposit.CustomerCode,
                deposit.PaymentReceivedAmount!.Value,
                deposit.BankAccountNo!.Replace("-", ""),
                deposit.BankName!,
                deposit.BankCode!,
                deposit.BankFee,
                deposit.UserId,
                deposit.AccountCode,
                deposit.Product
            )
        );
    }
}
