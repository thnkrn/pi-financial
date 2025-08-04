using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Application.Services.OcrService;

namespace Pi.BackofficeService.API.Controllers;

[Authorize]
[ApiController]
[Route("ocr")]
public class OcrController : ControllerBase
{
    private readonly IOcrService _ocrService;

    public OcrController(IOcrService ocrService)
    {
        _ocrService = ocrService;
    }

    /// <summary>
    /// This API sends the documents to the OCR service and returns the data captured
    /// </summary>
    /// <param name="files">Documents to be scanned</param>
    /// <param name="documentType">Document Type, please refer to OcrDocumentType enum</param>
    /// <param name="output">Please refer to OcrOutputType, outputs the result in List of data or in csv format</param>
    /// <param name="password">password for protected pdf file</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    [Authorize(Policy = "OcrPortalAccess")]
    [HttpPost("Process")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OcrThirdPartyApiResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> OcrProcess([FromForm] List<IFormFile> files, [FromForm] OcrDocumentType documentType, [FromForm] OcrOutputType output, [FromForm] string? password)
    {
        try
        {
            var list = new List<OcrFileUploadModel>();

            foreach (IFormFile file in files)
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    var bytes = stream.ToArray();
                    list.Add(new OcrFileUploadModel(bytes, file.FileName));
                }
            }

            var result = await _ocrService.ScanDocumentsAsync(documentType, list, output, password);
            return Ok(result);
        }
        catch (Exception e)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
        }
    }
}