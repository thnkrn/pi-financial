using System.ComponentModel.DataAnnotations;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.User.Application.Commands;
using Pi.User.Application.Models.BankAccount;
using Pi.User.Application.Models.ErrorCode;
using Pi.User.Application.Queries.BankAccount;
using Pi.User.Application.Queries.Storage;
using Pi.User.Application.Utils;
using Pi.User.Domain.AggregatesModel.DocumentAggregate;

namespace Pi.User.API.Controllers;

[ApiController]
public class UserBankAccountController : ControllerBase
{
    private readonly IBankAccountQueries _bankAccountQueries;
    private readonly IBus _bus;
    private readonly IStorageQueries _storageQueries;

    public UserBankAccountController(
        IStorageQueries storageQueries,
        IBankAccountQueries bankAccountQueries,
        IBus bus)
    {
        _storageQueries = storageQueries;
        _bankAccountQueries = bankAccountQueries;
        _bus = bus;
    }

    [HttpPost("secure/bank-account", Name = "UpdateBankAccount")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateBankAccount(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [FromForm][Required] BankAccountInfoPayload bankAccountInfo
    )
    {
        try
        {
            string? fileUrl = null;
            string? fileName = null;
            if (bankAccountInfo.Bookbank is not null)
            {
                // TODO: move this validation logic to FluentValidator
                var maxFileLength = 1 * 1000 * 1000 * 10; // 10mb
                if (bankAccountInfo.Bookbank.Length > maxFileLength)
                    return Problem(statusCode: StatusCodes.Status400BadRequest,
                        detail: $"{bankAccountInfo.Bookbank.FileName} was exceed maximum size");

                var fileExt = Path.GetExtension(bankAccountInfo.Bookbank.FileName).ToLower();
                if (!CommonUtil.IsSupportFileExtension(fileExt))
                    return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"{fileExt} was not support");

                (fileUrl, fileName) =
                    await _storageQueries.UploadFile(userId, bankAccountInfo.Bookbank, DocumentType.BookBank.ToString());
            }


            var client = _bus.CreateRequestClient<SubmitBankAccountRequest>();
            var (status, error) = await client.GetResponse<SubmitBankAccountResponse, ErrorCodeResponse>(
                new SubmitBankAccountRequest(
                    Guid.Parse(userId),
                    bankAccountInfo.BankAccountNo,
                    bankAccountInfo.BankAccountName,
                    bankAccountInfo.BankCode,
                    bankAccountInfo.BankBranchCode,
                    fileUrl,
                    fileName
                )
            );

            if (status.IsCompletedSuccessfully) return Accepted();

            var resp = await error;
            Enum.TryParse<BankAccountErrorCode>(resp.Message.ErrorCode, out var bankAccountError);
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: bankAccountError.ToString(),
                detail: bankAccountError.ToDescriptionString());
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [RequestSizeLimit(50_000_000)]
    [HttpPost("secure/bank-account/upload-document", Name = "UploadBankAccountDocument")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadBankAccountDocument(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [FromForm][Required] BankAccountDocumentPayload bankAccountInfo
    )
    {
        try
        {
            // TODO: move this validation logic to FluentValidator
            if (bankAccountInfo.Statements.Count > 5)
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Upload file was exceeded maximum");

            var maxFileLength = 1 * 1000 * 1000 * 10; // 10mb
            var exceedFiles = bankAccountInfo.Statements.Where(f => f.Length > maxFileLength).ToList();
            if (exceedFiles.Any())
                return Problem(statusCode: StatusCodes.Status400BadRequest,
                    detail: $"{exceedFiles.First().FileName} was exceed maximum size");

            var filesExt = bankAccountInfo.Statements.Select(x => Path.GetExtension(x.FileName).ToLower());
            if (filesExt.Any(x => !CommonUtil.IsSupportFileExtension(x)))
                return Problem(statusCode: StatusCodes.Status400BadRequest,
                    detail: $"Only {string.Join(", ", CommonUtil.SUPPORT_FILE_TYPES)} was support");

            var documents = new List<SubmitDocument>();
            for (var i = 0; i < bankAccountInfo.Statements.Count; i++)
            {
                var (fileUrl, fileName) = await _storageQueries.UploadFile(userId, bankAccountInfo.Statements[i],
                    $"{DocumentType.Statement}_{i + 1}");
                documents.Add(new SubmitDocument(DocumentType.Statement, fileUrl, fileName));
            }

            await _bus.Publish(
                new SubmitDocumentRequest(
                    Guid.Parse(userId),
                    documents
                )
            );

            return Accepted();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [HttpGet("internal/bank-account/{userId}", Name = "GetBankAccountByUserId")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<BankAccountDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBankAccountByUserId(
        [FromRoute] string userId
    )
    {
        try
        {
            var result = await _bankAccountQueries.GetBankAccountByUserId(Guid.Parse(userId));

            return Ok(new ApiResponse<BankAccountDto>(result));
        }
        catch (InvalidDataException ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [HttpDelete("internal/bank-account/{userId}", Name = "DeleteBankAccountByUserId")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteBankAccountByUserId(
        [FromRoute] string userId
    )
    {
        try
        {
            var client = _bus.CreateRequestClient<DeleteBankAccountRequest>();
            var (status, error) = await client.GetResponse<DeleteBankAccountResponse, ErrorCodeResponse>(
                new DeleteBankAccountRequest(
                    Guid.Parse(userId)
                )
            );

            if (status.IsCompletedSuccessfully) return Accepted();

            var resp = await error;
            Enum.TryParse<BankAccountErrorCode>(resp.Message.ErrorCode, out var bankAccountError);
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: bankAccountError.ToString(),
                detail: bankAccountError.ToDescriptionString());
        }
        catch (InvalidDataException ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }
}