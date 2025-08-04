using Amazon.S3;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Financial.Client.PdfService.Api;
using Pi.Financial.Client.PdfService.Model;
using Pi.User.Application.Options;
using Pi.User.Application.Services.Pdf;

namespace Pi.User.Infrastructure.Services;

public class PdfService : IPdfService
{
    private readonly IPdfServiceApi _pdfServiceApi;
    private readonly ILogger<PdfService> _logger;
    private readonly IOptionsSnapshot<AwsS3Option> _options;

    public PdfService(
        IPdfServiceApi pdfServiceApi,
        ILogger<PdfService> logger,
        IOptionsSnapshot<AwsS3Option> options
    )
    {
        _pdfServiceApi = pdfServiceApi;
        _logger = logger;
        _options = options;
    }

    public async Task<string> GenerateCrsForm(
        FatcaCRSFormDto crsForm,
        string fileName
    )
    {
        try
        {

            await _pdfServiceApi.FundsControllerGenerateFatcaCRSFormAsync(
                new GenerateFatcaCRSDocumentsDto(crsForm, _options.Value.DocumentBucketName, fileName)
            );

            var fileUrl = $"https://{_options.Value.DocumentBucketName}.s3.{S3Region.APSoutheast1.Value}.amazonaws.com/{fileName}";

            return fileUrl;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PdfService:GenerateCrsForm: ");
            throw;
        }
    }

}