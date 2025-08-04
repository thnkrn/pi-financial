using Amazon.Runtime.Internal.Util;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.User.Application.Models.ErrorCode;
using Pi.User.Domain.AggregatesModel.BankAccountAggregate;

namespace Pi.User.Application.Commands;

public record DeleteBankAccountRequest(Guid UserId);

public record DeleteBankAccountResponse;

public class DeleteBankAccountConsumer : IConsumer<DeleteBankAccountRequest>
{
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly ILogger<DeleteBankAccountConsumer> _logger;

    public DeleteBankAccountConsumer(
        IBankAccountRepository bankAccountRepository,
        ILogger<DeleteBankAccountConsumer> logger)
    {
        _bankAccountRepository = bankAccountRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DeleteBankAccountRequest> context)
    {
        try
        {
            var bankAccount = await _bankAccountRepository.GetByUserIdAsync(context.Message.UserId);
            if (bankAccount is not null)
            {
                _bankAccountRepository.Delete(bankAccount);

                await _bankAccountRepository.UnitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new InvalidDataException($"Bank account not found. UserId: {context.Message.UserId}");
            }

            await context.RespondAsync(new DeleteBankAccountResponse());
        }
        catch (InvalidDataException ex)
        {
            await context.RespondAsync(new ErrorCodeResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to delete bank account");
            await context.RespondAsync(new ErrorCodeResponse(ex.Message));
        }
    }
}