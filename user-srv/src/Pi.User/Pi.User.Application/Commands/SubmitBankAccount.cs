using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pi.User.Application.Models.ErrorCode;
using Pi.User.Application.Services.Cryptography;
using Pi.User.Domain.AggregatesModel.BankAccountAggregate;
using Pi.User.Domain.AggregatesModel.DocumentAggregate;

namespace Pi.User.Application.Commands;

public record BankAccountInfoPayload(string BankAccountNo, string? BankAccountName, string BankCode, string? BankBranchCode, IFormFile? Bookbank);

public record BankAccountDocumentPayload(List<IFormFile> Statements);

public record SubmitBankAccountRequest(Guid UserId, string BankAccountNo, string? BankAccountName, string BankCode,
    string? BankBranchCode, string? FileUrl, string? FileName);

public record SubmitBankAccountResponse(Guid BankAccountId);

public class SubmitBankAccountConsumer : IConsumer<SubmitBankAccountRequest>
{
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly ICryptographyService _cryptographyService;
    private readonly ILogger<SubmitBankAccountConsumer> _logger;

    public SubmitBankAccountConsumer(
        IBankAccountRepository bankAccountRepository,
        ICryptographyService cryptographyService,
        ILogger<SubmitBankAccountConsumer> logger)
    {
        _bankAccountRepository = bankAccountRepository;
        _cryptographyService = cryptographyService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SubmitBankAccountRequest> context)
    {
        try
        {
            var bankAccount = await _bankAccountRepository.GetByUserIdAsync(context.Message.UserId);

            if (bankAccount is null)
            {
                var bankAccountName = string.IsNullOrEmpty(context.Message.BankAccountName) ? "-" : context.Message.BankAccountName;
                bankAccount = new BankAccount(
                    Guid.NewGuid(),
                    context.Message.UserId,
                    context.Message.BankAccountNo,
                    _cryptographyService.Hash(context.Message.BankAccountNo),
                    bankAccountName,
                    _cryptographyService.Hash(bankAccountName),
                    context.Message.BankCode,
                    context.Message.BankBranchCode ?? "0000"
                );
                await _bankAccountRepository.AddAsync(bankAccount);
            }
            else
            {
                var bankAccountName = string.IsNullOrEmpty(context.Message.BankAccountName) ? bankAccount.AccountName : context.Message.BankAccountName;
                bankAccount.Update(
                    context.Message.BankAccountNo,
                    _cryptographyService.Hash(context.Message.BankAccountNo),
                    bankAccountName,
                    _cryptographyService.Hash(bankAccountName),
                    context.Message.BankCode,
                    context.Message.BankBranchCode ?? "0000"
                );
            }


            if (!string.IsNullOrEmpty(context.Message.FileUrl) && !string.IsNullOrEmpty(context.Message.FileName))
                await context.Publish(
                    new SubmitDocumentRequest(
                        context.Message.UserId,
                        new List<SubmitDocument>
                        {
                            new(DocumentType.BookBank, context.Message.FileUrl, context.Message.FileName)
                        }
                    )
                );


            await _bankAccountRepository.UnitOfWork.SaveChangesAsync();

            await context.RespondAsync(new SubmitBankAccountResponse(bankAccount.Id));
        }
        catch (InvalidDataException ex)
        {
            await context.RespondAsync(new ErrorCodeResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to save bank account");
            await context.RespondAsync(new ErrorCodeResponse(ex.Message));
        }
    }
}