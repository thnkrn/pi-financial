using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi.BackofficeService.API.Models;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using Pi.BackofficeService.Application.Models.Ats;
using Pi.BackofficeService.Application.Services.OnboardService;
using Pi.Client.OnboardService.Model;
using Pi.Common.Http;


namespace Pi.BackofficeService.API.Controllers;

[Authorize]
[ApiController]
[Route("ats_registration")]
public class AtsRegistrationController : ControllerBase
{
    private readonly IOnboardService _onboardService;
    private const string TEMP_ATS_UPLOAD_FILE_NAME = "temp_ats_upload.csv";

    public AtsRegistrationController(IOnboardService onboardService)
    {
        _onboardService = onboardService;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }


    /// <summary>
    /// Get all update ats request
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("requests")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PiOnboardServiceAPIModelsAtsAtsRequestsPaginated))]
    public async Task<IActionResult> GetAtsUpdateRequests(
        [FromQuery] AtsRequestQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _onboardService.GetAtsUpdateRequests(query.AtsUploadType, query.RequestDate?.ToDateTime(new TimeOnly()), query.Page,
            query.PageSize, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Download ats update result in xlsx format
    /// </summary>
    /// <param name="atsRequestId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{atsRequestId}/download", Name = "DownloadAtsUpdateResult")]
    [Produces("text/xlsx")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DownloadAtsUpdateResult(
        [FromRoute(Name = "atsRequestId")] Guid atsRequestId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var atsResult = await _onboardService.GetAtsUpdateResults(atsRequestId, cancellationToken);
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection(atsResult.Data.Data, true);
                await package.SaveAsync(cancellationToken);
            }
            stream.Position = 0;
            var fileName = $"{atsResult.Data.FileName.Split('.')[0]}_result.xlsx";

            HttpContext.Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");
            HttpContext.Response.Headers.Append("Content-Disposition", $"attachment; filename={fileName}");

            // Create file download response
            return File(stream, "application/octet-stream", fileName);
        }
        catch (InvalidDataException ex)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: ex.Message);
        }
    }

    /// <summary>
    /// Update bank effective date by upload excel file
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("internal/ats/upload", Name = "InternalUploadAtsBankAccount")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<AtsRequestResultDto>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> InternalUploadAtsBankAccount(
        [FromForm][Required] UploadAtsPayload payload,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var atsBanks = payload.UploadFile.FileName.Contains(".csv")
                ? await MapFromCsvFile(payload, cancellationToken)
                : await MapFromExcelFile(payload, cancellationToken);

            if (atsBanks.Count > 0)
            {
                await _onboardService.AddAtsBankAccount(payload.UploadFile.FileName, payload.UserName,
                    payload.UploadType.ToString(), atsBanks, cancellationToken);
            }

            return Ok(new ApiResponse<AtsRequestResultDto>(new AtsRequestResultDto(atsBanks.Count)));
        }
        catch (InvalidDataException ex)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    private static async Task<List<PiOnboardServiceApplicationCommandsUpdateAtsBankEffectiveDateRow>> MapFromExcelFile(UploadAtsPayload payload, CancellationToken cancellationToken)
    {
        await using var stream = new MemoryStream();
        await payload.UploadFile.CopyToAsync(stream, cancellationToken);
        var package = new ExcelPackage(stream);

        var data = MapToAtsBankAccounts(package.Workbook.Worksheets[0], payload.UploadType);

        return data;
    }

    private static async Task<List<PiOnboardServiceApplicationCommandsUpdateAtsBankEffectiveDateRow>> MapFromCsvFile(
        UploadAtsPayload payload, CancellationToken cancellationToken)
    {
        var format = new ExcelTextFormat
        {
            DataTypes = [
                eDataTypes.String,
                eDataTypes.String,
                eDataTypes.String,
                eDataTypes.String,
                eDataTypes.String
            ],
            Delimiter = ','
        };
        var filePath = Path.Combine(string.Empty, TEMP_ATS_UPLOAD_FILE_NAME);
        if (payload.UploadFile.Length > 0)
        {
            await using Stream fileStream = new FileStream(filePath, FileMode.Create);
            await payload.UploadFile.CopyToAsync(fileStream, cancellationToken);
        }
        var file = new FileInfo(filePath);
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Sheet1");
        await worksheet.Cells["A1"].LoadFromTextAsync(file, format, TableStyles.Dark1, FirstRowIsHeader: true);

        file.Delete();

        var data = MapToAtsBankAccounts(worksheet, payload.UploadType);

        return data;
    }

    private static List<PiOnboardServiceApplicationCommandsUpdateAtsBankEffectiveDateRow> MapToAtsBankAccounts(ExcelWorksheet worksheet, AtsUploadType uploadType)
    {
        var atsBanks = new List<PiOnboardServiceApplicationCommandsUpdateAtsBankEffectiveDateRow>();
        int rowCount = worksheet.Dimension.Rows;
        for (int row = 2; row <= rowCount; row++)
        {
            string customerCode = worksheet.Cells[row, 1].Text.Trim();
            string bankAccountNo = worksheet.Cells[row, 2].Text.Trim();
            string bankCode = worksheet.Cells[row, 3].Text.Trim();
            string bankBranchCode = worksheet.Cells[row, 4].Text.Trim();
            string effectiveDate = worksheet.Cells[row, 5].Text.Trim();
            if (string.IsNullOrEmpty(customerCode)
                && string.IsNullOrEmpty(bankAccountNo)
                && string.IsNullOrEmpty(bankCode)
                && string.IsNullOrEmpty(bankBranchCode))
            {
                break;
            }

            if (string.IsNullOrEmpty(customerCode)
                || string.IsNullOrEmpty(bankAccountNo)
                || string.IsNullOrEmpty(bankCode)
                || string.IsNullOrEmpty(bankBranchCode)
                || (uploadType is AtsUploadType.OverrideBankInfo && string.IsNullOrEmpty(effectiveDate)))
            {
                throw new InvalidDataException("Missing some values");
            }

            atsBanks.Add(new PiOnboardServiceApplicationCommandsUpdateAtsBankEffectiveDateRow(customerCode, bankAccountNo, bankCode, bankBranchCode,
                uploadType is AtsUploadType.OverrideBankInfo ? effectiveDate : string.Empty));
        }

        return atsBanks;
    }
}
