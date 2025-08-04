using Pi.BackofficeService.Application.Models;

namespace Pi.BackofficeService.Application.Services.OcrService
{
    public interface IOcrService
    {
        Task<OcrThirdPartyApiResponse> ScanDocumentsAsync(OcrDocumentType documentType, IList<OcrFileUploadModel> model, OcrOutputType output, string? password);
    }
}