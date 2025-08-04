using System.ComponentModel.DataAnnotations;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.User.Application.Commands;
using Pi.User.Application.Models.Document;
using Pi.User.Application.Queries;
using Pi.User.Application.Queries.Document;
using Pi.User.Application.Queries.Storage;

namespace Pi.User.API.Controllers;

[ApiController]
public class DocumentController : ControllerBase
{
    private readonly IBus _bus;
    private readonly IStorageQueries _storageQueries;
    private readonly IDocumentQueries _documentQueries;

    public DocumentController(
        IBus bus,
        IStorageQueries storageQueries,
        IDocumentQueries documentQueries)
    {
        _bus = bus;
        _storageQueries = storageQueries;
        _documentQueries = documentQueries;
    }

    [HttpPost("secure/document", Name = "UploadDocument")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadDocument(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [FromBody] [Required]
        UploadDocumentRequest uploadDocumentRequest
    )
    {
        try
        {
            var maxFileLengthMb = 10;
            var maxFileLengthBytes = maxFileLengthMb * 1000 * 1000; // 10mb
            var imageFileBytes = System.Convert.FromBase64String(uploadDocumentRequest.Image);

            if (imageFileBytes.Length > maxFileLengthBytes)
                return Problem(statusCode: StatusCodes.Status400BadRequest,
                    detail: $"{uploadDocumentRequest.DocType} document exceeds maximum size");

            var (fileUrl, fileName) = await _storageQueries.UploadFile(userId, uploadDocumentRequest.Image, uploadDocumentRequest.DocType.ToString());

            await _bus.Publish(
                new SubmitDocumentRequest(
                    Guid.Parse(userId),
                    new List<SubmitDocument> {
                        new SubmitDocument(uploadDocumentRequest.DocType, fileUrl, fileName)
                    }
                )
            );

            return Accepted();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [HttpGet("internal/document/{userId}", Name = "GetDocumentsByUserId")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<DocumentDto>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDocumentsByUserId(
        [FromRoute] string userId
    )
    {
        try
        {
            var result = await _documentQueries.GetDocumentsWithPreSignedUrlByUserId(Guid.Parse(userId));

            return Ok(new ApiResponse<List<DocumentDto>>(result));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }
}